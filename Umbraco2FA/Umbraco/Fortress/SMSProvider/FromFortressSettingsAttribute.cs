using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orc.Fortress.SMSProvider
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class FromFortressSettingsAttribute:Attribute
    {
        public FromFortressSettingsAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SmsProviderAttribute : Attribute
    {
        public SmsProviderAttribute(string Name, string Description)
        {
            this.Description = Description;
            this.Name = Name;
        }

        public string Description { get; set; }
        public string Name { get; set; }
    }
}
