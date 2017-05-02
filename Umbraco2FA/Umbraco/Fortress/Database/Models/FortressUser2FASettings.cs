using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Orc.Fortress.Database.Models
{
    [TableName(FortressConstants.TableNames.FortressUser2FASettings)]
    [PrimaryKey("UserId", autoIncrement = false)]
    [ExplicitColumns]
    public class FortressUser2FASettings
    {
        [Column("UserId")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        public int UserId { get; set; }

        [Column("Provider")]
        public string Provider { get; set; }

        [Column("Configuration")]
        public string Configuration { get; set; }

        [Column("IsValidated")]
        public bool IsValidated { get; set; }

        [Column("CurrentCodeGenerated")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? CurrentCodeGenerated { get; set; }

        [NullSetting(NullSetting = NullSettings.Null)]
        [Column("CurrentCode")]
        public string CurrentCode { get; set; }
    }
}