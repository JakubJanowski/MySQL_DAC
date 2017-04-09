using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using MySQL_DAC.Database;

namespace MySQL_DAC {
	public partial class ManagementTab: UserControl {
		private DataTable usersTable = new DataTable();
		const int nOfPermissions = 43;
		static private Dictionary<string, Permissions> schemaPermissionDictionary = new Dictionary<string, Permissions>(nOfPermissions);
		static private Dictionary<string, Permissions> permissionDictionary = new Dictionary<string, Permissions>(nOfPermissions);
		
		static ManagementTab() {
			permissionDictionary.Add("Select_priv", Permissions.Select);
			permissionDictionary.Add("Insert_priv", Permissions.Insert);
			permissionDictionary.Add("Update_priv", Permissions.Update);
			permissionDictionary.Add("Delete_priv", Permissions.Delete);
			permissionDictionary.Add("Create_priv", Permissions.Create);
			permissionDictionary.Add("Drop_priv", Permissions.Drop);
			permissionDictionary.Add("Reload_priv", Permissions.Reload);
			permissionDictionary.Add("Shutdown_priv", Permissions.Shutdown);
			permissionDictionary.Add("Process_priv", Permissions.Process);
			permissionDictionary.Add("File_priv", Permissions.File);
			permissionDictionary.Add("Grant_priv", Permissions.GrantOption);
			permissionDictionary.Add("References_priv", Permissions.References);
			permissionDictionary.Add("Index_priv", Permissions.Index);
			permissionDictionary.Add("Alter_priv", Permissions.Alter);
			permissionDictionary.Add("Show_db_priv", Permissions.ShowDatabases);
			permissionDictionary.Add("Super_priv", Permissions.Super);
			permissionDictionary.Add("Create_tmp_table_priv", Permissions.CreateTemporaryTables);
			permissionDictionary.Add("Lock_tables_priv", Permissions.LockTables);
			permissionDictionary.Add("Execute_priv", Permissions.Execute);
			permissionDictionary.Add("Repl_slave_priv", Permissions.ReplicationSlave);
			permissionDictionary.Add("Repl_client_priv", Permissions.ReplicationClient);
			permissionDictionary.Add("Create_view_priv", Permissions.CreateView);
			permissionDictionary.Add("Show_view_priv", Permissions.ShowView);
			permissionDictionary.Add("Create_routine_priv", Permissions.CreateRoutine);
			permissionDictionary.Add("Alter_routine_priv", Permissions.AlterRoutine);
			permissionDictionary.Add("Create_user_priv", Permissions.CreateUser);
			permissionDictionary.Add("Event_priv", Permissions.Event);
			permissionDictionary.Add("Trigger_priv", Permissions.Trigger);
			permissionDictionary.Add("Create_tablespace_priv", Permissions.CreateTablespace);
			permissionDictionary.Add("ssl_type", Permissions.None);///
			permissionDictionary.Add("ssl_cipher", Permissions.None);///
			permissionDictionary.Add("x509_issuer", Permissions.None);///
			permissionDictionary.Add("x509_subject", Permissions.None);///
			permissionDictionary.Add("max_questions", Permissions.None);///
			permissionDictionary.Add("max_updates", Permissions.None);///
			permissionDictionary.Add("max_connections", Permissions.None);///
			permissionDictionary.Add("max_user_connections", Permissions.None);///
			permissionDictionary.Add("plugin", Permissions.None);///
			permissionDictionary.Add("authentication_string", Permissions.None);///
			permissionDictionary.Add("password_expired", Permissions.None);///
			permissionDictionary.Add("password_last_changed", Permissions.None);///
			permissionDictionary.Add("password_lifetime", Permissions.None);///
			permissionDictionary.Add("account_locked", Permissions.None);///
		}

		public ManagementTab() {
			InitializeComponent();
		}

		internal void LoadUsers() {
			DatabaseManager.GetUsers(ref usersTable);
			var tableNames = DatabaseManager.GetTableNames();
			Permissions permissions;
			foreach (var tableName in tableNames) {
				usersTable.Columns.Add(tableName, typeof(string));
				foreach (DataColumn dc in usersTable.Columns) {
					Debug.WriteLine(dc.ColumnName);
				}
				foreach(DataRow row in usersTable.Rows) {
					permissions = Permissions.None;
					foreach (KeyValuePair<string, Permissions> entry in permissionDictionary) {
						if (row[entry.Key].ToString().Equals("Y")) {
							permissions |= entry.Value;
						}
					}
					row[tableName] = permissions.ToString();
				}
			}
			usersDataGrid.ItemsSource = usersTable.DefaultView;
		}

		private void usersDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
			DataGridBoundColumn column = e.Column as DataGridBoundColumn;
			if (column != null) {
				Style elementStyle = new Style(typeof(TextBlock));
				elementStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.WrapWithOverflow));
				column.ElementStyle = elementStyle;
			}
		}

		private void usersDataGrid_Loaded(object sender, RoutedEventArgs e) {
			foreach (var column in usersDataGrid.Columns) {
				if (column.ActualWidth > 150)
					column.Width = 150;
			}
		}
	}
}
