using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MySQL_DAC.Database;

namespace MySQL_DAC.Views.UserPermissions {
	public partial class UserPermissionsView: UserControl {
		private DataTable usersTable;
		private MainView mainView;
		private Dictionary<string, Dictionary<string, Permissions>> userPermissions;
		private int userId;
		private string username;

		public UserPermissionsView(MainView mainView) {
			InitializeComponent();
			this.mainView = mainView;
		}

		internal void LoadUsers(string username) {
			DataTable usersTmpTable = new DataTable();
			usersTable = new DataTable();
			DatabaseManager.GetUsers(ref usersTmpTable);
			userPermissions = new Dictionary<string, Dictionary<string, Permissions>>(usersTmpTable.Rows.Count);
			this.username = username;

			foreach (DataColumn column in usersTmpTable.Columns) {
				if (column.ColumnName.Equals("id") || column.ColumnName.Equals("ancestorId"))
					usersTable.Columns.Add(column.ColumnName, typeof(int));
				else
					usersTable.Columns.Add(column.ColumnName, typeof(string));
			}

			foreach (DataRow row in usersTmpTable.Rows) {
				int i = 0;
				DataRow newRow = usersTable.NewRow();
				userPermissions.Add((string)row["user"], new Dictionary<string, Permissions>());
				for (; i < 2; i++)
					newRow[i] = row[i];
				for (; i < usersTmpTable.Columns.Count - 2; i++) {
					newRow[i] = ((Permissions)(row[i])).ShortNotation();
					userPermissions[(string)row["user"]][usersTmpTable.Columns[i].ColumnName] = (Permissions)row[i];
				}
				newRow[i] = row[i];
				newRow[i+1] = row[i+1];
				usersTable.Rows.Add(newRow);

				if (((string)row["user"]).Equals(username))
					userId = (int)row["id"];
			}

			usersDataGrid.ItemsSource = usersTable.DefaultView;
		}

		internal Dictionary<string, Permissions> GetPermissions(string username) {
			return userPermissions[username];
		}

		internal bool HasTakenOver(string username) {
			var row = usersTable.AsEnumerable().SingleOrDefault(r => r.Field<string>("user") == username);
			return row["tookOverFrom"] != DBNull.Value;
		}
		internal int? GetAncestorId(string username) {
			var row = usersTable.AsEnumerable().SingleOrDefault(r => r.Field<string>("user") == username);
			if (row["ancestorId"] == DBNull.Value)
				return null;
			return (int)row["ancestorId"];
		}
		internal int GetId(string username) {
			var row = usersTable.AsEnumerable().SingleOrDefault(r => r.Field<string>("user") == username);
			return (int)row["id"];
		}

		internal List<string> GetUsersWithAncestorId(int ancestorId) {
			List<string> userList = new List<string>();
			var rows = usersTable.AsEnumerable().Where(r => r.Field<int?>("ancestorId") == ancestorId);
			foreach(var row in rows)
				userList.Add((string)row["user"]);
			return userList;
		}

		internal void Prepare(string username) {
			//bool canEditUserPermissions = false;
			if (!mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.CreateUser))
				addUserButton.IsEnabled = false;
			if (!mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.ViewPermissions)) {
				usersDataGrid.Visibility = Visibility.Collapsed;
				editUserButton.Visibility = Visibility.Collapsed;
				editInfoTextBlock.Visibility = Visibility.Collapsed;
			}

