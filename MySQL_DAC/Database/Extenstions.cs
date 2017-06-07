using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQL_DAC.Database {
	public static class Extension {
		public static string ShortNotation(this Permissions value) {
			if (value.HasFlag(Permissions.UserPermissions)) {
				StringBuilder userPermissions = new StringBuilder("----");
				if (value.HasFlag(Permissions.DelegateCreateUser))
					userPermissions[0] = 'C';
				else if (value.HasFlag(Permissions.CreateUser))
					userPermissions[0] = 'c';
				if (value.HasFlag(Permissions.DelegateDeleteUser))
					userPermissions[1] = 'D';
				else if (value.HasFlag(Permissions.DeleteUser))
					userPermissions[1] = 'd';
				if (value.HasFlag(Permissions.DelegateViewPermissions))
					userPermissions[2] = 'V';
				else if (value.HasFlag(Permissions.ViewPermissions))
					userPermissions[2] = 'v';
				if (value.HasFlag(Permissions.DelegateCanTakeOver))
					userPermissions[3] = 'P';
				else if (value.HasFlag(Permissions.CanTakeOver))
					userPermissions[3] = 'p';
				return userPermissions.ToString();
			}

			StringBuilder permissions = new StringBuilder("----");

			if (value.HasFlag(Permissions.DelegateAdd))
				permissions[0] = 'A';
			else if (value.HasFlag(Permissions.Add))
				permissions[0] = 'a';
			if (value.HasFlag(Permissions.DelegateDelete))
				permissions[1] = 'D';
			else if (value.HasFlag(Permissions.Delete))
				permissions[1] = 'd';
			if (value.HasFlag(Permissions.DelegateEdit))
				permissions[2] = 'E';
			else if (value.HasFlag(Permissions.Edit))
				permissions[2] = 'e';
			if (value.HasFlag(Permissions.DelegateView))
				permissions[3] = 'V';
			else if (value.HasFlag(Permissions.View))
				permissions[3] = 'v';

			return permissions.ToString();
		}

		public static string Present(this Dictionary<string, Permissions> userPermissions) {
			string present = "";
			foreach (var entry in userPermissions)
				present += $"{entry.Key}: {entry.Value.ShortNotation()}, ";
			present = present.Remove(present.Length - 2);
			return present;
		}
		public static string Present(this DataTable changes) {
			string present = "{";
			foreach (DataRow row in changes.Rows) {
				if (row == null || row.RowState == DataRowState.Deleted) {
					present += "{row deleted}, ";
					continue;
				}
				present += "{";
				for (int i = 0; i < row.ItemArray.Count(); i++)
					present += $"{changes.Columns[i]}: {row.ItemArray[i].ToString()}, ";
				present = present.Remove(present.Length - 2);
				present += "}, ";
			}
			if(present.Length > 2)
			present = present.Remove(present.Length - 2);
			present += "}";
			return present;
		}
	}
}
