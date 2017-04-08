using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MySQL_DAC {
	/// <summary>
	/// Interaction logic for LogInTab.xaml
	/// </summary>
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
