using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orc.Fortress.Models
{
    public class SmsProviderViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Classname { get; set; }
        //public List<SMSCustomProperty> Properties { get; set; }
    }
    /*public class SMSCustomProperty
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
    }*/
}
