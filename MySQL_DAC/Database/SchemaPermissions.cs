using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQL_DAC.Database {
	[Flags]
	enum SchemaPermissions {
		None = 0x00,

		// Object rights
		Select		= 0x01,
		Insert		= 0x02,
		Update		= 0x04,
		Delete		= 0x08,
		Execute		= 0x10,
		ShowView	= 0x20,

		// DLL rights
		Create			= 0x0040,
		Alter			= 0x0080,
		References		= 0x0100,
		Index			= 0x0200,
		CreateView		= 0x0400,
		CreateRoutine	= 0x0800,
		AlterRoutine	= 0x1000,
		Event			= 0x2000,
		Drop			= 0x4000,
		Trigger			= 0x8000,

		// Other rights
		GrantOption				= 0x10000,
		CreateTemporaryTables	= 0x20000,
		LockTables				= 0x40000,

		// Combined rights
		AllObject	= Select | Insert | Update | Delete | Execute | ShowView,
		AllDLL		= Create | Alter | References | Index | CreateView | CreateRoutine | AlterRoutine | Event | Drop | Trigger,
		AllOther	= GrantOption | CreateTemporaryTables | LockTables,
		All			= AllObject | AllDLL | AllOther
	}
}
