using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orc.Fortress.SMSProvider
{
    [SmsProvider("File", "Test provider for writing to a sms file")]
    public class TextFileSmsProvider : BaseSMSProvider
    {
        [FromFortressSettings("The is the api key provided by your provider")]
        public string ApiKey { get; set; }
        [FromFortressSettings("This is the username for you account with the provider")]
        public string UserName { get; set; }

        public override void SendSms(string number, string message)
        {
            var file = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/sms.txt");
            var line =
@"{0}
Recipient: {1}
Message: {2}
ApiKey: {3}\n\n";
            var text = string.Format(line, DateTime.UtcNow.ToString(), number, message, ApiKey);
            File.AppendAllText(file, text);
        }
    }
}
