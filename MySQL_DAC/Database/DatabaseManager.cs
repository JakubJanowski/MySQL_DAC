using System.Collections.Generic;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace MySQL_DAC.Database {
	static class DatabaseManager {
		private const string databaseName = "test";
		static private MySqlConnection connection;

		static public bool Connect(string username, string password) {
			connection = new MySqlConnection();
			connection.ConnectionString = "server=127.0.0.1;uid=" + username + ";pwd=" + password + ";database=" + databaseName + ";";

			try {
				connection.Open();
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
			MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM " + tableName, connection);
			adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
			adapter.Fill(dataSet, tableName);
		}

		internal static void GetUsers(ref DataTable usersTable) {
			MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM mysql.user", connection);
			adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
			adapter.Fill(usersTable);
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
	}
}