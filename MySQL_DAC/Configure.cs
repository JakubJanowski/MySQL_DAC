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
					Logger.WriteEntry(e.Message);
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
					Logger.WriteEntry(e.Message);
				}
			}
		}
		public static string ServerIP {
			get {
				try {
					return config.AppSettings.Settings["ServerIP"].Value;
				} catch (Exception e) {
					MessageBox.Show("Config file not found.");
					Logger.WriteEntry(e.Message);
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
					Logger.WriteEntry(e.Message);
				}
			}
		}
	}
}
