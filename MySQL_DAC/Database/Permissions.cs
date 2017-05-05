using System;
namespace MySQL_DAC.Database {
	[Flags]
	enum Permissions {
		None = 0x00,

		//Alter					= 0x01,
		//AlterRoutine			= 0x02,
		//Create					= 0x04,
		//CreateRoutine			= 0x08,
		Add=1,
		Delete=2,
		Edit=4,
		View=8,
		
		CreateTablespace		= 0x10,
		CreateTemporaryTables	= 0x20,
		CreateUser				= 0x0040,
		CreateView				= 0x0080,
		///Delete					= 0x0100,
		Drop					= 0x0200,
		Event					= 0x0400,
		Execute					= 0x0800,
		File					= 0x1000,
		GrantOption				= 0x2000,
		Index					= 0x4000,
		Insert					= 0x8000,
		LockTables				= 0x010000,
		Process					= 0x020000,
		References				= 0x040000,
		Reload					= 0x080000,
		ReplicationClient		= 0x100000,
		ReplicationSlave		= 0x200000,
		Select					= 0x400000,
		ShowDatabases			= 0x800000,
		ShowView				= 0x01000000,
		Shutdown				= 0x02000000,
		Super					= 0x04000000,
		Trigger					= 0x08000000,
		Update					= 0x10000000,

		// Roles
		BackupAdmin			= Event | LockTables | Select | ShowDatabases,
		//DBDesigner			= Alter | AlterRoutine | Create | CreateRoutine | CreateView | Index | ShowDatabases | ShowView | Trigger,
		MonitorAdmin		= Process,
		ProcessAdmin		= Reload | Super,
		ReplicationAdmin	= ReplicationClient | ReplicationSlave | Super,
		UserAdmin			= CreateUser | Reload,
		MaintenanceAdmin	= ProcessAdmin | Event | ShowDatabases | Shutdown,
		SecurityAdmin		= UserAdmin | GrantOption | ShowDatabases,
		//DBManager			= BackupAdmin | DBDesigner | CreateTemporaryTables | Delete | Drop | GrantOption | Insert | Update,
		//DBA					= DBManager | SecurityAdmin | MaintenanceAdmin | ReplicationAdmin | MonitorAdmin | CreateTablespace | Execute | File | References,
		All					= 15//DBA
	}
}
