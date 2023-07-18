//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class TaskAttachmentCrud {
		///<summary>Gets one TaskAttachment object from the database using the primary key.  Returns null if not found.</summary>
		public static TaskAttachment SelectOne(long taskAttachmentNum) {
			string command="SELECT * FROM taskattachment "
				+"WHERE TaskAttachmentNum = "+POut.Long(taskAttachmentNum);
			List<TaskAttachment> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one TaskAttachment object from the database using a query.</summary>
		public static TaskAttachment SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<TaskAttachment> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of TaskAttachment objects from the database using a query.</summary>
		public static List<TaskAttachment> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<TaskAttachment> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<TaskAttachment> TableToList(DataTable table) {
			List<TaskAttachment> retVal=new List<TaskAttachment>();
			TaskAttachment taskAttachment;
			foreach(DataRow row in table.Rows) {
				taskAttachment=new TaskAttachment();
				taskAttachment.TaskAttachmentNum= PIn.Long  (row["TaskAttachmentNum"].ToString());
				taskAttachment.TaskNum          = PIn.Long  (row["TaskNum"].ToString());
				taskAttachment.DocNum           = PIn.Long  (row["DocNum"].ToString());
				taskAttachment.TextValue        = PIn.String(row["TextValue"].ToString());
				taskAttachment.Description      = PIn.String(row["Description"].ToString());
				retVal.Add(taskAttachment);
			}
			return retVal;
		}

		///<summary>Converts a list of TaskAttachment into a DataTable.</summary>
		public static DataTable ListToTable(List<TaskAttachment> listTaskAttachments,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="TaskAttachment";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("TaskAttachmentNum");
			table.Columns.Add("TaskNum");
			table.Columns.Add("DocNum");
			table.Columns.Add("TextValue");
			table.Columns.Add("Description");
			foreach(TaskAttachment taskAttachment in listTaskAttachments) {
				table.Rows.Add(new object[] {
					POut.Long  (taskAttachment.TaskAttachmentNum),
					POut.Long  (taskAttachment.TaskNum),
					POut.Long  (taskAttachment.DocNum),
					            taskAttachment.TextValue,
					            taskAttachment.Description,
				});
			}
			return table;
		}

		///<summary>Inserts one TaskAttachment into the database.  Returns the new priKey.</summary>
		public static long Insert(TaskAttachment taskAttachment) {
			return Insert(taskAttachment,false);
		}

		///<summary>Inserts one TaskAttachment into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(TaskAttachment taskAttachment,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				taskAttachment.TaskAttachmentNum=ReplicationServers.GetKey("taskattachment","TaskAttachmentNum");
			}
			string command="INSERT INTO taskattachment (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="TaskAttachmentNum,";
			}
			command+="TaskNum,DocNum,TextValue,Description) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(taskAttachment.TaskAttachmentNum)+",";
			}
			command+=
				     POut.Long  (taskAttachment.TaskNum)+","
				+    POut.Long  (taskAttachment.DocNum)+","
				+    DbHelper.ParamChar+"paramTextValue,"
				+"'"+POut.String(taskAttachment.Description)+"')";
			if(taskAttachment.TextValue==null) {
				taskAttachment.TextValue="";
			}
			OdSqlParameter paramTextValue=new OdSqlParameter("paramTextValue",OdDbType.Text,POut.StringNote(taskAttachment.TextValue));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramTextValue);
			}
			else {
				taskAttachment.TaskAttachmentNum=Db.NonQ(command,true,"TaskAttachmentNum","taskAttachment",paramTextValue);
			}
			return taskAttachment.TaskAttachmentNum;
		}

		///<summary>Inserts one TaskAttachment into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(TaskAttachment taskAttachment) {
			return InsertNoCache(taskAttachment,false);
		}

		///<summary>Inserts one TaskAttachment into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(TaskAttachment taskAttachment,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO taskattachment (";
			if(!useExistingPK && isRandomKeys) {
				taskAttachment.TaskAttachmentNum=ReplicationServers.GetKeyNoCache("taskattachment","TaskAttachmentNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="TaskAttachmentNum,";
			}
			command+="TaskNum,DocNum,TextValue,Description) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(taskAttachment.TaskAttachmentNum)+",";
			}
			command+=
				     POut.Long  (taskAttachment.TaskNum)+","
				+    POut.Long  (taskAttachment.DocNum)+","
				+    DbHelper.ParamChar+"paramTextValue,"
				+"'"+POut.String(taskAttachment.Description)+"')";
			if(taskAttachment.TextValue==null) {
				taskAttachment.TextValue="";
			}
			OdSqlParameter paramTextValue=new OdSqlParameter("paramTextValue",OdDbType.Text,POut.StringNote(taskAttachment.TextValue));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramTextValue);
			}
			else {
				taskAttachment.TaskAttachmentNum=Db.NonQ(command,true,"TaskAttachmentNum","taskAttachment",paramTextValue);
			}
			return taskAttachment.TaskAttachmentNum;
		}

		///<summary>Updates one TaskAttachment in the database.</summary>
		public static void Update(TaskAttachment taskAttachment) {
			string command="UPDATE taskattachment SET "
				+"TaskNum          =  "+POut.Long  (taskAttachment.TaskNum)+", "
				+"DocNum           =  "+POut.Long  (taskAttachment.DocNum)+", "
				+"TextValue        =  "+DbHelper.ParamChar+"paramTextValue, "
				+"Description      = '"+POut.String(taskAttachment.Description)+"' "
				+"WHERE TaskAttachmentNum = "+POut.Long(taskAttachment.TaskAttachmentNum);
			if(taskAttachment.TextValue==null) {
				taskAttachment.TextValue="";
			}
			OdSqlParameter paramTextValue=new OdSqlParameter("paramTextValue",OdDbType.Text,POut.StringNote(taskAttachment.TextValue));
			Db.NonQ(command,paramTextValue);
		}

		///<summary>Updates one TaskAttachment in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(TaskAttachment taskAttachment,TaskAttachment oldTaskAttachment) {
			string command="";
			if(taskAttachment.TaskNum != oldTaskAttachment.TaskNum) {
				if(command!="") { command+=",";}
				command+="TaskNum = "+POut.Long(taskAttachment.TaskNum)+"";
			}
			if(taskAttachment.DocNum != oldTaskAttachment.DocNum) {
				if(command!="") { command+=",";}
				command+="DocNum = "+POut.Long(taskAttachment.DocNum)+"";
			}
			if(taskAttachment.TextValue != oldTaskAttachment.TextValue) {
				if(command!="") { command+=",";}
				command+="TextValue = "+DbHelper.ParamChar+"paramTextValue";
			}
			if(taskAttachment.Description != oldTaskAttachment.Description) {
				if(command!="") { command+=",";}
				command+="Description = '"+POut.String(taskAttachment.Description)+"'";
			}
			if(command=="") {
				return false;
			}
			if(taskAttachment.TextValue==null) {
				taskAttachment.TextValue="";
			}
			OdSqlParameter paramTextValue=new OdSqlParameter("paramTextValue",OdDbType.Text,POut.StringNote(taskAttachment.TextValue));
			command="UPDATE taskattachment SET "+command
				+" WHERE TaskAttachmentNum = "+POut.Long(taskAttachment.TaskAttachmentNum);
			Db.NonQ(command,paramTextValue);
			return true;
		}

		///<summary>Returns true if Update(TaskAttachment,TaskAttachment) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(TaskAttachment taskAttachment,TaskAttachment oldTaskAttachment) {
			if(taskAttachment.TaskNum != oldTaskAttachment.TaskNum) {
				return true;
			}
			if(taskAttachment.DocNum != oldTaskAttachment.DocNum) {
				return true;
			}
			if(taskAttachment.TextValue != oldTaskAttachment.TextValue) {
				return true;
			}
			if(taskAttachment.Description != oldTaskAttachment.Description) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one TaskAttachment from the database.</summary>
		public static void Delete(long taskAttachmentNum) {
			string command="DELETE FROM taskattachment "
				+"WHERE TaskAttachmentNum = "+POut.Long(taskAttachmentNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many TaskAttachments from the database.</summary>
		public static void DeleteMany(List<long> listTaskAttachmentNums) {
			if(listTaskAttachmentNums==null || listTaskAttachmentNums.Count==0) {
				return;
			}
			string command="DELETE FROM taskattachment "
				+"WHERE TaskAttachmentNum IN("+string.Join(",",listTaskAttachmentNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}