using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace MySQL_DAC.Database {
	static class DatabaseManager {
		private const string databaseName = "test";
		static private MySqlConnection connection;
		private static Dictionary<string, MySqlDataAdapter> adapter;
		private static MySqlDataAdapter userAdapter;

		static public bool Connect(string username, string password) {
			connection = new MySqlConnection();
			connection.ConnectionString = "server=127.0.0.1;uid=" + username + ";pwd=" + password + ";database=" + databaseName + ";";

			try {
				connection.Open();
				using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '" + databaseName + "';", connection)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						reader.Read();
						adapter = new Dictionary<string, MySqlDataAdapter>(reader.GetInt32(0));
					}
				}
				return true;
			} catch (MySqlException ex) {
				switch (ex.Number) {
					case 0:
						MessageBox.Show(ex.Message);
						MessageBox.Show("Cannot connect to server. Contact administrator");
						break;
					case 1045:
						MessageBox.Show(ex.Message);
						MessageBox.Show("Invalid username/password, please try again");
						break;
					default:
						MessageBox.Show(ex.Message);
						break;
				}
				return false;
			}
		}

		static public void Close() {
			connection.Close();
		}

		internal static void GetTableContents(string tableName, ref DataSet dataSet) {
			if (!adapter.ContainsKey(tableName)) {
				adapter[tableName] = new MySqlDataAdapter("SELECT * FROM " + tableName, connection);
				adapter[tableName].MissingSchemaAction = MissingSchemaAction.AddWithKey;
				MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(adapter[tableName]);
				adapter[tableName].DeleteCommand = commandBuilder.GetDeleteCommand();
				adapter[tableName].InsertCommand = commandBuilder.GetInsertCommand();
				adapter[tableName].UpdateCommand = commandBuilder.GetUpdateCommand();
			}
			adapter[tableName].Fill(dataSet, tableName);
		}

		//internal static void GetUsers(ref DataTable usersTable) {
		//	if (userAdapter == null)
		//		userAdapter = new MySqlDataAdapter("SELECT * FROM mysql.user", connection);
		//	userAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
		//	userAdapter.Fill(usersTable);
		//}
		internal static void GetUsers(ref DataTable usersTable) {
			if (userAdapter == null)
				userAdapter = new MySqlDataAdapter("SELECT * FROM permissions", connection);
			userAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
			userAdapter.Fill(usersTable);
		}

		static public List<string> GetTableNames() {
			List<string> tableNamesList = new List<string>();
			using (MySqlCommand cmd = new MySqlCommand("show tables", connection)) {
				using (MySqlDataReader reader = cmd.ExecuteReader()) {
					while (reader.Read())
						tableNamesList.Add(reader.GetString(0));
				}
			}
			return tableNamesList;
		}

		static public void SetTableContents(string tableName, ref DataSet dataSet) {
				adapter[tableName].Update(dataSet.Tables[tableName].Select(null, null, DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent | DataViewRowState.Added));
		}
	}
}