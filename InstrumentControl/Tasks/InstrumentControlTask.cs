using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentControl
{
    public abstract class InstrumentControlTask 
    {
        #region Properties

        protected MyTask TaskType { get; set; }
        protected TaskResults? TaskResults { get; set; }

        #endregion

        public InstrumentControlTask(MyTask taskType)
        {
            TaskType = taskType;
        }

        public TaskResults Run()
        {
            var myTaskResult = RunSpecific();
            return myTaskResult;
        }

        public abstract TaskResults RunSpecific();

    }
}
