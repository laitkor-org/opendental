//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class MountDefCrud {
		///<summary>Gets one MountDef object from the database using the primary key.  Returns null if not found.</summary>
		public static MountDef SelectOne(long mountDefNum) {
			string command="SELECT * FROM mountdef "
				+"WHERE MountDefNum = "+POut.Long(mountDefNum);
			List<MountDef> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one MountDef object from the database using a query.</summary>
		public static MountDef SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<MountDef> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of MountDef objects from the database using a query.</summary>
		public static List<MountDef> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<MountDef> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<MountDef> TableToList(DataTable table) {
			List<MountDef> retVal=new List<MountDef>();
			MountDef mountDef;
			foreach(DataRow row in table.Rows) {
				mountDef=new MountDef();
				mountDef.MountDefNum  = PIn.Long  (row["MountDefNum"].ToString());
				mountDef.Description  = PIn.String(row["Description"].ToString());
				mountDef.ItemOrder    = PIn.Int   (row["ItemOrder"].ToString());
				mountDef.Width        = PIn.Int   (row["Width"].ToString());
				mountDef.Height       = PIn.Int   (row["Height"].ToString());
				mountDef.ColorBack    = Color.FromArgb(PIn.Int(row["ColorBack"].ToString()));
				mountDef.ColorFore    = Color.FromArgb(PIn.Int(row["ColorFore"].ToString()));
				mountDef.ColorTextBack= Color.FromArgb(PIn.Int(row["ColorTextBack"].ToString()));
				mountDef.ScaleValue   = PIn.String(row["ScaleValue"].ToString());
				retVal.Add(mountDef);
			}
			return retVal;
		}

		///<summary>Converts a list of MountDef into a DataTable.</summary>
		public static DataTable ListToTable(List<MountDef> listMountDefs,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="MountDef";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("MountDefNum");
			table.Columns.Add("Description");
			table.Columns.Add("ItemOrder");
			table.Columns.Add("Width");
			table.Columns.Add("Height");
			table.Columns.Add("ColorBack");
			table.Columns.Add("ColorFore");
			table.Columns.Add("ColorTextBack");
			table.Columns.Add("ScaleValue");
			foreach(MountDef mountDef in listMountDefs) {
				table.Rows.Add(new object[] {
					POut.Long  (mountDef.MountDefNum),
					            mountDef.Description,
					POut.Int   (mountDef.ItemOrder),
					POut.Int   (mountDef.Width),
					POut.Int   (mountDef.Height),
					POut.Int   (mountDef.ColorBack.ToArgb()),
					POut.Int   (mountDef.ColorFore.ToArgb()),
					POut.Int   (mountDef.ColorTextBack.ToArgb()),
					            mountDef.ScaleValue,
				});
			}
			return table;
		}

		///<summary>Inserts one MountDef into the database.  Returns the new priKey.</summary>
		public static long Insert(MountDef mountDef) {
			return Insert(mountDef,false);
		}

		///<summary>Inserts one MountDef into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(MountDef mountDef,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				mountDef.MountDefNum=ReplicationServers.GetKey("mountdef","MountDefNum");
			}
			string command="INSERT INTO mountdef (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="MountDefNum,";
			}
			command+="Description,ItemOrder,Width,Height,ColorBack,ColorFore,ColorTextBack,ScaleValue) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(mountDef.MountDefNum)+",";
			}
			command+=
				 "'"+POut.String(mountDef.Description)+"',"
				+    POut.Int   (mountDef.ItemOrder)+","
				+    POut.Int   (mountDef.Width)+","
				+    POut.Int   (mountDef.Height)+","
				+    POut.Int   (mountDef.ColorBack.ToArgb())+","
				+    POut.Int   (mountDef.ColorFore.ToArgb())+","
				+    POut.Int   (mountDef.ColorTextBack.ToArgb())+","
				+"'"+POut.String(mountDef.ScaleValue)+"')";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				mountDef.MountDefNum=Db.NonQ(command,true,"MountDefNum","mountDef");
			}
			return mountDef.MountDefNum;
		}

		///<summary>Inserts one MountDef into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(MountDef mountDef) {
			return InsertNoCache(mountDef,false);
		}

		///<summary>Inserts one MountDef into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(MountDef mountDef,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO mountdef (";
			if(!useExistingPK && isRandomKeys) {
				mountDef.MountDefNum=ReplicationServers.GetKeyNoCache("mountdef","MountDefNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="MountDefNum,";
			}
			command+="Description,ItemOrder,Width,Height,ColorBack,ColorFore,ColorTextBack,ScaleValue) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(mountDef.MountDefNum)+",";
			}
			command+=
				 "'"+POut.String(mountDef.Description)+"',"
				+    POut.Int   (mountDef.ItemOrder)+","
				+    POut.Int   (mountDef.Width)+","
				+    POut.Int   (mountDef.Height)+","
				+    POut.Int   (mountDef.ColorBack.ToArgb())+","
				+    POut.Int   (mountDef.ColorFore.ToArgb())+","
				+    POut.Int   (mountDef.ColorTextBack.ToArgb())+","
				+"'"+POut.String(mountDef.ScaleValue)+"')";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				mountDef.MountDefNum=Db.NonQ(command,true,"MountDefNum","mountDef");
			}
			return mountDef.MountDefNum;
		}

		///<summary>Updates one MountDef in the database.</summary>
		public static void Update(MountDef mountDef) {
			string command="UPDATE mountdef SET "
				+"Description  = '"+POut.String(mountDef.Description)+"', "
				+"ItemOrder    =  "+POut.Int   (mountDef.ItemOrder)+", "
				+"Width        =  "+POut.Int   (mountDef.Width)+", "
				+"Height       =  "+POut.Int   (mountDef.Height)+", "
				+"ColorBack    =  "+POut.Int   (mountDef.ColorBack.ToArgb())+", "
				+"ColorFore    =  "+POut.Int   (mountDef.ColorFore.ToArgb())+", "
				+"ColorTextBack=  "+POut.Int   (mountDef.ColorTextBack.ToArgb())+", "
				+"ScaleValue   = '"+POut.String(mountDef.ScaleValue)+"' "
				+"WHERE MountDefNum = "+POut.Long(mountDef.MountDefNum);
			Db.NonQ(command);
		}

		///<summary>Updates one MountDef in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(MountDef mountDef,MountDef oldMountDef) {
			string command="";
			if(mountDef.Description != oldMountDef.Description) {
				if(command!="") { command+=",";}
				command+="Description = '"+POut.String(mountDef.Description)+"'";
			}
			if(mountDef.ItemOrder != oldMountDef.ItemOrder) {
				if(command!="") { command+=",";}
				command+="ItemOrder = "+POut.Int(mountDef.ItemOrder)+"";
			}
			if(mountDef.Width != oldMountDef.Width) {
				if(command!="") { command+=",";}
				command+="Width = "+POut.Int(mountDef.Width)+"";
			}
			if(mountDef.Height != oldMountDef.Height) {
				if(command!="") { command+=",";}
				command+="Height = "+POut.Int(mountDef.Height)+"";
			}
			if(mountDef.ColorBack != oldMountDef.ColorBack) {
				if(command!="") { command+=",";}
				command+="ColorBack = "+POut.Int(mountDef.ColorBack.ToArgb())+"";
			}
			if(mountDef.ColorFore != oldMountDef.ColorFore) {
				if(command!="") { command+=",";}
				command+="ColorFore = "+POut.Int(mountDef.ColorFore.ToArgb())+"";
			}
			if(mountDef.ColorTextBack != oldMountDef.ColorTextBack) {
				if(command!="") { command+=",";}
				command+="ColorTextBack = "+POut.Int(mountDef.ColorTextBack.ToArgb())+"";
			}
			if(mountDef.ScaleValue != oldMountDef.ScaleValue) {
				if(command!="") { command+=",";}
				command+="ScaleValue = '"+POut.String(mountDef.ScaleValue)+"'";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE mountdef SET "+command
				+" WHERE MountDefNum = "+POut.Long(mountDef.MountDefNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(MountDef,MountDef) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(MountDef mountDef,MountDef oldMountDef) {
			if(mountDef.Description != oldMountDef.Description) {
				return true;
			}
			if(mountDef.ItemOrder != oldMountDef.ItemOrder) {
				return true;
			}
			if(mountDef.Width != oldMountDef.Width) {
				return true;
			}
			if(mountDef.Height != oldMountDef.Height) {
				return true;
			}
			if(mountDef.ColorBack != oldMountDef.ColorBack) {
				return true;
			}
			if(mountDef.ColorFore != oldMountDef.ColorFore) {
				return true;
			}
			if(mountDef.ColorTextBack != oldMountDef.ColorTextBack) {
				return true;
			}
			if(mountDef.ScaleValue != oldMountDef.ScaleValue) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one MountDef from the database.</summary>
		public static void Delete(long mountDefNum) {
			string command="DELETE FROM mountdef "
				+"WHERE MountDefNum = "+POut.Long(mountDefNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many MountDefs from the database.</summary>
		public static void DeleteMany(List<long> listMountDefNums) {
			if(listMountDefNums==null || listMountDefNums.Count==0) {
				return;
			}
			string command="DELETE FROM mountdef "
				+"WHERE MountDefNum IN("+string.Join(",",listMountDefNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}