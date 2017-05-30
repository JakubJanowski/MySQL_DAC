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
		CanTakeOver				= 0x0800,
		DelegateCreateUser		= 0x1000,
		DelegateDeleteUser		= 0x2000,
		DelegateViewPermissions	= 0x4000,
		DelegateCanTakeOver		= 0x10000,

		UserPermissions			= 0x8000,

		AllNormal				= Add | Delete | Edit | View,
		DelegateAllNormal		= DelegateAdd | DelegateDelete | DelegateEdit | DelegateView,
		AllUser					= CreateUser | DeleteUser | ViewPermissions,
		DelegateAllUser			= DelegateCreateUser | DelegateDeleteUser | DelegateViewPermissions,
		All						= AllNormal | DelegateAllNormal | AllUser | DelegateAllUser
	}
}
