using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using OpenDentBusiness.Crud;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PromotionLogs {
		///<summary>Returns one promotion log for the given email hosting fk.</summary>
		public static PromotionLog GetOneByEmailHostingFK(long emailHostingFK) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PromotionLog>(MethodBase.GetCurrentMethod(),emailHostingFK);
			}
			return PromotionLogCrud.SelectOne($"SELECT * FROM promotionlog WHERE EmailHostingFK = {POut.Long(emailHostingFK)}");
		}

		///<summary>Returns a list of promotion logs for the passed in promotion nums.</summary>
		public static List<PromotionLog> GetForPromotion(List<long> listPromotionNums) {
			if(listPromotionNums.IsNullOrEmpty()) {
				return new List<PromotionLog>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) { 
				return Meth.GetObject<List<PromotionLog>>(MethodBase.GetCurrentMethod(),listPromotionNums);
			}
			return PromotionLogCrud.SelectMany($"SELECT * FROM promotionlog WHERE PromotionNum IN ({string.Join(",",listPromotionNums)})");
		}

		///<summary>For the given email hosting fk, will update the status and any associated properties. If log is not found for this fk, will do nothing.</summary>
		public static void UpdatePromotionLogStatus(long emailHostingFK,PromotionLogStatus status) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailHostingFK,status);
				return;
			}
			PromotionLog log=GetOneByEmailHostingFK(emailHostingFK);
			//Can't do anything here.
			if(log==null) {
				return;
			}
			PromotionLog logOld=log.Clone();
			log.PromotionStatus=status;
			//Only if they are going from pending to sent or complaint will we update the date time.
			if(logOld.PromotionStatus==PromotionLogStatus.Pending && ListTools.In(status,PromotionLogStatus.Delivered,PromotionLogStatus.Complaint)) {
				log.DateTimeSent=DateTime.Now;
			}
			if(ListTools.In(status,PromotionLogStatus.Failed,PromotionLogStatus.Bounced,PromotionLogStatus.Unsubscribed)) {
				EmailMessage message=EmailMessages.GetOne(log.EmailMessageNum);
				if(message!=null) {
					EmailMessage messageOld=message.Copy();
					message.BodyText+=status.GetDescription().ToUpper()+"\r\n"+message.BodyText;
					EmailMessages.Update(message,messageOld);
				}
			}
			PromotionLogCrud.Update(log,logOld);
		}
		
		///<summary>Returns a list of PatNum/MostRecentPromotion.</summary>
		public static List<MostRecent> GetMostRecentDate() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MostRecent>>(MethodBase.GetCurrentMethod());
			}
			string command=$@"SELECT promotionlog.PatNum,MAX(DATE(promotionlog.DateTimeSent)) DateTimeLastEmail
					FROM promotionlog
					GROUP BY promotionlog.PatNum";
			return Db.GetTable(command).Rows.AsEnumerable<DataRow>()
				.Select(x => new MostRecent(PIn.Long(x["PatNum"].ToString()),PIn.Date(x["DateTimeLastEmail"].ToString())))
				.ToList();
		}

		///<summary>Returns true if the patient's email is valid and has not received a Mass Email more recently than the given TimeSpan.</summary>
		public static bool DoIncludePatient(PatientInfo patient,TimeSpan timeSpanExcludeMoreRecentThan,DateTime dateMostRecentPromotionLog) {
			return EmailAddresses.IsValidEmail(patient.Email,out _)	&& DateTools.IsOlderThan(dateMostRecentPromotionLog,timeSpanExcludeMoreRecentThan);
		}

		#region Modification Methods
		///<summary></summary>
		public static long Insert(PromotionLog promotionLog){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				promotionLog.PromotionLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),promotionLog);
				return promotionLog.PromotionLogNum;
			}
			return Crud.PromotionLogCrud.Insert(promotionLog);
		}

		///<summary>Inserts all given promotion logs. Will not return primary keys.</summary>
		public static void InsertMany(List<PromotionLog> listLogs) {
			if(listLogs.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listLogs);
				return;
			}
			PromotionLogCrud.InsertMany(listLogs);
		}

		#endregion Modification Methods
		public class MostRecent {
			public long PatNum;
			public DateTime DateTime;

			///<summary>Parameterless constructor for middle tier. Use parametered constructor instead</summary>
			public MostRecent() { }


			public MostRecent(long patNum,DateTime dateTime) {
				PatNum=patNum;
				DateTime=dateTime;
			}
		}
	}
}