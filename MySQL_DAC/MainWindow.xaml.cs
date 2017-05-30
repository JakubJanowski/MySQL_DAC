using System.Windows;
using MySQL_DAC.Database;
using MySQL_DAC.Views;

namespace MySQL_DAC {
	public partial class MainWindow: Window {
		public MainWindow() {
			DataContext = new LogInView();
			InitializeComponent();
        }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DataContext is MainView)
				Logger.WriteEntry($"{((MainView)DataContext).username} logged out");
		}
	}
}
