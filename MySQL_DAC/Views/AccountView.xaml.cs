using System;
using System.Windows;
using System.Windows.Controls;
using MySQL_DAC.Database;

namespace MySQL_DAC.Views {
	public partial class AccountView: UserControl {
		private MainView mainView;

		public AccountView() {
			InitializeComponent();
		}
		public AccountView(MainView mainView) {
			InitializeComponent();
			this.mainView = mainView;
			usernameTextBlock.Text = mainView.username;
		}

		private void changePasswordButton_Click(object sender, RoutedEventArgs e) {
			changePasswordButton.Visibility = Visibility.Collapsed;
			changePasswordStackPanel.Visibility = Visibility.Visible;
			successTextBox.Visibility = Visibility.Collapsed;
		}

		private void savePasswordButton_Click(object sender, RoutedEventArgs e) {
			successTextBox.Visibility = Visibility.Collapsed;
			if (!DatabaseManager.PasswordMatches(oldPasswordBox.Password)) {
				infoTextBox.Text = "Old password is incorrect";
				infoTextBox.Visibility = Visibility.Visible;
				return;
			}
			if (!newPasswordBox.Password.Equals(repeatedNewPasswordBox.Password)) {
				infoTextBox.Text = "New passwords do not match!";
				infoTextBox.Visibility = Visibility.Visible;
				return;
			}
			if (string.IsNullOrEmpty(newPasswordBox.Password) ||
				string.IsNullOrEmpty(repeatedNewPasswordBox.Password)) {
				infoTextBox.Text = "New password can't be empty!";
				infoTextBox.Visibility = Visibility.Visible;
				return;
			}
			DatabaseManager.SetPassword(newPasswordBox.Password);
			changePasswordButton.Visibility = Visibility.Visible;
			changePasswordStackPanel.Visibility = Visibility.Collapsed;
			successTextBox.Visibility = Visibility.Visible;
		}

		private void checkPasswords(object sender, RoutedEventArgs e) {
			if (string.IsNullOrEmpty(newPasswordBox.Password) ||
				string.IsNullOrEmpty(repeatedNewPasswordBox.Password) ||
				newPasswordBox.IsFocused == true ||
				repeatedNewPasswordBox.IsFocused == true) {
				infoTextBox.Visibility = Visibility.Collapsed;
			}
			else {
				if (!newPasswordBox.Password.Equals(repeatedNewPasswordBox.Password)) {
					infoTextBox.Text = "New passwords do not match!";
					infoTextBox.Visibility = Visibility.Visible;
				}
			}
		}

		private void cancelChangePasswordButton_Click(object sender, RoutedEventArgs e) {
			changePasswordButton.Visibility = Visibility.Visible;
			changePasswordStackPanel.Visibility = Visibility.Collapsed;
		}

		private void returnButton_Click(object sender, RoutedEventArgs e) {
			((MainWindow)Application.Current.MainWindow).DataContext = mainView;
		}

		private void deleteAccountButton_Click(object sender, RoutedEventArgs e) {
			var result = MessageBox.Show("Deleting account is irreversible. Are you sure you want to delete your account?", "Warning", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes) {
				DatabaseManager.DropUser(mainView.username);
				Logger.WriteEntry($"{mainView.username} dropped his account");
				DatabaseManager.Close();
				((MainWindow)Application.Current.MainWindow).DataContext = new LogInView();
			}
		}
	}
}
