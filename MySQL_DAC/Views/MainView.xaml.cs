using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MySQL_DAC.Database;
using MySQL_DAC.Views.UserPermissions;

namespace MySQL_DAC.Views {
	public partial class MainView: UserControl {
		public UserPermissionsView userPermissionsView { get; set; }
		public AddUserView addUserView { get; set; }
		public EditPermissionsView editPermissionsView { get; set; }
		public Dictionary<string, Permissions> thisUserPermissions { get; set; }

		public MainView() {
			InitializeComponent();
		}

		public MainView(string username) {
			InitializeComponent();
			databaseView.tableNamesComboBox.ItemsSource = DatabaseManager.GetTableNames();
			userPermissionsView = new UserPermissionsView(this);
			userPermissionsView.LoadUsers(username);
			thisUserPermissions = userPermissionsView.GetPermissions(username);
			DataContext = userPermissionsView;
			if (!thisUserPermissions["userPermissions"].HasFlag(Permissions.ViewPermissions) && !thisUserPermissions["userPermissions"].HasFlag(Permissions.CreateUser)) {
				permissionsTab = null;
				userPermissionsView = null;
			}
			else
				userPermissionsView.Prepare(username);
			loginTextBlock.Text = "Logged in as ";
			loginTextBlock.Inlines.Add(new Bold(new Run(username)));
		}

		// log out
		private void Button_Click(object sender, RoutedEventArgs e) {
			DatabaseManager.Close();
			((MainWindow)Application.Current.MainWindow).DataContext = new LogInView();
		}
	}
}
