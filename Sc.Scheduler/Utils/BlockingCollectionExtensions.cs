using System;
using System.Collections.Concurrent;

namespace Sc.Scheduler.Utils
{
    internal static class BlockingCollectionExtensions
    {
        public static bool SafeTryTake<T>(this BlockingCollection<T> blockingCollection, out T item, int millisecondsTimeout)
        {
            try
            {
                return blockingCollection.TryTake(out item, millisecondsTimeout);
            }
            catch (Exception)
            {
                item = default(T);

                return false;
            }
        }
    }
}
