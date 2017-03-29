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
			Debug.WriteLine(usernameTextBox.Text);
			Debug.WriteLine(passwordBox.SecurePassword);
			Process.Start("Notepad.exe", usernameTextBox.Text, passwordBox.SecurePassword, "");
		}
	}
}
