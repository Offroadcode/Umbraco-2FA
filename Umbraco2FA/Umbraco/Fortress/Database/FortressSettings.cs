using System;
using System.Collections.Generic;
using System.Linq;

using Orc.Fortress.Database.Models;
using Umbraco.Core.Persistence;
using System.Reflection;
using Orc.Fortress.SMSProvider;
using Orc.Fortress.BackOffice.Controllers;

namespace Orc.Fortress.Database
{
    public class FortressSettings
    {
        private readonly Dictionary<string,FortressSettingEntry> Data;
        
        public FortressSettings(List<FortressSettingEntry> data)
        {
            Data = data.ToDictionary(x => x.Key, x => x);
        }

     
        public FirewallMode FrontEndFirewallMode
        {
            get { return GetEnum<FirewallMode>("FrontEndFirewallMode"); }
            set { SetEnum("FrontEndFirewallMode", value); }
        }
        public FirewallMode BackofficeFirewallMode
        {
            get { return GetEnum<FirewallMode>("BackofficeFirewallMode"); }
            set { SetEnum("BackofficeFirewallMode", value); }
        }

        public bool GoogleAuthenticator_Enabled {
            get { return GetBool("GoogleAuthenticator_Enabled", true); }
            set { SetBool("GoogleAuthenticator_Enabled", value); }
        }
        public string GoogleAuthenticator_Name {
            get { return GetString("GoogleAuthenticator_Name"); }
            set { SetString("GoogleAuthenticator_Name", value); }
        }
        public string SMS_CurrentSMSProvider
        {
            get { return GetString("SMS_CurrentSMSProvider"); }
            set { SetString("SMS_CurrentSMSProvider", value); }
        }

        public string SMS_MessageFormat {
            get { return GetString("SMS_MessageFormat"); }
            set { SetString("SMS_MessageFormat", value); }
        }
        public bool SMS_Enabled {
            get { return GetBool("SMS_Enabled", false); }
            set { SetBool("SMS_Enabled", value); }
        }

        private bool GetBool(string key, bool defaultVal)
        {
            if (Data.ContainsKey(key))
            {
                return Data[key].BoolValue.Value;
            }
            return defaultVal;
        }
        private void SetBool(string key, bool value)
        {
            if (!Data.ContainsKey(key))
            {
                Data.Add(key, new FortressSettingEntry(key,value));
            }
            else
            {
                Data[key].BoolValue = value;
            }
        }

        private int GetInt(string key)
        {
            return Data[key].IntValue.Value;
        }
        private void SetInt(string key, int value)
        {
            if (!Data.ContainsKey(key))
            {
                Data.Add(key, new FortressSettingEntry(key, value));
            }
            else
            {
                Data[key].IntValue = value;
            }
        }
        private string GetString(string key)
        {
            if (Data.ContainsKey(key))
            {
                return Data[key].StrValue;
            }else
            {
                return null;
            }
        }

       

        private void SetString(string key, string value)
        {
            if (!Data.ContainsKey(key))
            {
                Data.Add(key, new FortressSettingEntry(key, value));
            }
            else
            {
                Data[key].StrValue = value;
            }
            
        }
        private TEnum GetEnum<TEnum>(string key) where TEnum : struct
        {
            var strValue = GetString(key);
            return (TEnum) Enum.Parse(typeof(TEnum), strValue);
        }
        private void SetEnum<TEnum>(string key, TEnum val) where TEnum : struct
        {
            SetString(key, val.ToString()); 
        }

        internal Dictionary<string, FortressSettingEntry> GetRawData()
        {
            return Data;
        }



        internal Type GetSmsProviderType()
        {
            var strValue = SMS_CurrentSMSProvider;
            if (string.IsNullOrEmpty(strValue))
            {
                return typeof(TextFileSmsProvider);
            }else
            {
                return Type.GetType(strValue);
            }
            
        }
        public static IEnumerable<FortressSettingEntry> GetDefaultSettings()
        {
            var model = new FortressSettings(new List<FortressSettingEntry>());
            model.BackofficeFirewallMode = FirewallMode.Disabled;
            model.FrontEndFirewallMode = FirewallMode.Disabled;
            model.GoogleAuthenticator_Enabled = true;
            model.GoogleAuthenticator_Name = "My Umbraco Site";
            model.SMS_Enabled = false;
            model.SMS_MessageFormat = "Your auth key is {0}";
            model.SMS_CurrentSMSProvider = "";
            var data = model.GetRawData().Select(x => x.Value);
            return data;
        }

        public void PopulateSmsPropertiesOnObject(BaseSMSProvider obj)
        {
            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    FromFortressSettingsAttribute authAttr = attr as FromFortressSettingsAttribute;
                    if (authAttr != null)
                    {
                        string propName = prop.Name;
                        var key = GenerateCustomSettingKey(obj, propName);
                        
                        prop.SetValue(obj, Convert.ChangeType(key, prop.PropertyType), null);
                    }
                }
            }
        }

        internal void SaveSmsProviderSettings(SMSProviderSettingsModel model)
        {
            var type = Type.GetType(model.ClassName);
            foreach(var setting in model.Settings)
            {
                var key = GenerateCustomSettingKey(type, setting.Name);
                SetString(key, setting.Value);
            }
        }
        public List<SMSProviderSettingModel> GetPropertiesOnType(Type type)
        {
            List<SMSProviderSettingModel> settings = new List<SMSProviderSettingModel>();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    FromFortressSettingsAttribute authAttr = attr as FromFortressSettingsAttribute;
                    if (authAttr != null)
                    {
                        string propName = prop.Name;
                        var key = GenerateCustomSettingKey(type, propName);
                        var model = new SMSProviderSettingModel();
                        model.Name = propName;
                        model.Value = GetString(key);
                        settings.Add(model);
                    }
                }
            }
            return settings;
        }
        internal static string GenerateCustomSettingKey(Type type, string propertyName)
        {
            return "CustomSetting_" + type.FullName + "_" + propertyName;
        }
        internal static string GenerateCustomSettingKey(object obj, string propertyName)
        {
            return "CustomSetting_" + obj.GetType().FullName + "_" + propertyName;
        }
    }
    public enum FirewallMode
    {
        Disabled,
        WhiteList,
        BlackList
    }
    public enum FirewallArea
    {
        FrontEnd,
        BackOffice
    }
}