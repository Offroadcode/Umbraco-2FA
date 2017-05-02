using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Orc.Fortress.Database.Models
{
    [TableName(FortressConstants.TableNames.FortressLoginEvents)]
    [ExplicitColumns]
    public class FortressLoginEvent
    {
        [Column("UserId")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? UserId { get; set; }


        [Column("UserName")]
        public string UserName { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }

        [Column("Status")]
        public string Status { get; set; }

        [Column("WasFromTwoFactorStage")]
        public bool WasFromTwoFactorStage { get; set; }
        [Column("IPAddress")]
        public string IpAddress { get; set; }
        [Column("Hostname")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Hostname { get; set; }
    }
}