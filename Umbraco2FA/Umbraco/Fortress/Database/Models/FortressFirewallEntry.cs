using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Orc.Fortress.Database.Models
{
    [TableName(FortressConstants.TableNames.FortressFirewallEntry)]
    [PrimaryKey("Id", autoIncrement = true)]
    [ExplicitColumns]
    public class FortressFirewallEntry
    {
        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("IPAddress")]
        public string IPAddress { get; set; }

        [Column("Area")]
        public string Area { get; set; }

        [Column("FirewallMode")]
        public string FirewallMode { get; set; }
    }
}