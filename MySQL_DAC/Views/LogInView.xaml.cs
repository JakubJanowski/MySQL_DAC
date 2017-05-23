using System.Windows;
using System.Windows.Controls;
using MySQL_DAC.Database;

namespace MySQL_DAC.Views {
	public partial class LogInView: UserControl {
		public LogInView() {
			InitializeComponent();
		}

		private void loginButton_Click(object sender, RoutedEventArgs e) {
			if (DatabaseManager.Connect(usernameTextBox.Text, passwordBox.Password)) {
				((MainWindow)Application.Current.MainWindow).DataContext = new MainView(usernameTextBox.Text);
			}
		}

		private void closeButton_Click(object sender, RoutedEventArgs e) {
			((MainWindow)Application.Current.MainWindow).Close();
		}
	}
}
