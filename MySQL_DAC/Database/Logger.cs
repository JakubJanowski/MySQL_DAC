using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQL_DAC.Database {
	public static class Logger {
		public static void WriteEntry(string text) {
			System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt", true);
			file.WriteLine($"[{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}] {text}");
			file.Close();
		}
	}
}
