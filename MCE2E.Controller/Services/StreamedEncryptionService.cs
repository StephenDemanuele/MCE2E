using System;
using System.IO;
using System.Linq;
using MCE2E.Contracts;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using MCE2E.Controller.Contracts;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MCE2E.Controller.Services
{
	public class StreamedEncryptionService : IEncryptionService
	{
		private readonly IConfiguration _configuration;
		private readonly IEncryptionAlgorithm _encryptionAlgorithm;
		private readonly ISymmetricKeyProvider _keyFactory;
		private readonly IStreamProvider[] _streamProviders;


		public event OverallProgressReport OnProgressChanged;

		private event FileProgressReport OnFileProgressChanged;

		public StreamedEncryptionService(
			IConfigurationService configurationService,
			IEncryptionAlgorithm encryptionAlgorithm,
			ISymmetricKeyProvider keyFactory,
			IEnumerable<IStreamProvider> streamProviders)
		{
			_configuration = configurationService.Get();
			_encryptionAlgorithm = encryptionAlgorithm;
			_keyFactory = keyFactory;
			_streamProviders = streamProviders.ToArray();
		}

		public async Task<List<EncryptionResult>> EncryptAsync(
			FileInfo[] sourceFiles,
			string targetLocation, 
			TargetType targetType,
			CancellationToken cancellationToken)
		{ 
			var result = new List<EncryptionResult>();
			
			if (!sourceFiles.Any())
			{
				return result;
			}

			float fileIndex = 0;
			OnFileProgressChanged += (sender, currentFileProgress) =>
			{
				OnProgressChanged?.Invoke(this, new EncryptionProgress(
					(fileIndex/sourceFiles.Length)*100, currentFileProgress));
			};
			foreach (var file in sourceFiles)
			{
				var encryptionResult = await EncryptAsync(file, targetLocation, TargetType.File, cancellationToken);
				result.Add(encryptionResult);
				fileIndex++;
			}

			OnProgressChanged?.Invoke(this, new EncryptionProgress(100, new FileEncryptionProgress(100, "Ready")));
			return result;
		}

		private async Task<EncryptionResult> EncryptAsync(
			FileInfo sourceFileToEncrypt,
			string targetLocation,
			TargetType targetType,
			CancellationToken cancellationToken)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var streamProvider = _streamProviders.First(x => x.Target == targetType);

			//add sanity checks on argument
			var targetFilepath = Path.Combine(targetLocation, $"{sourceFileToEncrypt.Name}.enc");
			var symmetricKey = _keyFactory.Get(_configuration.SymmetricKeyLength);

			if (!Directory.Exists(targetLocation))
			{
				Directory.CreateDirectory(targetLocation);
			}

			var operationCompleted = true;
			using (var targetStream = streamProvider.Get(targetFilepath))
			{
				var cryptoTransform = _encryptionAlgorithm.InitializeEncryption(symmetricKey, targetStream);
				using (var cryptoStream = new CryptoStream(targetStream, cryptoTransform, CryptoStreamMode.Write))
				{
					using (var cryptoStreamWriter = new StreamWriter(cryptoStream))
					{
						using (var sourceFileStreamReader = new StreamReader(sourceFileToEncrypt.FullName))
						{
							var chunksCount = sourceFileToEncrypt.Length / _configuration.ChunkSize;
							float chunkIndex = 1;
							var buffer = new char[_configuration.ChunkSize];
							int bytesRead;
							while ((bytesRead = await sourceFileStreamReader.ReadAsync(buffer, 0, buffer.Length)) > 0)
							{
								if (cancellationToken.IsCancellationRequested)
								{
									operationCompleted = false;
									break;
								}

								await cryptoStreamWriter.WriteAsync(buffer, 0, bytesRead);
								OnFileProgressChanged?.Invoke(this, 
									new FileEncryptionProgress((chunkIndex / chunksCount) * 100, sourceFileToEncrypt.Name));

								chunkIndex++;
							}
							sourceFileStreamReader.Close();
						}
						cryptoStreamWriter.Close();
					}
					cryptoStream.Close();
				}
				targetStream.Close();
			}

			if (operationCompleted)
			{
				//encrypt encryptedSymmetricKey and save with file
				var encryptedSymmetricKeyFilepath = Path.Combine(targetLocation, $"{sourceFileToEncrypt.Name}.enc.key");
				var encryptedSymmetricKey = _encryptionAlgorithm.EncryptSymmetricKey(symmetricKey, _configuration.PathToPublicKey);
				File.WriteAllBytes(encryptedSymmetricKeyFilepath, encryptedSymmetricKey);

				stopwatch.Stop();
				return new EncryptionResult(targetFilepath, encryptedSymmetricKeyFilepath, stopwatch.Elapsed);
			}
			//cleanup after operation cancellation
			streamProvider.CleanUp(targetFilepath);
			stopwatch.Stop();

			return null;
		}
	}
}
