namespace MCE2E.Controller
{
	public delegate void OverallProgressReport(object sender, EncryptionProgress progress);

	internal delegate void FileProgressReport(object sender, FileEncryptionProgress fileEncryptionProgress);
		
	public class EncryptionProgress
	{
		public EncryptionProgress(float overallProgress, FileEncryptionProgress fileEncryptionProgress)
		{
			OverallProgress = overallProgress;
			CurrentFileProgress = fileEncryptionProgress.CurrentFileProgress;
			Status = fileEncryptionProgress.Status;
		}

		public float OverallProgress { get; }

		public float CurrentFileProgress { get; }

		public string Status { get; }

		public override string ToString()
		{
			return $"{OverallProgress.ToString("N1")}% - { CurrentFileProgress.ToString("N0")}% ({Status})";
		}
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