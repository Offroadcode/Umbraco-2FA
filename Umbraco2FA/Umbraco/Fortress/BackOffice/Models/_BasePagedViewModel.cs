using System.Collections.Generic;

namespace Orc.Fortress.BackOffice.Models
{
    public abstract class _BasePagedViewModel<T>
    {

        public List<T> Entries { get; set; }
        public long TotalPages { get; set; }
        public long CurrentPage { get; set; }
        public long TotalEntries { get; set; }

    }
}