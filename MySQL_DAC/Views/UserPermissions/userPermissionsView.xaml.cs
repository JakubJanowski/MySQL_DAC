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
				if (column.ColumnName.Equals("idpermissions") || column.ColumnName.Equals("giverId"))
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
				for (; i < usersTmpTable.Columns.Count - 1; i++) {
					newRow[i] = ((Permissions)(row[i])).ShortNotation();
					userPermissions[(string)row["user"]][usersTmpTable.Columns[i].ColumnName] = (Permissions)row[i];
				}
				newRow[i] = row[i];
				usersTable.Rows.Add(newRow);

				if (((string)row["user"]).Equals(username))
					userId = (int)row["idpermissions"];
			}

			usersDataGrid.ItemsSource = usersTable.DefaultView;
		}

		internal Dictionary<string, Permissions> GetPermissions(string username) {
			return userPermissions[username];
		}

		internal void Prepare(string username) {
			bool canEditUserPermissions = false;
			if (!mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.CreateUser))
				addUserButton.IsEnabled = false;
			if (!mainView.thisUserPermissions["userPermissions"].HasFlag(Permissions.ViewPermissions)) {
				usersDataGrid.Visibility = Visibility.Collapsed;
				editUserButton.Visibility = Visibility.Collapsed;
				editInfoTextBlock.Visibility = Visibility.Collapsed;
			}

			foreach (var p in userPermissions[username]) {
				if ((p.Value & Permissions.DelegateAllNormal) > 0) {
					canEditUserPermissions = true;
					break;
				}
			}

			if (!canEditUserPermissions) {
				editUserButton.Visibility = Visibility.Collapsed;
				editInfoTextBlock.Visibility = Visibility.Collapsed;
			}

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
			for (; i < usersDataGrid.Columns.Count - 1; i++) {
				usersDataGrid.Columns[i].CellStyle = style2;
				if (usersDataGrid.Columns[i].ActualWidth > 150)
					usersDataGrid.Columns[i].Width = 150;
			}
			if (i < usersDataGrid.Columns.Count) {
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
			mainView.editPermissionsView = new EditPermissionsView(mainView, selectedUserName, userPermissions[selectedUserName], userId);
			mainView.DataContext = mainView.editPermissionsView;
		}
		
		private void refreshButton_Click(object sender, RoutedEventArgs e) {
			Refresh();
		}

		private void usersDataGrid_Loaded(object sender, RoutedEventArgs e) {
			setStyle();
		}

		private void usersDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
			if (usersDataGrid.SelectedItem == null)
				return;
			int? giverId;
			if (((DataRowView)usersDataGrid.SelectedItem)["giverId"] == DBNull.Value)
				giverId = null;
			else
				giverId = (int)((DataRowView)usersDataGrid.SelectedItem)["giverId"];
			while (giverId != userId && giverId != null) {
				var row = usersTable.AsEnumerable().SingleOrDefault(r => r.Field<int>("idpermissions") == giverId);
				if (row["giverId"] == DBNull.Value)
					giverId = null;
				else
					giverId = (int)row["giverId"];
			}
			if (giverId == userId) {
				editInfoTextBlock.Visibility = Visibility.Collapsed;
				editUserButton.IsEnabled = true;
			} else if (giverId == null) {
				editInfoTextBlock.Text = "You do not have enough rights to edit this user";
				editInfoTextBlock.Visibility = Visibility.Visible;
				editUserButton.IsEnabled = false;
			}
		}
	}
}
