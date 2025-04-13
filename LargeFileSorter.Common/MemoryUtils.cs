namespace LargeFileSorter.Common
{
    public static class MemoryUtils
    {
        public static long LargeBufferSize { get; }
        public static int SmallBufferSize { get; }

        static MemoryUtils()
        {
            var memory = GC.GetGCMemoryInfo();
            long availableMemory = memory.TotalAvailableMemoryBytes;
            LargeBufferSize = availableMemory / 1024; // ~ 32 Mb for 32Gb RAM
            SmallBufferSize = (int)Math.Min(3 * 1024 * 1024, availableMemory / (10 * 1024)); // ~ 3Mb for 32Gm RAM
        }

        public static long GetAvailableDiskSpace(string outputPath)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Path cannot be null or whitespace.", nameof(outputPath));

            // Get the root of the path (e.g., "C:\", "/")
            string root = Path.GetPathRoot(outputPath);

            if (string.IsNullOrEmpty(root))
                throw new ArgumentException("Invalid path. Could not determine root.", nameof(outputPath));

            DriveInfo drive = new DriveInfo(root);

            if (!drive.IsReady)
                throw new IOException($"Drive {root} is not ready.");

            return drive.AvailableFreeSpace;
        }
    }
}
