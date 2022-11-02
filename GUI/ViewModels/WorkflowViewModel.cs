using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    public class WorkflowViewModel : BaseViewModel
    {
        #region Private Members

        private string name;

        #endregion

        #region Public Properties

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        #endregion

        #region Commands

        #endregion

        #region Constructor

        #endregion

        #region Private Helpers

        #endregion
    }
}
