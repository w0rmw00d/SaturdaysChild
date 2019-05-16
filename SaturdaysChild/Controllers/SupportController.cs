using System;
using System.Linq;
using System.Web.Mvc;
using SaturdaysChild.Models.ViewModels;
using SaturdaysChild.Models.EntityModel;

/// <summary>
/// Class includes basic CRUD operations on the SupportTickets table and view retrieval for support tickets.
/// </summary>
namespace SaturdaysChild.Controllers
{
    public class SupportController : Controller
    {
        // reference to Entity model
        private SaturdaysChildDbEntities satChilddb;
        public SaturdaysChildDbEntities SatChilddb { get => satChilddb; set => satChilddb = value; }

        /// <summary>
        /// Gets page used to add a new support ticket to the database. Instantiates AddSupportTicket.
        /// NOTE: If Add is being called from SupportController, bugId will be null. If called from 
        /// BugController, BugId will not be null.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin, employee")]
        public ActionResult Add(int? bugId)
        {
            return View(new AddSupportTicket { BugId = ( bugId == null ? null : bugId )});
        }

        /// <summary>
        /// Posts user-entered data from Add page to the SupportTicket table of the database. If model is 
        /// valid, instantiates new Support Ticket and populates it with user-entered data before asynchronously 
        /// saving it to the database and redirecting to Index. If model is not valid, redirects to Add page.
        /// NOTE: BugId may be null, if Add is being called from SupportController. If called from BugController,
        /// BugId will not be null.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, employee")]
        public ActionResult Add(AddSupportTicket model)
        {
            if (ModelState.IsValid)
            {
                var ticket = new SupportTicket
                {
                    EmpId = satChilddb.Employees.FirstOrDefault(a => a.Name.Equals(HttpContext.User.Identity.Name)).Id,
                    ClientId = satChilddb.Clients.FirstOrDefault(a => a.Name.Equals(model.Client)).Id,
                    ContractId = satChilddb.Contracts.FirstOrDefault(a => a.Title.Equals(model.Contract)).Id,
                    ContactName = model.ContactName,
                    Contact = model.Contact,
                    TicketStart = DateTime.Now,
                    Notes = model.Notes,
                    BugId = model.BugId
                };
                satChilddb.SupportTickets.Add(ticket);
                satChilddb.SaveChangesAsync();
                // finding last entry
                // var lastEntry = satChilddb.SupportTickets.LastOrDefault(a => a.Author.Equals(model.Author));
                // adding new entry to lucene_index
                // TODO: add implementation for searching support tickets
                return RedirectToAction("Index", new { message = SupportMessages.AddSuccess });
            }
            return RedirectToAction("Add", model);
        }

        /// <summary>
        /// Posts delete to the Support Ticket table of the database. Takes id from Detail or Edit pages, 
        /// retrieves record from Support Ticket table, tries to remove record. If successful, redirects 
        /// to Index with success message. If unsuccessful, redirects to Index with error message.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            try
            {
                satChilddb.SupportTickets.Remove(satChilddb.SupportTickets.FirstOrDefault(a => a.Id == id));
                satChilddb.SaveChangesAsync();
                // removing entry from lucene_index
                // var indexId = LuceneSearch.Search("", "");
                return RedirectToAction("Index", new { message = SupportMessages.DeleteSuccess });
            }
            catch
            {
                return RedirectToAction("Index", new { message = SupportMessages.Error });
            }
        }
        
        /// <summary>
        /// Gets a page displaying a support ticket entry for it to be edited. Instantiates EditSupportTicket
        /// and fills model fields with record fetched via Linq search using the record id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            var entry = satChilddb.SupportTickets.FirstOrDefault(a => a.Id == id);
            var model = new EditSupportTicket
            {
                Id = id,
                Employee = satChilddb.Employees.FirstOrDefault(a => a.Id == entry.EmpId).Name,
                Client = satChilddb.Clients.FirstOrDefault(a => a.Id == entry.ClientId).Name,
                Contract = satChilddb.Contracts.FirstOrDefault(a => a.Id == entry.ContractId).Title,
                ContactName = entry.ContactName,
                Contact = entry.Contact,
                TicketStart = entry.TicketStart,
                TicketEnd = entry.TicketEnd,
                Notes = entry.Notes
            };
            return View(model);
        }

        /// <summary>
        /// Edits records in SQL database. If Model State is valid, locates record via LINQ search using model id, then
        /// checks record against user provided data. On changes, overwrites SQL entry with new data and saves async,
        /// then redirects to Index with success message. If Model State is not valid, redirects to Edit with model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(EditSupportTicket model)
        {
            if (ModelState.IsValid)
            {
                var entry = satChilddb.SupportTickets.FirstOrDefault(a => a.Id == model.Id);
                entry.EmpId = (entry.EmpId == satChilddb.Employees.FirstOrDefault(a => a.Name.Equals(model.Employee)).Id) ? entry.EmpId : satChilddb.Employees.FirstOrDefault(a => a.Name.Equals(model.Employee)).Id;
                entry.ClientId = (entry.ClientId == satChilddb.Clients.FirstOrDefault(a => a.Name.Equals(model.Client)).Id) ? entry.ClientId : satChilddb.Clients.FirstOrDefault(a => a.Name.Equals(model.Client)).Id;
                entry.ContractId = (entry.ContractId == satChilddb.Contracts.FirstOrDefault(a => a.Title.Equals(model.Contract)).Id) ? entry.ContractId : satChilddb.Contracts.FirstOrDefault(a => a.Title.Equals(model.Contract)).Id;
                entry.ContactName = (entry.ContactName.Equals(model.ContactName) ? entry.ContactName : model.ContactName);
                entry.Contact = entry.Contact.Equals(model.Contact) ? entry.Contact : model.Contact;
                entry.TicketStart = entry.TicketStart == model.TicketStart ? entry.TicketStart : model.TicketStart;
                entry.TicketEnd = entry.TicketEnd == model.TicketEnd ? entry.TicketEnd : model.TicketEnd;
                entry.Notes = entry.Notes.Equals(model.Notes) ? entry.Notes : model.Notes;
                entry.BugId = entry.BugId == model.BugId ? entry.BugId : model.BugId;

                satChilddb.SaveChangesAsync();
                return RedirectToAction("Index", new { message = SupportMessages.EditSuccess });
            }
            return RedirectToAction("Index", new { message = SupportMessages.Error });
        }

        /// <summary>
        /// Index page. Displays success, failure, or error messages. Default page redirected to for all other pages in the Blog folder.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ActionResult Index(SupportMessages? message)
        {
            // reports success, failure, or error on actions in blog controller
            ViewBag.IndexMessage = message == SupportMessages.AddSuccess ? "The support ticket was successfully added." :
                                   message == SupportMessages.EditSuccess ? "The support ticket was successfully edited." :
                                   message == SupportMessages.DeleteSuccess ? "The support ticket was successfully deleted." :
                                   message == SupportMessages.Error ? "An error has occurred. Please try again." :
                                   "";
            return View();
        }

        #region Helpers
        public enum SupportMessages
        {
            AddSuccess,
            EditSuccess,
            Error,
            DeleteSuccess
        }
        #endregion
    }
}