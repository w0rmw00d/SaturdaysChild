using System;
using System.Linq;
using System.Web.Mvc;
using SaturdaysChild.Models.ViewModels;
using SaturdaysChild.Models.EntityModel;
using System.Data.Entity;

/// <summary>
/// Class used to display bugs to admin users.
/// NOTE: Support Tickets are linked to bug reports.
/// </summary>
namespace SaturdaysChild.Controllers
{
    [Authorize(Roles = "admin")]
    public class BugController : Controller
    {
        // reference to the Entity model
        private SaturdaysChildDbEntities satChilddb;
        public SaturdaysChildDbEntities SatChilddb { get => satChilddb; set => satChilddb = value; }

        /// <summary>
        /// Gets page displaying closed site bugs sorted by date from recent to less recent.
        /// </summary>
        /// <returns></returns>
        public ActionResult BugHistory()
        {
            var model = new BugHistoryViewModel();

            // joining BugReport and SupportTicket on Id and BugId, where the ticket is not open (has been given a close date), ordered by bug date newest to oldest
            var closed = satChilddb.BugReports.Join(satChilddb.SupportTickets, report => report.Id, ticket => ticket.BugId, (report, ticket) => new { Report = report, Ticket = ticket })
                                            .Where(a => a.Ticket.TicketEnd != null).OrderByDescending(a => a.Report.BugTime);
            foreach (var item in closed)
            {
                model.ClosedBugList.Add( new BugItems
                {
                    Id = item.Report.Id,
                    BugTime = item.Report.BugTime,
                    BugType = item.Report.BugType,
                    BugDescription = item.Report.BugDescription
                });
            }
            return View(model);
        }

        /// <summary>
        /// Default page for Bug folder. Displays list of open site bugs sorted by date from recent to least recent for admin users.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string message)
        {
            var model = new BugViewModel();
            // joining BugReport and SupportTicket on Id and BugId, where the ticket is still open (has not been given a close date), ordered by bug date newest to oldest
            var open = satChilddb.BugReports.Join(satChilddb.SupportTickets, report => report.Id, ticket => ticket.BugId, (report, ticket) => new { Report = report, Ticket = ticket })
                                            .Where(a => a.Ticket.TicketEnd == null).OrderByDescending(a => a.Report.BugTime);
            foreach (var item in open)
            {
                model.OpenBugList.Add(new BugItems
                {
                    Id = item.Report.Id,
                    BugTime = item.Report.BugTime,
                    BugType = item.Report.BugType,
                    BugDescription = item.Report.BugDescription
                });
            }
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Search()
        {
            return View(new BugSearchViewModel());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult Results(BugSearchViewModel search)
        {
            // if all fields in BugSearchViewModel are null, redirect
            if (search.BugStart == null && search.BugEnd == null && search.BugType == null)
            {
                return RedirectToAction("Index", new { message = "The search requires parameters. Please try again." });
            }
            // filtering contents of BugReports table on any of the three values in BugSearchViewModel
            var query = satChilddb.BugReports.Where(a => a.BugTime >= (search.BugStart == null ? DateTime.MinValue : search.BugStart))
                                             .Where(a => a.BugTime <= (search.BugEnd == null ? DateTime.MaxValue : search.BugEnd))
                                             .Where(a => a.BugType.Equals(string.IsNullOrEmpty(search.BugType) ? a.BugType : search.BugType));
            var model = new BugViewModel();
            foreach (var item in query)
            {
                var entry = new BugItems
                {
                    Id = item.Id,
                    BugTime = item.BugTime,
                    BugType = item.BugType,
                    BugDescription = item.BugDescription
                };
            }
            return View(model);
        }
    }
}