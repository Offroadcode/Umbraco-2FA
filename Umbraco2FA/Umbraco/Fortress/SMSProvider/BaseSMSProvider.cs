using Orc.Fortress.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orc.Fortress.SMSProvider
{
    public abstract class BaseSMSProvider
    {
        public BaseSMSProvider()
        {

        }

        public abstract void SendSms(string number, string message);

     
    }
}
