using System.Diagnostics;
using System.Windows;

namespace MySQL_DAC {
	public partial class MainWindow: Window {
		public DatabaseTab DatabaseTabUserControl { get { return databaseTab; } }
		public ManagementTab ManagementTabUserControl { get { return managementTab; } }

		public MainWindow() {
            InitializeComponent();
        }
    }
}
