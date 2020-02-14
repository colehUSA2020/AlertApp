using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.Validation;
using Twilio;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using AlertApp.SMSMessageConsole.Models;

namespace AlertApp
{
    class Program
    {

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {

            using (var db = new TestAlertContext())
            {

                while (true)//THIS MAKES THE PROGRAM RUN CONTINUALLY WHEN ACTIVATED
                {

                    // Get currrent day of week.
                    DayOfWeek today = DateTime.Today.DayOfWeek;

                    Boolean testValue = true;//THIS TESTS THE NEED FOR ALERTS


                    // Test current day of week and time of day. 
                    if (today == DayOfWeek.Sunday && DateTime.Now.Hour == 16 && DateTime.Now.Minute == 30)
                    {

                        while (testValue == true)
                        {

                            int testCounter = 0;//THIS VAR VALIDATES TESTVALUE
                            string email = "email";//THIS STRING WILL BE USED TO SEND EMAIL
                            string mobile = "mobile";//THIS STRING WILL BE USED TO SEND SMS
                            string firstName = " ";
                            string lastName = " ";


                            //DATABASE SECTION VERY IMPORTANT!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                            var alertQuery =
                                          from a in db.Agents
                                          where a.NoCheckFlag == true
                                          select a;

                            foreach (var alert in alertQuery)
                            {

                                //THIS INCREMENTS THE TESTCOUNTER THAT COUNTS THE FLAGS
                                if (alert.NoCheckFlag == true)
                                {
                                    testCounter++;
                                }

                                //SET FOR ALERT MESSAGES
                                firstName = alert.FirstName;
                                lastName = alert.LastName;
                                email = alert.EmailAddress;
                                mobile = alert.Mobile;

                                //instantiate internal TwilioService class object
                                SMSMessageConsole.SMSServices.TwilioService twilioSvc =
                                    new SMSMessageConsole.SMSServices.TwilioService();

                                twilioSvc.SendSMSMessage(mobile);//call the SendSMSMessage method

                                //instantiate internal EmailService class object
                                SMSMessageConsole.EmailServices.EmailService emailSvc =
                                    new SMSMessageConsole.EmailServices.EmailService();

                                emailSvc.SendEmails(firstName, lastName, email);//was supposed to be IEnumerable <Agent> agents

                            }//END DATABASE FOREACH!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                            if (testCounter == 0)//if the testCounter is zero, that means everything was reported
                            {
                                testValue = false;//this makes the program sleep until the next DateTime trigger point
                            }

                            if (testCounter > 0)//THIS ACTIVATES THE TIMER!!!!!!!!!!!!!!!!!!!!!
                            {
                                //THIS MAKES THE THREAD GO TO SLEEP VERY IMPORTANT!!!!!!!!!!!!!!!!!!!!!!!!!
                                System.Threading.Thread.Sleep(TimeSpan.FromMinutes(30));

                            }

                            testCounter = 0;//RESET THE TESTCOUNTER 

                        }//end inner testValue while 

                    }//end OUTER Date Time if

                }//end perpetual while I don't know if this is needed

            }//end using db scope


        }//end main


        //nested class to create db
        public class TestAlertContext : DbContext
        {
            public TestAlertContext()
                : base()
            {

            }

            //This creates the database
            public DbSet<Agent> Agents { get; set; }
            public DbSet<Builder> Builders { get; set; }
            public DbSet<Community> Communities { get; set; }
            public DbSet<AgentCommunity> AgentCommunities { get; set; }
            public DbSet<CommunityData> CommunityData { get; set; }

        }//end TestAlertContext


    }//end class Program

    //seperate namespaces and internal classes
    namespace SMSMessageConsole.SMSServices
    {
        internal class TwilioService
        {
            private const string ACCOUNT_SID = "SID-SID";
            private const string ACCOUNT_TOKEN = "TOKEN-HERE";

            private const string FROM_TELEPHONE_NUMBER = "SENDING TELEPHONE HERE";

            private const string MESSAGE = "Community Weekly Report Data must be entered by 6:00 PM tonight.";

