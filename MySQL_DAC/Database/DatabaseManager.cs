using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MySQL_DAC {
	class DatabaseManager {
		private const string databaseName = "test";
		MySql.Data.MySqlClient.MySqlConnection connection;
		//public void Connect(string username, SecureString password) {}

		public void Connect(string username, string password) {
			string connectionString = "server=127.0.0.1;uid=" + username + ";pwd=" + password + ";database=" + databaseName + ";";

			try {
				connection = new MySql.Data.MySqlClient.MySqlConnection();
				connection.ConnectionString = connectionString;
				connection.Open();
				System.Windows.MessageBox.Show("Database connection established");
			} catch (MySql.Data.MySqlClient.MySqlException ex) {
				System.Windows.MessageBox.Show(ex.Message);
			}
		}
	}
}
