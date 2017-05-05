using System.Windows;
using System.Windows.Controls;

namespace MySQL_DAC.Views.UserPermissions {
	public partial class EditPermissionsView: UserControl {
		private MainView mainView;

		public EditPermissionsView(MainView mainView) {
			InitializeComponent();
			this.mainView = mainView;
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e) {
			mainView.DataContext = mainView.userPermissionsView;
		}

		private void confirmButton_Click(object sender, RoutedEventArgs e) {
			//save
			mainView.DataContext = mainView.userPermissionsView;
		}
	}
}
