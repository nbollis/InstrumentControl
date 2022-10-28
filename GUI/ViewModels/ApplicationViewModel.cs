using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    /// <summary>
    /// ViewModel for the entire Instrument Control Application
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        #region Private Members


        #endregion

        #region Public Properties

        public MethodConstructionPageViewModel MethodConstructionPageViewModel { get; set; }

        #endregion

        #region Commands

        #endregion

        #region Constructor

        public ApplicationViewModel()
        {
            MethodConstructionPageViewModel = new();
        }

        #endregion

        #region Private Helpers

        #endregion

    }
}
