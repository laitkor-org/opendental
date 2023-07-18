//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Crud{
	public class UserOdPrefCrud {
		///<summary>Gets one UserOdPref object from the database using the primary key.  Returns null if not found.</summary>
		public static UserOdPref SelectOne(long userOdPrefNum) {
			string command="SELECT * FROM userodpref "
				+"WHERE UserOdPrefNum = "+POut.Long(userOdPrefNum);
			List<UserOdPref> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one UserOdPref object from the database using a query.</summary>
		public static UserOdPref SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<UserOdPref> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of UserOdPref objects from the database using a query.</summary>
		public static List<UserOdPref> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<UserOdPref> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<UserOdPref> TableToList(DataTable table) {
			List<UserOdPref> retVal=new List<UserOdPref>();
			UserOdPref userOdPref;
			foreach(DataRow row in table.Rows) {
				userOdPref=new UserOdPref();
				userOdPref.UserOdPrefNum= PIn.Long  (row["UserOdPrefNum"].ToString());
				userOdPref.UserNum      = PIn.Long  (row["UserNum"].ToString());
				userOdPref.Fkey         = PIn.Long  (row["Fkey"].ToString());
				userOdPref.FkeyType     = (OpenDentBusiness.UserOdFkeyType)PIn.Int(row["FkeyType"].ToString());
				userOdPref.ValueString  = PIn.String(row["ValueString"].ToString());
				userOdPref.ClinicNum    = PIn.Long  (row["ClinicNum"].ToString());
				retVal.Add(userOdPref);
			}
			return retVal;
		}

		///<summary>Converts a list of UserOdPref into a DataTable.</summary>
		public static DataTable ListToTable(List<UserOdPref> listUserOdPrefs,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="UserOdPref";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("UserOdPrefNum");
			table.Columns.Add("UserNum");
			table.Columns.Add("Fkey");
			table.Columns.Add("FkeyType");
			table.Columns.Add("ValueString");
			table.Columns.Add("ClinicNum");
			foreach(UserOdPref userOdPref in listUserOdPrefs) {
				table.Rows.Add(new object[] {
					POut.Long  (userOdPref.UserOdPrefNum),
					POut.Long  (userOdPref.UserNum),
					POut.Long  (userOdPref.Fkey),
					POut.Int   ((int)userOdPref.FkeyType),
					            userOdPref.ValueString,
					POut.Long  (userOdPref.ClinicNum),
				});
			}
			return table;
		}

		///<summary>Inserts one UserOdPref into the database.  Returns the new priKey.</summary>
		public static long Insert(UserOdPref userOdPref) {
			return Insert(userOdPref,false);
		}

		///<summary>Inserts one UserOdPref into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(UserOdPref userOdPref,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				userOdPref.UserOdPrefNum=ReplicationServers.GetKey("userodpref","UserOdPrefNum");
			}
			string command="INSERT INTO userodpref (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="UserOdPrefNum,";
			}
			command+="UserNum,Fkey,FkeyType,ValueString,ClinicNum) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(userOdPref.UserOdPrefNum)+",";
			}
			command+=
				     POut.Long  (userOdPref.UserNum)+","
				+    POut.Long  (userOdPref.Fkey)+","
				+    POut.Int   ((int)userOdPref.FkeyType)+","
				+    DbHelper.ParamChar+"paramValueString,"
				+    POut.Long  (userOdPref.ClinicNum)+")";
			if(userOdPref.ValueString==null) {
				userOdPref.ValueString="";
			}
			OdSqlParameter paramValueString=new OdSqlParameter("paramValueString",OdDbType.Text,POut.StringParam(userOdPref.ValueString));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramValueString);
			}
			else {
				userOdPref.UserOdPrefNum=Db.NonQ(command,true,"UserOdPrefNum","userOdPref",paramValueString);
			}
			return userOdPref.UserOdPrefNum;
		}

		///<summary>Inserts many UserOdPrefs into the database.</summary>
		public static void InsertMany(List<UserOdPref> listUserOdPrefs) {
			InsertMany(listUserOdPrefs,false);
		}

		///<summary>Inserts many UserOdPrefs into the database.  Provides option to use the existing priKey.</summary>
		public static void InsertMany(List<UserOdPref> listUserOdPrefs,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				foreach(UserOdPref userOdPref in listUserOdPrefs) {
					Insert(userOdPref);
				}
			}
			else {
				StringBuilder sbCommands=null;
				int index=0;
				int countRows=0;
				while(index < listUserOdPrefs.Count) {
					UserOdPref userOdPref=listUserOdPrefs[index];
					StringBuilder sbRow=new StringBuilder("(");
					bool hasComma=false;
					if(sbCommands==null) {
						sbCommands=new StringBuilder();
						sbCommands.Append("INSERT INTO userodpref (");
						if(useExistingPK) {
							sbCommands.Append("UserOdPrefNum,");
						}
						sbCommands.Append("UserNum,Fkey,FkeyType,ValueString,ClinicNum) VALUES ");
						countRows=0;
					}
					else {
						hasComma=true;
					}
					if(useExistingPK) {
						sbRow.Append(POut.Long(userOdPref.UserOdPrefNum)); sbRow.Append(",");
					}
					sbRow.Append(POut.Long(userOdPref.UserNum)); sbRow.Append(",");
					sbRow.Append(POut.Long(userOdPref.Fkey)); sbRow.Append(",");
					sbRow.Append(POut.Int((int)userOdPref.FkeyType)); sbRow.Append(",");
					sbRow.Append("'"+POut.String(userOdPref.ValueString)+"'"); sbRow.Append(",");
					sbRow.Append(POut.Long(userOdPref.ClinicNum)); sbRow.Append(")");
					if(sbCommands.Length+sbRow.Length+1 > TableBase.MaxAllowedPacketCount && countRows > 0) {
						Db.NonQ(sbCommands.ToString());
						sbCommands=null;
					}
					else {
						if(hasComma) {
							sbCommands.Append(",");
						}
						sbCommands.Append(sbRow.ToString());
						countRows++;
						if(index==listUserOdPrefs.Count-1) {
							Db.NonQ(sbCommands.ToString());
						}
						index++;
					}
				}
			}
		}

		///<summary>Inserts one UserOdPref into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(UserOdPref userOdPref) {
			return InsertNoCache(userOdPref,false);
		}

		///<summary>Inserts one UserOdPref into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(UserOdPref userOdPref,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO userodpref (";
			if(!useExistingPK && isRandomKeys) {
				userOdPref.UserOdPrefNum=ReplicationServers.GetKeyNoCache("userodpref","UserOdPrefNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="UserOdPrefNum,";
			}
			command+="UserNum,Fkey,FkeyType,ValueString,ClinicNum) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(userOdPref.UserOdPrefNum)+",";
			}
			command+=
				     POut.Long  (userOdPref.UserNum)+","
				+    POut.Long  (userOdPref.Fkey)+","
				+    POut.Int   ((int)userOdPref.FkeyType)+","
				+    DbHelper.ParamChar+"paramValueString,"
				+    POut.Long  (userOdPref.ClinicNum)+")";
			if(userOdPref.ValueString==null) {
				userOdPref.ValueString="";
			}
			OdSqlParameter paramValueString=new OdSqlParameter("paramValueString",OdDbType.Text,POut.StringParam(userOdPref.ValueString));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramValueString);
			}
			else {
				userOdPref.UserOdPrefNum=Db.NonQ(command,true,"UserOdPrefNum","userOdPref",paramValueString);
			}
			return userOdPref.UserOdPrefNum;
		}

		///<summary>Updates one UserOdPref in the database.</summary>
		public static void Update(UserOdPref userOdPref) {
			string command="UPDATE userodpref SET "
				+"UserNum      =  "+POut.Long  (userOdPref.UserNum)+", "
				+"Fkey         =  "+POut.Long  (userOdPref.Fkey)+", "
				+"FkeyType     =  "+POut.Int   ((int)userOdPref.FkeyType)+", "
				+"ValueString  =  "+DbHelper.ParamChar+"paramValueString, "
				+"ClinicNum    =  "+POut.Long  (userOdPref.ClinicNum)+" "
				+"WHERE UserOdPrefNum = "+POut.Long(userOdPref.UserOdPrefNum);
			if(userOdPref.ValueString==null) {
				userOdPref.ValueString="";
			}
			OdSqlParameter paramValueString=new OdSqlParameter("paramValueString",OdDbType.Text,POut.StringParam(userOdPref.ValueString));
			Db.NonQ(command,paramValueString);
		}

		///<summary>Updates one UserOdPref in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(UserOdPref userOdPref,UserOdPref oldUserOdPref) {
			string command="";
			if(userOdPref.UserNum != oldUserOdPref.UserNum) {
				if(command!="") { command+=",";}
				command+="UserNum = "+POut.Long(userOdPref.UserNum)+"";
			}
			if(userOdPref.Fkey != oldUserOdPref.Fkey) {
				if(command!="") { command+=",";}
				command+="Fkey = "+POut.Long(userOdPref.Fkey)+"";
			}
			if(userOdPref.FkeyType != oldUserOdPref.FkeyType) {
				if(command!="") { command+=",";}
				command+="FkeyType = "+POut.Int   ((int)userOdPref.FkeyType)+"";
			}
			if(userOdPref.ValueString != oldUserOdPref.ValueString) {
				if(command!="") { command+=",";}
				command+="ValueString = "+DbHelper.ParamChar+"paramValueString";
			}
			if(userOdPref.ClinicNum != oldUserOdPref.ClinicNum) {
				if(command!="") { command+=",";}
				command+="ClinicNum = "+POut.Long(userOdPref.ClinicNum)+"";
			}
			if(command=="") {
				return false;
			}
			if(userOdPref.ValueString==null) {
				userOdPref.ValueString="";
			}
			OdSqlParameter paramValueString=new OdSqlParameter("paramValueString",OdDbType.Text,POut.StringParam(userOdPref.ValueString));
			command="UPDATE userodpref SET "+command
				+" WHERE UserOdPrefNum = "+POut.Long(userOdPref.UserOdPrefNum);
			Db.NonQ(command,paramValueString);
			return true;
		}

		///<summary>Returns true if Update(UserOdPref,UserOdPref) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(UserOdPref userOdPref,UserOdPref oldUserOdPref) {
			if(userOdPref.UserNum != oldUserOdPref.UserNum) {
				return true;
			}
			if(userOdPref.Fkey != oldUserOdPref.Fkey) {
				return true;
			}
			if(userOdPref.FkeyType != oldUserOdPref.FkeyType) {
				return true;
			}
			if(userOdPref.ValueString != oldUserOdPref.ValueString) {
				return true;
			}
			if(userOdPref.ClinicNum != oldUserOdPref.ClinicNum) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one UserOdPref from the database.</summary>
		public static void Delete(long userOdPrefNum) {
			string command="DELETE FROM userodpref "
				+"WHERE UserOdPrefNum = "+POut.Long(userOdPrefNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many UserOdPrefs from the database.</summary>
		public static void DeleteMany(List<long> listUserOdPrefNums) {
			if(listUserOdPrefNums==null || listUserOdPrefNums.Count==0) {
				return;
			}
			string command="DELETE FROM userodpref "
				+"WHERE UserOdPrefNum IN("+string.Join(",",listUserOdPrefNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		///<summary>Inserts, updates, or deletes database rows to match supplied list.  Returns true if db changes were made.</summary>
		public static bool Sync(List<UserOdPref> listNew,List<UserOdPref> listDB) {
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<UserOdPref> listIns    =new List<UserOdPref>();
			List<UserOdPref> listUpdNew =new List<UserOdPref>();
			List<UserOdPref> listUpdDB  =new List<UserOdPref>();
			List<UserOdPref> listDel    =new List<UserOdPref>();
			listNew.Sort((UserOdPref x,UserOdPref y) => { return x.UserOdPrefNum.CompareTo(y.UserOdPrefNum); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			listDB.Sort((UserOdPref x,UserOdPref y) => { return x.UserOdPrefNum.CompareTo(y.UserOdPrefNum); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			int idxNew=0;
			int idxDB=0;
			int rowsUpdatedCount=0;
			UserOdPref fieldNew;
			UserOdPref fieldDB;
			//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.
			//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.
			while(idxNew<listNew.Count || idxDB<listDB.Count) {
				fieldNew=null;
				if(idxNew<listNew.Count) {
					fieldNew=listNew[idxNew];
				}
				fieldDB=null;
				if(idxDB<listDB.Count) {
					fieldDB=listDB[idxDB];
				}
				//begin compare
				if(fieldNew!=null && fieldDB==null) {//listNew has more items, listDB does not.
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew==null && fieldDB!=null) {//listDB has more items, listNew does not.
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				else if(fieldNew.UserOdPrefNum<fieldDB.UserOdPrefNum) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.UserOdPrefNum>fieldDB.UserOdPrefNum) {//dbPK less than newPK, dbItem is 'next'
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				//Both lists contain the 'next' item, update required
				listUpdNew.Add(fieldNew);
				listUpdDB.Add(fieldDB);
				idxNew++;
				idxDB++;
			}
			//Commit changes to DB
			for(int i=0;i<listIns.Count;i++) {
				Insert(listIns[i]);
			}
			for(int i=0;i<listUpdNew.Count;i++) {
				if(Update(listUpdNew[i],listUpdDB[i])) {
					rowsUpdatedCount++;
				}
			}
			DeleteMany(listDel.Select(x => x.UserOdPrefNum).ToList());
			if(rowsUpdatedCount>0 || listIns.Count>0 || listDel.Count>0) {
				return true;
			}
			return false;
		}

	}
}