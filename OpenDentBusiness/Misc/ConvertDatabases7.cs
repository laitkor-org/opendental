using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CodeBase;
using DataConnectionBase;
using static OpenDentBusiness.LargeTableHelper;//so you don't have to type the class name each time.

namespace OpenDentBusiness {
	public partial class ConvertDatabases {
		#region Helper Methods
		private static bool IsUsingReplication() {
			string command="SELECT COUNT(*) FROM replicationserver";
			if(Db.GetCount(command)!="0") {
				return true;
			}
			command="SHOW MASTER STATUS ";
			DataTable tableReplicationMasterStatus=Db.GetTable(command);
			command="SHOW SLAVE STATUS";
			DataTable tableSlaveStatus=Db.GetTable(command);
			if(tableReplicationMasterStatus.Rows.Count > 0 || tableSlaveStatus.Rows.Count > 0) {
				return true;
			}
			//Last check Galera cluster (NADG)
			command="SHOW GLOBAL VARIABLES LIKE '%wsrep_on%' ";
			tableSlaveStatus=Db.GetTable(command);
			for(int i = 0;i<tableSlaveStatus.Rows.Count;i++) {
				DataRow row=tableSlaveStatus.Rows[i];
				if(row["wsrep_on"]!=null && PIn.String(row["wsrep_on"].ToString())=="ON") {
					command=$"SELECT COUNT(DISTINCT wcm.node_uuid) ";
					command+="FROM mysql.wsrep_cluster wc ";
					command+="INNER JOIN mysql.wsrep_cluster_members wcm ON wc.cluster_uuid=wcm.cluster_uuid ";
					int count=Db.GetInt(command);
					if(count>0) {
						return true;
					}
				}
			}
			return false;
		}
		#endregion
		private static void To20_5_1() {
			string command;
			DataTable table;
			command="DROP TABLE IF EXISTS imagingdevice";
			Db.NonQ(command);
			command=@"CREATE TABLE imagingdevice (
				ImagingDeviceNum bigint NOT NULL auto_increment PRIMARY KEY,
				Description varchar(255) NOT NULL,
				ComputerName varchar(255) NOT NULL,
				DeviceType tinyint NOT NULL,
				TwainName varchar(255) NOT NULL,
				ItemOrder int NOT NULL,
				ShowTwainUI tinyint NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS eservicelog";
			Db.NonQ(command);
			command=@"CREATE TABLE eservicelog (
				EServiceLogNum bigint NOT NULL auto_increment PRIMARY KEY,
				EServiceCode tinyint NOT NULL,
				EServiceOperation tinyint NOT NULL,
				LogDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				AptNum bigint NOT NULL,
				PatNum bigint NOT NULL,
				PayNum bigint NOT NULL,
				SheetNum bigint NOT NULL,
				INDEX(EServiceCode),
				INDEX(EServiceOperation),
				INDEX(AptNum),
				INDEX(PatNum),
				INDEX(PayNum),
				INDEX(SheetNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="ALTER TABLE mountitemdef ADD RotateOnAcquire int NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitem ADD RotateOnAcquire int NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitemdef ADD ToothNumbers varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitem ADD ToothNumbers varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE wikilistheaderwidth ADD IsHidden tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE procedurecode ADD PaintText varchar(255) NOT NULL";
			Db.NonQ(command);
			command="UPDATE procedurecode SET PaintText='W' WHERE PaintType=15";//ToothPaintingType.Text
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE PrefName='EnterpriseExactMatchPhone'";
			//This preference might have already been added in 20.2.49.
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseExactMatchPhone','0')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseExactMatchPhoneNumDigits','10')";
				Db.NonQ(command);
			}
			command="INSERT INTO alertcategory (IsHQCategory,InternalName,Description) VALUES(1,'SupplementalBackups','Supplemental Backups')";
			long alertCatNum=Db.NonQ(command, true);
			command=$@"UPDATE alertcategorylink SET AlertCategoryNum={POut.Long(alertCatNum)} WHERE AlertType=23";//23=SupplementalBackups
			Db.NonQ(command);
			command="SELECT UserNum,ClinicNum FROM alertsub WHERE AlertCategoryNum=1";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				command=$@"INSERT INTO alertsub (UserNum,ClinicNum,AlertCategoryNum) 
					VALUES(
						{POut.Long(PIn.Long(table.Rows[i]["UserNum"].ToString()))},
						{POut.Long(PIn.Long(table.Rows[i]["ClinicNum"].ToString()))},
						{POut.Long(alertCatNum)}
					)";
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SameForFamilyCheckboxesUnchecked','0')";
			Db.NonQ(command);
			command="UPDATE procedurecode SET PaintType=17 WHERE ProcCode IN('D1510','D1516','D1517')";//17=SpaceMaintainer
			Db.NonQ(command);
			command="UPDATE procedurecode SET TreatArea=7 WHERE ProcCode IN('D1516','D1517')";//7=ToothRange for bilateral
			Db.NonQ(command);
			AlterTable("toothinitial","ToothInitialNum",new ColNameAndDef("DrawText","varchar(255) NOT NULL"));
			command="INSERT INTO preference(PrefName,ValueString) VALUES('IncomeTransfersTreatNegativeProductionAsIncome','1')";
			Db.NonQ(command);
			command="ALTER TABLE mount ADD ProvNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE document ADD ProvNum bigint NOT NULL";
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE PrefName='EnterpriseAllowRefreshWhileTyping'";
			//This preference might have already been added in 20.3.41
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseAllowRefreshWhileTyping','1')";
				Db.NonQ(command);
			}
			command="ALTER TABLE activeinstance ADD ConnectionType tinyint(4) NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SaveDXCSOAPAsXML','0')";
			Db.NonQ(command);		
			command="ALTER TABLE clinic ADD TimeZone varchar(75) NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('WebFormsDownloadAlertFrequency','3600000')";//1 hour in ms
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes'";
			alertCatNum=Db.GetLong(command);
			command=$@"INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES({POut.Long(alertCatNum)},30)";//30=WebformsReady
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('AutoImportFolder','')";
			Db.NonQ(command);
			try {
				command="SELECT * FROM userod";
				table=Db.GetTable(command);
				command="SELECT ValueString FROM preference WHERE PrefName='DomainObjectGuid'";
				string domainObjectGuid=Db.GetScalar(command);
				for(int i=0;i<table.Rows.Count;i++) {
					if(!table.Rows[i]["DomainUser"].ToString().IsNullOrEmpty()) {
						string newDomainUser=domainObjectGuid+"\\"+table.Rows[i]["DomainUser"].ToString();
						command="UPDATE userod ";
						command+="SET DomainUser='"+POut.String(newDomainUser)+"' ";
						command+="WHERE UserNum="+POut.String(table.Rows[i]["UserNum"].ToString());
						Db.NonQ(command);
					}
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowIncomeTransferManager','1')";
			Db.NonQ(command);	
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimPayByTotalSplitsAuto','1')";
			Db.NonQ(command);
			command="SELECT valuestring FROM preference WHERE PrefName='SheetsDefaultStatement'";
			long sheetDefNumDefault=PIn.Long(Db.GetScalar(command));
			command=$@"INSERT INTO preference(PrefName,ValueString) VALUES('SheetsDefaultReceipt','{POut.Long(sheetDefNumDefault)}')";
			Db.NonQ(command);	
			command=$@"INSERT INTO preference(PrefName,ValueString) VALUES('SheetsDefaultInvoice','{POut.Long(sheetDefNumDefault)}')";
			Db.NonQ(command);	
			command=$@"INSERT INTO preference(PrefName,ValueString) VALUES('SheetsDefaultLimited','{POut.Long(sheetDefNumDefault)}')";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				   +"VALUES("+POut.Long(groupNum)+",200)";//200 - FormAdded, Used for logging form creation in EClipboard.
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SalesTaxDefaultProvider','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SalesTaxDoAutomate','0')";
			Db.NonQ(command);
			command="ALTER TABLE program ADD CustErr varchar(255) NOT NULL";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS cert";
			Db.NonQ(command);
			command=@"CREATE TABLE cert (
				CertNum bigint NOT NULL auto_increment PRIMARY KEY,
				Description varchar(255) NOT NULL,
				WikiPageLink varchar(255) NOT NULL,
				ItemOrder int NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS certemployee";
			Db.NonQ(command);
			command=@"CREATE TABLE certemployee (
				CertEmployeeNum bigint NOT NULL auto_increment PRIMARY KEY,
				DateCompleted date NOT NULL DEFAULT '0001-01-01',
				Note varchar(255) NOT NULL,
				UserNum bigint NOT NULL,
				INDEX(UserNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS certlinkcategory";
			Db.NonQ(command);
			command=@"CREATE TABLE certlinkcategory (
				CertLinkCategoryNum bigint NOT NULL auto_increment PRIMARY KEY,
				CertNum bigint NOT NULL,
				CertCategoryNum bigint NOT NULL,
				INDEX(CertNum),
				INDEX(CertCategoryNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					+"VALUES("+POut.Long(groupNum)+",201)";//201 - Used to restrict access to Image Exporting when necessary.
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DefaultImageImportFolder','')";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				+"VALUES("+POut.Long(groupNum)+",202)";//202 - Used to restrict access to Image Create.
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS statementprod";
			Db.NonQ(command);
			command=@"CREATE TABLE statementprod (
					StatementProdNum bigint NOT NULL auto_increment PRIMARY KEY,
					StatementNum bigint NOT NULL,
					FKey bigint NOT NULL,
					ProdType tinyint NOT NULL,
					LateChargeAdjNum bigint NOT NULL,
					INDEX(StatementNum),
					INDEX(FKey),
					INDEX(ProdType),
					INDEX(LateChargeAdjNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=1";//1 is AdjTypes
			int order=PIn.Int(Db.GetCount(command));
			command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) "
				+"VALUES (1,'Late Charge',"+POut.Int(order)+",'+')";//1 is AdjTypes
			long defNum=Db.NonQ(command,true);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeAdjustmentType','"+POut.Long(defNum)+"')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeLastRunDate','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeExcludeAccountNoTil','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeExcludeBalancesLessThan','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargePercent','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeMin','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeMax','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeDateRangeStart','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeDateRangeEnd','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeDefaultBillingTypes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowFeatureLateCharges','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('RecurringChargesInactivateDeclinedCards','0')";
			Db.NonQ(command);//Unset by default (same effect as Off for now, for backwards compatibility).
			command="ALTER TABLE creditcard ADD IsRecurringActive tinyint NOT NULL";
			Db.NonQ(command);
			command="UPDATE creditcard SET IsRecurringActive=1";//Default to true for existing credit cards.
			Db.NonQ(command);
		}//End of 20_5_1() method

		private static void To20_5_2() {
			string command;
			DataTable table;
			command="DROP TABLE IF EXISTS discountplansub";
			Db.NonQ(command);
			command=@"CREATE TABLE discountplansub (
				DiscountSubNum bigint NOT NULL auto_increment PRIMARY KEY,
				DiscountPlanNum bigint NOT NULL,
				PatNum bigint NOT NULL,
				DateEffective date NOT NULL DEFAULT '0001-01-01',
				DateTerm date NOT NULL DEFAULT '0001-01-01',
				INDEX(DiscountPlanNum),
				INDEX(PatNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT PatNum,DiscountPlanNum FROM patient WHERE DiscountPlanNum>0";
			table=Db.GetTable(command);
			long patNum;
			for(int i=0;i<table.Rows.Count;i++) {
				patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
				command=$@"INSERT INTO discountplansub (DiscountPlanNum,PatNum) 
					VALUES(
						{POut.Long(PIn.Long(table.Rows[i]["DiscountPlanNum"].ToString()))},
						{POut.Long(patNum)}
					)";
				Db.NonQ(command);
				//Optionally set the DiscountPlanNum to 0
				command="UPDATE patient ";
				command+="SET DiscountPlanNum=0 ";
				command+="WHERE PatNum="+POut.Long(patNum);
				Db.NonQ(command);
			}
			command="DELETE FROM grouppermission WHERE PermType=89";//Permission already existed, but not enforced. Refreshing this Permission from scratch.
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				+"VALUES("+POut.Long(groupNum)+",89)";//89 - Used to restrict access to Image Edit.
				Db.NonQ(command);
			}
			command="ALTER TABLE programproperty ADD IsMasked tinyint NOT NULL";
			Db.NonQ(command);
			command="SELECT ProgramPropertyNum FROM programproperty WHERE PropertyDesc LIKE '%password%'";
			List<long> listPasswordPropNums=Db.GetListLong(command);
			for(int i=0;i<listPasswordPropNums.Count;i++) {
				command=$"UPDATE programproperty SET IsMasked={POut.Bool(true)} WHERE ProgramPropertyNum={POut.Long(listPasswordPropNums[i])}";
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmPostcardFamMessage','We would like to confirm your appointments. [FamilyApptList].')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmEmailFamMessage','We would like to confirm your appointments. [FamilyApptList].')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmTextFamMessage','We would like to confirm your appointments. [FamilyApptList].')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmGroupByFamily','0')";
			Db.NonQ(command);
			command="ALTER TABLE orthochart ADD ProvNum bigint NOT NULL";
			Db.NonQ(command);
		}//End of 20_5_2() method

		private static void To20_5_3() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeExcludeExistingLateCharges','0')";
			Db.NonQ(command);
			command="ALTER TABLE statementprod ADD DocNum bigint NOT NULL,ADD INDEX (DocNum)";
			Db.NonQ(command);
			command="ALTER TABLE emailaddress ADD DownloadInbox tinyint NOT NULL";
			Db.NonQ(command);
		}//End of 20_5_3() method

		private static void To20_5_4() {
			string command;
			DataTable table;
			command=$"SELECT * FROM ebill WHERE ElectPassword!=''";
			table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				CDT.Class1.Encrypt(row["ElectPassword"].ToString(),out string encPW);
				long ebillNum=PIn.Long(row["EbillNum"].ToString());
				command=$"UPDATE ebill SET ElectPassword='{POut.String(encPW)}' WHERE EbillNum={POut.Long(ebillNum)}";
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS certemployee";
			Db.NonQ(command);
			command=@"CREATE TABLE certemployee (
				CertEmployeeNum bigint NOT NULL auto_increment PRIMARY KEY,
				CertNum bigint NOT NULL,
				EmployeeNum bigint NOT NULL,
				DateCompleted date NOT NULL DEFAULT '0001-01-01',
				Note varchar(255) NOT NULL,
				UserNum bigint NOT NULL,
				INDEX(UserNum),
				INDEX (CertNum),
				INDEX (EmployeeNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			long groupNum;
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				+"VALUES("+POut.Long(groupNum)+",203)";//203 - Permission to update Employee Certifications.
				Db.NonQ(command);
			}
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				+"VALUES("+POut.Long(groupNum)+",204)";//204 - Permission to set up Certifications.
				Db.NonQ(command);
			}
		}//End of 20_5_4() method

