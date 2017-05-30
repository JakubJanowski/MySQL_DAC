using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySQL_DAC.Database;

namespace MySQL_DAC.Views {
	public partial class LogInView: UserControl {
		public LogInView() {
			InitializeComponent();
		}

		private void loginButton_Click(object sender, RoutedEventArgs e) {
			if (DatabaseManager.Connect(usernameTextBox.Text, passwordBox.Password)) {
				((MainWindow)Application.Current.MainWindow).DataContext = new MainView(usernameTextBox.Text);
				Logger.WriteEntry($"{usernameTextBox.Text} logged in");
			}
			else {
				Logger.WriteEntry($"Log in fail with username {usernameTextBox.Text}");
			}
		}

		private void closeButton_Click(object sender, RoutedEventArgs e) {
			((MainWindow)Application.Current.MainWindow).Close();
		}

		private void settingsButton_Click(object sender, RoutedEventArgs e) {
			((MainWindow)Application.Current.MainWindow).DataContext = new SettingsView();
		}

		private void usernameTextBox_Loaded(object sender, RoutedEventArgs e) {
			usernameTextBox.Focus();
		}

		private void usernameTextBoxEnterPressed(object sender, KeyEventArgs e) {
			if (e.Key == Key.Return) {
				if(string.IsNullOrEmpty(passwordBox.Password))
					passwordBox.Focus();
				else
					loginButton_Click(null, null);
			}
		}

		private void passwordBoxEnterPressed(object sender, KeyEventArgs e) {
			if (e.Key == Key.Return) {
				if(string.IsNullOrEmpty(usernameTextBox.Text))
					usernameTextBox.Focus();
				else
					loginButton_Click(null, null);
			}
		}
	}
}
