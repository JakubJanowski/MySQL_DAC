using System;
using System.Configuration;
using System.IO;
using System.Windows;
using MySQL_DAC.Database;

namespace MySQL_DAC {
	static public class Configure {
		private static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

		public static string DatabaseName {
			get {
				try {
					return config.AppSettings.Settings["DatabaseName"].Value;
				} catch (Exception e) {
					MessageBox.Show("Config file not found.");
					Logger.WriteEntry("Configure.DatabaseName read: " + e.Message);
					return "";
				}
			}
			set {
				try {
					config.AppSettings.Settings["DatabaseName"].Value = value;
					config.Save(ConfigurationSaveMode.Modified);
					ConfigurationManager.RefreshSection("appSettings");
				} catch (Exception e) {
					MessageBox.Show("Config file not found.");
					Logger.WriteEntry("Configure.DatabaseName save: " + e.Message);
				}
			}
		}

		public static string Port {
			get {
				try {
					return config.AppSettings.Settings["Port"].Value;
				} catch (Exception e) {
					MessageBox.Show("Config file not found.");
					Logger.WriteEntry("Configure.Port read: " + e.Message);
					return "";
				}
			}
			set {
				try {
					config.AppSettings.Settings["Port"].Value = value;
					config.Save(ConfigurationSaveMode.Modified);
					ConfigurationManager.RefreshSection("appSettings");
				} catch (Exception e) {
					MessageBox.Show("Config file not found.");
					Logger.WriteEntry("Configure.Port save: " + e.Message);
				}
			}
		}

		public static string ServerIP {
			get {
				try {
					return config.AppSettings.Settings["ServerIP"].Value;
				} catch (Exception e) {
					MessageBox.Show("Config file not found.");
					Logger.WriteEntry("Configure.ServerIP read: " + e.Message);
					return "";
				}
			}
			set {
				try {
					config.AppSettings.Settings["ServerIP"].Value = value;
					config.Save(ConfigurationSaveMode.Modified);
					ConfigurationManager.RefreshSection("appSettings");
				} catch (Exception e) {
					MessageBox.Show("Config file not found.");
					Logger.WriteEntry("Configure.ServerIP save:" + e.Message);
				}
			}
		}
	}
}
