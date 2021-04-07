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
		private IStreamProvider _streamProvider;

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
			ValidateArguments(sourceFiles, targetType, targetLocation);
			var result = new List<EncryptionResult>();
			if (!sourceFiles.Any())
			{
				return result;
			}

			_streamProvider = _streamProviders.First(x => x.Target == targetType);
			var fileIndex = 0;
			OnFileProgressChanged += (sender, currentFileProgress) =>
			{
				OnProgressChanged?.Invoke(this, new EncryptionProgress(fileIndex+1, sourceFiles.Length, currentFileProgress));
			};

			if (targetType == TargetType.File && !Directory.Exists(targetLocation))
			{
				Directory.CreateDirectory(targetLocation);
			}

			for (;fileIndex<sourceFiles.Length;fileIndex++)
			{
				var file = sourceFiles[fileIndex];
				if (cancellationToken.IsCancellationRequested)
				{
					CleanUpOnCancellation(result, sourceFiles.Length);
					result.Clear();

					return result;
				}
				var encryptionResult = await EncryptAsync(file, targetLocation, cancellationToken);
				
				if (encryptionResult != null) //can be null if cancellation was triggered when a file was being encrypted.
				{
					result.Add(encryptionResult);
				}
			}

			OnProgressChanged?.Invoke(this, new EncryptionProgress(sourceFiles.Length, sourceFiles.Length, new FileEncryptionProgress(100, "Ready")));
			return result;
		}

		private async Task<EncryptionResult> EncryptAsync(
			FileInfo sourceFileToEncrypt,
			string targetLocation,
			CancellationToken cancellationToken)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			//add sanity checks on argument
			var targetFilepath = Path.Combine(targetLocation, $"{sourceFileToEncrypt.Name}.enc");
			var symmetricKey = _keyFactory.Get(_configuration.SymmetricKeyLength);

			var operationCompleted = true;
			using (var targetStream = _streamProvider.Get(targetFilepath))
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
			_streamProvider.CleanUp(targetFilepath);
			stopwatch.Stop();

			return null;
		}

		private void ValidateArguments(FileInfo[] sourceFiles, TargetType targetType, string targetLocation)
		{
			if (sourceFiles == null)
			{
				throw new ArgumentNullException(nameof(sourceFiles));
			}

			if (targetType == TargetType.NotSet)
			{
				throw new ArgumentException(nameof(targetType));
			}

			if (string.IsNullOrEmpty(targetLocation))
			{
				throw new ArgumentNullException(nameof(targetLocation));
			}

			if (targetType == TargetType.File && !Path.IsPathRooted(targetLocation))
			{
				throw new ArgumentException($"Target location must be a rooted path when target type is File",
					nameof(targetLocation));
			}

			if (_streamProviders.All(x => x.Target != targetType))
			{
				throw new InvalidOperationException(
					$"No stream providers can be found for {nameof(targetType)}:{targetType}");
			}
		}

		private void CleanUpOnCancellation(List<EncryptionResult> encryptedFiles, int countOfSourceFilesToEncrypt)
		{
			if (!encryptedFiles.Any())
			{
				return;
			}

			var deletedFileCount = 0;
			foreach (var encryptedFile in encryptedFiles)
			{
				_streamProvider.CleanUp(encryptedFile.EncryptedFilePath);
				_streamProvider.CleanUp(encryptedFile.KeyFilePath);
				deletedFileCount++;

				//var progress = ((float)(encryptedFiles.Count - deletedFileCount) / countOfSourceFilesToEncrypt) * 100;
				OnProgressChanged?.Invoke(this,
					new EncryptionProgress((encryptedFiles.Count - deletedFileCount), countOfSourceFilesToEncrypt,
						new FileEncryptionProgress(100, $"Deleted {encryptedFile.EncryptedFilePath}")));
			}
		}
	}
}
