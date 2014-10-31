using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetCopy
{
    class ShareQueue
    {
        private static int ActiveThreads = 1;
        private EventWaitHandle allDone = new EventWaitHandle(false, EventResetMode.ManualReset);


        private static ShareQueue instance;

        public static ShareQueue Instance 
        { 
            get 
            {
                if (instance == null)
                    instance = new ShareQueue();
                return instance;
            } 
        }

        private ShareQueue() {}

        public ConcurrentQueue<string> QueuedShares { get; set; }

        public void AddShare(string sharePath)
        {
            ThreadPool.QueueUserWorkItem(GetFiles, sharePath);
        }

        private void GetFiles(object directoryNameObject)
        {
            var directoryName = directoryNameObject as string;

            //Console.WriteLine(directoryName);

            var files = System.IO.Directory.EnumerateFiles(directoryName).Where(f => SessionConfiguration.Instance.FileFilters.Any(filter => Regex.IsMatch(f, filter))).ToList();

            foreach (var file in files)
                Console.WriteLine(file);

            
            foreach (var directory in System.IO.Directory.EnumerateDirectories(directoryName))
            { 
                try
                {
                    GetFiles(directory);
                }
                catch (UnauthorizedAccessException e) {}
            }
        }

        private void PostActionCheck()
        {
            Interlocked.Decrement(ref ActiveThreads);
        }
    }
}
