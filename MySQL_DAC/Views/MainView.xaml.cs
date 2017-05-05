using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MySQL_DAC.Database;
using MySQL_DAC.Views.UserPermissions;

namespace MySQL_DAC.Views {
	public partial class MainView: UserControl {
		public UserPermissionsView userPermissionsView;
		public AddUserView addUserView;
		public EditPermissionsView editPermissionsView;

		public MainView() {
			InitializeComponent();
		}

		public MainView(string username) {
			InitializeComponent();
			databaseView.tableNamesComboBox.ItemsSource = DatabaseManager.GetTableNames();
			addUserView = new AddUserView(this);
			editPermissionsView = new EditPermissionsView(this);
			userPermissionsView = new UserPermissionsView(this);
			userPermissionsView.LoadUsers();
			DataContext = userPermissionsView;
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
