using System;
using System.Threading.Tasks;

namespace NServiceBus.Gateway.Sql
{
    class SqlDeduplicationSession : IDeduplicationSession
    {
        public bool IsDuplicate => throw new NotImplementedException();

        public Task MarkAsDispatched()
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
