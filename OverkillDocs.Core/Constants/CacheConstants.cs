namespace OverkillDocs.Core.Constants
{
    public static class CacheConstants
    {
        public static readonly TimeSpan DefaultObjectExpiration = TimeSpan.FromMinutes(10);
        public static readonly TimeSpan DefaultListExpiration = TimeSpan.FromMinutes(10);
        public static readonly int DefaultListSize = 50;

        public static readonly TimeSpan ChatExpiration = TimeSpan.FromMinutes(30);
        public static readonly int ChatSize = 20;
    }
}
