using System;
using System.IO;
using System.Net;
using MCE2E.Controller.Contracts;

namespace MCE2E.Controller.Providers.Ftp
{
	internal class FtpStreamProvider : IStreamProvider
	{
		private readonly FtpStreamConfiguration _configuration;
		private readonly bool _isAvailable;

		public FtpStreamProvider(FtpStreamConfigurationProvider ftpStreamConfigurationProvider)
		{
			_isAvailable = ftpStreamConfigurationProvider.TryGet(out _configuration);
			
		}

		public TargetType Target => TargetType.Ftp;

		public Stream Get(string targetFilepath)
		{
			if (!_isAvailable)
			{
				throw new InvalidOperationException("Could not load FTP configuration, streaming over FTP is not possible.");
			}

			//Create a FTPWebRequest
			var request = (FtpWebRequest)WebRequest.Create("ftp://" + "FTPServer" + @"/" + targetFilepath);
			request.Method = WebRequestMethods.Ftp.UploadFile;
			request.Credentials = new NetworkCredential("FTPUserName", "FTPPassword");

			return request.GetRequestStream();
		}

		public void CleanUp(string targetFilePath)
		{
			throw new NotImplementedException();
		}
	}
}
