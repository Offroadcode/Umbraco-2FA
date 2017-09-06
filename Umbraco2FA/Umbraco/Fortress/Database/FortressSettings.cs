using System;
using System.Collections.Generic;
using System.Linq;

using Orc.Fortress.Database.Models;
using Umbraco.Core.Persistence;
using System.Reflection;
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
             
        public bool GoogleAuthenticator_Enabled {
            get { return GetBool("GoogleAuthenticator_Enabled", true); }
            set { SetBool("GoogleAuthenticator_Enabled", value); }
        }
        public string GoogleAuthenticator_Name {
            get { return GetString("GoogleAuthenticator_Name"); }
            set { SetString("GoogleAuthenticator_Name", value); }
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



      
        public static IEnumerable<FortressSettingEntry> GetDefaultSettings()
        {
            var model = new FortressSettings(new List<FortressSettingEntry>());
          
            model.GoogleAuthenticator_Enabled = true;
            model.GoogleAuthenticator_Name = "My Umbraco Site";
          
            var data = model.GetRawData().Select(x => x.Value);
            return data;
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
  
}