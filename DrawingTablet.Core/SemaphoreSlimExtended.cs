using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DrawingTablet.Core
{
    public sealed class SemaphoreSlimExtended
    {
        private readonly SemaphoreSlim semaphoreSlim;

        public SemaphoreSlimExtended(int initialCount, int maxCount)
        {
            semaphoreSlim = new SemaphoreSlim(initialCount, maxCount);
        }

        public SemaphoreLock Wait()
        {
            semaphoreSlim.Wait();
            return new SemaphoreLock(this);
        }

        public async Task<SemaphoreLock> WaitAsync(CancellationToken token)
        {
            await semaphoreSlim.WaitAsync(token);
            return new SemaphoreLock(this);
        }

        public void Release()
        {
            semaphoreSlim.Release();
        }

        public void Dispose()
        {
            semaphoreSlim.Dispose();
        }

        public struct SemaphoreLock : IDisposable
        {
            public static SemaphoreLock Empty => new SemaphoreLock(null);

            private readonly SemaphoreSlimExtended internalSemaphore;

            public SemaphoreLock(SemaphoreSlimExtended semaphoreExtended)
            {
                internalSemaphore = semaphoreExtended;
            }

            public void Dispose()
            {
                internalSemaphore?.Release();
            }
        }
    }
}