		private static void To20_5_5() {
			string command;
			DataTable table;
			command="ALTER TABLE cert ADD IsHidden tinyint NOT NULL";
			Db.NonQ(command);
			if(!ColumnExists(GetCurrentDatabase(),"deposit","IsSentToQuickBooksOnline")) {
				command="ALTER TABLE deposit ADD IsSentToQuickBooksOnline tinyint NOT NULL";
				Db.NonQ(command);
			}
		}//End of 20_5_5() method

		private static void To20_5_11() {
			string command="ALTER TABLE carecreditwebresponse ADD HasLogged tinyint NOT NULL";
			Db.NonQ(command);
			//Allen says this is what we want to do.
			command=@"SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=15";//SheetTypeEnum.Statement
			long sheetDefNumDefault=PIn.Long(Db.GetScalar(command));
			command=$@"UPDATE preference SET ValueString={POut.Long(sheetDefNumDefault)} WHERE PrefName='SheetsDefaultStatement'";
			Db.NonQ(command);	
			command=$@"UPDATE preference SET ValueString={POut.Long(sheetDefNumDefault)} WHERE PrefName='SheetsDefaultReceipt'";
			Db.NonQ(command);	
			command=$@"UPDATE preference SET ValueString={POut.Long(sheetDefNumDefault)} WHERE PrefName='SheetsDefaultInvoice'";
			Db.NonQ(command);	
			command=$@"UPDATE preference SET ValueString={POut.Long(sheetDefNumDefault)} WHERE PrefName='SheetsDefaultLimited'";
			Db.NonQ(command);
		}

