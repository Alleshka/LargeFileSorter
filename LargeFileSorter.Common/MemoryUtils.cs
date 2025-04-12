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
    }
}
