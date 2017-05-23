using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using MySQL_DAC.Views;

namespace MySQL_DAC.Database {
	static class DatabaseManager {
		//private const string databaseName = "db6008378_dac";
		private const string databaseName = "elektrownia";
		private static MySqlConnection connection;
		private static Dictionary<string, MySqlDataAdapter> adapter;
		private static MySqlDataAdapter userAdapter;
		private static MainView mainView;
		private static Dictionary<string, MySqlDbType> sqlTypeMap;
		private static Dictionary<MySqlDbType, Type> typeMap;

		static DatabaseManager() {
			sqlTypeMap = new Dictionary<string, MySqlDbType>();
			typeMap = new Dictionary<MySqlDbType, Type>();

			sqlTypeMap.Add("bit(1)", MySqlDbType.Bit);
			sqlTypeMap.Add("date", MySqlDbType.Date);
			sqlTypeMap.Add("datetime", MySqlDbType.DateTime);
			sqlTypeMap.Add("decimal(10,0)", MySqlDbType.Decimal);
			sqlTypeMap.Add("decimal(18,2)", MySqlDbType.Decimal);
			sqlTypeMap.Add("int(11)", MySqlDbType.Int32);
			sqlTypeMap.Add("tinytext", MySqlDbType.TinyText);
			sqlTypeMap.Add("varchar(11)", MySqlDbType.VarChar);
			sqlTypeMap.Add("varchar(12)", MySqlDbType.VarChar);
			sqlTypeMap.Add("varchar(30)", MySqlDbType.VarChar);
			sqlTypeMap.Add("varchar(45)", MySqlDbType.VarChar);
			sqlTypeMap.Add("varchar(50)", MySqlDbType.VarChar);
			sqlTypeMap.Add("varchar(100)", MySqlDbType.VarChar);
			sqlTypeMap.Add("varchar(300)", MySqlDbType.VarChar);
			
			typeMap[MySqlDbType.Bit] = typeof(bool);
			typeMap[MySqlDbType.Date] = typeof(DateTime);
			typeMap[MySqlDbType.DateTime] = typeof(DateTime);
			typeMap[MySqlDbType.Decimal] = typeof(decimal);
			typeMap[MySqlDbType.Int32] = typeof(int);
			typeMap[MySqlDbType.TinyText] = typeof(byte);
			typeMap[MySqlDbType.VarChar] = typeof(string);
		}

		static public bool Connect(string username, string password) {
			connection = new MySqlConnection();
			connection.ConnectionString = "server=127.0.0.1;uid=" + username + ";pwd=" + password + ";database=" + databaseName + ";";
			//connection.ConnectionString = "server=127.0.0.1;uid=root;pwd=root;database=elektrownia;";
			//connection.ConnectionString = "server=mysql591.cp.az.pl;port=3306;uid=u6008378_dac;pwd=KqcSGoz4W;database=" + databaseName + ";";
			//connection.ConnectionString = "server=10.128.49.45;uid=" + username + ";pwd=" + password + ";database=" + databaseName + ";";

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
				MessageBox.Show(ex.Message);
				return false;
			}
		}

		internal static void SetMainView(MainView mainView) {
			DatabaseManager.mainView = mainView;
		}

		static public void Close() {
			try {
				connection.Close();
			} catch (MySqlException ex) {
				MessageBox.Show(ex.Message);
			}
		}

