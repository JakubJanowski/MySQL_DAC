using System;
using System.Windows;
using System.Windows.Controls;
using MySQL_DAC.Database;

namespace MySQL_DAC.Views {
	public partial class SettingsView: UserControl {
		public string databaseName { get; set; }
		public string port { get; set; }
		public string serverIP { get; set; }

		private LogInView logInView;

		public SettingsView() {
			InitializeComponent();
			databaseName = Configure.DatabaseName;
			port = Configure.Port;
			serverIP = Configure.ServerIP;
		}

		public SettingsView(LogInView logInView) : this() {
			this.logInView = logInView;
		}

		private void saveButton_Click(object sender, RoutedEventArgs e) {
			Configure.DatabaseName = databaseName;
			Configure.Port = port;
			Configure.ServerIP = serverIP;
			((MainWindow)Application.Current.MainWindow).DataContext = logInView;
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e) {
			((MainWindow)Application.Current.MainWindow).DataContext = logInView;
		}
	}
}
