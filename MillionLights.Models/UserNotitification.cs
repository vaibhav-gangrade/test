using Millionlights.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace Millionlights.Models
{
    public class UserNotitification
    {
        private MillionlightsContext db = new MillionlightsContext();

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
 
        [DisplayName("Receiver")]
        public int Receiver { get; set; }
        public string Receivers
        {
            get
            {
                var receivers = db.Users.Find(Receiver).EmailId;

                return receivers;
            }
        }

        [DisplayName("Sender")]
        public string Sender { get; set; }

        [DisplayName("Subject")]
        public string Subject { get; set; }

        [DisplayName("Message")]
        public string Message { get; set; }

        [DisplayName("DateSent")]
        public DateTime DateSent { get; set; }
        public string DateSentString
        {
            get
            {
                if (DateSent != null)
                {
                    return ((DateTime)DateSent).ToString(@"dd/MM/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        [DisplayName("IsRead")]
        public bool IsRead { get; set; }

        [DisplayName("ReadDate")]
        public DateTime? ReadDate { get; set; }

        [DisplayName("IsAlert")]
        public bool IsAlert { get; set; }

        [DisplayName("SMSDate")]
        public DateTime? SMSDate { get; set; }

        [DisplayName("MailDate")]
        public DateTime? MailDate { get; set; }
        public string MailDateString
        {
            get
            {
                if (MailDate != null)
                {
                    return ((DateTime)MailDate).ToString(@"dd/MM/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public int? NotificationStatusId { get; set; }
        //[ForeignKey("NotificationStatusId")]
        //public NotificationStatus NotStatus { get; set; }
        public string NotificationStatus
        {
            get
            {
                if (NotificationStatusId != null)
                {
                    var st=db.NotificationStatus.Where(x => x.Id == NotificationStatusId).FirstOrDefault();
                    if (st != null)
                    {
                        return (st.Status);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
       


    }
}