		internal static void GetTableContents(string tableName, ref DataSet dataSet, bool returnRows = true) {
			try {
				if (returnRows) {
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
				else {
					var typeLibrary = new Dictionary<string, MySqlDbType>();

					if (!dataSet.Tables.Contains(tableName)) {
						dataSet.Tables.Add(tableName);
						using (MySqlCommand cmd = new MySqlCommand("DESCRIBE " + tableName, connection)) {
							using (MySqlDataReader reader = cmd.ExecuteReader()) {
								while (reader.Read()) {
									string column = reader.GetString("Field");
									string type = reader.GetString("Type");
									dataSet.Tables[tableName].Columns.Add(column, typeMap[sqlTypeMap[type]]);
									typeLibrary.Add(column, sqlTypeMap[type]);
								}
							}
						}
					}
					if (!adapter.ContainsKey(tableName)) {
						adapter[tableName] = new MySqlDataAdapter();
						string commandText = "INSERT INTO " + tableName + " (";
						int i = 0;
						for (; i < dataSet.Tables[tableName].Columns.Count - 1; i++)
							commandText += dataSet.Tables[tableName].Columns[i].ColumnName + ",";
						commandText += dataSet.Tables[tableName].Columns[i].ColumnName + ") VALUES (";
						i = 0;
						for (; i < dataSet.Tables[tableName].Columns.Count - 1; i++)
							commandText += "@" + dataSet.Tables[tableName].Columns[i].ColumnName + ",";
						commandText += "@" + dataSet.Tables[tableName].Columns[i].ColumnName + ")";

						adapter[tableName].InsertCommand = new MySqlCommand(commandText, connection);

						for (i = 0; i < dataSet.Tables[tableName].Columns.Count; i++) {
							string columnName = dataSet.Tables[tableName].Columns[i].ColumnName;
							adapter[tableName].InsertCommand.Parameters.Add("@" + columnName, typeLibrary[columnName]).SourceColumn = columnName;
						}
					}
				}
			} catch (MySqlException ex) {
				MessageBox.Show(ex.Message);
				mainView.Refresh();
			}
		}

		internal static void GetUsers(ref DataTable usersTable) {
			try {
				if (userAdapter == null) {
					userAdapter = new MySqlDataAdapter("SELECT * FROM permissions", connection);
					userAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
				}
				userAdapter.Fill(usersTable);
			} catch (MySqlException ex) {
				MessageBox.Show(ex.Message);
				mainView.Refresh();
			}
		}
		internal static List<string> GetUsernames() {
			List<string> usernamesList = new List<string>();
			try {
				using (MySqlCommand cmd = new MySqlCommand("SELECT user FROM permissions", connection)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							usernamesList.Add(reader.GetString(0));
					}
				}
			} catch (MySqlException ex) {
				MessageBox.Show(ex.Message);
				mainView.Refresh();
			}
			return usernamesList;
		}

		public static List<string> GetTableNames() {
			List<string> tableNamesList = new List<string>();
			try {
				using (MySqlCommand cmd = new MySqlCommand("show tables", connection)) {
					using (MySqlDataReader reader = cmd.ExecuteReader()) {
						while (reader.Read())
							tableNamesList.Add(reader.GetString(0));
					}
				}
				tableNamesList.Remove("permissions");
			} catch (MySqlException ex) {
				MessageBox.Show(ex.Message);
				mainView.Refresh();
			}
			return tableNamesList;
		}

		static public void SetTableContents(string tableName, ref DataSet dataSet) {
			try {
				adapter[tableName].Update(dataSet.Tables[tableName].Select(null, null, DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent | DataViewRowState.Added));
			} catch (MySqlException ex) {
				MessageBox.Show(ex.Message);
				mainView.Refresh();
			}
		}

