using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySQL_DAC.Database;

namespace MySQL_DAC.Views.UserPermissions {
	public partial class AddUserView: UserControl {
		private MainView mainView;
		private Dictionary<string, Permissions> userPermissions = new Dictionary<string, Permissions>();
		private string tableName;
		private int userId;

		public AddUserView(MainView mainView, int userId) {
			InitializeComponent();
			this.mainView = mainView;
			this.userId = userId;
			tableNameComboBox.ItemsSource = DatabaseManager.GetTableNames();
			if (mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.DelegateCreateUser)) {
				createUsersPermissionCheckBox.IsEnabled = true;
				delegateCreateUsersPermissionCheckBox.IsEnabled = true;
			}
			if (mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.DelegateDeleteUser)) {
				deleteUserPermissionCheckBox.IsEnabled = true;
				delegateDeleteUserPermissionCheckBox.IsEnabled = true;
			}
			if (mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.DelegateViewPermissions)) {
				viewUserPermissionsCheckBox.IsEnabled = true;
				delegateViewUserPermissionsCheckBox.IsEnabled = true;
			}
			userPermissions.Add("userPermissions", Permissions.UserPermissions);
		}

		private void tableChosen(object sender, SelectionChangedEventArgs e) {
			tableName = ((ComboBox)sender).SelectedItem.ToString();

			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateAdd)) {
				addPermissionCheckBox.IsEnabled = true;
				delegateAddPermissionCheckBox.IsEnabled = true;
			}
			else {
				addPermissionCheckBox.IsEnabled = false;
				delegateAddPermissionCheckBox.IsEnabled = false;
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateDelete)) {
				deletePermissionCheckBox.IsEnabled = true;
				delegateDeletePermissionCheckBox.IsEnabled = true;
			}
			else {
				deletePermissionCheckBox.IsEnabled = false;
				delegateDeletePermissionCheckBox.IsEnabled = false;
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateEdit)) {
				editPermissionCheckBox.IsEnabled = true;
				delegateEditPermissionCheckBox.IsEnabled = true;
			}
			else {
				editPermissionCheckBox.IsEnabled = false;
				delegateEditPermissionCheckBox.IsEnabled = false;
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateView)) {
				viewPermissionCheckBox.IsEnabled = true;
				delegateViewPermissionCheckBox.IsEnabled = true;
			}
			else {
				viewPermissionCheckBox.IsEnabled = false;
				delegateViewPermissionCheckBox.IsEnabled = false;
			}

			checkAllButton.IsEnabled = true;
			uncheckAllButton.IsEnabled = true;
		
			if (!userPermissions.ContainsKey(tableName)) 
				userPermissions.Add(tableName, Permissions.None);

			addPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.Add);
			delegateAddPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateAdd);
			deletePermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.Delete);
			delegateDeletePermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateDelete);
			editPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.Edit);
			delegateEditPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateEdit);
			viewPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.View);
			delegateViewPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateView);
		}

		private void addUserButton_Click(object sender, RoutedEventArgs e) {
			if (!passwordBox.Password.Equals(repeatedPasswordBox.Password)) {
				infoTextBox.Text = "Passwords do not match!";
				infoTextBox.Visibility = Visibility.Visible;
				return;
			}
			if (string.IsNullOrEmpty(passwordBox.Password) ||
				string.IsNullOrEmpty(repeatedPasswordBox.Password)) {
				infoTextBox.Text = "Password can't be empty!";
				infoTextBox.Visibility = Visibility.Visible;
				return;
			}
			if(DatabaseManager.GetUsernames().Contains(usernameTextBox.Text)) {
				usernameInfoTextBox.Text = "Username is already taken!";
				usernameInfoTextBox.Visibility = Visibility.Visible;
				return;
			}
			DatabaseManager.AddUser(usernameTextBox.Text, passwordBox.Password, userPermissions, userId);

			mainView.userPermissionsView.Refresh();
			mainView.DataContext = mainView.userPermissionsView;
		}

		private void cancelUserButton_Click(object sender, RoutedEventArgs e) {
			mainView.DataContext = mainView.userPermissionsView;
		}

		private void hideUsernameInfo(object sender, RoutedEventArgs e) {
			usernameInfoTextBox.Visibility = Visibility.Collapsed;
		}

		private void checkPasswords(object sender, RoutedEventArgs e) {
			if (string.IsNullOrEmpty(passwordBox.Password) ||
				string.IsNullOrEmpty(repeatedPasswordBox.Password) ||
				passwordBox.IsFocused == true ||
				repeatedPasswordBox.IsFocused == true) {
				infoTextBox.Visibility = Visibility.Collapsed;
			}
			else {
				if (!passwordBox.Password.Equals(repeatedPasswordBox.Password)) {
					infoTextBox.Text = "Passwords do not match!";
					infoTextBox.Visibility = Visibility.Visible;
				}
			}
		}


		#region checkbox_toggling
		private void addPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.Add;
		}

		private void delegateAddPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.Add | Permissions.DelegateAdd;
			addPermissionCheckBox.IsChecked = true;
		}

		private void deletePermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.Delete | Permissions.View;
			viewPermissionCheckBox.IsChecked = true;
		}

		private void delegateDeletePermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.Delete | Permissions.DelegateDelete | Permissions.View | Permissions.DelegateView;
			deletePermissionCheckBox.IsChecked = true;
			viewPermissionCheckBox.IsChecked = true;
			delegateViewPermissionCheckBox.IsChecked = true;
		}

		private void editPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.Edit | Permissions.View;
			viewPermissionCheckBox.IsChecked = true;
		}

		private void delegateEditPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.Edit | Permissions.DelegateEdit | Permissions.View | Permissions.DelegateView;
			editPermissionCheckBox.IsChecked = true;
			viewPermissionCheckBox.IsChecked = true;
			delegateViewPermissionCheckBox.IsChecked = true;
		}

		private void viewPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.View;
		}

		private void delegateViewPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.View | Permissions.DelegateView;
			viewPermissionCheckBox.IsChecked = true;
		}

		private void addPermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] &= ~(Permissions.Add | Permissions.DelegateAdd);
			delegateAddPermissionCheckBox.IsChecked = false;
		}

		private void delegateAddPermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] &= ~Permissions.DelegateAdd;
		}

		private void deletePermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] &= ~Permissions.Delete;
			userPermissions[tableName] &= ~Permissions.DelegateDelete;
			delegateDeletePermissionCheckBox.IsChecked = false;
		}

		private void delegateDeletePermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] &= ~Permissions.DelegateDelete;
		}

		private void editPermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] &= ~Permissions.Edit;
			userPermissions[tableName] &= ~Permissions.DelegateEdit;
			delegateEditPermissionCheckBox.IsChecked = false;
		}

		private void delegateEditPermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] &= ~Permissions.DelegateEdit;
		}

		private void viewPermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] &= ~(Permissions.View | Permissions.DelegateView | Permissions.Delete | Permissions.DelegateDelete | Permissions.Edit | Permissions.DelegateEdit);
			delegateViewPermissionCheckBox.IsChecked = false;
			deletePermissionCheckBox.IsChecked = false;
			delegateDeletePermissionCheckBox.IsChecked = false;
			editPermissionCheckBox.IsChecked = false;
			delegateEditPermissionCheckBox.IsChecked = false;
		}

		private void delegateViewPermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] &= ~(Permissions.DelegateView | Permissions.DelegateDelete | Permissions.DelegateEdit);
			delegateDeletePermissionCheckBox.IsChecked = false;
			delegateEditPermissionCheckBox.IsChecked = false;
		}


		private void checkAllButton_Click(object sender, RoutedEventArgs e) {
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateAdd)) {
				userPermissions[tableName] |= Permissions.Add;
				addPermissionCheckBox.IsChecked = true;
				userPermissions[tableName] |= Permissions.DelegateAdd;
				delegateAddPermissionCheckBox.IsChecked = true;
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateDelete)) {
				userPermissions[tableName] |= Permissions.Delete;
				deletePermissionCheckBox.IsChecked = true;
				userPermissions[tableName] |= Permissions.DelegateDelete;
				delegateDeletePermissionCheckBox.IsChecked = true;
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateEdit)) {
				userPermissions[tableName] |= Permissions.Edit;
				editPermissionCheckBox.IsChecked = true;
				userPermissions[tableName] |= Permissions.DelegateEdit;
				delegateEditPermissionCheckBox.IsChecked = true;
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateView)) {
				userPermissions[tableName] |= Permissions.View;
				viewPermissionCheckBox.IsChecked = true;
				userPermissions[tableName] |= Permissions.DelegateView;
				delegateViewPermissionCheckBox.IsChecked = true;
			}
		}

		private void uncheckAllButton_Click(object sender, RoutedEventArgs e) {
			userPermissions[tableName] &= ~Permissions.Add;
			addPermissionCheckBox.IsChecked = false;
			userPermissions[tableName] &= ~Permissions.DelegateAdd;
			delegateAddPermissionCheckBox.IsChecked = false;
			userPermissions[tableName] &= ~Permissions.Delete;
			deletePermissionCheckBox.IsChecked = false;
			userPermissions[tableName] &= ~Permissions.DelegateDelete;
			delegateDeletePermissionCheckBox.IsChecked = false;
			userPermissions[tableName] &= ~Permissions.Edit;
			editPermissionCheckBox.IsChecked = false;
			userPermissions[tableName] &= ~Permissions.DelegateEdit;
			delegateEditPermissionCheckBox.IsChecked = false;
			userPermissions[tableName] &= ~Permissions.View;
			viewPermissionCheckBox.IsChecked = false;
			userPermissions[tableName] &= ~Permissions.DelegateView;
			delegateViewPermissionCheckBox.IsChecked = false;
		}

		#region userPermissions
		private void createUsersPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions["userPermissions"] |= Permissions.CreateUser;
		}

		private void delegateCreateUsersPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			createUsersPermissionCheckBox.IsChecked = true;
			userPermissions["userPermissions"] |= Permissions.CreateUser | Permissions.DelegateCreateUser;
		}
		private void deleteUserPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions["userPermissions"] |= Permissions.DeleteUser;
		}

		private void delegateDeleteUserPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			deleteUserPermissionCheckBox.IsChecked = true;
			userPermissions["userPermissions"] |= Permissions.DeleteUser | Permissions.DelegateDeleteUser;
		}
		
		private void viewUserPermissionsCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions["userPermissions"] |= Permissions.ViewPermissions;
		}

		private void delegateViewUserPermissionsCheckBox_Checked(object sender, RoutedEventArgs e) {
			viewUserPermissionsCheckBox.IsChecked = true;
			userPermissions["userPermissions"] |= Permissions.ViewPermissions | Permissions.DelegateViewPermissions;
		}

		private void createUsersPermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			delegateCreateUsersPermissionCheckBox.IsChecked = false;
			userPermissions["userPermissions"] &= ~(Permissions.CreateUser | Permissions.DelegateCreateUser);
		}

		private void delegateCreateUsersPermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			userPermissions["userPermissions"] &= ~Permissions.DelegateCreateUser;
		}

		private void deleteUserPermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			delegateDeleteUserPermissionCheckBox.IsChecked = false;
			userPermissions["userPermissions"] &= ~(Permissions.DeleteUser | Permissions.DelegateDeleteUser);
		}

		private void delegateDeleteUserPermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			userPermissions["userPermissions"] &= ~Permissions.DelegateDeleteUser;
		}

		private void viewUserPermissionsCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			delegateViewUserPermissionsCheckBox.IsChecked = false;
			userPermissions["userPermissions"] &= ~(Permissions.ViewPermissions | Permissions.DelegateViewPermissions);
		}

		private void delegateViewUserPermissionsCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			userPermissions["userPermissions"] &= ~Permissions.DelegateViewPermissions;
		}


		private void userCheckAllButton_Click(object sender, RoutedEventArgs e) {
			if (mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.DelegateCreateUser)) {
				userPermissions["userPermissions"] |= Permissions.CreateUser;
				createUsersPermissionCheckBox.IsChecked = true;
				userPermissions["userPermissions"] |= Permissions.DelegateCreateUser;
				delegateCreateUsersPermissionCheckBox.IsChecked = true;
			}
			if (mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.DelegateDeleteUser)) {
				userPermissions["userPermissions"] |= Permissions.DeleteUser;
				deleteUserPermissionCheckBox.IsChecked = true;
				userPermissions["userPermissions"] |= Permissions.DelegateDeleteUser;
				delegateDeleteUserPermissionCheckBox.IsChecked = true;
			}
			if (mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.DelegateViewPermissions)) {
				userPermissions["userPermissions"] |= Permissions.ViewPermissions;
				viewUserPermissionsCheckBox.IsChecked = true;
				userPermissions["userPermissions"] |= Permissions.DelegateViewPermissions;
				delegateViewUserPermissionsCheckBox.IsChecked = true;
			}
		}

		private void userUncheckAllButton_Click(object sender, RoutedEventArgs e) {
			userPermissions["userPermissions"] &= ~Permissions.CreateUser;
			createUsersPermissionCheckBox.IsChecked = false;
			userPermissions["userPermissions"] &= ~Permissions.DelegateCreateUser;
			delegateCreateUsersPermissionCheckBox.IsChecked = false;
			userPermissions["userPermissions"] &= ~Permissions.DeleteUser;
			deleteUserPermissionCheckBox.IsChecked = false;
			userPermissions["userPermissions"] &= ~Permissions.DelegateDeleteUser;
			delegateDeleteUserPermissionCheckBox.IsChecked = false;
			userPermissions["userPermissions"] &= ~Permissions.ViewPermissions;
			viewUserPermissionsCheckBox.IsChecked = false;
			userPermissions["userPermissions"] &= ~Permissions.DelegateViewPermissions;
			delegateViewUserPermissionsCheckBox.IsChecked = false;
		}
		#endregion
		#endregion
	}
}
