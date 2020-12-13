using System;
using ArchiSteamFarm;

namespace BadgeFarmer
{
    public class SteamHelper : IDisposable
    {
        private readonly ArchiWebHandler ArchiWebHandler;
        private readonly bool DisposeWebHandler;

        public SteamHelper(ArchiWebHandler archiWebHandler, bool disposeWebHandler = false)
        {
            ArchiWebHandler = archiWebHandler;
            DisposeWebHandler = disposeWebHandler;
            if (DisposeWebHandler)
                GC.SuppressFinalize(this);
        }

        #region IDisposable

        public void Dispose() => Dispose(true);

        private void Dispose(bool manual)
        {
            if (manual)
                GC.SuppressFinalize(this);
            if (DisposeWebHandler)
                ArchiWebHandler.Dispose();
        }

        ~SteamHelper()
        {
            Dispose(false);
        }

        #endregion
    }
}