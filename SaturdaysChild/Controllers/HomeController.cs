using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Web.Mvc;
using System.Net.Mail;
using SaturdaysChild.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using SaturdaysChild.Models.ViewModels;
using SaturdaysChild.Models.EntityModel;
using SaturdaysChild.Controllers.Lucene;

/// <summary>
/// Class provides functionality for pages in the Home folder. 
/// </summary>
namespace SaturdaysChild.Controllers
{
    public class HomeController : Controller
    {
        // reference to the Entity model
        private SaturdaysChildDbEntities satChilddb;
        public SaturdaysChildDbEntities SatChilddb { get => satChilddb; set => satChilddb = value; }

        /// <summary>
        /// Gets the About page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Gets the BugReport page. Instantiates BugReportViewModel and populates a list of bug types before sending it to the view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult BugReport()
        {
            var model = new BugReportViewModel
            {
                BugTime = DateTime.Now.ToLocalTime(),
                BugTypes = new List<BugReportViewModel.BugTypeModel>
                {
                    new BugReportViewModel.BugTypeModel { TypeId = 0, TypeName = "missing pages/broken links" },
                    new BugReportViewModel.BugTypeModel { TypeId = 1, TypeName = "unable to send message/email" },
                    new BugReportViewModel.BugTypeModel { TypeId = 2, TypeName = "unable to login" },
                    new BugReportViewModel.BugTypeModel { TypeId = 3, TypeName = "unable to register" },
                    new BugReportViewModel.BugTypeModel { TypeId = 4, TypeName = "missing content/pictures" },
                    new BugReportViewModel.BugTypeModel { TypeId = 5, TypeName = "poor image quality" },
                    new BugReportViewModel.BugTypeModel { TypeId = 6, TypeName = "visible html/code on pages" },
                    new BugReportViewModel.BugTypeModel { TypeId = 7, TypeName = "unable to reset password" },
                    new BugReportViewModel.BugTypeModel { TypeId = 8, TypeName = "other" }
                }
            };
            return View(model);
        }

        /// <summary>
        /// Posts new bug report from user entry to BugReport table via Entity model. Redirects to 
        /// thank you page with type of redirect if record stored, else redirects to BugReport (GET.)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BugReport(BugReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var report = new BugReport
                {
                    BugTime = model.BugTime,
                    BugType = model.BugType,
                    BugDescription = model.BugDescription
                };

                satChilddb.BugReports.Add(report);
                await satChilddb.SaveChangesAsync();