		private static void To20_5_13() {
			string command;
			command="SELECT program.Path FROM program WHERE ProgName='DentalEye'";
			string programPath=Db.GetScalar(command);//only one path
			string newPath=programPath.Replace("DentalEye.exe","CmdLink.exe");//shouldn't launch from this exe anymore
			command="UPDATE program SET Path='"+POut.String(newPath)+"' WHERE ProgName='DentalEye'";
			Db.NonQ(command);
			string note="Please set the file path to open CmdLink.exe in order to send patient data to DentalEye. Ex: C:\\Program Files (x86)\\DentalEye\\CmdLink.exe.";
			command="UPDATE program SET Note='"+POut.String(note)+"' WHERE ProgName='DentalEye'";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('BillingElectIncludeAdjustDescript',1)";
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE preference.PrefName='PdfLaunchWindow';";
			if(PIn.Int(Db.GetCount(command))==0) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('PdfLaunchWindow','0');";//false by default
				Db.NonQ(command);
			}
		}//End of 20_5_13() method

		private static void To20_5_17() {
			string command;
			command="ALTER TABLE cert ADD CertCategoryNum bigint NOT NULL";
			Db.NonQ(command);
			command="UPDATE cert SET CertCategoryNum=(SELECT CertCategoryNum FROM certlinkcategory WHERE cert.CertNum=certlinkcategory.CertNum)";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS certlinkcategory";
			Db.NonQ(command);
		}//End of 20_5_17() method

		private static void To20_5_32() {
			string command;
			//alertcategorylink.AlertCategoryNum for SBs may have been set incorrectly in 20.5.1
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='SupplementalBackups' AND IsHQCategory=1";
			long alertCatNum=Db.GetLong(command);
			command=$@"UPDATE alertcategorylink SET AlertCategoryNum={POut.Long(alertCatNum)} WHERE AlertType=23";//23=SupplementalBackups
			Db.NonQ(command);
			command="UPDATE alertitem SET ClinicNum=-1 WHERE Type=23 AND ClinicNum=0";//23=SupplementalBackups
			Db.NonQ(command);
		}//End of 20_5_32() method

		private static void To20_5_33() {
			string command;
			DoseSpotSelfReportedInvalidNote();
		}

		private static void To20_5_34() {
			string command;
			DataTable table;
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=149";//Currently has Payment Plan Edit permissions
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				   +"VALUES("+POut.Long(groupNum)+",208)";//208 - Will be given Payment Plan Charge Date Edit permissions
				Db.NonQ(command);
			}
		}

		private static void To20_5_38() {
			string command;
			command="UPDATE program SET Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PracticeManagementIntegration\DentalDesktopCmd.exe")+"' "
				+"WHERE Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PMSIntegration\DentalDesktopCmd.exe")+"' "
				+"AND ProgName='"+POut.String("ThreeShape")+"'";
			Db.NonQ(command);
		}//End of 20_5_38() method

		private static void To20_5_41() {
			string command;
			command="SELECT program.Path FROM program WHERE ProgName='DentalEye'";
			string programPath=Db.GetScalar(command);//only one path
			if(programPath.Contains("DentalEye.exe\\CmdLink.exe")) {//this isn't a valid path, so fix it
				programPath=programPath.Replace("\\DentalEye.exe","");
				command="UPDATE program SET Path='"+POut.String(programPath)+"' WHERE ProgName='DentalEye'";
				Db.NonQ(command);
			}
		}//End of 20_5_41() method

		private static void To20_5_48() {
			string command;
			if(!IndexExists("claim","PatNum,ClaimStatus,ClaimType")) {
				command="ALTER TABLE claim ADD INDEX PatStatusType (PatNum,ClaimStatus,ClaimType)";
				List<string> listIndexNames=GetIndexNames("claim","PatNum");
				if(listIndexNames.Count>0) {
					command+=","+string.Join(",",listIndexNames.Select(x => $"DROP INDEX {x}"));
				}
				Db.NonQ(command);
			}
		}

		private static void To20_5_57() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraRefreshOnLoad','1')";//Default to true.
			Db.NonQ(command);
		}//End of 20_5_57() method

		private static void To20_5_61() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraStrictClaimMatching','0')"; //Default to false.
			Db.NonQ(command);
		}//End of 20_5_61() method

		private static void To20_5_65() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraShowStatusAndClinic','1')"; //Default to true.
			Db.NonQ(command);
		}//End of 20_5_65() method

		private static void To21_1_1() {
			string command;
			DataTable table;
			//Set the ExistingPat 2FA prefs pref to defautl vals for all clinics
			command="SELECT ClinicNum FROM clinic";
			List<long> listClinicNums=Db.GetListLong(command);
			if(listClinicNums.Count>0) {
				foreach(long clinicNum in listClinicNums) {
					command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'WebSchedExistingPatDoAuthEmail','1')"; //Default to true
					Db.NonQ(command);
					command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'WebSchedExistingPatDoAuthText','0')"; //Default to false
					Db.NonQ(command);
				}
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedExistingPatDoAuthEmail','1')"; //Default to true
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedExistingPatDoAuthText','0')"; //Default to false
			Db.NonQ(command);
			command="ALTER TABLE payplan ADD DynamicPayPlanTPOption tinyint NOT NULL";//will default to 0 'None' which should be ok since 1 will get set once TP added and OK clicked.
			Db.NonQ(command);
			if(!IndexExists("smsfrommobile","ClinicNum,SmsStatus,IsHidden")) {
				command="ALTER TABLE smsfrommobile ADD INDEX ClinicStatusHidden (ClinicNum,SmsStatus,IsHidden)";
				List<string> listIndexesToDrop=GetIndexNames("smsfrommobile","ClinicNum");
				if(!listIndexesToDrop.IsNullOrEmpty()) {
					command+=","+string.Join(",",listIndexesToDrop.Select(x => "DROP INDEX "+x));
				}
				Db.NonQ(command);
			}
			command="ALTER TABLE tsitranslog CHANGE DemandType ServiceType TINYINT NOT NULL";
			Db.NonQ(command);
			command="UPDATE program SET ProgDesc='Central Data Storage from centraldatastorage.com' "
				+"WHERE ProgName='CentralDataStorage' "
				+"AND ProgDesc='Cental Data Storage from centraldatastorage.com'";
			Db.NonQ(command);
			command="ALTER TABLE discountplan ADD PlanNote text NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE discountplansub ADD SubNote text NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailHostingUseNoReply','1')";//default to true.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailSecureStatus','0')";//default to NotActivated or Enabled.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('Ins834IsEmployerCreate','1')"; //Default to true
			Db.NonQ(command);
			command="ALTER TABLE document MODIFY COLUMN Note MEDIUMTEXT NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD TemplateFailureAutoReply text NOT NULL";
			Db.NonQ(command);
			command="UPDATE apptreminderrule SET TemplateFailureAutoReply='There was an error confirming your appointment with [OfficeName]."
				+" Please call [OfficePhone] to confirm.' WHERE TypeCur=1"; //1 - ConfirmationFutureDay
			Db.NonQ(command);
			command="ALTER TABLE discountplan ADD ExamFreqLimit int NOT NULL,ADD XrayFreqLimit int NOT NULL,ADD ProphyFreqLimit int NOT NULL,"
				+"ADD FluorideFreqLimit int NOT NULL,ADD PerioFreqLimit int NOT NULL,ADD LimitedExamFreqLimit int NOT NULL,ADD PAFreqLimit int NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanExamCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanXrayCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanProphyCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanFluorideCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanPerioCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanLimitedCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanPACodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('CloudAllowedIpAddresses','')";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.Long(groupNum)+",206)";
				 Db.NonQ(command);
			}
			command="TRUNCATE TABLE eservicelog";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN SheetNum";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN PayNum";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN AptNum";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN EServiceOperation";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN EServiceCode";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD EServiceType tinyint";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD EServiceAction smallint";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD KeyType smallint";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD LogGuid VARCHAR(16)";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD ClinicNum bigint";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD FKey bigint";
			Db.NonQ(command);
			if(!IndexExists("eservicelog","LogDateTime,ClinicNum")) {
				command="ALTER TABLE eservicelog ADD INDEX ClinicDateTime (LogDateTime,ClinicNum)";
				List<string> listIndexesToDrop=GetIndexNames("eservicelog","ClinicNum");
				if(!listIndexesToDrop.IsNullOrEmpty()) {
					command+=","+string.Join(",",listIndexesToDrop.Select(x => "DROP INDEX "+x));
				}
				Db.NonQ(command);
			}
			command="ALTER TABLE discountplan ADD AnnualMax double NOT NULL DEFAULT -1";
			Db.NonQ(command);
		}//End of 21_1_1() method

		private static void To21_1_3() {
			string command;
			DataTable table;
			//This was done way back in 16.4 but the SheetsDefaultTreatmentPlan preference did not get fully implemented.
			//This commit is to officially implement SheetsDefaultTreatmentPlan (along with clinic specific overrides for the pref).
			//Therefore, the current value in the preference table needs to be updated with the most recent SheetDefNum that would be currently used.
			//The following code mimics the behavior of SheetDefs.GetInternalOrCustom() which is being used for TP sheets at the time of this commit.
			//E.g. ListSheetDefs.OrderBy(x => x.Description).ThenBy(x => x.SheetDefNum).FirstOrDefault()
			command=@"SELECT SheetDefNum
				FROM sheetdef 
				WHERE SheetType=17
				ORDER BY Description,SheetDefNum
				LIMIT 1";
			table=Db.GetTable(command);//GetScalar won't work with this particular query because it may not return a row (no custom sheet def).
			long treatmentPlanSheetDefNum=0;
			if(table.Rows.Count > 0) {
				treatmentPlanSheetDefNum=PIn.Long(table.Rows[0]["SheetDefNum"].ToString());
			}
			command=$@"UPDATE preference SET ValueString='{POut.Long(treatmentPlanSheetDefNum)}' WHERE PrefName='SheetsDefaultTreatmentPlan'";
			Db.NonQ(command);
		}

		private static void To21_1_5() {
			string command;
			DataTable table;
			command="UPDATE alertitem SET ClinicNum=-1 WHERE Type=23 AND ClinicNum=0";//23=SupplementalBackups
			Db.NonQ(command);
		}

		private static void To21_1_6() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EraAutomationBehavior','1')";//1 - EraAutomationMode.ReviewAll by default.
			Db.NonQ(command);
			command="ALTER TABLE carrier ADD EraAutomationOverride tinyint NOT NULL";//0 - EraAutomationMode.UseGlobal by default.
			Db.NonQ(command);
			DoseSpotSelfReportedInvalidNote();
			command="SELECT preference.ValueString FROM preference WHERE preference.PrefName IN ('AppointmentTimeArrivedTrigger')";
			List<long> listDefNums=Db.GetListLong(command,hasExceptions:false).Where(x => x!=0).Distinct().ToList();
			string defNums=string.Join(",",listDefNums);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('ApptConfirmByodEnabled','"+defNums+"')";
			Db.NonQ(command);
			command="SELECT preference.ValueString FROM preference WHERE preference.PrefName IN ('AppointmentTimeArrivedTrigger',"
				+"'AppointmentTimeSeatedTrigger','AppointmentTimeDismissedTrigger')";
			listDefNums=Db.GetListLong(command,hasExceptions:false).Where(x => x!=0).Distinct().ToList();
			defNums=string.Join(",",listDefNums);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('ApptConfirmExcludeEclipboard','"+defNums+"')";
			Db.NonQ(command);
			//Set default to 'Legacy' to set the column value without updating the timestamp.
			command="ALTER TABLE emailmessage ADD MsgType varchar(255) NOT NULL DEFAULT 'Legacy',ADD FailReason varchar(255) NOT NULL";
			Db.NonQ(command);
			//Set default back to ''
			command="ALTER TABLE emailmessage MODIFY MsgType varchar(255) NOT NULL";
			Db.NonQ(command);
		}//End of 21_1_6() method

		private static void To21_1_9() {
			string command;
			DataTable table;
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=208";
			table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=149";//Currently has Payment Plan Edit permissions
				table=Db.GetTable(command);
				long groupNum;
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",208)";//208 - Will be given Payment Plan Charge Date Edit permissions
					Db.NonQ(command);
				}
			}
		}//End of 21_1_9() method

		private static void To21_1_13() {
			string command;
			command="SELECT DISTINCT ClinicNum FROM clinicpref";
			List<long> listClinicNums=Db.GetListLong(command);
			for(int i=0;i<listClinicNums.Count;i++) {
				command=$@"SELECT ClinicPrefNum FROM clinicpref WHERE PrefName='WebSchedExistingPatDoAuthEmail' AND ClinicNum={POut.Long(listClinicNums[i])}";
				List<long> listClinicPrefNums=Db.GetListLong(command);
				if(listClinicPrefNums.Count>1) {
					//We will not delete the last one.
					for(int j=0;j<listClinicPrefNums.Count-1;j++) {
						command=@$"DELETE FROM clinicpref WHERE ClinicPrefNum={POut.Long(listClinicPrefNums[j])}";
						Db.NonQ(command);
					}
				}
				command=$@"SELECT ClinicPrefNum FROM clinicpref WHERE PrefName='WebSchedExistingPatDoAuthText' AND ClinicNum={POut.Long(listClinicNums[i])}";
				listClinicPrefNums=Db.GetListLong(command);
				if(listClinicPrefNums.Count>1) {
					//We will not delete the last one.
					for(int j=0;j<listClinicPrefNums.Count-1;j++) {
						command=@$"DELETE FROM clinicpref WHERE ClinicPrefNum={POut.Long(listClinicPrefNums[j])}";
						Db.NonQ(command);
					}
				}
			}
			command="UPDATE program SET Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PracticeManagementIntegration\DentalDesktopCmd.exe")+"' "
				+"WHERE Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PMSIntegration\DentalDesktopCmd.exe")+"' "
				+"AND ProgName='"+POut.String("ThreeShape")+"'";
			Db.NonQ(command);
		}//End of 21_1_13() method

		private static void To21_1_16() {
			string command;
			DataTable table;
			if(!IndexExists("payment","PayType")) {
				command="ALTER TABLE payment ADD INDEX (PayType)";//FK to definition.DefNum with category 10
				Db.NonQ(command);
			}
			command="SELECT program.Path FROM program WHERE ProgName='DentalEye'";
			string programPath=Db.GetScalar(command);//only one path
			if(programPath.Contains("DentalEye.exe\\CmdLink.exe")) {//this isn't a valid path, so fix it
				programPath=programPath.Replace("\\DentalEye.exe","");
				command="UPDATE program SET Path='"+POut.String(programPath)+"' WHERE ProgName='DentalEye'";
				Db.NonQ(command);
			}
		}//End of 21_1_16() method

		private static void To21_1_22() {
			string command;
			if(!IndexExists("claim","PatNum,ClaimStatus,ClaimType")) {
				command="ALTER TABLE claim ADD INDEX PatStatusType (PatNum,ClaimStatus,ClaimType)";
				List<string> listIndexNames=GetIndexNames("claim","PatNum");
				if(listIndexNames.Count>0) {
					command+=","+string.Join(",",listIndexNames.Select(x => $"DROP INDEX {x}"));
				}
				Db.NonQ(command);
			}
		}//End of 21_1_22() method

		private static void To21_1_28() {
			//Update existing discountplans with all frequency limitations set to 0, to have unlimited frequency limitation.
			string command=$@"UPDATE discountplan
					SET ExamFreqLimit=-1,XrayFreqLimit=-1,ProphyFreqLimit=-1,FluorideFreqLimit=-1,PerioFreqLimit=-1,LimitedExamFreqLimit=-1,PAFreqLimit=-1
					WHERE ExamFreqLimit=0 AND XrayFreqLimit=0 AND ProphyFreqLimit=0 AND FluorideFreqLimit=0 AND PerioFreqLimit=0 AND LimitedExamFreqLimit=0 AND PAFreqLimit=0";
			Db.NonQ(command);
		}//End of 21_1_28() method

		private static void To21_1_31() {
			string command="SELECT * FROM preference WHERE PrefName='EraRefreshOnLoad'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraRefreshOnLoad','1')";//Default to true.
				Db.NonQ(command);
			}
		}//End of 21_1_31() method

		private static void To21_1_35() {
			string command="SELECT * FROM preference WHERE PrefName='EraStrictClaimMatching'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraStrictClaimMatching','0')"; //Default to false.
				Db.NonQ(command);
			}
		}//End of 21_1_35() method

		private static void To21_1_37() {
			string command="SELECT * FROM preference WHERE PrefName='EraShowStatusAndClinic'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraShowStatusAndClinic','1')"; //Default to true.
				Db.NonQ(command);
			}
		}//End of 21_1_37() method

		private static void To21_2_1() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailDefaultSendPlatform','Secure')";//Defaults to SecureEmail(EmailHosting)
			Db.NonQ(command);
			command="ALTER TABLE payplancharge ADD IsOffset tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE eclipboardsheetdef ADD MinAge INT NOT NULL DEFAULT -1";
			Db.NonQ(command);
			command="ALTER TABLE eclipboardsheetdef ADD MaxAge INT NOT NULL DEFAULT -1";
			Db.NonQ(command);
			command="ALTER TABLE apptview ADD WaitingRmName tinyint NOT NULL";
			Db.NonQ(command);
			//This set of code will get and encrypt third party passwords that are stored in the database as plaintext.
			//XVWeb, Appriss Client, and XCharge passwords have already been encrypted.
			command=$"SELECT ProgramNum from program WHERE ProgName In ('XVWeb','Xcharge','SFTP')";
			List<long> listProgNums=Db.GetListLong(command);
			string listStrPrognums=string.Join(",",listProgNums);
			command=$"SELECT ProgramPropertyNum,PropertyValue from programproperty WHERE IsMasked=1 " +//Find all passwords
				$"AND PropertyValue!='' "	+																															 //that have a value
				$"AND PropertyDesc!='Appriss Client Password' " +																				 //aren't the client key password for Appriss
				$"AND ProgramNum NOT IN ({listStrPrognums})";																					   //and aren't in our list of programnums
			table=Db.GetTable(command);
			long progPropertyNum;
			string password;
			string obfuscatedPassword="";
			for(int i=0;i<table.Rows.Count;i++) {
				progPropertyNum=PIn.Long(table.Rows[i]["ProgramPropertyNum"].ToString());
				password=PIn.String(table.Rows[i]["PropertyValue"].ToString());
				try {
					if(CDT.Class1.Decrypt(password,out _) || !CDT.Class1.Encrypt(password,out obfuscatedPassword)) {
						continue;
					}
					command=$@"UPDATE programproperty SET PropertyValue='{obfuscatedPassword}' WHERE ProgramPropertyNum={POut.Long(progPropertyNum)}";
					Db.NonQ(command);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
			command="DROP TABLE IF EXISTS transactioninvoice";
			Db.NonQ(command);
			command=@"CREATE TABLE transactioninvoice (
					TransactionInvoiceNum bigint NOT NULL auto_increment PRIMARY KEY,
					FileName varchar(255) NOT NULL,
					InvoiceData text NOT NULL
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="ALTER TABLE transaction ADD TransactionInvoiceNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE transaction ADD INDEX (TransactionInvoiceNum)";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog MODIFY LogGuid VARCHAR(36) NOT NULL";
			Db.NonQ(command);
			command="UPDATE program SET Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PracticeManagementIntegration\DentalDesktopCmd.exe")+"' "
				+"WHERE Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PMSIntegration\DentalDesktopCmd.exe")+"' "
				+"AND ProgName='"+POut.String("ThreeShape")+"'";
			Db.NonQ(command);
			command="ALTER TABLE referral ADD BusinessName varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE referral ADD DisplayNote varchar(4000) NOT NULL";
			Db.NonQ(command);
			command="ALTER table transactioninvoice MODIFY COLUMN InvoiceData mediumtext";
			Db.NonQ(command);
			//This set of code will encrypt HL7 passwords (each location has it's own HL7 table row, so there may be multiple).
			command=@"SELECT HL7DefNum,SftpPassword
				FROM hl7def
				WHERE SftpPassword!=''";//Don't have to set if password has no value.
			table=Db.GetTable(command);
			long hl7DefNum;
			for(int i=0;i<table.Rows.Count;i++) {
				hl7DefNum=PIn.Long(table.Rows[i]["HL7DefNum"].ToString());
				password=PIn.String(table.Rows[i]["SftpPassword"].ToString());
				try {
					if(CDT.Class1.Decrypt(password,out _) || !CDT.Class1.Encrypt(password,out obfuscatedPassword)) {
						continue;
					}
					command=$@"UPDATE hl7def SET SftpPassword='{obfuscatedPassword}' WHERE HL7DefNum={POut.Long(hl7DefNum)}";
					Db.NonQ(command);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
			//This set of code will update abbreviations for 2020 and 2021 ADA codes if the user has already used D code tools.
			//A list of the codes we need to add abbreviations for.
			List<string> listStrCodes=new List<string>() {"D0419","D1551","D1552","D1553","D1556","D1557","D1558","D2753","D5284","D5286","D6082","D6083",
				"D6084","D6086","D6087","D6088","D6097","D6098","D6099","D6120","D6121","D6122","D6123","D6195","D6243","D6753","D6784","D7922","D8696","D8697",
				"D8698","D8699","D8701","D8702","D8703","D8704","D9997","D0604","D0605","D0701","D0702","D0703","D0704","D0705","D0706","D0707","D0708","D0709",
				"D1321","D1355","D2928","D3471","D3472","D3473","D3501","D3502","D3503","D5995","D5996","D6191","D6192","D7961","D7962","D7993","D7994"};
			string strCodes=string.Join(",",listStrCodes.Select(x => $"'{POut.String(x)}'"));
			List<string> listAbbrs=new List<string>() {"SLFL","RBSMMAX","RBSMMAN","REMFSMQ","REMFUSMQ","REMFSPMAX","REMFBSMAN","PFMT","RPDUFQ","RPDURQ",
				"IMBPFMB","IMPPFMN","IMPPFMT","IMPFMCB","IMPFMCN","IMPFMCT","ABUFPMT","IMPRETPFMB","IMPRETPFMN","IMPRETPFMT","IMPRETFMCB","IMPRETFMCN",
				"IMPRETFMCT","ABURETPFMT","PONPFMT","RETPFMT","3/4RETFMCT","SOCKMED","ORREPAIRMAX","ORREPAIRMAN","ORRECMAX","ORRECMAN","REFRETMAX","REFRETMAN",
				"REPLRETMAX","REPLRETMAN","CASEMANAGE","Antig","Antib","PanC","CephC","2DC","3DC","EOC","OCCC","PAC","BWXC","FMXC","ConSA","CPM","PCR","RRA",
				"RRB","RRM","SERA","SERB","SERM","PMCU","PMCL","SPABPL","SMATPL","FRENB","FRENL","CRANIMP","ZYGIMP"};
			command=$@"SELECT CodeNum,AbbrDesc,ProcCode FROM procedurecode
				WHERE ProcCode IN ({strCodes})";  
			table=Db.GetTable(command);
			long codeNum;
			string abbrDesc;
			string procCode;
			int index;
			for(int i=0;i<table.Rows.Count;i++) {
				codeNum=PIn.Long(table.Rows[i]["CodeNum"].ToString());
				abbrDesc=PIn.String(table.Rows[i]["AbbrDesc"].ToString());
				procCode=PIn.String(table.Rows[i]["ProcCode"].ToString());
				if(!abbrDesc.IsNullOrEmpty()) {//Customers can add their own abbreviations, so skip abbreviation if not blank.
					continue;
				}
				index=listStrCodes.IndexOf(procCode);//Get index of code
				if(index==-1) {
					continue;
				}
				command=$@"UPDATE procedurecode SET AbbrDesc='{POut.String(listAbbrs[index])}' WHERE CodeNum={POut.Long(codeNum)}";
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS referralcliniclink";
			Db.NonQ(command);
			command=@"CREATE TABLE referralcliniclink (
				ReferralClinicLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
				ReferralNum bigint NOT NULL,
				ClinicNum bigint NOT NULL,
				INDEX(ReferralNum),
				INDEX(ClinicNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardDoTwoFactorAuth','0')"; //Default to false
			Db.NonQ(command);
			if(!IndexExists("fee","FeeSched,CodeNum,ClinicNum,ProvNum")) {//may have been added manually
				command="ALTER TABLE fee ADD INDEX FeeSchedCodeClinicProv (FeeSched,CodeNum,ClinicNum,ProvNum)";
				List<string> listIndexNames=GetIndexNames("fee","FeeSched");
				if(listIndexNames.Count>0) {
					command+=","+string.Join(",",listIndexNames.Select(x => $"DROP INDEX {x}"));
				}
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS hieclinic";
			Db.NonQ(command);
			command=@"CREATE TABLE hieclinic (
					HieClinicNum bigint NOT NULL auto_increment PRIMARY KEY,
					ClinicNum bigint NOT NULL,
					SupportedCarrierFlags tinyint NOT NULL,
					PathExportCCD varchar(255) NOT NULL,
					TimeOfDayExportCCD bigint NOT NULL,
					IsEnabled tinyint NOT NULL,
					INDEX(ClinicNum),
					INDEX(TimeOfDayExportCCD)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS hiequeue";
			Db.NonQ(command);
			command=@"CREATE TABLE hiequeue (
					HieQueueNum bigint NOT NULL auto_increment PRIMARY KEY,
					PatNum bigint NOT NULL,
					INDEX(PatNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//Set of code to disable Dental Intel.
			command=$"SELECT ProgramNum from program WHERE ProgName='DentalIntel'";
			long progNum=Db.GetLong(command);
			command=$@"UPDATE program SET Enabled=0,IsDisabledByHQ=1 WHERE ProgramNum={POut.Long(progNum)}";
			Db.NonQ(command);//Disable the program, since this will not appear anywhere anymore.
			command=$@"UPDATE programproperty SET PropertyValue='1'
				WHERE ProgramNum={POut.Long(progNum)}
				AND PropertyDesc='Disable Advertising HQ'";//Set 'Disable Advertising HQ' affiliated to Dental Intel to true.
			Db.NonQ(command);
			//Insert RayBridge bridge (new version of SMARTDent)----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				+") VALUES(" 
				+"'RayBridge', " 
				+"'SMARTDent New from www.raymedical.com', " 
				+"'0', " 
				+"'"+POut.String(@"C:\Ray\RayBridge\RayBridge.exe")+"', " 
				+"'"+POut.String(@"")+"', "//leave blank if none 
				+"'')"; 
			long programNum=Db.NonQ(command,true); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				+"'0')"; 
			Db.NonQ(command); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				+") VALUES("
				+"'"+POut.Long(programNum)+"',"
				+"'Xml output file path',"
				+"'"+POut.String(@"C:\Ray\PatientInfo.xml")+"'" 
				+")";
			Db.NonQ(command);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				+"VALUES (" 
				+"'"+POut.Long(programNum)+"', " 
				+"'2', "//ToolBarsAvail.ChartModule 
				+"'SmartDent')"; 
			Db.NonQ(command); 
			//end RayBridge bridge
			command="SELECT ValueString FROM preference WHERE PrefName='PracticePhone'";
			string practicePhone=Db.GetScalar(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PracticeBillingPhone','"+POut.String(practicePhone)+"')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PracticePayToPhone','"+POut.String(practicePhone)+"')";
			Db.NonQ(command);
			//Insert VisionX bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				+") VALUES("
				+"'VisionX', "
				+"'VisionX from www.airtechniques.com', "
				+"'0', "
				+"'"+POut.String(@"C:\Program Files\Air Techniques\VisionX\Clients\VisionX.exe")+"', "
				+"'', "
				+"'"+POut.String(@"No command line or path is needed.")+"')";
			programNum=Db.NonQ(command,true);//we now have a ProgramNum to work with
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				+") VALUES("
				+"'"+programNum.ToString()+"', "
				+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				+"'0')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				+") VALUES("
				+"'"+programNum.ToString()+"', "
				+"'Text file path', "
				+"'"+POut.String(@"C:\ProgramData\Air Techniques\VisionX\WorkstationService\Examination\DBSWINLegacySupport\patimport.txt")+"')";
			Db.NonQ(command);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				+"VALUES ("
				+"'"+programNum.ToString()+"', "
				+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
				+"'VisionX')";
			Db.NonQ(command);
			//end VisionX bridge
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ADPRunIID','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('RecurringChargesShowInactive','0')";
			Db.NonQ(command);string upgrading="Upgrading database to version: 21.2.1";
			ODEvent.Fire(ODEventType.ConvertDatabases,upgrading);//No translation in convert script.
			//New alert categorylink, Update Complete - Action Required
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes'";
			long alertCatNum=Db.GetLong(command);
			command=$@"INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES({POut.Long(alertCatNum)},32)";//32=Update
			Db.NonQ(command);
			command="ALTER TABLE carrier ADD OrthoInsPayConsolidate tinyint NOT NULL";//0 - OrthoInsPayConsolidate.Global by default.
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS cloudaddress";
			Db.NonQ(command);
			command=@"CREATE TABLE cloudaddress (
				CloudAddressNum bigint NOT NULL auto_increment PRIMARY KEY,
				IpAddress varchar(50) NOT NULL,
				UserNumLastConnect bigint NOT NULL,
				DateTimeLastConnect datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				INDEX(UserNumLastConnect)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT ValueString FROM preference WHERE PrefName='CloudAllowedIpAddresses'";
			string str=Db.GetScalar(command);
			string[] addresses=str.Split(",",StringSplitOptions.RemoveEmptyEntries);
			if(addresses.Length>0) {//If the database has no allowed addresses then we don't need to insert any into the new table.
				command=$"INSERT INTO cloudaddress (IpAddress) Values {string.Join(",",addresses.Select(x => "('"+POut.String(x)+"')"))}";
				Db.NonQ(command);
			}
			//Sync Patient Portal Invites into Automated Messaging preference
			command="SELECT ClinicNum,IsConfirmEnabled,IsConfirmDefault FROM clinic";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				long clinicNum=PIn.Long(table.Rows[i]["ClinicNum"].ToString());
				bool isAutoCommEnabled=PIn.Bool(table.Rows[i]["IsConfirmEnabled"].ToString());
				bool isConfirmDefault=PIn.Bool(table.Rows[i]["IsConfirmDefault"].ToString());
				command="SELECT ValueString FROM clinicpref WHERE PrefName='PatientPortalInviteEnabled' AND ClinicNum="+POut.Long(clinicNum);
				string clinicPrefValueString=Db.GetScalar(command);
				bool isPatientPortalInviteEnabled=PIn.Bool(clinicPrefValueString);
				command="SELECT COUNT(*) FROM clinicpref WHERE PrefName='PatientPortalInviteUseDefaults' AND ClinicNum="+POut.Long(clinicNum);
				long count=Db.GetLong(command);
				//If the PatientPortalInviteUseDefaults clinicpref doesn't exist, create it for this clinic.
				//On previous versions, the absence of this preference set use defaults to true. Starting with this version, use defaults will be false if the clinicpref doesn't exist.
				//We need to create this clinicpref to preserve old logic.
				if(count==0) {
					command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'PatientPortalInviteUseDefaults','1')";
					Db.NonQ(command);
				}
				//If automated messaging is enabled but patient portal invites are not, disable all patient portal invite rules for that clinic
				if(isAutoCommEnabled && !isPatientPortalInviteEnabled) {
					//Disable all of the rules for patient portal invite for the clinic
					command="UPDATE apptreminderrule SET IsEnabled=0 WHERE TypeCur='3'" //ApptReminderType.PatientPortalInvite
						+" AND ClinicNum="+POut.Long(clinicNum);
					Db.NonQ(command);
					//Set patient portal invite use defaults to false for the clinic
					command="UPDATE clinicpref SET ValueString='0' WHERE PrefName='PatientPortalInviteUseDefaults'"
						+" AND ClinicNum="+POut.Long(clinicNum);
					Db.NonQ(command);
				}
				//If automated messaging is not enabled but patient portal invites are, disable all other automated messaging rules for that clinic except for birthdays.
				else if(!isAutoCommEnabled && isPatientPortalInviteEnabled) {
					command="UPDATE apptreminderrule SET IsEnabled=0 WHERE TypeCur NOT IN('3','6')" //ApptReminderType.PatientPortalInvite, ApptReminderType.Birthday
						+" AND ClinicNum="+POut.Long(clinicNum);
					Db.NonQ(command);
					//Turn on autocomm for this clinic, which will allow invites to remain on. We will turn off all other varieties of autocomm below.
					command="UPDATE clinic SET IsConfirmEnabled='1' WHERE ClinicNum="+POut.Long(clinicNum);
					Db.NonQ(command);
					//Set 'UseDefaults' to be false when created.
					isConfirmDefault=false;
				}
				//Create the 'UseDefaults' preferences.
				command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'ApptArrivalUseDefaults','"+POut.Bool(isConfirmDefault)+"')";
				Db.NonQ(command);
				command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'ApptConfirmUseDefaults','"+POut.Bool(isConfirmDefault)+"')";
				Db.NonQ(command);
				command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'ApptReminderUseDefaults','"+POut.Bool(isConfirmDefault)+"')";
				Db.NonQ(command);
				command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'ApptThankYouUseDefaults','"+POut.Bool(isConfirmDefault)+"')";
				Db.NonQ(command);
			}
			command="ALTER TABLE mobileappdevice ADD IsBYODDevice TINYINT NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TextPaymentLinkAppointmentBalance','[nameF] please visit this [StatementShortURL] to pay your balance of [StatementBalance] for your recent appointment.')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TextPaymentLinkAccountBalance','[nameF] please visit this [StatementShortURL] to pay your balance of [StatementBalance]')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('EClipboardImageCaptureDefs','')";
			Db.NonQ(command);
			command="SELECT MAX(ItemOrder) FROM definition WHERE Category=18";
			int order=PIn.Int(Db.GetScalar(command))+1;
			command=$"INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES({POut.Long(18)},{POut.Long(order)},'eClipboard','',0,0)";
			Db.NonQ(command);
			//53 - DefCat.eClipboardImageCapture
			command="INSERT INTO definition (Category,ItemName,ItemValue,ItemOrder) VALUES (53,'Photo ID Front','Please take a picture of the front side of your photo ID',0)";
			Db.NonQ(command);
			command="INSERT INTO definition (Category,ItemName,ItemValue,ItemOrder) VALUES (53,'Photo ID Back','Please take a picture of the back side of your photo ID',1)";
			Db.NonQ(command);
			command="INSERT INTO definition (Category,ItemName,ItemValue,ItemOrder) VALUES (53,'Insurance Card Front','Please take a picture of the front side of your insurance card',2)";
			Db.NonQ(command);
			command="INSERT INTO definition (Category,ItemName,ItemValue,ItemOrder) VALUES (53,'Insurance Card Back','Please take a picture of the back side of your insurance card',3)";
			Db.NonQ(command);
			LargeTableHelper.AlterTable("claimproc","ClaimProcNum",new ColNameAndDef("ClaimAdjReasonCodes","varchar(255) NOT NULL"));
			command="ALTER TABLE paysplit ADD PayPlanDebitType tinyint NOT NULL";
			Db.NonQ(command);
			LargeTableHelper.AlterTable("treatplan","TreatPlanNum",new ColNameAndDef("MobileAppDeviceNum","bigint NOT NULL"),new IndexColsAndName("MobileAppDeviceNum",""));
			try {
				if(IsUsingReplication()) {
					string replicationMonitorMsg="Monitoring the slave status is now monitored by the OpenDentalReplicationService. "
					+"Each replication server will need the new OpenDentalReplicationService installed. "
					+"Please visit https://opendental.com and search for 'Slave Monitor' for more information.";
					command=$"INSERT INTO alertitem (ClinicNum,Description,Type,Severity,Actions,FormToOpen,FKey,ItemValue,UserNum) VALUES (-1,'{POut.String(replicationMonitorMsg)}',33,3,5,0,0,'',0)";
					Db.NonQ(command);
					command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes'";
					alertCatNum=Db.GetLong(command);
					command=$@"INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES({POut.Long(alertCatNum)},33)";//33=ReplicationService warning 
					Db.NonQ(command);
				}
			}
			catch {
				//Do nothing. Treat the office as not using replication.
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('RefundAdjustmentType','0')";
			Db.NonQ(command);
			command="DELETE FROM userodpref WHERE ValueString='' AND FkeyType=0";//FkeyType 0=Definition, expanded imaging categories. ValueString '' meant collapsed, which was meaningless.
			Db.NonQ(command);
			command="UPDATE userodpref SET ValueString='' WHERE FkeyType=0";//Expanded imaging categories were full of meaningless junk strings. No ValueString needed.
			Db.NonQ(command);
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('ApptGeneralMessageAutoEnabled','0')"; //Defaults to disabled
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS apptgeneralmessagesent";
			Db.NonQ(command);
			command=@"CREATE TABLE apptgeneralmessagesent (
				ApptGeneralMessageSentNum bigint NOT NULL auto_increment PRIMARY KEY,
				ApptNum bigint NOT NULL,
				PatNum bigint NOT NULL,
				ClinicNum bigint NOT NULL,
				DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				TSPrior bigint NOT NULL,
				ApptReminderRuleNum bigint NOT NULL,
				SmsSendStatus tinyint NOT NULL,
				EmailSendStatus tinyint NOT NULL,
				INDEX(ApptNum),
				INDEX(PatNum),
				INDEX(ClinicNum),
				INDEX(ApptReminderRuleNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT * FROM preference WHERE PrefName='EraRefreshOnLoad'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraRefreshOnLoad','1')"; //Default to true.
				Db.NonQ(command);
			}
		}//End of 21_2_1() method

		private static void To21_2_2() {
			string command;
			if(CultureInfo.CurrentCulture.Name=="en-US"){
				command="UPDATE procedurecode SET TreatArea=0 "//None
					+"WHERE TreatArea=3";//Mouth. ~337 rows
				Db.NonQ(command);
				command="UPDATE procedurecode SET TreatArea=3 "//Mouth
					+"WHERE TreatArea=0 "//some codes my have been set by user to something like quad, so this won't touch those
					+"AND ProcCode IN('D0330','D0701','D1110','D1120','D1206','D1208','D6190','D7285','D7286','D7287','D7288','D8050','D8060','D8070','D8080','D8090',"
					+"'D8210','D8220','D8660','D8670','D8680','D8690','D8695')";//only 23 codes are ever supposed to be Mouth
				Db.NonQ(command);
			}
			command="ALTER TABLE procedurecode ADD AreaAlsoToothRange tinyint NOT NULL";
			Db.NonQ(command);
			command=$"SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=29";//PaySplitUnearnedType
			int itemOrderNext=PIn.Int(Db.GetScalar(command));
			command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) "
				+$"VALUES (29,'Payment Plan Prepay',{POut.Int(itemOrderNext)},'X')";//29 is PaySplitUnearnedType and X is hidden / Do Not Show.
			long defNum=Db.NonQ(command,true);
			command=$"INSERT INTO preference(PrefName,ValueString) VALUES('DynamicPayPlanPrepaymentUnearnedType','{defNum}')";
			Db.NonQ(command);
		}//End of 21_2_2() method
	
		private static void To21_2_7() {
			string command;
			command="DROP TABLE IF EXISTS treatplanparam";
			Db.NonQ(command);
			command=@"CREATE TABLE treatplanparam (
				TreatPlanParamNum bigint NOT NULL auto_increment PRIMARY KEY,
				PatNum bigint NOT NULL,
				TreatPlanNum bigint NOT NULL,
				ShowDiscount tinyint NOT NULL,
				ShowMaxDed tinyint NOT NULL,
				ShowSubTotals tinyint NOT NULL,
				ShowTotals tinyint NOT NULL,
				ShowCompleted tinyint NOT NULL,
				ShowFees tinyint NOT NULL,
				ShowIns tinyint NOT NULL,
				INDEX(PatNum),
				INDEX(TreatPlanNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
		}//End of 21_2_7() method

		private static void To21_2_8() {
			string command="SELECT * FROM preference WHERE PrefName='EraStrictClaimMatching'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraStrictClaimMatching','0')"; //Default to false.
				Db.NonQ(command);
			}
			command="ALTER TABLE orthochart ADD OrthoChartRowNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE orthochart ADD INDEX (OrthoChartRowNum)";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS orthochartrow";
			Db.NonQ(command);
			command=@"CREATE TABLE orthochartrow (
					OrthoChartRowNum bigint NOT NULL auto_increment PRIMARY KEY,
					PatNum bigint NOT NULL,
					DateTimeService datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					UserNum bigint NOT NULL,
					ProvNum bigint NOT NULL,
					Signature text NOT NULL,
					INDEX(PatNum),
					INDEX(UserNum),
					INDEX(ProvNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT DISTINCT(PatNum) FROM orthochart";
			List<long> listPatNums=Db.GetListLong(command);
			command="SELECT Description,(CASE WHEN InternalName='Signature' THEN 1 ELSE 0 END) isSignature "
				+"FROM displayfield WHERE Category=8 AND InternalName!='Provider'";
			DataTable tableDisplayFields=Db.GetTable(command);
			string fieldNameSig=tableDisplayFields.Select().Where(x => PIn.Bool(x["isSignature"].ToString())).Select(x => PIn.String(x["Description"].ToString())).FirstOrDefault();
			for(int i=0;i<listPatNums.Count;i++) {
				command=$@"SELECT * 
				FROM orthochart
				WHERE PatNum={listPatNums[i]}
				ORDER BY OrthoChartNum";
				DataTable table=Db.GetTable(command);
				//Group orthocharts into groups of PatNum,Prov, and DateService
				List<OrthoObj> listOrthoObjs=table.Select().GroupBy(x => new {
					patnum=PIn.Long(x["PatNum"].ToString()),
					dateservice=PIn.Date(x["DateService"].ToString()),
					provNum=PIn.Long(x["ProvNum"].ToString()),
					FieldName=PIn.String(x["FieldName"].ToString())
				}).GroupBy(x => new {
					patnum=x.Key.patnum,
					dateservice=x.Key.dateservice,
					provnum=x.Key.provNum
				}).Select(x => new OrthoObj() {
					PatNum=x.Key.patnum,
					ProvNum=x.Key.provnum,
					DateTService=x.Key.dateservice,
					ListDataRows=x.Select(y => new OrthoDataRows() {
						FieldName=y.Key.FieldName,
						listRows=y.ToList()
					}).ToList()
				}).ToList();
				for(int j=0;j<listOrthoObjs.Count;j++) {
					OrthoObj orthoObj=listOrthoObjs[j];
					List<DataRow> listDataRowsForOrthoChartRow;
					for(int k=0;k<orthoObj.ListDataRows.Max(x => x.listRows.Count);k++) {
						listDataRowsForOrthoChartRow=new List<DataRow>();
						listDataRowsForOrthoChartRow.AddRange(orthoObj.ListDataRows.Where(x => x.listRows.Count>k).Select(x => x.listRows[k]));
						string sigVal="";
						if(!string.IsNullOrEmpty(fieldNameSig)) {
							DataRow dataRowSig=listDataRowsForOrthoChartRow.FirstOrDefault(x => PIn.String(x["FieldName"].ToString())==fieldNameSig);
							if(dataRowSig!=null) {
								sigVal=dataRowSig["FieldValue"].ToString();
							}
						}
						command=$@"INSERT INTO orthochartrow(PatNum,DateTimeService,UserNum,ProvNum,Signature)
						VALUES({POut.Long(orthoObj.PatNum)},{POut.DateT(orthoObj.DateTService)},{listDataRowsForOrthoChartRow.First()["UserNum"].ToString()},{POut.Long(orthoObj.ProvNum)},'{sigVal}')";
						long orthoChartRowNum=Db.NonQ(command,true);
						command=$@"UPDATE orthochart SET OrthoChartRowNum={POut.Long(orthoChartRowNum)} WHERE OrthoChartNum IN ({string.Join(",",listDataRowsForOrthoChartRow.Select(x => x["OrthoChartNum"].ToString()))})";
						Db.NonQ(command);
					}
				}
			}
		}//End of 21_2_8() method

		private static void To21_2_9() {
			string command="SELECT * FROM preference WHERE PrefName='EraShowStatusAndClinic'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraShowStatusAndClinic','1')"; //Default to true.
				Db.NonQ(command);
			}
		}//End of 21_2_9() method

		private static void To21_2_14() {
			string command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=92";//92 - FeeSchedEdit
			DataTable table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					+"VALUES("+POut.Long(groupNum)+",211)";//211 - AllowFeeEditWhileReceivingClaim
				Db.NonQ(command);
			}
		}//End of 21_2_14() method

		private static void To21_2_20() {
			string command="DROP TABLE IF EXISTS etrans835";
			Db.NonQ(command);
			command=@"CREATE TABLE etrans835 (
				Etrans835Num bigint NOT NULL auto_increment PRIMARY KEY,
				EtransNum bigint NOT NULL,
				PayerName varchar(60) NOT NULL,
				TransRefNum varchar(50) NOT NULL,
				InsPaid double NOT NULL,
				ControlId varchar(9) NOT NULL,
				PaymentMethodCode varchar(3) NOT NULL,
				PatientName varchar(100) NOT NULL,
				Status tinyint NOT NULL,
				INDEX(EtransNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('AsapPromptEnabled','1')";//Default to true to maintain behavior
			Db.NonQ(command);
		}//End of 21_2_20() method

		private static void To21_2_22() {
			string command="SELECT ProgramNum FROM program WHERE ProgName='PreXionAquire'";
			long progNum=Db.GetLong(command);
			command=$"UPDATE program SET ProgName='PreXionAcquire', ProgDesc='PreXion Acquire' WHERE ProgramNum={POut.Long(progNum)}";
			Db.NonQ(command);
			command=$"UPDATE toolbutitem SET ButtonText='PreXion Acquire' WHERE ProgramNum={POut.Long(progNum)} AND ButtonText='PreXion Aquire'";
			Db.NonQ(command);
		}//End of 21_2_22() method

		private static void To21_2_25() {
			string command="INSERT INTO preference(PrefName,ValueString) VALUES('AgingCalculateOnBatchClaimReceipt','0')"; //Default to false
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailHostingEndpoint','https://emailhosting.opendental.cloud:8200/')";
			Db.NonQ(command);
		}//End of the 21_2_25() method

		private static void To21_2_27() {
			string command="ALTER TABLE insplan ADD IsBlueBookEnabled tinyint NOT NULL DEFAULT 1";
			Db.NonQ(command);
			command="UPDATE insplan SET IsBlueBookEnabled=0 WHERE FeeSched>0"; // set all insplans with a fee scheduled to blue book disabled.
			Db.NonQ(command);
		}

		private static void To21_2_28() {
			string command="INSERT INTO preference(PrefName,ValueString) VALUES('ReportsIncompleteProcsExcludeCodes','D9986,D9987')"; //Default to broken appointment codes
			Db.NonQ(command);
		}

		private static void To21_2_30() {
			string command="ALTER TABLE etrans835 ADD AutoProcessed tinyint NOT NULL,ADD IsApproved tinyint NOT NULL";
			Db.NonQ(command);
		}

		private static void To21_2_36() {
			string command;
			if(!IndexExists("mount","PatNum")) {
				command="ALTER TABLE mount ADD INDEX (PatNum)";
				Db.NonQ(command);
			}
			if(!IndexExists("mountitem","MountNum")) {
				command="ALTER TABLE mountitem ADD INDEX (MountNum)";
				Db.NonQ(command);
			}
		}//End of the 21_2_36() method

		private static void To21_2_45() {
			string command;
			if(!IndexExists("document","MountItemNum")) {
				command="ALTER TABLE document ADD INDEX (MountItemNum)";
				Db.NonQ(command);
			}
		}

		private static void To21_2_47() {
			string command;
			//Moving codes to the Obsolete category that were deleted in CDT 2022.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				//Move deprecated codes to the Obsolete procedure code category.
				//Make sure the procedure code category exists before moving the procedure codes.
				string procCatDescript="Obsolete";
				long defNum=0;
				command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
				DataTable dtDef=Db.GetTable(command);
				if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
					command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
					int countCats=PIn.Int(Db.GetCount(command));
						command="INSERT INTO definition (Category,ItemName,ItemOrder) "
								+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
					defNum=Db.NonQ(command,true);
				}
				else { //The procedure code category already exists, get the existing defnum
					defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
				}
				string[] cdtCodesDeleted=new string[] {
					"D4320",
					"D4321",
					"D8050",
					"D8060",
					"D8690"
				};
				//Change the procedure codes' category to Obsolete.
				command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)
					+" WHERE ProcCode IN('"+string.Join("','",cdtCodesDeleted.Select(x => POut.String(x)))+"') ";
				Db.NonQ(command);
			}//end United States CDT codes update
		}

		private static void To21_2_51() {
			//The url we inserted for these programs is no longer valid. This is us fixing that going forward.
			string command="SELECT * FROM program where ProgName='Trophy' OR ProgName='TrophyEnhanced'";
			DataTable listTrophyProgs=Db.GetTable(command);
			for(int i=0;i<listTrophyProgs.Rows.Count;i++) {
				DataRow progCur=listTrophyProgs.Rows[i];
				if(progCur["ProgDesc"].ToString().Contains("from www.trophy-imaging.com")) {
					progCur["ProgDesc"]=progCur["ProgDesc"].ToString().Replace("from www.trophy-imaging.com","");
					long progNum=PIn.Long(progCur["ProgramNum"].ToString());
					command=$"UPDATE program SET ProgDesc='{POut.String(progCur["ProgDesc"].ToString())}' WHERE ProgramNum={POut.Long(progNum)}";
					Db.NonQ(command);
				}
			}
		} 

		private static void To21_2_53() {
			string command;
			//Updating D9613 to default to TreatmentArea.Quad in CDT 2022 (only if user did not change from TreatmentArea.Mouth).
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				command="UPDATE procedurecode SET procedurecode.TreatArea=4 WHERE procedurecode.ProcCode='D9613' AND procedurecode.TreatArea=3";//4 - Quad
				Db.NonQ(command);
			}//end United States CDT codes update
		}//End of 21_2_53()

		private class OrthoObj {
			public long ProvNum;
			public long PatNum;
			public DateTime DateTService;
			public List<OrthoDataRows> ListDataRows;
		}

		private class OrthoDataRows {
			public string FieldName;
			public List<DataRow> listRows;
		}

		private static void To21_3_1() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardHasMultiPageCheckIn','0')"; //Default to false
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD SendMultipleInvites tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD TimeSpanMultipleInvites bigint NOT NULL";
			Db.NonQ(command);
			command="UPDATE apptreminderrule SET TimeSpanMultipleInvites=25920000000000";//30 days in ticks
			Db.NonQ(command);
			//check databasemaintenance for DiscountPlanSubWithInvalidDiscountPlanNum, insert if not there and set IsOld to True or update to set IsOld to true
			command="SELECT MethodName "
				+"FROM databasemaintenance "
				+"WHERE MethodName='DiscountPlanSubWithInvalidDiscountPlanNum'";
			string methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				command="INSERT INTO databasemaintenance "
					+"(MethodName, IsOld) "
					+"VALUES ('DiscountPlanSubWithInvalidDiscountPlanNum',1)";//true by default
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance "
					+"SET IsOld = 1 "
					+"WHERE MethodName='DiscountPlanSubWithInvalidDiscountPlanNum'";//true by default
			}
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb ADD EmailResponse varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE xwebresponse ADD EmailResponse varchar(255) NOT NULL";
			Db.NonQ(command);
			command="SELECT ValueString FROM preference WHERE PrefName='ReportingServerCompName'";
			string reportingServerCompName=Db.GetScalar(command);
			command="SELECT ValueString FROM preference WHERE PrefName='ReportingServerURI'";
			string reportingServerURI=Db.GetScalar(command);
			bool isReportingServerInUse=reportingServerCompName!="" || reportingServerURI!="";
			command=$"INSERT INTO preference(PrefName,ValueString) VALUES('AuditTrailUseReportingServer','{POut.Bool(isReportingServerInUse)}')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EserviceLogUploadTimeNext','0001-01-01 00:00:00')";
			Db.NonQ(command);
			command=$"SELECT ProgramNum FROM program WHERE ProgName='XVWeb'";
			long programNumXVWeb=Db.GetLong(command);
			if(programNumXVWeb>0) {
				command=$"UPDATE programproperty SET PropertyDesc='Image Category' WHERE PropertyDesc='ImageCategory' AND ProgramNum={POut.Long(programNumXVWeb)}";
				Db.NonQ(command);
			}
			command="SELECT * FROM preference WHERE PrefName='EraStrictClaimMatching'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraStrictClaimMatching','0')"; //Default to false.
				Db.NonQ(command);
			}
			command="SELECT * FROM preference WHERE PrefName='EraShowStatusAndClinic'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraShowStatusAndClinic','1')"; //Default to true.
				Db.NonQ(command);
			}
			command="ALTER TABLE patientnote ADD UserNumOrthoLocked bigint NOT NULL";
			Db.NonQ(command);
			AlterTable("patient","PatNum",listIndexColsAndNames: new List<IndexColsAndName>() {
				new IndexColsAndName("ClinicNum,PatStatus","ClinicPatStatus"),
				new IndexColsAndName("Birthdate,PatStatus","BirthdateStatus")
			});
			AlterTable("recall","RecallNum",indexColsAndName:new IndexColsAndName("DateDue,IsDisabled,RecallTypeNum,DateScheduled","DateDisabledType"));
			AlterTable("procedurelog","ProcNum",indexColsAndName:new IndexColsAndName("ProcDate,ClinicNum,ProcStatus","DateClinicStatus"));
			command="ALTER TABLE smsfrommobile ADD INDEX StatusHiddenClinic (SmsStatus,IsHidden,ClinicNum),DROP INDEX ClinicStatusHidden";
			List<string> listRedundantIndexNames=GetRedundantIndexNames("smsfrommobile","SmsStatus,IsHidden,ClinicNum");
			if(listRedundantIndexNames.Any()) {
				command+=","+string.Join(",",listRedundantIndexNames.Select(x => "DROP INDEX "+x));
			}
			Db.NonQ(command);
			command="ALTER TABLE insverify ADD INDEX (DateLastAssigned)";
			Db.NonQ(command);
			command="ALTER TABLE disease ADD INDEX (DiseaseDefNum)";
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE PrefName='UpdateAlterLargeTablesDirectly'";
			if(Db.GetInt(command)==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('UpdateAlterLargeTablesDirectly','1')";
				Db.NonQ(command);
			}
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8"; // 8 - Setup permission
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					+"VALUES("+POut.Long(groupNum)+",212)"; // 212 - ManageHighSecurityProgProperties permission
				Db.NonQ(command);
			}
			command="ALTER TABLE programproperty ADD IsHighSecurity tinyint NOT NULL";
			Db.NonQ(command);
			command=$"UPDATE programproperty SET IsHighSecurity={POut.Bool(true)} WHERE IsMasked=1";
			Db.NonQ(command);
			command="ALTER TABLE xchargetransaction ADD BatchTotal double NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LicenseAgreementAccepted','0')"; //Default to false
			Db.NonQ(command);
			command="ALTER TABLE updatehistory ADD Signature text NOT NULL";
			Db.NonQ(command);
			command=$"INSERT INTO preference(PrefName,ValueString) VALUES('ApptReminderPremedTemplate','Remember to take your Pre-Med. ')";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD DateTimeUploaded datetime NOT NULL DEFAULT '0001-01-01 12:00:00'";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD INDEX (DateTimeUploaded)";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS imagedraw";
			Db.NonQ(command);
			command=@"CREATE TABLE imagedraw (
				ImageDrawNum bigint NOT NULL auto_increment PRIMARY KEY,
				DocNum bigint NOT NULL,
				MountNum bigint NOT NULL,
				ColorDraw int NOT NULL,
				ColorBack int NOT NULL,
				DrawingSegment text NOT NULL,
				DrawText varchar(255) NOT NULL,
				FontSize float NOT NULL,
				DrawType tinyint NOT NULL,
				INDEX(DocNum),
				INDEX(MountNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardAllowPaymentOnCheckin','0')"; //Default to false
			Db.NonQ(command);
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				+") VALUES(" 
				+"'SteriSimple', " 
				+"'SteriSimple from www.sterisimple.ca', " 
				+"'0', " 
				+"'"+POut.String(@"C:\SteriSimple\SteriSoft\SteriRecall\RecallRepeater.exe")+"', " 
				+"'', "//leave blank if none 
				+"'')"; 
			long programNum=Db.NonQ(command,true); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Enter 0 to use PatientNum, or 1 to use ChartNum', " 
				+"'0')"; 
			Db.NonQ(command); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				+"VALUES (" 
				+"'"+POut.Long(programNum)+"', " 
				+"'7', "//ToolBarsAvail.MainToolbar 
				+"'SteriSimple')";
			Db.NonQ(command);
			command="ALTER TABLE sheetdef ADD AutoCheckSaveImage tinyint NOT NULL DEFAULT 1";//defaults to true to maintain current behavior
			Db.NonQ(command);
			command=$"SELECT ProgramNum FROM program WHERE ProgName='DexisIntegrator'";
			long programNumDexisIntegrator=Db.GetLong(command);
			if(programNumDexisIntegrator>0) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
					 +") VALUES(" 
					 +POut.Long(programNumDexisIntegrator)+", " 
					 +"'Enter 0 to use DDE, or 1 to use communication file', " 
					 +"'0')"; 
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
					 +") VALUES(" 
					 +POut.Long(programNumDexisIntegrator)+", " 
					 +"'Communication files folder path', " 
					 +"'')"; 
				Db.NonQ(command); 
			}
			command="ALTER TABLE eservicelog ";
			List<string> listIndexNames=GetIndexNames("eservicelog","LogDateTime,ClinicNum");
			if(listIndexNames.Any()) {
				command+=string.Join(",",listIndexNames.Select(x => "DROP INDEX "+x))+",";
			}
			command+="ADD INDEX ClinicDateTime (ClinicNum,LogDateTime)";
			Db.NonQ(command);
			#region HQ Only
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0 && PIn.Bool(table.Rows[0][0].ToString())) {
				command="DROP TABLE IF EXISTS jobteam";
				Db.NonQ(command);
				command=@"CREATE TABLE jobteam (
					JobTeamNum bigint NOT NULL auto_increment PRIMARY KEY,
					TeamName varchar(100) NOT NULL,
					TeamDescription varchar(100) NOT NULL,
					TeamFocus tinyint NOT NULL,
					IsHidden tinyint NOT NULL
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS jobteamuser";
				Db.NonQ(command);
				command=@"CREATE TABLE jobteamuser (
					JobTeamUserNum bigint NOT NULL auto_increment PRIMARY KEY,
					JobTeamNum bigint NOT NULL,
					UserNumEngineer bigint NOT NULL,
					IsTeamLead tinyint NOT NULL,
					IsHidden tinyint NOT NULL,
					INDEX(JobTeamNum),
					INDEX(UserNumEngineer)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			#endregion
		}//End of 21_3_1() method
	
		private static void To21_3_2() {
			string command;
			DataTable table;
			//Insert Pixel Bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				+") VALUES(" 
				+"'Pixel', " 
				+"'Pixel by Digital Doc', " 
				+"'0', " 
				+"'"+POut.String(@"""C:\Program Files (x86)\DigitalDoc\Pixel\PixelBridge.exe""")+"', " 
				+"'"+POut.String(@"[LName] [FName] [PatNum]")+"', "//leave blank if none 
				+"'')"; 
			long programNum=Db.NonQ(command,true);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				+"VALUES (" 
				+"'"+POut.Long(programNum)+"', " 
				+"'2', "//ToolBarsAvail.ChartModule 
				+"'Pixel')"; 
			Db.NonQ(command); 
			//end Pixel bridge
			//Add permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",214)"; // 214 - MedicationDefEdit permission
				 Db.NonQ(command);
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",215)"; // 215 - AllergyDefEdit permission
				 Db.NonQ(command);
			}
		}//End of 21_3_2() method

		private static void To21_3_3() {
			string command;
			DataTable table;
			//Add PatientEdit permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",108)"; // 108 - PatientEdit permission
				 Db.NonQ(command);
			}
			command="ALTER TABLE payplan ADD MobileAppDeviceNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payplan ADD INDEX (MobileAppDeviceNum)";
			Db.NonQ(command);
			if(!ColumnExists(GetCurrentDatabase(),"etrans835","AutoProcessed")) {
				command="ALTER TABLE etrans835 ADD AutoProcessed tinyint NOT NULL,ADD IsApproved tinyint NOT NULL";
				Db.NonQ(command);
			}
		}//End of 21_3_3() method

		private static void To21_3_4() {
			string command;
			DataTable table;
			//Add Rate3 to TimeCards and ClockEvents-------------------------------------------
			command="ALTER TABLE timecardrule ADD HasWeekendRate3 TINYINT NOT NULL";
			Db.NonQ(command);
			command=$"UPDATE timecardrule SET HasWeekendRate3={POut.Bool(false)}";
			Db.NonQ(command);
			LargeTableHelper.AlterTable("clockevent","ClockEventNum",new ColNameAndDef("Rate3Hours","TIME NOT NULL"));
			command="UPDATE clockevent SET Rate3Hours='-01:00:00'";
			Db.NonQ(command);
			LargeTableHelper.AlterTable("clockevent","ClockEventNum",new ColNameAndDef("Rate3Auto","TIME NOT NULL"));
		}//End of 21_3_4() method

		private static void To21_3_6() {
			string command;
			DataTable table;
			command="SELECT ProgramNum FROM program WHERE ProgName='SteriSimple'";
			long steriSimpleProgramNum=Db.GetLong(command);
			if(steriSimpleProgramNum!=0) {
				//Users that updated to v21.3 prior to v21.3.6 were missing this button. Only add the button if it is not already present.
				command=$"SELECT COUNT(*) FROM toolbutitem WHERE ProgramNum={POut.Long(steriSimpleProgramNum)}";
				if(Db.GetCount(command)=="0") {
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
						+"VALUES (" 
						+POut.Long(steriSimpleProgramNum)+", " 
						+"7, "//ToolBarsAvail.MainToolbar 
						+"'SteriSimple')";
					Db.NonQ(command);
				}
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PayPlanSaveSignedToPdf','0')"; //Default to false
			Db.NonQ(command);
		}//End of 21_3_6() method
	
		private static void To21_3_8() {
			string command;
			command="SELECT ProgramNum FROM program where ProgName='DrCeph'";
			long drCephNum=Db.GetLong(command);
			if(drCephNum!=0) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(drCephNum)+"', " 
				+"'Custom Patient Race Options', "
				+"'')";
				Db.NonQ(command);
			}
		}//End of 21_3_8()

		private static void To21_3_10() {
			string command;
			if(!IndexExists("mount","PatNum")) {
				command="ALTER TABLE mount ADD INDEX (PatNum)";
				Db.NonQ(command);
			}
			if(!IndexExists("mountitem","MountNum")) {
				command="ALTER TABLE mountitem ADD INDEX (MountNum)";
				Db.NonQ(command);
			}
			DataTable table;
			command=@"INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('AdvertisingPostCardGuid','','')";
			Db.NonQ(command);
			//Add Advertising permission to groups with existing permission eServiceSetup.
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=91";  //91 is the existing Permission number for eServiceSetup.
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",216)";  //216 is the enum value of the new permission Advertising.
				 Db.NonQ(command);
			}
		}//End of 21_3_10()

		private static void To21_3_11() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedExistingPatRecallName','Teeth Cleaning')";
			Db.NonQ(command);
		}//End of 21_3_11()

		private static void To21_3_13() {
			string upgrading="Upgrading database to version: 21.3.13";
			ODEvent.Fire(ODEventType.ConvertDatabases,upgrading);//No translation in convert script.
			string command;
			command=$"INSERT INTO preference(PrefName,ValueString) VALUES('EmailSecureDefaultClinic','0')";
			Db.NonQ(command);
		}//End of 21_3_13()

		private static void To21_3_14() {
			string command;
			#region HQ Only
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.  But this change needs to happen as soon as we update to this version.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			if(PIn.Bool(Db.GetScalar(command))) {
				command=$@"UPDATE job SET PhaseCur=(CASE PhaseCur
					WHEN 'Concept' THEN 0
					WHEN 'Definition' THEN 1
					WHEN 'Development' THEN 2
					WHEN 'Documentation' THEN 3
					WHEN 'Complete' THEN 4
					WHEN 'Cancelled' THEN 5
					WHEN 'Quote' THEN 6
					ELSE PhaseCur END
				),
				Category=(CASE Category
					WHEN 'Feature' THEN 0
					WHEN 'Bug' THEN 1
					WHEN 'Enhancement' THEN 2
					WHEN 'Query' THEN 3
					WHEN 'ProgramBridge' THEN 4
					WHEN 'InternalRequest' THEN 5
					WHEN 'HqRequest' THEN 6
					WHEN 'Conversion' THEN 7
					WHEN 'Research' THEN 8
					WHEN 'SpecialProject' THEN 9
					WHEN 'NeedNoApproval' THEN 10
					WHEN 'MarketingDesign' THEN 11
					WHEN 'UnresolvedIssue' THEN 12
					ELSE Category END
				)";
				Db.NonQ(command);
				command="ALTER TABLE job MODIFY PhaseCur TINYINT NOT NULL,MODIFY Category TINYINT NOT NULL,ADD INDEX (PhaseCur),ADD INDEX (Category)";
				Db.NonQ(command);
			}
			#endregion
		}//End of 21_3_14()

		private static void To21_3_17() {
			string command;
			//Add permissions to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			List<long> listNums=Db.GetListLong(command);
			for(int i=0;i<listNums.Count;i++) {
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES"
						+"("+POut.Long(listNums[i])+",71)," // 71 - PatProblemListEdit permission
						+"("+POut.Long(listNums[i])+",72)," // 72 - PatMedicationListEdit permission
						+"("+POut.Long(listNums[i])+",73)"; // 73 - PatAllergyListEdit permission
				 Db.NonQ(command);
			}
		}//End of 21_3_17()

		private static void To21_3_20() {
			string command;
			if(!IndexExists("document","MountItemNum")) {
				command="ALTER TABLE document ADD INDEX (MountItemNum)";
				Db.NonQ(command);
			}
		}

		private static void To21_3_21() {
			string command;
			List<SheetTypeEnum> listSheetTypeEnums = Enum.GetValues(typeof(SheetTypeEnum)).Cast<SheetTypeEnum>()
					.Where(x => !EnumTools.GetAttributeOrDefault<SheetTypeAttribute>(x).CanAutoSave).ToList();
			command=$@"UPDATE sheetdef SET sheetdef.AutoCheckSaveImage = 0 WHERE sheetdef.SheetType IN ({String.Join(",",listSheetTypeEnums.Select(x => ((long)x)).ToList())})";
			DataCore.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PayPlanTermsAndConditions','I agree to the terms of this payment plan at an interest rate of %[APR] and agree to pay an amount of [PaymentAmt] [ChargeFrequency]. I understand that the cost and duration of the loan may change if I change the ''Total Amount'' of treatment financed.')";
			Db.NonQ(command);
			command="SELECT MAX(displayreport.ItemOrder) FROM displayreport WHERE displayreport.Category=3";//3=Lists report category
			long itemOrder=Db.GetLong(command)+1;//get the next available ItemOrder for the Category specified.
			command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden,IsVisibleInSubMenu) "
				+"VALUES('ODEraAutoProcessed',"+POut.Long(itemOrder)+",'ERAs Automatically Processed',3,0,0)";
			long reportNumNew=Db.NonQ(command,getInsertID:true);
			//Only usergroups with the InsPayCreate permission (PermType 36) can access the ERAs window.
			//By default, only these usergroups will have access to the ERAs Automatically Processed report.
			List<long> listGroupNums=Db.GetListLong("SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=36");
			for(int i=0;i<listGroupNums.Count;i++) {
				command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
					 +"VALUES('0001-01-01',0,"+POut.Long(listGroupNums[i])+",22,"+POut.Long(reportNumNew)+")";//22=Reports PermType
				Db.NonQ(command);
			}
		}//End of 21_3_21()

		private static void To21_3_22() {
			string command;
			//Moving codes to the Obsolete category that were deleted in CDT 2022.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				//Move deprecated codes to the Obsolete procedure code category.
				//Make sure the procedure code category exists before moving the procedure codes.
				string procCatDescript="Obsolete";
				long defNum=0;
				command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
				DataTable dtDef=Db.GetTable(command);
				if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
					command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
					int countCats=PIn.Int(Db.GetCount(command));
						command="INSERT INTO definition (Category,ItemName,ItemOrder) "
								+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
					defNum=Db.NonQ(command,true);
				}
				else { //The procedure code category already exists, get the existing defnum
					defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
				}
				string[] cdtCodesDeleted=new string[] {
					"D4320",
					"D4321",
					"D8050",
					"D8060",
					"D8690"
				};
				//Change the procedure codes' category to Obsolete.
				command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)
					+" WHERE ProcCode IN('"+string.Join("','",cdtCodesDeleted.Select(x => POut.String(x)))+"') ";
				Db.NonQ(command);
			}//end United States CDT codes update
			//Default to PrefName.InsVerifyFutureDateBenefitYear for InsVerifyFutureDatePatEnrollmentYear.  Maintain old value for backward compatibility.
			command="SELECT ValueString FROM preference WHERE PrefName='InsVerifyFutureDateBenefitYear'";
			bool insVerifyFutureDateBenefitYear=PIn.Bool(Db.GetScalar(command));
			command=$"INSERT INTO preference(PrefName,ValueString) VALUES('InsVerifyFutureDatePatEnrollmentYear','{POut.Bool(insVerifyFutureDateBenefitYear)}')";
			Db.NonQ(command);
		}//End of 21_3_22()


		private static void To21_3_28() {
			//The url we inserted for these programs is no longer valid. This is us fixing that going forward.
			//Adding extra check here because most of our customers will have already gotten this fixed in 21.2
			string command="SELECT * FROM program where (ProgName='Trophy' OR ProgName='TrophyEnhanced') AND ProgDesc LIKE '%from www.trophy-imaging.com%'";
			DataTable table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				DataRow progCur=table.Rows[i];
				if(progCur["ProgDesc"].ToString().Contains("from www.trophy-imaging.com")) {
					progCur["ProgDesc"]=progCur["ProgDesc"].ToString().Replace("from www.trophy-imaging.com","");
					long progNum=PIn.Long(progCur["ProgramNum"].ToString());
					command=$"UPDATE program SET ProgDesc='{POut.String(progCur["ProgDesc"].ToString())}' WHERE ProgramNum={POut.Long(progNum)}";
					Db.NonQ(command);
				}
			}
		}//End of 21_3_28()

		private static void To21_3_29() {
			string command;
			//Updating D9613 to default to TreatmentArea.Quad in CDT 2022 (only if user did not change from TreatmentArea.Mouth).
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				command="UPDATE procedurecode SET procedurecode.TreatArea=4 WHERE procedurecode.ProcCode='D9613' AND procedurecode.TreatArea=3";//4 - Quad
				Db.NonQ(command);
			}//end United States CDT codes update
		}//End of 21_3_29()

		private static void To21_3_50() {
			string command;
			//E30604 - Enterprise Pref to use PriProv's PPO fee for Hyg procs
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseHygProcUsePriProvFee','0')"; //Default to false
			Db.NonQ(command);
		}//End of 21_3_50()

		private static void To21_3_51() {
			string command;
			//B34518 - Preventing invalidated procedures from overwriting graphics on the chart
			command="UPDATE procedurelog SET HideGraphics=1 WHERE ProcStatus=6 AND IsLocked=1";
			Db.NonQ(command);
		}

		private static void To21_4_1() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('BillingSelectInsFilingCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('OrthoDebondProcCompletedSetsMonthsTreat','0')"; //Default to false
			Db.NonQ(command);
			command=$"INSERT INTO preference(PrefName,ValueString) VALUES('DefaultImageCategoryImportFolder','0')";
			Db.NonQ(command);
			//Add TextMessageView permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					   +"VALUES("+POut.Long(groupNum)+",217)"; // 217 - Text Message View
				Db.NonQ(command);
			}
			//Add TextMessageSend permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					   +"VALUES("+POut.Long(groupNum)+",218)"; // 218 - Text Message Send
				Db.NonQ(command);
			}
			//Updating a database to or beyond version 21.1.1 will default DynamicPayPlanTPOption to 0-None for existing Dynamic Payment Plans,
			//but None should not be an option for them. Update that column to 1-AwaitComplete for those plans which is the default for new Dynamic Payment Plans.
			command="UPDATE payplan SET payplan.DynamicPayPlanTPOption=1 WHERE payplan.DynamicPayPlanTPOption=0 AND payplan.IsDynamic=1";
			Db.NonQ(command);
			command=@"ALTER TABLE resellerservice ADD HostedUrl VARCHAR(255)";
			Db.NonQ(command);
			command=@"ALTER TABLE mountdef ADD ColorFore int NOT NULL";
			Db.NonQ(command);
			command=@"ALTER TABLE mountdef ADD ColorTextBack int NOT NULL";
			Db.NonQ(command);
			command=@"ALTER TABLE mountdef ADD ScaleValue varchar(255) NOT NULL";
			Db.NonQ(command);
			command=@"ALTER TABLE mount ADD ColorFore int NOT NULL";
			Db.NonQ(command);
			command=@"ALTER TABLE mount ADD ColorTextBack int NOT NULL";
			Db.NonQ(command);
			string black="-16777216";
			string white="-1";
			command=@"UPDATE mountdef SET ColorFore="+white+" WHERE ColorBack="+black;
			Db.NonQ(command);
			command=@"UPDATE mountdef SET ColorTextBack="+black+" WHERE ColorBack="+black;
			Db.NonQ(command);
			command=@"UPDATE mountdef SET ColorFore="+black+" WHERE ColorFore=0";
			Db.NonQ(command);
			command=@"UPDATE mountdef SET ColorTextBack="+white+" WHERE ColorTextBack=0";
			Db.NonQ(command);
			command=@"UPDATE mount SET ColorFore="+white+" WHERE ColorBack="+black;
			Db.NonQ(command);
			command=@"UPDATE mount SET ColorTextBack="+black+" WHERE ColorBack="+black;
			Db.NonQ(command);
			command=@"UPDATE mount SET ColorFore="+black+" WHERE ColorFore=0";
			Db.NonQ(command);
			command=@"UPDATE mount SET ColorTextBack="+white+" WHERE ColorTextBack=0";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardShowPerioGraphical','1')"; //Default to true
			Db.NonQ(command);
			command="ALTER TABLE mobileappdevice ADD DevicePage tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitemdef ADD TextShowing text NOT NULL";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS taskattachment";
			Db.NonQ(command);
			command=@"CREATE TABLE taskattachment (
								TaskAttachmentNum bigint NOT NULL auto_increment PRIMARY KEY,
								TaskNum bigint NOT NULL,
								DocNum bigint NOT NULL,
								TextValue text NOT NULL,
								Description varchar(255) NOT NULL,
								INDEX(TaskNum),
								INDEX(DocNum)
								) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TaskAttachmentCategory','0')";
			Db.NonQ(command);
			//Add multi-column indexes to several financial tables which will be used within the Family Balancer window.
			IndexColsAndName indexSecDateTEditPatNum=new IndexColsAndName("SecDateTEdit,PatNum","SecDateTEditPN");
			//The procedurelog table is the only table that does not have a SecDateTEdit column but instead has a 'timestamp' data type column called DateTStamp.
			IndexColsAndName indexDateTStampPatNum=new IndexColsAndName("DateTStamp,PatNum","DateTStampPN");
			AlterTable("adjustment","AdjNum",indexColsAndName:indexSecDateTEditPatNum);
			AlterTable("claimproc","ClaimProcNum",indexColsAndName:indexSecDateTEditPatNum);
			AlterTable("payplancharge","PayPlanChargeNum",indexColsAndName:indexSecDateTEditPatNum);
			AlterTable("paysplit","SplitNum",indexColsAndName:indexSecDateTEditPatNum);
			AlterTable("procedurelog","ProcNum",indexColsAndName:indexDateTStampPatNum);
			command="ALTER TABLE mountitemdef ADD FontSize float NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitem ADD TextShowing text NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitem ADD FontSize float NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE eclipboardsheetdef ADD IgnoreSheetDefNums text NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptPrintColorBehavior','0')";//Default to ApptPrintColorBehavior.FullColor
			Db.NonQ(command);
			//Add RxMerge permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",219)";// 219 - Permission for RxMerge
				 Db.NonQ(command);
			}
			command="ALTER TABLE paysplit ADD SecurityHash varchar(255) NOT NULL";
			Db.NonQ(command);
			Misc.SecurityHash.UpdateHashing();
			AlterTable("document","DocNum",new ColNameAndDef("DegreesRotated","float NOT NULL"));
			AlterTable("document","DocNum",new ColNameAndDef("IsCropOld","tinyint unsigned NOT NULL"));
			command="UPDATE document SET IsCropOld=1 WHERE CropW>0 AND CropH>0";
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='eServices'";
			long alertCategoryNum=Db.GetLong(command);
			command=$@"INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES({POut.Long(alertCategoryNum)},34)";//34=WebSchedRecallsNotSending alert 
			Db.NonQ(command);
			AlterTable("appointment","AptNum",new ColNameAndDef("SecurityHash","varchar(255) NOT NULL"));
			AlterTable("histappointment","HistApptNum",new ColNameAndDef("SecurityHash","varchar(255) NOT NULL"));
			Misc.SecurityHash.UpdateHashing();
			//Add Definition Edit permission to everyone who has Setup permissions--------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8";//Setup
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					   +"VALUES("+POut.Long(groupNum)+",220)"; // 220 - Definition Edit
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseCommlogOmitDefaults','0')"; //Default false
			Db.NonQ(command);
			LargeTableHelper.AlterTable("eservicelog","EServiceLogNum",new ColNameAndDef("Note","varchar(255) NOT NULL"));
			//Add Update Install permission to everyone who has Setup permissions---------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8";//Setup
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",221)"; // 221 - Update Install
				Db.NonQ(command);
			}
			command = "INSERT INTO preference(PrefName,ValueString) VALUES('RxHideProvsWithoutDEA','0')"; //Default false
			Db.NonQ(command);
			command="ALTER TABLE alertitem ADD SecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS limitedbetafeature";
			Db.NonQ(command);
			command=@"CREATE TABLE limitedbetafeature (
				LimitedBetaFeatureNum bigint NOT NULL auto_increment PRIMARY KEY,
				LimitedBetaFeatureTypeNum bigint NOT NULL,
				ClinicNum bigint NOT NULL,
				IsSignedUp tinyint NOT NULL,
				INDEX(LimitedBetaFeatureTypeNum),
				INDEX(ClinicNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS apikey";
			Db.NonQ(command);
			command=@"CREATE TABLE apikey (
					APIKeyNum bigint NOT NULL auto_increment PRIMARY KEY,
					CustApiKey varchar(255) NOT NULL,
					DevName varchar(255) NOT NULL
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS insplanpreference";
			Db.NonQ(command);
			command=@"CREATE TABLE insplanpreference (
				InsPlanPrefNum bigint NOT NULL auto_increment PRIMARY KEY,
				PlanNum bigint NOT NULL,
				FKey bigint NOT NULL,
				FKeyType tinyint NOT NULL,
				ValueString text NOT NULL,
				INDEX(PlanNum),
				INDEX(FKey)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			LargeTableHelper.AlterTable("patient","PatNum",new ColNameAndDef("SecurityHash","varchar(255) NOT NULL"));
			Misc.SecurityHash.UpdateHashing();
			command="DROP TABLE IF EXISTS webschedcarrierrule";
			Db.NonQ(command);
			command=@"CREATE TABLE webschedcarrierrule (
				WebSchedCarrierRuleNum bigint NOT NULL auto_increment PRIMARY KEY,
				ClinicNum bigint NOT NULL,
				CarrierName varchar(255) NOT NULL,
				DisplayName varchar(255) NOT NULL,
				Message text NOT NULL,
				Rule tinyint NOT NULL,
				INDEX(ClinicNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedExistingPatRequestInsurance','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedNewPatRequestInsurance','0')";
			Db.NonQ(command);
			command="ALTER TABLE apptfielddef ADD ItemOrder int NOT NULL";
			Db.NonQ(command);
			command="SELECT * FROM apptfielddef ORDER BY FieldName";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				long appFieldDefNum=PIn.Long(table.Rows[i]["ApptFieldDefNum"].ToString());
				command="UPDATE apptfielddef SET ItemOrder="+POut.Int(i)+" WHERE ApptFieldDefNum="+POut.Long(appFieldDefNum);
				Db.NonQ(command);
			}
		}//End of 21_4_1() method

		private static void To21_4_4() {
			string command;
			DataTable table;
			command="ALTER TABLE xwebresponse ADD LogGuid varchar(36)";
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb ADD LogGuid varchar(36)";
			Db.NonQ(command);
		}//End of 21_4_4() method

		private static void To21_4_7() {
			string command;
			DataTable table;
			command="ALTER TABLE payplan ADD SecurityHash varchar(255) NOT NULL";
			Db.NonQ(command);
			Misc.SecurityHash.UpdateHashing();
		}//End of 21_4_7() method

		private static void To21_4_8() {
			string command;
			DataTable table;
			Misc.SecurityHash.UpdateHashing();
		}//End of 21_4_8() method

		private static void To21_4_9() {
			string command;
			DataTable table;
			//Insert Ai-Dental bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				 +") VALUES(" 
				 +"'AiDental', " 
				 +"'Ai-Dental', " 
				 +"'0', " 
				 +"'"+POut.String(@"C:\Ai-Dental\Ai-Dental-Client\Ai-Dental.exe")+"', " 
				 +"'"+POut.String(@"link")+"', "//leave blank if none 
				 +"'')"; 
			long programNum=Db.NonQ(command,true);  
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Text file path for Ai-Dental', " 
				 +"'"+POut.String(@"C:\Ai-Dental\Ai-Dental-Client\patdata.txt")+"')"; 
			Db.NonQ(command); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				 +"VALUES (" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'4', "//ToolBarsAvail.FamilyModule, Based on attached video for the job P31954
				 +"'Ai-Dental')"; 
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('IncomeTransfersMadeUponClaimReceived','0')"; //Default to unknown
			Db.NonQ(command);
			command="UPDATE displayreport SET Description='Broken Appointments' WHERE InternalName='ODBrokenAppointments' AND Description='BrokenAppointments'";
			Db.NonQ(command);		
		}//End of 21_4_9() method

		private static void To21_4_16() {
			Misc.SecurityHash.UpdateHashing();
			Db.NonQ("ALTER TABLE emailaddress ADD QueryString varchar(1000) NOT NULL");
		}//End of 21_4_16() method

		private static void To21_4_17() {
			string command;
			DataTable table;
			command="SELECT ProgramNum FROM program WHERE ProgName='AiDental'";
			long programNum=Db.GetLong(command);
			if(programNum!=0) {
				command="UPDATE program SET CommandLine='"+POut.String(@"[PatNum].[LName].[FName]")+"' WHERE ProgramNum='"+POut.Long(programNum)+"'";
				Db.NonQ(command);
				command="DELETE FROM programproperty WHERE ProgramNum='"+POut.Long(programNum)+"' AND PropertyDesc='Text file path for Ai-Dental'";
				Db.NonQ(command);
			}  
		}//End of 21_4_17() method

		private static void To21_4_20() {
			string command;
			DataTable table;
			command="UPDATE patient SET SecurityHash=''";
			Db.NonQ(command);
			Misc.SecurityHash.ResetPatientHashing();
			Misc.SecurityHash.UpdateHashing();
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedRecallApptSearchMaximumMonths','12')";//Default to 12 months
			Db.NonQ(command);
		}//End of 21_4_20() method

		private static void To21_4_21() {
			Misc.SecurityHash.UpdateHashing();
		}//End of 21_4_21() method

		private static void To21_4_23() {
			string command="";
			//E34356 - Default Prexion bridge to use PatNum instead of ChartNum
			//we don't want to alter a bridge that is currently in use
			command="SELECT ProgramNum FROM program WHERE ProgName='PreXionViewer' AND Enabled=0";
			long prexionViewerProgNum=Db.GetLong(command);
			if(prexionViewerProgNum>0){
				command="SELECT ProgramNum FROM program WHERE ProgName='PreXionAcquire'";
				long prexionAcquireProgNum=Db.GetLong(command);
				long usePatNum=0;
				//If acquire is set up a specific way then we want viewer to be set up the same way
				if(prexionAcquireProgNum>0){
					command=$"SELECT PropertyValue from programproperty WHERE PropertyDesc='Enter 0 to use PatientNum, or 1 to use ChartNum' AND ProgramNum={POut.Long(prexionAcquireProgNum)}";
					usePatNum=Db.GetLong(command);
				}
				command=$"UPDATE programproperty SET PropertyValue='{POut.Long(usePatNum)}' WHERE PropertyDesc='Enter 0 to use PatientNum, or 1 to use ChartNum' AND ProgramNum={POut.Long(prexionViewerProgNum)}";
				Db.NonQ(command);
			}
			//Add the new PaySimple PaySimplePrintReceipt property------------------------------------------------------
			command="SELECT ClinicNum FROM clinic";
			List<long> listClinicNums=Db.GetListLong(command);
			listClinicNums.Add(0);//Add HQ
			command="SELECT ProgramNum FROM program WHERE ProgName='PaySimple'";
			long progNum=PIn.Long(Db.GetScalar(command));
			foreach(long clinicNum in listClinicNums) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("+POut.Long(progNum)+",'PaySimplePrintReceipt','1','',"+POut.Long(clinicNum)+")";
				Db.NonQ(command);
			}
		}//End of 21_4_23() method

		private static void To21_4_24() {
			string command;
			//E30604 - Enterprise Pref to use PriProv's PPO fee for Hyg procs
			command="SELECT * FROM preference WHERE PrefName='EnterpriseHygProcUsePriProvFee'";
			if(Db.GetTable(command).Rows.Count==0) { //Check to see if it's already been added
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EnterpriseHygProcUsePriProvFee','0')";//Default to false.
				Db.NonQ(command);
			}
		}//End of 21_4_24() method

		private static void To21_4_25() {
			string command;
			//B34518 - Preventing invalidated procedures from overwriting graphics on the chart
			command="UPDATE procedurelog SET HideGraphics=1 WHERE ProcStatus=6 AND IsLocked=1";
			Db.NonQ(command);
			Misc.SecurityHash.UpdateHashing();
		}

		private static void To21_4_27() {
			Misc.SecurityHash.UpdateHashing();
		}

		private static void To21_4_30() {
			Misc.SecurityHash.UpdateHashing();
		}
		
		private static void To21_4_38() {
			string command;
			//B35468 - Default to using new Image module for everyone.
			command="UPDATE preference SET ValueString='0' WHERE PrefName='ImagesModuleUsesOld2020'";
			Db.NonQ(command);
		}//End of 21_4_38() method

		private static void To21_4_41() {
			Misc.SecurityHash.UpdateHashing();
		}//End of 21_4_41() method

		private static void To21_4_49() {
			string command;
			command="ALTER TABLE covcat MODIFY CovOrder INT NOT NULL";
			Db.NonQ(command);
		}//End of 21_4_49() method
	}
}



				/*
				command="ALTER TABLE alertitem ADD SecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
				*/

				/*

				command="DROP TABLE IF EXISTS apikey";
				Db.NonQ(command);
				command=@"CREATE TABLE apikey (
					APIKeyNum bigint NOT NULL auto_increment PRIMARY KEY,
					CustApiKey varchar(255) NOT NULL,
					DevName varchar(255) NOT NULL
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				*/

				/*
				LargeTableHelper.AlterTable("appointment","AptNum",new ColNameAndDef("SecurityHash","varchar(255) NOT NULL"));
				*/

				/*
				command="ALTER TABLE apptfielddef ADD ItemOrder int NOT NULL";
				Db.NonQ(command);
				*/