using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using SaturdaysChild.Models.ViewModels;
using SaturdaysChild.Models.EntityModel;

/// <summary>
/// Controller for Members section of site. Responsible for displaying any messages left on the system for the current user.
/// </summary>
namespace SaturdaysChild.Controllers
{
    [Authorize(Roles = "admin, employee, user")]
    public class MembersController : Controller
    {
        // reference to the Entity model
        private SaturdaysChildDbEntities satChilddb;
        public SaturdaysChildDbEntities SatChilddb { get => satChilddb; set => satChilddb = value; }

        /// <summary>
        /// Page seen by default whenever user logs into system. Instantiates MemberIndexViewModel and 
        /// populates list of messages and alerts retrieved from the MemberMessage table in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            // fetching member messages, if any exist for the user
            var membermessages = satChilddb.MemberMessages.Where(a => a.MemberName.Equals(HttpContext.User.Identity.Name)).OrderByDescending(a => a.Date);
            // creating lists for alerts and messages
            var messages = new List<MemberIndexViewModel.MessageData>();
            var alerts = new List<MemberIndexViewModel.AlertData>();
            // NOTE: the Alert tag is used to draw extra attention to the contents of the message, and will only be non-null if the message needs extra attention
            foreach(var message in membermessages)
            {
                messages.Add(new MemberIndexViewModel.MessageData { Date = message.Date, From = message.From, Message = message.Message });
                if(!string.IsNullOrEmpty(message.Alert)) alerts.Add(new MemberIndexViewModel.AlertData { Date = message.Date, Alert = message.Alert });
                // setting message read receipt to true
                message.IsRead = true;
                message.ReadDate = DateTime.Now;
            }
            // saving changes to database (saving changes to the read receipts)
            satChilddb.SaveChangesAsync();

            return View(new MemberIndexViewModel { Messages = messages, Name = HttpContext.User.Identity.Name, Alerts = alerts });
        }
    }
}