            public void SendSMSMessage(string mobile)
            {
                //Instantiate Twilio Rest API !!!!!I believe this is now depreciated
                TwilioRestClient twilio = new TwilioRestClient(ACCOUNT_SID, ACCOUNT_TOKEN);

                from twilio.rest import Client

                //Send SMS Message
                Message message = twilio.SendMessage(FROM_TELEPHONE_NUMBER, mobile, MESSAGE);

                //Check for Message Error--no errors return
                if (message.RestException == null)
                    return;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("_______________________________");
                sb.AppendLine(" Error Code:....." + message.RestException.Code);
                sb.AppendLine(" Error Message:....." + message.RestException.Code);
                sb.AppendLine(" More Error Data:....." + message.RestException.MoreInfo);
                sb.AppendLine(" Mobile number:....." + mobile);
                sb.AppendLine("_______________________________");
                sb.AppendLine();
                Console.WriteLine(sb.ToString());

            }//end method

        }//end TwilioService

    }//end namespace SMSMessageConsole.SMSServices

    namespace SMSMessageConsole.Models
    {
        internal class SMTP
        {
            public string Server { get; set; }
            public int Port { get; set; }
            public bool SSL { get; set; }
            public string FromAddr { get; set; }
            public string FromName { get; set; }
            public string Password { get; set; }
        }
    }//end namespace SMSMessageConsole.Models

    namespace SMSMessageConsole.EmailServices
    {
        internal class EmailService
        {
            public void SendEmails(string email, string firstName, string lastName)//IEnumerable<Agent> agents
            {
                string errorMessage;
                //	1. Build email log on credentials
                NetworkCredential cred = new NetworkCredential("FROM-EMAIL-ADDRESS", "FROM-EMAIL-ADDRESS-PASSWORD");
                //	2. Instantiate Email Connection
                SmtpClient mailClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = cred
                };

                //	3. Build Email default data
                MailMessage message = new MailMessage
                {
                    From = new MailAddress("FROM-EMAIL-ADDRESS", "FROM-EMAIL-NAME"),    // I.E. "roy.bradley@gmail.com", "RW Bradley"
                    Subject = "Community Weekly Report Due",
                    Body = GetEmailBody(),
                    Priority = MailPriority.High,
                    IsBodyHtml = false
                };

                //	54 Process each Agent Email Address
                /*
                foreach (Agent agent in agents)
                {
                    //	5. Add Agent Email addresses to Email data
                    message.To.Add(new MailAddress(agent.EmailAddress, agent.FirstName + " " + agent.LastName));
                } */

                message.To.Add(new MailAddress(email, firstName + " " + lastName));

                try
                {
                    //	6. Send Email Message
                    mailClient.Send(message);
                }
                catch (SmtpException smtp)
                {
                    //	7. Email Error
                    errorMessage = "---------------------------------------------------------------------" + Environment.NewLine;
                    errorMessage += "SMTP Error: [" + smtp.StatusCode + "]" + Environment.NewLine;
                    errorMessage += "Message: " + smtp.Message + Environment.NewLine;
                    errorMessage += "Data: " + smtp.Data + Environment.NewLine;
                    errorMessage += "Stack Trace: " + smtp.StackTrace + Environment.NewLine;
                    errorMessage += "Inner Exception: " + smtp.InnerException;
                    errorMessage += "---------------------------------------------------------------------" + Environment.NewLine;
                    Console.WriteLine(errorMessage);
                }
                catch (Exception ex)
                {
                    //	8. Other Error
                    errorMessage = "---------------------------------------------------------------------" + Environment.NewLine;
                    errorMessage += "Error Message: " + ex.Message + Environment.NewLine;
                    errorMessage += "Stack Trace: " + ex.StackTrace + Environment.NewLine;
                    errorMessage += "Inner Exception: " + ex.InnerException;
                    errorMessage += "---------------------------------------------------------------------" + Environment.NewLine;
                    Console.WriteLine(errorMessage);
                }
                //	9. Dispose all
                message.Dispose();
                mailClient.Dispose();
            }

            private string GetEmailBody()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("\"Happy Sunday\" Reminder to send your Sunday numbers in by 6PM tonight");
                sb.AppendLine();
                sb.AppendLine("Thanks so much!");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("Lori Hamilton");
                return sb.ToString();
            }
        }
    }//end namespace SMSMessageConsole.EmailServices

}//end namespace