                return RedirectToAction("ThankYou", new { type = "bug" });
            }
            // d'oh, something bad happened
            return RedirectToAction("BugReport", model);
        }

        /// <summary>
        /// Gets Contact page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        /// <summary>
        /// Posts user-entered data from the Contact page to both a local log file and emails the contents via 
        /// SMTP on valid input and redirects to thank you page with type, else redirects to Contact (GET.)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                // model.Email = from, to = saturdays.child.designs
                using (var message = new MailMessage(model.Email, "saturdays.child.designs@gmail.com"))
                {
                    message.Subject = "Contact from SaturdaysChild";
                    message.Body = "Name: " + model.Name + '\n' + "Phone: " + model.Phone + '\n' + "Best Time to Contact: "
                            + model.BestContactStart + '\n' + model.Message;
                    message.IsBodyHtml = false;
                    message.BodyEncoding = Encoding.UTF8;

                    using (var client = new SmtpClient("smtp.gmail.com", 465))
                    {
                        client.Credentials = new NetworkCredential("saturdays.child.designs@gmail.com", "L1ghtb00x!");
                        client.EnableSsl = true;
                        client.Timeout = 10000;
                        client.UseDefaultCredentials = false;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Send(message);
                        // tries async send. if send fails, writes email content
                        // to local file and adds a bug to the BugReport table.
                        //try
                        //{
                        //    await client.SendMailAsync(message);
                        //}
                        //catch
                        //{
                        //    if (!System.IO.File.Exists(@"~\App_Data\Error_Data\EmailDump.txt")) System.IO.File.Create(@"~\App_Data\Error_Data\EmailDump.txt");
                        //    System.IO.File.AppendAllText(@"~\App_Data\Error_Data\EmailDump.txt", message.Body + '\n' + "<----->" + '\n');
                        //    var bug = new BugReport
                        //    {
                        //        BugTime = DateTime.Now,
                        //        BugType = "email",
                        //        BugDescription = "Email failed send."
                        //    };
                        //    satChilddb.BugReports.Add(bug);
                        //    await satChilddb.SaveChangesAsync();
                        //}
                        return RedirectToAction("ThankYou", new { type = "contact" });
                    }
                }
            }
            // d'oh, something bad happened
            return RedirectToAction("Contact", model);
        }

        /// <summary>
        /// Gets the HireMe page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult HireMe()
        {
            return View();
        }

        /// <summary>
        /// Gets the Index page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gets the PrivacyPolicy page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        /// <summary>
        /// Gets the Result page after running LuceneSearch on a formatted search string with fields, 
        /// instantiating and populating ResultsViewModel with results of search, if any exist.
        /// </summary>
        /// <param name="searchstring"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Results(string searchstring, string fields)
        {
            // validating lucene_index
            if (!Directory.Exists(LuceneSearch.LuceneDirectory)) Directory.CreateDirectory(LuceneSearch.LuceneDirectory);
            // assigning results
            List<LuceneData> results = LuceneSearch.Search(searchstring, fields).ToList();
            var list = new List<ResultsViewModel.ResultData>();
            // converting results to display model
            foreach(var result in results)
            {
                var item = new ResultsViewModel.ResultData
                {
                    PubDate = DateTime.Parse(result.PubDate),
                    Author = result.Author,
                    Title = result.Title,
                    Tags = result.Tags,
                    Link = result.Link,
                    Entry = result.Description//.Take(30).ToString() // gets first 30 words
                };
                list.Add(item);
            }
            // assigning model
            var model = new ResultsViewModel { Results = list };
            return View(model);
        }

        /// <summary>
        /// Gets the Samples page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Samples()
        {
            return View();
        }

        /// <summary>
        /// Gets the Search page. Instantiates and populates lists for author and type in SearchViewModel.
        /// TEMP: Model lists are instantiated with temporary values for search engine testing.
        /// </summary>
        /// <param name="searchterms"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Search(string searchterms)
        {
            //var authors = satChilddb.Employees.Select(a => new { Id = a.Id, Author = a.Name })
            //                                .Join(satChilddb.Blogs, a => a.Id, b => b.AuthorId, (a, b) => new { a, b })
            //                                .Join(satChilddb.Recommendations, c => c.a.Id, d => d.AuthorId, (c, d) => new { c, d })
            //                                .Select(e => new { AuthorId = e.c.a.Id, Author = e.c.a.Author })
            //                                .Distinct();
            // empty string added to prevent default selection
            var authorList = new List<string> { string.Empty };
            //foreach (var author in authors)
            //{
            //    authorList.Add(new Tuple<int, string>(author.AuthorId, author.Author));
            //}
            // TEMP VALUES for author
            authorList.Add("Robbie the Robot");
            authorList.Add("the Rickiest Rick");
            var model = new SearchViewModel
            {
                AuthorList = authorList,
                SearchString = searchterms,
                TypeList = new List<string> { string.Empty, "blog", "recommendation", "web page" }
            };
            return View(model);
        }

        /// <summary>
        /// Posts user-entered contents of Search page via redirect to Result page, if model is valid, or
        /// redirects to Search page with previous model contents if model is not valid. If model is valid,
        /// redirection to search page is sent with formatted search string and formatted fields string.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(SearchViewModel model)
        {
            // ensures search term is present
            if (ModelState.IsValid)
            {
                // TODO: add check for SQL injection
                // adding wildcards to non-special terms in searchstring and removing '-'
                var searchstring = string.Join(" ", model.SearchString.Replace('-', ' ').Split(' ').Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.Equals("AND") || a.Equals("OR") ? a.Trim() : a.Trim() + "*"));
                var fields = "Author, PubDate, Tags, Title, Type, Description";
                // adding additional parameters to the search string, if present
                // adding author
                if (!string.IsNullOrEmpty(model.Author))
                {
                    searchstring = string.Concat(searchstring, " AND Author: \"", model.Author, "*\"");
                }
                // adding tags, removing comma for formatting in Lucene search
                if (!string.IsNullOrEmpty(model.Tags))
                {
                    searchstring = string.Concat(searchstring, " AND Tags: \"", model.Tags.Replace(",", " "), "*\"");
                }
                // adding type
                if (!string.IsNullOrEmpty(model.Type))
                {
                    searchstring = string.Concat(searchstring, " AND Type: \"", model.Type, "*\"");
                }
                return RedirectToAction("Results", new { searchstring = searchstring, fields = fields });
            }
            return RedirectToAction("Search", model);
        }

        /// <summary>
        /// Gets Thank You page. Instantiates ThanksViewModel and populates 
        /// message as per type of call to the thank you page.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ThankYou(string type)
        {
            var model = new ThanksViewModel
            {
                Message = type.Equals("bug") ? "Thank you for reporting a bug. Your help is appreciated." 
                        : type.Equals("newaccount") ? "Thank you for registering a new account and welcome to Saturday's Child." 
                        : type.Equals("contact") ? "Thank you for sending a message. You should be contacted within the next two business days." 
                        : "Thank you, your time is appreciated."
            };
            return View(model);
        }
    }
}