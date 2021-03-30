using System.IO;
using System.Net;
using MCE2E.Controller.Contracts;

namespace MCE2E.Controller.Providers
{
	internal class FtpStreamProvider : IStreamProvider
	{
		private readonly FtpStreamConfiguration _configuration;

		public FtpStreamProvider(FtpStreamConfiguration configuration)
		{
			_configuration = configuration;
		}

		public TargetType Target => TargetType.Ftp;

		public Stream Get(string targetFilepath)
		{
			//Create a FTPWebRequest
			var request = (FtpWebRequest)WebRequest.Create("ftp://" + "FTPServer" + @"/" + targetFilepath);
			request.Method = WebRequestMethods.Ftp.UploadFile;
			request.Credentials = new NetworkCredential("FTPUserName", "FTPPassword");

			return request.GetRequestStream();
		}
	}

	internal class FtpStreamConfiguration
	{
		public string ServerUrl { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }
	}
}
