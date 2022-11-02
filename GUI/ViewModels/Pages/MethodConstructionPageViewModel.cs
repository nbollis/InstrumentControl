using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Client;

namespace GUI
{
    public class MethodConstructionPageViewModel : BaseViewModel
    {
        #region Private Members

        private InstrumentType selectedInstrument;
        private WorkflowViewModel workflowViewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// List of Instrument Types to be selected from
        /// </summary>
        public ObservableCollection<InstrumentType> InstrumentTypes { get; set; }

        /// <summary>
        /// Currently Selected Instrument
        /// </summary>
        public InstrumentType SelectedInstrument
        {
            get => selectedInstrument;
            set => SetProperty(ref selectedInstrument, value);
        }

        /// <summary>
        /// Current workflow being displayed
        /// </summary>
        public WorkflowViewModel WorkflowViewModel
        {
            get => workflowViewModel;
            set => SetProperty(ref workflowViewModel, value);
        }

        #endregion

        #region Commands
        public ICommand RunWorkflowCommand { get; set; }
        public ICommand SaveWorkflowCommand { get; set; }
        public ICommand NewWorkflowCommand { get; set; }
        public ICommand SaveAsWorkflowCommand { get; set; }
        public ICommand DeleteWorkflowCommand { get; set; }

        #endregion

        #region Constructor

        public MethodConstructionPageViewModel()
        {
            InstrumentTypes = new ObservableCollection<InstrumentType>(Enum.GetValues<InstrumentType>());

            SaveWorkflowCommand = new RelayCommand(SaveWorkflow);
            NewWorkflowCommand = new RelayCommand(NewWorkflow);
            RunWorkflowCommand = new RelayCommand(RunWorkflow);
        }

        #endregion

        #region Private Helpers

        private void SaveWorkflow()
        {

        }

        private void NewWorkflow()
        {

        }

        private void RunWorkflow()
        {

        }

        private void SaveAsWorkflow()
        {

        }

        private void DeleteWorkflow()
        {

        }

        #endregion
    }
}
