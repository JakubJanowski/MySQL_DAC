using System;
using System.Windows;
using System.Windows.Controls;
using MySQL_DAC.Database;

namespace MySQL_DAC.Views {
	public partial class SettingsView: UserControl {
		public string databaseName { get; set; }
		public string serverIP { get; set; }

		public SettingsView() {
			InitializeComponent();
			databaseName = Configure.DatabaseName;
			serverIP = Configure.ServerIP;
		}

		private void saveButton_Click(object sender, RoutedEventArgs e) {
			Configure.DatabaseName = databaseName;
			Configure.ServerIP = serverIP;
			((MainWindow)Application.Current.MainWindow).DataContext = new LogInView();
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e) {
			((MainWindow)Application.Current.MainWindow).DataContext = new LogInView();
		}
	}
}
