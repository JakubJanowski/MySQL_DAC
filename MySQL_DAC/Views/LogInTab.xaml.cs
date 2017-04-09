using System.Windows;
using System.Windows.Controls;
using MySQL_DAC.Database;

namespace MySQL_DAC {
	public partial class LogInTab: UserControl {
		public LogInTab() {
			InitializeComponent();
		}

		private void loginButton_Click(object sender, RoutedEventArgs e) {

			if (DatabaseManager.Connect(usernameTextBox.Text, passwordBox.Password)) {
				((MainWindow)Application.Current.MainWindow).DatabaseTabUserControl.tableNamesComboBox.ItemsSource = DatabaseManager.GetTableNames();
				((MainWindow)Application.Current.MainWindow).ManagementTabUserControl.LoadUsers();
			}
		}
	}
}
