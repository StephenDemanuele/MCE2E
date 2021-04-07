namespace MCE2E.Controller
{
	public delegate void OverallProgressReport(object sender, EncryptionProgress progress);

	internal delegate void FileProgressReport(object sender, FileEncryptionProgress fileEncryptionProgress);
		
	public class EncryptionProgress
	{
		public EncryptionProgress(int currentFile, int totalFiles, FileEncryptionProgress fileEncryptionProgress)
		{
			CurrentFile = currentFile;
			TotalFiles = totalFiles;
			CurrentFileProgress = fileEncryptionProgress.CurrentFileProgress;
			Status = fileEncryptionProgress.Status;
		}

		public int CurrentFile { get; }

		public int TotalFiles { get; }

		public float CurrentFileProgress { get; }

		public string Status { get; }

		public override string ToString() => $"File {CurrentFile} of {TotalFiles}    { CurrentFileProgress.ToString("N0")}% of ({Status})";

	}

	public class FileEncryptionProgress
	{
		public FileEncryptionProgress(float currentFileProgress, string status)
		{
			CurrentFileProgress = currentFileProgress;
			Status = status;
		}

		public float CurrentFileProgress { get; }

		public string Status { get; }
	}
}