			/*foreach (var p in userPermissions[username]) {
				if ((p.Value & Permissions.DelegateAllNormal) > 0) {
					canEditUserPermissions = true;
					break;
				}
			}

			if (!canEditUserPermissions) {
				editUserButton.Visibility = Visibility.Collapsed;
				editInfoTextBlock.Visibility = Visibility.Collapsed;
			}*/

		}

		private void usersDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
			DataGridBoundColumn column = e.Column as DataGridBoundColumn;
			if (column != null) {
				Style elementStyle = new Style(typeof(TextBlock));
				elementStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.WrapWithOverflow));
				column.ElementStyle = elementStyle;
			}
		}

		private void setStyle() {
			int i = 0;
			Style style1 = new Style(typeof(DataGridCell));
			style1.Setters.Add(new Setter(BorderThicknessProperty, new Thickness(0)));
			Style style2 = new Style(typeof(DataGridCell));
			style2.Setters.Add(new Setter(FontFamilyProperty, new FontFamily("Consolas")));
			style2.Setters.Add(new Setter(BorderThicknessProperty, new Thickness(0)));

			for (; i < 2 && i < usersDataGrid.Columns.Count; i++) {
				usersDataGrid.Columns[i].CellStyle = style1;
				if (usersDataGrid.Columns[i].ActualWidth > 150)
					usersDataGrid.Columns[i].Width = 150;
			}
			for (; i < usersDataGrid.Columns.Count - 2; i++) {
				usersDataGrid.Columns[i].CellStyle = style2;
				if (usersDataGrid.Columns[i].ActualWidth > 150)
					usersDataGrid.Columns[i].Width = 150;
			}
			for (; i < usersDataGrid.Columns.Count; i++) {
				usersDataGrid.Columns[i].CellStyle = style1;
				if (usersDataGrid.Columns[i].ActualWidth > 150)
					usersDataGrid.Columns[i].Width = 150;
			}
		}

		internal void Refresh() {
			LoadUsers(username);
			setStyle();
		}


		private void addUserButton_Click(object sender, RoutedEventArgs e) {
			mainView.addUserView = new AddUserView(mainView, userId);
			mainView.DataContext = mainView.addUserView;
		}

		private void editUserButton_Click(object sender, RoutedEventArgs e) {
			string selectedUserName = (string)((DataRowView)usersDataGrid.SelectedItem)["user"];
			Dictionary<string, Permissions> selectedUserPermissions = new Dictionary<string, Permissions>();
			foreach (var entry in userPermissions[selectedUserName])
				selectedUserPermissions[entry.Key] = entry.Value;

			mainView.editPermissionsView = new EditPermissionsView(mainView, selectedUserName, selectedUserPermissions, userId);
			mainView.DataContext = mainView.editPermissionsView;
		}
		
		private void refreshButton_Click(object sender, RoutedEventArgs e) {
			Refresh();
		}

		private void usersDataGrid_Loaded(object sender, RoutedEventArgs e) {
			setStyle();
		}

		private void usersDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
			if (usersDataGrid.SelectedItem == null) {
				editInfoTextBlock.Text = "Select user from the table to edit.";
				editInfoTextBlock.Visibility = Visibility.Visible;
				editUserButton.IsEnabled = false;
				return;
			}
			int? ancestorId;
			if (((DataRowView)usersDataGrid.SelectedItem)["ancestorId"] == DBNull.Value)
				ancestorId = null;
			else
				ancestorId = (int)((DataRowView)usersDataGrid.SelectedItem)["ancestorId"];
			while (ancestorId != userId && ancestorId != null && ancestorId != -1) {
				var row = usersTable.AsEnumerable().SingleOrDefault(r => r.Field<int>("id") == ancestorId);
				if (row == null)
					ancestorId = -1;
				else if (row["ancestorId"] == DBNull.Value)
					ancestorId = null;
				else
					ancestorId = (int)row["ancestorId"];
			}
			if (ancestorId == userId || ancestorId == -1) {
				if (((DataRowView)usersDataGrid.SelectedItem)["user"].Equals(username)) {
					editInfoTextBlock.Text = "You do not have enough rights to edit this user";
					editInfoTextBlock.Visibility = Visibility.Visible;
					editUserButton.IsEnabled = false;
				}
				else {
					editInfoTextBlock.Visibility = Visibility.Collapsed;
					editUserButton.IsEnabled = true;
				}
			} else if (ancestorId == null) {
				editInfoTextBlock.Text = "You do not have enough rights to edit this user";
				editInfoTextBlock.Visibility = Visibility.Visible;
				editUserButton.IsEnabled = false;
			}
		}
	}
}
