using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySQL_DAC.Database;

namespace MySQL_DAC.Views.UserPermissions {
	public partial class EditPermissionsView: UserControl {
		private MainView mainView;
		private Dictionary<string, Permissions> userPermissions = new Dictionary<string, Permissions>();
		private Dictionary<string, Permissions> previousPermissions = new Dictionary<string, Permissions>();
		private Dictionary<string, Permissions> permissionsBeforeRelinquishCheck = new Dictionary<string, Permissions>();
		private string tableName;
		private int userId;		// this user
		private bool willRelinquish;
		private bool tookOver;

		public EditPermissionsView(MainView mainView, string username, Dictionary<string, Permissions> userPermissions, int userId) {
			InitializeComponent();
			this.mainView = mainView;
			this.userId = userId;
			this.userPermissions = userPermissions;
			this.previousPermissions = new Dictionary<string, Permissions>(userPermissions);
			usernameTextBlock.Text = username;
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
			if (mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.DelegateCanTakeOver)) {
				canTakeOverCheckBox.IsEnabled = true;
			}
			else {
				canTakeOverCheckBox.IsEnabled = false;
			}

			createUsersPermissionCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.CreateUser);
			delegateCreateUsersPermissionCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.DelegateCreateUser);
			deleteUserPermissionCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.DeleteUser);
			delegateDeleteUserPermissionCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.DelegateDeleteUser);
			viewUserPermissionsCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.ViewPermissions);
			delegateViewUserPermissionsCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.DelegateViewPermissions);
			canTakeOverCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.CanTakeOver);

			/*tookOver = mainView.userPermissionsView.HasTakenOver(username);
			if (tookOver)
				relinquishCheckBox.IsEnabled = false;
			else */if (userPermissions["userPermissions"].HasFlag(Permissions.CanTakeOver))
				relinquishCheckBox.IsEnabled = true;
			else
				relinquishCheckBox.IsEnabled = false;

			if (mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.DeleteUser))
				deleteUserButton.IsEnabled = true;
			else
				deleteUserButton.IsEnabled = false;

			if (relinquishCheckBox.IsEnabled) {
				willRelinquish = true;
				relinquishCheckBox.IsChecked = true;
				foreach (var entry in mainView.thisUserPermissions)
					userPermissions[entry.Key] = entry.Value;

				userPermissions["userPermissions"] |= Permissions.CanTakeOver;

				createUsersPermissionCheckBox.IsEnabled = false;
				delegateCreateUsersPermissionCheckBox.IsEnabled = false;
				deleteUserPermissionCheckBox.IsEnabled = false;
				delegateDeleteUserPermissionCheckBox.IsEnabled = false;
				viewUserPermissionsCheckBox.IsEnabled = false;
				delegateViewUserPermissionsCheckBox.IsEnabled = false;
				addPermissionCheckBox.IsEnabled = false;
				delegateAddPermissionCheckBox.IsEnabled = false;
				deletePermissionCheckBox.IsEnabled = false;
				delegateDeletePermissionCheckBox.IsEnabled = false;
				editPermissionCheckBox.IsEnabled = false;
				delegateEditPermissionCheckBox.IsEnabled = false;
				viewPermissionCheckBox.IsEnabled = false;
				delegateViewPermissionCheckBox.IsEnabled = false;
			}
			else
				willRelinquish = false;
		}
		private void tableChosen(object sender, SelectionChangedEventArgs e) {
			tableName = ((ComboBox)sender).SelectedItem.ToString();

			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateAdd) && !willRelinquish) {
				addPermissionCheckBox.IsEnabled = true;
				delegateAddPermissionCheckBox.IsEnabled = true;
			}
			else {
				addPermissionCheckBox.IsEnabled = false;
				delegateAddPermissionCheckBox.IsEnabled = false;
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateDelete) && !willRelinquish) {
				deletePermissionCheckBox.IsEnabled = true;
				delegateDeletePermissionCheckBox.IsEnabled = true;
			}
			else {
				deletePermissionCheckBox.IsEnabled = false;
				delegateDeletePermissionCheckBox.IsEnabled = false;
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateEdit) && !willRelinquish) {
				editPermissionCheckBox.IsEnabled = true;
				delegateEditPermissionCheckBox.IsEnabled = true;
			}
			else {
				editPermissionCheckBox.IsEnabled = false;
				delegateEditPermissionCheckBox.IsEnabled = false;
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateView) && !willRelinquish) {
				viewPermissionCheckBox.IsEnabled = true;
				delegateViewPermissionCheckBox.IsEnabled = true;
			}
			else {
				viewPermissionCheckBox.IsEnabled = false;
				delegateViewPermissionCheckBox.IsEnabled = false;
			}

			checkAllButton.IsEnabled = true;
			uncheckAllButton.IsEnabled = true;

			addPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.Add);
			delegateAddPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateAdd);
			deletePermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.Delete);
			delegateDeletePermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateDelete);
			editPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.Edit);
			delegateEditPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateEdit);
			viewPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.View);
			delegateViewPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateView);

			permissionInfoTextBlock.Visibility = Visibility.Collapsed;
		}

		private void confirmUserButton_Click(object sender, RoutedEventArgs e) {
			userPermissions["userPermissions"] |= Permissions.UserPermissions;
			if (relinquishCheckBox.IsChecked == true && willRelinquish && userPermissions["userPermissions"].HasFlag(Permissions.CanTakeOver)) {
				RemoveAllPermissionsFromChildren(userId, usernameTextBlock.Text);
				DatabaseManager.UpdateUser(usernameTextBlock.Text, userPermissions, previousPermissions, mainView.userPermissionsView.GetAncestorId(mainView.username), mainView.username, userId, mainView.thisUserPermissions);
				Logger.WriteEntry($"{mainView.username} relinquished his privileges to {usernameTextBlock.Text}");
				mainView.Refresh();
			}
			else {
				int? ancestorId = mainView.userPermissionsView.GetAncestorId(usernameTextBlock.Text);
				if (becomeGiverCheckBox.IsChecked == true) {
					ancestorId = userId;
					Logger.WriteEntry($"{mainView.username} became direct ancestor of {usernameTextBlock.Text}");
				}
				Dictionary<string, Permissions> takenPermissions = new Dictionary<string, Permissions>();
				foreach (var entry in userPermissions) {
					if (entry.Key.Equals("userPermissions"))
						takenPermissions[entry.Key] = (Permissions.DelegateAllUser | Permissions.AllUser) & ~((previousPermissions[entry.Key] ^ userPermissions[entry.Key]) & previousPermissions[entry.Key]);
					else
						takenPermissions[entry.Key] = (Permissions.DelegateAllNormal | Permissions.AllNormal) & ~((previousPermissions[entry.Key] ^ userPermissions[entry.Key]) & previousPermissions[entry.Key]);
				}
				RemovePermissionsFromChildren(mainView.userPermissionsView.GetId(usernameTextBlock.Text), takenPermissions);
				DatabaseManager.UpdateUser(usernameTextBlock.Text, userPermissions, previousPermissions, ancestorId);
			}
			Logger.WriteEntry($"{mainView.username} edited user {usernameTextBlock.Text} and left him with permissions [{userPermissions.Present()}]");
			mainView.userPermissionsView.Refresh();
			mainView.DataContext = mainView.userPermissionsView;
		}

		private void RemoveAllPermissionsFromChildren(int ancestorId, string ignoreUser) {
			foreach (string username in mainView.userPermissionsView.GetUsersWithAncestorId(ancestorId)) {
				RemoveAllPermissionsFromChildren(mainView.userPermissionsView.GetId(username), ignoreUser);
				if (!username.Equals(ignoreUser)) {
					DatabaseManager.RemoveAllPrivileges(username, mainView.thisUserPermissions, mainView.userPermissionsView.GetPermissions(username));
					Logger.WriteEntry($"removed all privileges from {username}");
				}
			}
		}

		private void RemovePermissionsFromChildren(int ancestorId, Dictionary<string, Permissions> newPermissions) {
			foreach (string username in mainView.userPermissionsView.GetUsersWithAncestorId(ancestorId)) {
				RemovePermissionsFromChildren(mainView.userPermissionsView.GetId(username), newPermissions);
				DatabaseManager.RemovePrivileges(username, newPermissions, mainView.userPermissionsView.GetPermissions(username));
				Logger.WriteEntry($"removed privileges from {username}");
			}
		}

		private void cancelUserButton_Click(object sender, RoutedEventArgs e) {
			mainView.DataContext = mainView.userPermissionsView;
		}

		private void relinquishCheckBox_Checked(object sender, RoutedEventArgs e) {
			if (willRelinquish) {
				warnTextBox.Text = "You will lose all permissions!";
				warnTextBox.Visibility = Visibility.Visible;
				becomeGiverCheckBox.IsEnabled = false;
				becomeGiverCheckBox.IsChecked = false;
			}

			foreach (var entry in userPermissions)
				permissionsBeforeRelinquishCheck[entry.Key] = entry.Value;

			foreach (var entry in mainView.thisUserPermissions)
				userPermissions[entry.Key] = entry.Value;

			if (tableName != null) {
				addPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.Add);
				delegateAddPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateAdd);
				deletePermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.Delete);
				delegateDeletePermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateDelete);
				editPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.Edit);
				delegateEditPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateEdit);
				viewPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.View);
				delegateViewPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateView);
			}

			createUsersPermissionCheckBox.IsEnabled = false;
			delegateCreateUsersPermissionCheckBox.IsEnabled = false;
			deleteUserPermissionCheckBox.IsEnabled = false;
			delegateDeleteUserPermissionCheckBox.IsEnabled = false;
			viewUserPermissionsCheckBox.IsEnabled = false;
			delegateViewUserPermissionsCheckBox.IsEnabled = false;
			addPermissionCheckBox.IsEnabled = false;
			delegateAddPermissionCheckBox.IsEnabled = false;
			deletePermissionCheckBox.IsEnabled = false;
			delegateDeletePermissionCheckBox.IsEnabled = false;
			editPermissionCheckBox.IsEnabled = false;
			delegateEditPermissionCheckBox.IsEnabled = false;
			viewPermissionCheckBox.IsEnabled = false;
			delegateViewPermissionCheckBox.IsEnabled = false;
		}

		private void relinquishCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			warnTextBox.Visibility = Visibility.Collapsed;
			if (!willRelinquish)
				becomeGiverCheckBox.IsEnabled = true;

			foreach (var entry in permissionsBeforeRelinquishCheck)
				userPermissions[entry.Key] = entry.Value;

			if (tableName != null) {
				addPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.Add);
				delegateAddPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateAdd);
				deletePermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.Delete);
				delegateDeletePermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateDelete);
				editPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.Edit);
				delegateEditPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateEdit);
				viewPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.View);
				delegateViewPermissionCheckBox.IsChecked = userPermissions[tableName].HasFlag(Permissions.DelegateView);
			}

			createUsersPermissionCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.CreateUser);
			delegateCreateUsersPermissionCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.DelegateCreateUser);
			deleteUserPermissionCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.DeleteUser);
			delegateDeleteUserPermissionCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.DelegateDeleteUser);
			viewUserPermissionsCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.ViewPermissions);
			delegateViewUserPermissionsCheckBox.IsChecked = userPermissions["userPermissions"].HasFlag(Permissions.DelegateViewPermissions);
		}
		private void deleteUserButton_Click(object sender, RoutedEventArgs e) {
			var result = MessageBox.Show("Deleting account is irreversible. Are you sure you want to delete this user's account?", "Warning", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes) {
				RemoveAllPermissionsFromChildren(mainView.userPermissionsView.GetId(usernameTextBlock.Text), usernameTextBlock.Text);
				DatabaseManager.DropUser(usernameTextBlock.Text);
				Logger.WriteEntry($"{mainView.username} dropped user {usernameTextBlock.Text}");
				mainView.userPermissionsView.Refresh();
				mainView.DataContext = mainView.userPermissionsView;
			}
		}

		#region checkbox_toggling
		#region table_permissions
		private void addPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.Add;
		}

		private void delegateAddPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.Add | Permissions.DelegateAdd;
			addPermissionCheckBox.IsChecked = true;
		}

		private void deletePermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.Delete | Permissions.View;
			if (viewPermissionCheckBox.IsChecked == false)
				permissionInfoTextBlock.Visibility = Visibility.Visible;
			viewPermissionCheckBox.IsChecked = true;
		}

		private void delegateDeletePermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.Delete | Permissions.DelegateDelete | Permissions.View | Permissions.DelegateView;
			deletePermissionCheckBox.IsChecked = true;
			if (viewPermissionCheckBox.IsChecked == false || delegateViewPermissionCheckBox.IsChecked == false)
				permissionInfoTextBlock.Visibility = Visibility.Visible;
			viewPermissionCheckBox.IsChecked = true;
			delegateViewPermissionCheckBox.IsChecked = true;
		}

		private void editPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.Edit | Permissions.View;
			if (viewPermissionCheckBox.IsChecked == false)
				permissionInfoTextBlock.Visibility = Visibility.Visible;
			viewPermissionCheckBox.IsChecked = true;
		}

		private void delegateEditPermissionCheckBox_Checked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] |= Permissions.Edit | Permissions.DelegateEdit | Permissions.View | Permissions.DelegateView;
			editPermissionCheckBox.IsChecked = true;
			if (viewPermissionCheckBox.IsChecked == false || delegateViewPermissionCheckBox.IsChecked == false)
				permissionInfoTextBlock.Visibility = Visibility.Visible;
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
			if (deletePermissionCheckBox.IsChecked == true || editPermissionCheckBox.IsChecked == true)
				permissionInfoTextBlock.Visibility = Visibility.Visible;
			deletePermissionCheckBox.IsChecked = false;
			delegateDeletePermissionCheckBox.IsChecked = false;
			editPermissionCheckBox.IsChecked = false;
			delegateEditPermissionCheckBox.IsChecked = false;
		}

		private void delegateViewPermissionCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			userPermissions[tableName] &= ~(Permissions.DelegateView | Permissions.DelegateDelete | Permissions.DelegateEdit);
			if (delegateDeletePermissionCheckBox.IsChecked == true || delegateEditPermissionCheckBox.IsChecked == true)
				permissionInfoTextBlock.Visibility = Visibility.Visible;
			delegateDeletePermissionCheckBox.IsChecked = false;
			delegateEditPermissionCheckBox.IsChecked = false;
		}


		private void checkAllButton_Click(object sender, RoutedEventArgs e) {
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateView)) {
				if (viewPermissionCheckBox.IsEnabled) {
					userPermissions[tableName] |= Permissions.View;
					viewPermissionCheckBox.IsChecked = true;
				}
				if (delegateViewPermissionCheckBox.IsEnabled) {
					userPermissions[tableName] |= Permissions.DelegateView;
					delegateViewPermissionCheckBox.IsChecked = true;
				}
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateAdd)) {
				if (addPermissionCheckBox.IsEnabled) {
					userPermissions[tableName] |= Permissions.Add;
					addPermissionCheckBox.IsChecked = true;
				}
				if (delegateAddPermissionCheckBox.IsEnabled) {
					userPermissions[tableName] |= Permissions.DelegateAdd;
					delegateAddPermissionCheckBox.IsChecked = true;
				}
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateDelete)) {
				if (deletePermissionCheckBox.IsEnabled) {
					userPermissions[tableName] |= Permissions.Delete;
					deletePermissionCheckBox.IsChecked = true;
				}
				if (delegateDeletePermissionCheckBox.IsEnabled) {
					userPermissions[tableName] |= Permissions.DelegateDelete;
					delegateDeletePermissionCheckBox.IsChecked = true;
				}
			}
			if (mainView.thisUserPermissions[tableName].HasFlag(Permissions.DelegateEdit)) {
				if (editPermissionCheckBox.IsEnabled) {
					userPermissions[tableName] |= Permissions.Edit;
					editPermissionCheckBox.IsChecked = true;
				}
				if (delegateEditPermissionCheckBox.IsEnabled) {
					userPermissions[tableName] |= Permissions.DelegateEdit;
					delegateEditPermissionCheckBox.IsChecked = true;
				}
			}
		}

		private void uncheckAllButton_Click(object sender, RoutedEventArgs e) {
			if (addPermissionCheckBox.IsEnabled) {
				userPermissions[tableName] &= ~Permissions.Add;
				addPermissionCheckBox.IsChecked = false;
			}
			if (delegateAddPermissionCheckBox.IsEnabled) {
				userPermissions[tableName] &= ~Permissions.DelegateAdd;
				delegateAddPermissionCheckBox.IsChecked = false;
			}
			if (deletePermissionCheckBox.IsEnabled) {
				userPermissions[tableName] &= ~Permissions.Delete;
				deletePermissionCheckBox.IsChecked = false;
			}
			if (delegateDeletePermissionCheckBox.IsEnabled) {
				userPermissions[tableName] &= ~Permissions.DelegateDelete;
				delegateDeletePermissionCheckBox.IsChecked = false;
			}
			if (editPermissionCheckBox.IsEnabled) {
				userPermissions[tableName] &= ~Permissions.Edit;
				editPermissionCheckBox.IsChecked = false;
			}
			if (delegateEditPermissionCheckBox.IsEnabled) {
				userPermissions[tableName] &= ~Permissions.DelegateEdit;
				delegateEditPermissionCheckBox.IsChecked = false;
			}
			if (viewPermissionCheckBox.IsEnabled) {
				userPermissions[tableName] &= ~Permissions.View;
				viewPermissionCheckBox.IsChecked = false;
			}
			if (delegateViewPermissionCheckBox.IsEnabled) {
				userPermissions[tableName] &= ~Permissions.DelegateView;
				delegateViewPermissionCheckBox.IsChecked = false;
			}
		}
		#endregion

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

		private void canTakeOverCheckBox_Checked(object sender, RoutedEventArgs e) {
			if (willRelinquish) {
				relinquishCheckBox.IsEnabled = true;
				relinquishCheckBox.IsChecked = true;
			}
			userPermissions["userPermissions"] |= Permissions.CanTakeOver;
		}

		private void canTakeOverCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			relinquishCheckBox.IsEnabled = false;
			relinquishCheckBox.IsChecked = false;
			userPermissions["userPermissions"] &= ~(Permissions.CanTakeOver | Permissions.DelegateCanTakeOver);
		}


		private void userCheckAllButton_Click(object sender, RoutedEventArgs e) {
			if (mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.DelegateCreateUser)) {
				if (createUsersPermissionCheckBox.IsEnabled) {
					userPermissions["userPermissions"] |= Permissions.CreateUser;
					createUsersPermissionCheckBox.IsChecked = true;
				}
				if (delegateCreateUsersPermissionCheckBox.IsEnabled) {
					userPermissions["userPermissions"] |= Permissions.DelegateCreateUser;
					delegateCreateUsersPermissionCheckBox.IsChecked = true;
				}
			}
			if (mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.DelegateDeleteUser)) {
				if (deleteUserPermissionCheckBox.IsEnabled) {
					userPermissions["userPermissions"] |= Permissions.DeleteUser;
					deleteUserPermissionCheckBox.IsChecked = true;
				}
				if (delegateDeleteUserPermissionCheckBox.IsEnabled) {
					userPermissions["userPermissions"] |= Permissions.DelegateDeleteUser;
					delegateDeleteUserPermissionCheckBox.IsChecked = true;
				}
			}
			if (mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.DelegateViewPermissions)) {
				if (viewUserPermissionsCheckBox.IsEnabled) {
					userPermissions["userPermissions"] |= Permissions.ViewPermissions;
					viewUserPermissionsCheckBox.IsChecked = true;
				}
				if (delegateViewUserPermissionsCheckBox.IsEnabled) {
					userPermissions["userPermissions"] |= Permissions.DelegateViewPermissions;
					delegateViewUserPermissionsCheckBox.IsChecked = true;
				}
			}
			if (mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.DelegateCanTakeOver)) {
				if (canTakeOverCheckBox.IsEnabled) {
					userPermissions["userPermissions"] |= Permissions.CanTakeOver;
					canTakeOverCheckBox.IsChecked = true;
				}
			}
		}

		private void userUncheckAllButton_Click(object sender, RoutedEventArgs e) {
			if (createUsersPermissionCheckBox.IsEnabled) {
				userPermissions["userPermissions"] &= ~Permissions.CreateUser;
				createUsersPermissionCheckBox.IsChecked = false;
			}
			if (delegateCreateUsersPermissionCheckBox.IsEnabled) {
				userPermissions["userPermissions"] &= ~Permissions.DelegateCreateUser;
				delegateCreateUsersPermissionCheckBox.IsChecked = false;
			}
			if (deleteUserPermissionCheckBox.IsEnabled) {
				userPermissions["userPermissions"] &= ~Permissions.DeleteUser;
				deleteUserPermissionCheckBox.IsChecked = false;
			}
			if (delegateDeleteUserPermissionCheckBox.IsEnabled) {
				userPermissions["userPermissions"] &= ~Permissions.DelegateDeleteUser;
				delegateDeleteUserPermissionCheckBox.IsChecked = false;
			}
			if (viewUserPermissionsCheckBox.IsEnabled) {
				userPermissions["userPermissions"] &= ~Permissions.ViewPermissions;
				viewUserPermissionsCheckBox.IsChecked = false;
			}
			if (delegateViewUserPermissionsCheckBox.IsEnabled) {
				userPermissions["userPermissions"] &= ~Permissions.DelegateViewPermissions;
				delegateViewUserPermissionsCheckBox.IsChecked = false;
			}
			if (canTakeOverCheckBox.IsEnabled) {
				userPermissions["userPermissions"] &= ~Permissions.CanTakeOver;
				canTakeOverCheckBox.IsChecked = false;
			}
		}
		#endregion

		#endregion

	}
}
