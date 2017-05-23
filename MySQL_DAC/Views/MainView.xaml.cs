using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using MySQL_DAC.Database;
using MySQL_DAC.Views.UserPermissions;

namespace MySQL_DAC.Views {
	public partial class MainView: UserControl {
		public UserPermissionsView userPermissionsView { get; set; }
		public AddUserView addUserView { get; set; }
		public EditPermissionsView editPermissionsView { get; set; }
		public Dictionary<string, Permissions> thisUserPermissions { get; set; }
		private string username;

		public MainView() {
			InitializeComponent();
		}

		public MainView(string username) {
			this.username = username;
			InitializeComponent();
			Refresh();
			DatabaseManager.SetMainView(this);
			databaseView.SetMainView(this);
		}

		public void Refresh() {
			List<string> tableNames;
			tableNames = DatabaseManager.GetTableNames();
			databaseView.tableNamesComboBox.ItemsSource = tableNames;
			userPermissionsView = new UserPermissionsView(this);
			userPermissionsView.LoadUsers(username);
			thisUserPermissions = userPermissionsView.GetPermissions(username);
			DataContext = userPermissionsView;
			if (!thisUserPermissions["userPermissions"].HasFlag(Permissions.ViewPermissions) && !thisUserPermissions["userPermissions"].HasFlag(Permissions.CreateUser)) {
				DataContext = userPermissionsView = null;
				tabControl.Items.Remove(permissionsTab);
			}
			else
				userPermissionsView.Prepare(username);
			if (tableNames.Count == 0) {
				TextBlock info = new TextBlock();
				info.Text = "You don't have permissions to any table";
				info.HorizontalAlignment = HorizontalAlignment.Center;
				databaseView.Content = info;
				databaseView.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xE5, 0xE5, 0xE5));
			}
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
