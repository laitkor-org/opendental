//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using EhrLaboratories;

namespace OpenDentBusiness.Crud{
	public class EhrLabNoteCrud {
		///<summary>Gets one EhrLabNote object from the database using the primary key.  Returns null if not found.</summary>
		public static EhrLabNote SelectOne(long ehrLabNoteNum) {
			string command="SELECT * FROM ehrlabnote "
				+"WHERE EhrLabNoteNum = "+POut.Long(ehrLabNoteNum);
			List<EhrLabNote> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one EhrLabNote object from the database using a query.</summary>
		public static EhrLabNote SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<EhrLabNote> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of EhrLabNote objects from the database using a query.</summary>
		public static List<EhrLabNote> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<EhrLabNote> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<EhrLabNote> TableToList(DataTable table) {
			List<EhrLabNote> retVal=new List<EhrLabNote>();
			EhrLabNote ehrLabNote;
			foreach(DataRow row in table.Rows) {
				ehrLabNote=new EhrLabNote();
				ehrLabNote.EhrLabNoteNum  = PIn.Long  (row["EhrLabNoteNum"].ToString());
				ehrLabNote.EhrLabNum      = PIn.Long  (row["EhrLabNum"].ToString());
				ehrLabNote.EhrLabResultNum= PIn.Long  (row["EhrLabResultNum"].ToString());
				ehrLabNote.Comments       = PIn.String(row["Comments"].ToString());
				retVal.Add(ehrLabNote);
			}
			return retVal;
		}

		///<summary>Converts a list of EhrLabNote into a DataTable.</summary>
		public static DataTable ListToTable(List<EhrLabNote> listEhrLabNotes,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="EhrLabNote";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("EhrLabNoteNum");
			table.Columns.Add("EhrLabNum");
			table.Columns.Add("EhrLabResultNum");
			table.Columns.Add("Comments");
			foreach(EhrLabNote ehrLabNote in listEhrLabNotes) {
				table.Rows.Add(new object[] {
					POut.Long  (ehrLabNote.EhrLabNoteNum),
					POut.Long  (ehrLabNote.EhrLabNum),
					POut.Long  (ehrLabNote.EhrLabResultNum),
					            ehrLabNote.Comments,
				});
			}
			return table;
		}

		///<summary>Inserts one EhrLabNote into the database.  Returns the new priKey.</summary>
		public static long Insert(EhrLabNote ehrLabNote) {
			return Insert(ehrLabNote,false);
		}

		///<summary>Inserts one EhrLabNote into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(EhrLabNote ehrLabNote,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				ehrLabNote.EhrLabNoteNum=ReplicationServers.GetKey("ehrlabnote","EhrLabNoteNum");
			}
			string command="INSERT INTO ehrlabnote (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="EhrLabNoteNum,";
			}
			command+="EhrLabNum,EhrLabResultNum,Comments) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(ehrLabNote.EhrLabNoteNum)+",";
			}
			command+=
				     POut.Long  (ehrLabNote.EhrLabNum)+","
				+    POut.Long  (ehrLabNote.EhrLabResultNum)+","
				+    DbHelper.ParamChar+"paramComments)";
			if(ehrLabNote.Comments==null) {
				ehrLabNote.Comments="";
			}
			OdSqlParameter paramComments=new OdSqlParameter("paramComments",OdDbType.Text,POut.StringParam(ehrLabNote.Comments));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramComments);
			}
			else {
				ehrLabNote.EhrLabNoteNum=Db.NonQ(command,true,"EhrLabNoteNum","ehrLabNote",paramComments);
			}
			return ehrLabNote.EhrLabNoteNum;
		}

		///<summary>Inserts one EhrLabNote into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(EhrLabNote ehrLabNote) {
			return InsertNoCache(ehrLabNote,false);
		}

		///<summary>Inserts one EhrLabNote into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(EhrLabNote ehrLabNote,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO ehrlabnote (";
			if(!useExistingPK && isRandomKeys) {
				ehrLabNote.EhrLabNoteNum=ReplicationServers.GetKeyNoCache("ehrlabnote","EhrLabNoteNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="EhrLabNoteNum,";
			}
			command+="EhrLabNum,EhrLabResultNum,Comments) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(ehrLabNote.EhrLabNoteNum)+",";
			}
			command+=
				     POut.Long  (ehrLabNote.EhrLabNum)+","
				+    POut.Long  (ehrLabNote.EhrLabResultNum)+","
				+    DbHelper.ParamChar+"paramComments)";
			if(ehrLabNote.Comments==null) {
				ehrLabNote.Comments="";
			}
			OdSqlParameter paramComments=new OdSqlParameter("paramComments",OdDbType.Text,POut.StringParam(ehrLabNote.Comments));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramComments);
			}
			else {
				ehrLabNote.EhrLabNoteNum=Db.NonQ(command,true,"EhrLabNoteNum","ehrLabNote",paramComments);
			}
			return ehrLabNote.EhrLabNoteNum;
		}

		///<summary>Updates one EhrLabNote in the database.</summary>
		public static void Update(EhrLabNote ehrLabNote) {
			string command="UPDATE ehrlabnote SET "
				+"EhrLabNum      =  "+POut.Long  (ehrLabNote.EhrLabNum)+", "
				+"EhrLabResultNum=  "+POut.Long  (ehrLabNote.EhrLabResultNum)+", "
				+"Comments       =  "+DbHelper.ParamChar+"paramComments "
				+"WHERE EhrLabNoteNum = "+POut.Long(ehrLabNote.EhrLabNoteNum);
			if(ehrLabNote.Comments==null) {
				ehrLabNote.Comments="";
			}
			OdSqlParameter paramComments=new OdSqlParameter("paramComments",OdDbType.Text,POut.StringParam(ehrLabNote.Comments));
			Db.NonQ(command,paramComments);
		}

		///<summary>Updates one EhrLabNote in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(EhrLabNote ehrLabNote,EhrLabNote oldEhrLabNote) {
			string command="";
			if(ehrLabNote.EhrLabNum != oldEhrLabNote.EhrLabNum) {
				if(command!="") { command+=",";}
				command+="EhrLabNum = "+POut.Long(ehrLabNote.EhrLabNum)+"";
			}
			if(ehrLabNote.EhrLabResultNum != oldEhrLabNote.EhrLabResultNum) {
				if(command!="") { command+=",";}
				command+="EhrLabResultNum = "+POut.Long(ehrLabNote.EhrLabResultNum)+"";
			}
			if(ehrLabNote.Comments != oldEhrLabNote.Comments) {
				if(command!="") { command+=",";}
				command+="Comments = "+DbHelper.ParamChar+"paramComments";
			}
			if(command=="") {
				return false;
			}
			if(ehrLabNote.Comments==null) {
				ehrLabNote.Comments="";
			}
			OdSqlParameter paramComments=new OdSqlParameter("paramComments",OdDbType.Text,POut.StringParam(ehrLabNote.Comments));
			command="UPDATE ehrlabnote SET "+command
				+" WHERE EhrLabNoteNum = "+POut.Long(ehrLabNote.EhrLabNoteNum);
			Db.NonQ(command,paramComments);
			return true;
		}

		///<summary>Returns true if Update(EhrLabNote,EhrLabNote) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(EhrLabNote ehrLabNote,EhrLabNote oldEhrLabNote) {
			if(ehrLabNote.EhrLabNum != oldEhrLabNote.EhrLabNum) {
				return true;
			}
			if(ehrLabNote.EhrLabResultNum != oldEhrLabNote.EhrLabResultNum) {
				return true;
			}
			if(ehrLabNote.Comments != oldEhrLabNote.Comments) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one EhrLabNote from the database.</summary>
		public static void Delete(long ehrLabNoteNum) {
			string command="DELETE FROM ehrlabnote "
				+"WHERE EhrLabNoteNum = "+POut.Long(ehrLabNoteNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many EhrLabNotes from the database.</summary>
		public static void DeleteMany(List<long> listEhrLabNoteNums) {
			if(listEhrLabNoteNums==null || listEhrLabNoteNums.Count==0) {
				return;
			}
			string command="DELETE FROM ehrlabnote "
				+"WHERE EhrLabNoteNum IN("+string.Join(",",listEhrLabNoteNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}