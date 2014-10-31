using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCopy
{
    class FileCopyWorker
    {

        private int _runningWorkers = 0;


        internal void LookForWork()
        {
            Parallel.Invoke(
                StartWork,
                StartWork,
                StartWork,
                StartWork,
                StartWork
                );
        }

        private void StartWork()
        {
            while(true)
            {

                
            }
        }
    }
}
