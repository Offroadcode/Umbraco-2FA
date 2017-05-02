using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Orc.Fortress.Database.Models
{
    [TableName(FortressConstants.TableNames.FortressSettings)]
    [PrimaryKey("Key", autoIncrement = false)]
    [ExplicitColumns]
    public class FortressSettingEntry
    {
        public FortressSettingEntry()
        {

        }
        public FortressSettingEntry(string key, bool value)
        {
            Key = key;
            BoolValue = value;
        }

        public FortressSettingEntry(string key, string value)
        {
            Key = key;
            StrValue = value;
        }

        public FortressSettingEntry(string key, int value)
        {
            Key = key;
            IntValue = value;
        }

        [Column("Key")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        public string Key { get; set; }

        [Column("StrValue")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string StrValue { get; set; }

        [Column("IntValue")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? IntValue { get; set; }

        [Column("BoolValue")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public bool? BoolValue { get; set; }

        public static void InsertInitialSettings(UmbracoDatabase db)
        {
            var data = FortressSettings.GetDefaultSettings();
            

            db.BulkInsertRecords(data);
        }
    }
}