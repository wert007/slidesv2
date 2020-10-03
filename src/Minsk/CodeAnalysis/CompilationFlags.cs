namespace Minsk.CodeAnalysis
{
	public static class CompilationFlags
	{
		public static string Directory { get; private set; }
		public static string FileName { get; private set; }
		public static bool CompleteRebuild { get; private set; }
		public static bool SilentMode { get; private set; }

		public static void Init(string directory, string fileName, bool completeRebuild, bool silentMode)
		{
			Directory = directory;
			FileName = fileName;
			CompleteRebuild = completeRebuild;
			SilentMode = silentMode;
		}
	}
}