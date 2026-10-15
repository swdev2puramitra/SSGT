namespace FireWorksEntity
{
    public class FireworksAppStatus
    {
        public static bool IsReleaseMode
        {
            get
            {
                bool isRelease = false;
#if DEBUG
                isRelease = false;
#else
                isRelease = true;
#endif
                return isRelease;
            }
        }
    }
}
