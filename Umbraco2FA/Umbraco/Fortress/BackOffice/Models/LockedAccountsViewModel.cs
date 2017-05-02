using System.Collections.Generic;
using Orc.Fortress.Database.Models;

namespace Orc.Fortress.BackOffice.Models
{
    public class LockedAccountsViewModel
    {
        public List<LockedAccountViewModel> Entries { get; set; }
    }

    public class LockedAccountViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}