		public static void AddUser(string username, string password, Dictionary<string, Permissions> userPermissions, int giverId) {
			try {
				List<string> tableNamesList = GetTableNames();
				MySqlCommand command = connection.CreateCommand();
				bool grantUpdatePermissions = false;

				using (MySqlCommand cmd = new MySqlCommand($"CREATE USER @username IDENTIFIED BY @password ;", connection)) {
					cmd.Parameters.AddWithValue("@username", $"{username}");
					cmd.Parameters.AddWithValue("@password", $"{password}");
					cmd.Prepare();
					cmd.ExecuteNonQuery();
				}

				#region Grant Table Permissions
				foreach (var tablePermissions in userPermissions) {
					if ((tablePermissions.Value & Permissions.DelegateAllNormal) > 0)
						grantUpdatePermissions = true;
					if (tablePermissions.Value.HasFlag(Permissions.DelegateView)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT SELECT ON {databaseName}.{tablePermissions.Key} TO @username WITH GRANT OPTION;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
					else if (tablePermissions.Value.HasFlag(Permissions.View)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT SELECT ON {databaseName}.{tablePermissions.Key} TO @username ;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
					if (tablePermissions.Value.HasFlag(Permissions.DelegateAdd)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT INSERT ON {databaseName}.{tablePermissions.Key} TO @username WITH GRANT OPTION;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
					else if (tablePermissions.Value.HasFlag(Permissions.Add)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT INSERT ON {databaseName}.{tablePermissions.Key} TO @username ;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
					if (tablePermissions.Value.HasFlag(Permissions.DelegateDelete)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT DELETE ON {databaseName}.{tablePermissions.Key} TO @username WITH GRANT OPTION;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
					else if (tablePermissions.Value.HasFlag(Permissions.Delete)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT DELETE ON {databaseName}.{tablePermissions.Key} TO @username ;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
					if (tablePermissions.Value.HasFlag(Permissions.DelegateEdit)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT UPDATE ON {databaseName}.{tablePermissions.Key} TO @username WITH GRANT OPTION;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
					else if (tablePermissions.Value.HasFlag(Permissions.Edit)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT UPDATE ON {databaseName}.{tablePermissions.Key} TO @username ;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
				}
				#endregion

				using (MySqlCommand cmd = new MySqlCommand($"GRANT SELECT ON {databaseName}.permissions TO @username ;", connection)) {
					cmd.Parameters.AddWithValue("@username", $"{username}");
					cmd.Prepare();
					cmd.ExecuteNonQuery();
				}

				if (grantUpdatePermissions) {
					using (MySqlCommand cmd = new MySqlCommand($"GRANT UPDATE ON {databaseName}.permissions TO @username WITH GRANT OPTION;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
				}

				#region Grant User Permissions
				if (userPermissions.ContainsKey("userPermissions")) {
					if (userPermissions["userPermissions"].HasFlag(Permissions.DelegateCreateUser)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT CREATE USER ON *.* TO @username WITH GRANT OPTION;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
						using (MySqlCommand cmd = new MySqlCommand($"GRANT SELECT, INSERT ON {databaseName}.permissions TO @username WITH GRANT OPTION;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
					else if (userPermissions["userPermissions"].HasFlag(Permissions.CreateUser)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT CREATE USER ON *.* TO @username ;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
						using (MySqlCommand cmd = new MySqlCommand($"GRANT INSERT ON {databaseName}.permissions TO @username ;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
						using (MySqlCommand cmd = new MySqlCommand($"GRANT SELECT ON {databaseName}.permissions TO @username WITH GRANT OPTION;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
					else if (userPermissions["userPermissions"].HasFlag(Permissions.DelegateViewPermissions)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT SELECT ON {databaseName}.permissions TO @username WITH GRANT OPTION;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
					if (userPermissions["userPermissions"].HasFlag(Permissions.DelegateDeleteUser)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT DELETE ON {databaseName}.permissions TO @username WITH GRANT OPTION;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
					else if (userPermissions["userPermissions"].HasFlag(Permissions.DeleteUser)) {
						using (MySqlCommand cmd = new MySqlCommand($"GRANT DELETE ON {databaseName}.permissions TO @username ;", connection)) {
							cmd.Parameters.AddWithValue("@username", $"{username}");
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}
					}
				}
				#endregion

				command.CommandText = "INSERT INTO permissions(user";
				command.Parameters.Add("?user", MySqlDbType.VarChar).Value = username;

				foreach (string tableName in tableNamesList)
					command.CommandText += "," + tableName;
				command.CommandText += ",userPermissions,giverId) VALUES(?user";
				foreach (string tableName in tableNamesList) {
					command.CommandText += ",?" + tableName;
					if (userPermissions.ContainsKey(tableName))
						command.Parameters.Add("?" + tableName, MySqlDbType.Int32).Value = (int)userPermissions[tableName];
					else
						command.Parameters.Add("?" + tableName, MySqlDbType.Int32).Value = (int)Permissions.None;
				}
				if (userPermissions.ContainsKey("userPermissions"))
					command.Parameters.Add("?userPermissions", MySqlDbType.Int32).Value = (int)userPermissions["userPermissions"];
				else
					command.Parameters.Add("?userPermissions", MySqlDbType.Int32).Value = (int)Permissions.None;
				command.CommandText += ",?userPermissions,?giverId)";
				command.Parameters.Add("?giverId", MySqlDbType.Int32).Value = giverId;
				command.Prepare();

				command.ExecuteNonQuery();
			} catch (MySqlException ex) {
				MessageBox.Show(ex.Message);
				mainView.Refresh();
			}
		}

		internal static void UpdateUser(string username, string oldPassword, string newPassword, Dictionary<string, Permissions> userPermissions, Dictionary<string, Permissions> previousPermissions, int giverId) {
			try {
				// update password
				UpdateUser(username, userPermissions, previousPermissions, giverId);
			} catch (MySqlException ex) {
				MessageBox.Show(ex.Message);
			}
		}

		internal static void UpdateUser(string username, Dictionary<string, Permissions> userPermissions, Dictionary<string, Permissions> previousPermissions, int giverId) {
			try {
				MySqlCommand command = connection.CreateCommand();
				List<string> tableNamesList = GetTableNames();
				bool grantUpdatePermissions = false;
				bool previousGrantUpdatePermissions = false;

				foreach (var tablePermissions in userPermissions) {
					if (tablePermissions.Value != previousPermissions[tablePermissions.Key] && !tablePermissions.Key.Equals("userPermissions")) {
						if ((tablePermissions.Value & Permissions.DelegateAllNormal) > 0)
							grantUpdatePermissions = true;
						if ((previousPermissions[tablePermissions.Key] & Permissions.DelegateAllNormal) > 0)
							previousGrantUpdatePermissions = true;

						#region Grant Table Permissions
						if (tablePermissions.Value.HasFlag(Permissions.DelegateView) && !previousPermissions[tablePermissions.Key].HasFlag(Permissions.DelegateView)) {
							using (MySqlCommand cmd = new MySqlCommand($"GRANT SELECT ON {databaseName}.{tablePermissions.Key} TO @username WITH GRANT OPTION;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}
						else if (tablePermissions.Value.HasFlag(Permissions.View) && !previousPermissions[tablePermissions.Key].HasFlag(Permissions.View)) {
							using (MySqlCommand cmd = new MySqlCommand($"GRANT SELECT ON {databaseName}.{tablePermissions.Key} TO @username ;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}
						if (tablePermissions.Value.HasFlag(Permissions.DelegateAdd) && !previousPermissions[tablePermissions.Key].HasFlag(Permissions.DelegateAdd)) {
							using (MySqlCommand cmd = new MySqlCommand($"GRANT INSERT ON {databaseName}.{tablePermissions.Key} TO @username WITH GRANT OPTION;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}
						else if (tablePermissions.Value.HasFlag(Permissions.Add) && !previousPermissions[tablePermissions.Key].HasFlag(Permissions.Add)) {
							using (MySqlCommand cmd = new MySqlCommand($"GRANT INSERT ON {databaseName}.{tablePermissions.Key} TO @username ;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}
						if (tablePermissions.Value.HasFlag(Permissions.DelegateDelete) && !previousPermissions[tablePermissions.Key].HasFlag(Permissions.DelegateDelete)) {
							using (MySqlCommand cmd = new MySqlCommand($"GRANT DELETE ON {databaseName}.{tablePermissions.Key} TO @username WITH GRANT OPTION;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}
						else if (tablePermissions.Value.HasFlag(Permissions.Delete) && !previousPermissions[tablePermissions.Key].HasFlag(Permissions.Delete)) {
							using (MySqlCommand cmd = new MySqlCommand($"GRANT DELETE ON {databaseName}.{tablePermissions.Key} TO @username ;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}
						if (tablePermissions.Value.HasFlag(Permissions.DelegateEdit) && !previousPermissions[tablePermissions.Key].HasFlag(Permissions.DelegateEdit)) {
							using (MySqlCommand cmd = new MySqlCommand($"GRANT UPDATE ON {databaseName}.{tablePermissions.Key} TO @username WITH GRANT OPTION;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}
						else if (tablePermissions.Value.HasFlag(Permissions.Edit) && !previousPermissions[tablePermissions.Key].HasFlag(Permissions.Edit)) {
							using (MySqlCommand cmd = new MySqlCommand($"GRANT UPDATE ON {databaseName}.{tablePermissions.Key} TO @username ;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}
						#endregion

						#region Revoke Table Permissions
						if ((previousPermissions[tablePermissions.Key].HasFlag(Permissions.DelegateAdd) && !tablePermissions.Value.HasFlag(Permissions.DelegateAdd)) ||
							(previousPermissions[tablePermissions.Key].HasFlag(Permissions.Add) && !tablePermissions.Value.HasFlag(Permissions.Add))) {
							using (MySqlCommand cmd = new MySqlCommand($"REVOKE INSERT ON {databaseName}.{tablePermissions.Key} FROM @username ;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}
						if ((previousPermissions[tablePermissions.Key].HasFlag(Permissions.DelegateDelete) && !tablePermissions.Value.HasFlag(Permissions.DelegateDelete)) ||
							(previousPermissions[tablePermissions.Key].HasFlag(Permissions.Delete) && !tablePermissions.Value.HasFlag(Permissions.Delete))) {
							using (MySqlCommand cmd = new MySqlCommand($"REVOKE DELETE ON {databaseName}.{tablePermissions.Key} FROM @username ;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}
						if ((previousPermissions[tablePermissions.Key].HasFlag(Permissions.DelegateEdit) && !tablePermissions.Value.HasFlag(Permissions.DelegateEdit)) ||
							(previousPermissions[tablePermissions.Key].HasFlag(Permissions.Edit) && !tablePermissions.Value.HasFlag(Permissions.Edit))) {
							using (MySqlCommand cmd = new MySqlCommand($"REVOKE UPDATE ON {databaseName}.{tablePermissions.Key} FROM @username ;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}
						if ((previousPermissions[tablePermissions.Key].HasFlag(Permissions.DelegateView) && !tablePermissions.Value.HasFlag(Permissions.DelegateView)) ||
							(previousPermissions[tablePermissions.Key].HasFlag(Permissions.View) && !tablePermissions.Value.HasFlag(Permissions.View))) {
							using (MySqlCommand cmd = new MySqlCommand($"REVOKE SELECT ON {databaseName}.{tablePermissions.Key} FROM @username ;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}

						if ((previousPermissions[tablePermissions.Key] & Permissions.DelegateAllNormal) > 0 && (tablePermissions.Value & Permissions.DelegateAllNormal) == Permissions.None) {
							using (MySqlCommand cmd = new MySqlCommand($"REVOKE GRANT OPTION ON {databaseName}.{tablePermissions.Key} FROM @username ;", connection)) {
								cmd.Parameters.AddWithValue("@username", $"{username}");
								cmd.Prepare();
								cmd.ExecuteNonQuery();
							}
						}
						#endregion
					}
				}

				#region Grant User Permissions
				if (userPermissions["userPermissions"].HasFlag(Permissions.DelegateCreateUser) && !previousPermissions["userPermissions"].HasFlag(Permissions.DelegateCreateUser)) {
					using (MySqlCommand cmd = new MySqlCommand($"GRANT CREATE USER ON *.* TO @username WITH GRANT OPTION;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
					using (MySqlCommand cmd = new MySqlCommand($"GRANT SELECT, INSERT ON {databaseName}.permissions TO @username WITH GRANT OPTION;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
				}
				else if (userPermissions["userPermissions"].HasFlag(Permissions.CreateUser) && !previousPermissions["userPermissions"].HasFlag(Permissions.CreateUser)) {
					using (MySqlCommand cmd = new MySqlCommand($"GRANT CREATE USER ON *.* TO @username ;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
					using (MySqlCommand cmd = new MySqlCommand($"GRANT INSERT ON {databaseName}.permissions TO @username ;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
					using (MySqlCommand cmd = new MySqlCommand($"GRANT SELECT ON {databaseName}.permissions TO @username WITH GRANT OPTION;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
				}
				else if (userPermissions["userPermissions"].HasFlag(Permissions.DelegateViewPermissions) && !previousPermissions["userPermissions"].HasFlag(Permissions.DelegateViewPermissions)) {
					using (MySqlCommand cmd = new MySqlCommand($"GRANT SELECT ON {databaseName}.permissions TO @username WITH GRANT OPTION;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
				}
				if (userPermissions["userPermissions"].HasFlag(Permissions.DelegateDeleteUser) && !previousPermissions["userPermissions"].HasFlag(Permissions.DelegateDeleteUser)) {
					using (MySqlCommand cmd = new MySqlCommand($"GRANT DELETE ON {databaseName}.permissions TO @username WITH GRANT OPTION;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
				}
				else if (userPermissions["userPermissions"].HasFlag(Permissions.DeleteUser) && !previousPermissions["userPermissions"].HasFlag(Permissions.DeleteUser)) {
					using (MySqlCommand cmd = new MySqlCommand($"GRANT DELETE ON {databaseName}.permissions TO @username ;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
				}
				#endregion

				#region Revoke User Permissions
				if (!userPermissions["userPermissions"].HasFlag(Permissions.DelegateCreateUser) && previousPermissions["userPermissions"].HasFlag(Permissions.DelegateCreateUser)) {
					using (MySqlCommand cmd = new MySqlCommand($"REVOKE CREATE USER, GRANT OPTION ON *.* FROM @username ;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
					using (MySqlCommand cmd = new MySqlCommand($"REVOKE INSERT ON {databaseName}.permissions FROM @username ;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
				}
				else if (!userPermissions["userPermissions"].HasFlag(Permissions.CreateUser) && previousPermissions["userPermissions"].HasFlag(Permissions.CreateUser)) {
					using (MySqlCommand cmd = new MySqlCommand($"REVOKE CREATE USER ON *.* FROM @username ;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
					using (MySqlCommand cmd = new MySqlCommand($"REVOKE INSERT ON {databaseName}.permissions FROM @username ;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
				}
				if ((!userPermissions["userPermissions"].HasFlag(Permissions.DelegateDeleteUser) && previousPermissions["userPermissions"].HasFlag(Permissions.DelegateDeleteUser)) ||
					(!userPermissions["userPermissions"].HasFlag(Permissions.DeleteUser) && previousPermissions["userPermissions"].HasFlag(Permissions.DeleteUser))) {
					using (MySqlCommand cmd = new MySqlCommand($"REVOKE DELETE ON {databaseName}.permissions FROM @username ;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
				}

				if ((userPermissions["userPermissions"] & Permissions.DelegateAllUser) == Permissions.None && !userPermissions["userPermissions"].HasFlag(Permissions.CreateUser)) {
					using (MySqlCommand cmd = new MySqlCommand($"REVOKE GRANT OPTION ON {databaseName}.permissions FROM @username ;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
				}

				#endregion

				if (grantUpdatePermissions && !previousGrantUpdatePermissions) {
					using (MySqlCommand cmd = new MySqlCommand($"GRANT UPDATE ON {databaseName}.permissions TO @username WITH GRANT OPTION;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
				}
				else if (!grantUpdatePermissions && previousGrantUpdatePermissions) {
					using (MySqlCommand cmd = new MySqlCommand($"REVOKE UPDATE, GRANT OPTION ON {databaseName}.permissions TO @username ;", connection)) {
						cmd.Parameters.AddWithValue("@username", $"{username}");
						cmd.Prepare();
						cmd.ExecuteNonQuery();
					}
				}

				command.CommandText = "UPDATE permissions SET ";

				foreach (string tableName in tableNamesList) {
					command.CommandText += tableName + "=?" + tableName + ",";
					if (userPermissions.ContainsKey(tableName))
						command.Parameters.Add("?" + tableName, MySqlDbType.Int32).Value = (int)userPermissions[tableName];
					else
						command.Parameters.Add("?" + tableName, MySqlDbType.Int32).Value = (int)Permissions.None;
				}
				command.CommandText += "userPermissions=?userPermissions,giverId=?giverId WHERE user=?user ;";

				if (userPermissions.ContainsKey("userPermissions"))
					command.Parameters.Add("?userPermissions", MySqlDbType.Int32).Value = (int)userPermissions["userPermissions"];
				else
					command.Parameters.Add("?userPermissions", MySqlDbType.Int32).Value = (int)Permissions.None;

				command.Parameters.Add("?giverId", MySqlDbType.Int32).Value = giverId;
				command.Parameters.Add("?user", MySqlDbType.VarChar).Value = username;

				command.Prepare();
				command.ExecuteNonQuery();
			} catch (MySqlException ex) {
				MessageBox.Show(ex.Message);
				mainView.Refresh();
			}
		}
	}
}