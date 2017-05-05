using System.Windows;
using MySQL_DAC.Views;

namespace MySQL_DAC {
	public partial class MainWindow: Window {
		public MainWindow() {
			DataContext = new LogInView();
			InitializeComponent();
        }
    }
}
