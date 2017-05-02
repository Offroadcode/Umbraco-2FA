using Orc.Fortress.Attributes;
using Orc.Fortress.BackOffice.Models;

namespace Orc.Fortress.BackOffice.Controllers
{
    [FortressPluginController]
    public class LogsApiController : BaseFortressBackofficeController
    {
        public LogsViewModel GetData(int page = 1, string ipAddressFilter = "", string userName = "")
        {
            var model = new LogsViewModel();

            var paged = CustomDatabase.GetLogEntries(page, 50, ipAddressFilter, userName);

            model.CurrentPage = paged.CurrentPage;
            model.TotalPages = paged.TotalPages;
            model.TotalEntries = paged.TotalItems;
            model.Entries = paged.Items;

            return model;
        }
    }
}