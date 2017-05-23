using System;
using System.Text;

namespace MySQL_DAC.Database {
	[Flags]
	public enum Permissions {
		None				= 0x00,

		Add					= 0x01,
		Delete				= 0x02,
		Edit				= 0x04,
		View				= 0x08,

		DelegateAdd				= 0x10,
		DelegateDelete			= 0x20,
		DelegateEdit			= 0x40,
		DelegateView			= 0x80,

		CreateUser				= 0x0100,
		DeleteUser				= 0x0200,
		ViewPermissions			= 0x0400,
		DelegateCreateUser		= 0x1000,
		DelegateDeleteUser		= 0x2000,
		DelegateViewPermissions	= 0x4000,

		UserPermissions			= 0x8000,

		AllNormal				= Add | Delete | Edit | View,
		DelegateAllNormal		= DelegateAdd | DelegateDelete | DelegateEdit | DelegateView,
		AllUser					= CreateUser | DeleteUser | ViewPermissions,
		DelegateAllUser			= DelegateCreateUser | DelegateDeleteUser | DelegateViewPermissions,
		All						= AllNormal | DelegateAllNormal | AllUser | DelegateAllUser
	}

	public static class Extension {
		public static string ShortNotation(this Permissions value) {
			if (value.HasFlag(Permissions.UserPermissions)) {
				StringBuilder userPermissions = new StringBuilder("---");
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
	}
}
