using System;
using System.Linq;
using System.Web.Mvc;
using SaturdaysChild.Models.ViewModels;
using SaturdaysChild.Models.EntityModel;
using SaturdaysChild.Controllers.Lucene;
using System.Collections.Generic;

/// <summary>
/// Class includes basic CRUD operations and view retrieval for reading recommendations (book reviews,
/// article and white paper reviews). Content is separated from blog and articles for the organization.
/// </summary>
namespace SaturdaysChild.Controllers
{
    public class RecommendationsController : Controller
    {
        // Entity model reference, encapsulated for local reference 
        private SaturdaysChildDbEntities satChilddb;
        public SaturdaysChildDbEntities SatChilddb { get => satChilddb; set => satChilddb = value; }

        /// <summary>
        /// Gets page used to add a new recommendation to the database. Instantiates 
        /// AddBlogViewModel, filling Author with the identity of the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Add()
        {
            var model = new AddReccViewModel { Author = HttpContext.User.Identity.Name };
            return View(model);
        }

        /// <summary>
        /// Posts user-entered data from the Add page to Blogs table of the database. If the model is valid, 
        /// instantiates a new Blog entry and populates it with user-entered data before asynchorously saving 
        /// it to the database and redirecting to Index. If model is not valid, redirects to Add with model. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Add(AddReccViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entry = new Recommendation
                {
                    Author = model.Author,
                    AuthorId = satChilddb.Employees.FirstOrDefault(a => a.Name.Equals(model.Author)).Id,
                    RecommendationDate = DateTime.Now,
                    RecommendationTitle = model.ReccTitle,
                    RecommendationAuthor = model.ReccAuthor,
                    RecommendationLink = model.ReccLink,
                    Title = model.Title,
                    Entry = model.Entry,
                    Tags = model.Tags
                };
                satChilddb.Recommendations.Add(entry);
                satChilddb.SaveChangesAsync();
                // finding last entry
                // var lastEntry = satChilddb.Recommendations.LastOrDefault(a => a.Author.Equals(model.Author));
                // adding new entry to lucene_index
                // AddToLuceneIndex(new LuceneData {
                //                  InternalId = lastEntry.Id,
                //                  PubDate = lastEntry.RecommendationDate,
                //                  Author = lastEntry.Author,
                //                  Title = lastEntry.Title,
                //                  Type = "recommendation",
                //                  Tags = lastEntry.Tags,
                //                  Link = "",
                //                  Description = lastEntry.Entry
                // });
                return RedirectToAction("Index", new { message = RecommendationMessages.AddSuccess });
            }
            return RedirectToAction("Add", model);
        }

        /// <summary>
        /// Gets page that allows Recommendation records to be edited. Uses LINQ to 
        /// fetch record with matching id, then instantiates EditReccViewModel and 
        /// populates its fields with the SQL record. Returns page with the model.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            var entry = satChilddb.Recommendations.FirstOrDefault(a => a.Id == id);
            var model = new EditReccViewModel
            {
                Id = entry.Id,
                Author = entry.Author,
                ReccAuthor = entry.RecommendationAuthor,
                ReccTitle = entry.RecommendationTitle,
                ReccLink = entry.RecommendationLink,
                Title = entry.Title,
                Entry = entry.Entry,
                Tags = entry.Tags
            };
            return View(model);
        }

        /// <summary>
        /// Posts contents of page that allows Recommendation records to be edited. If Model State 
        /// is valid, uses LINQ to fetch record with matching id and checks it against model fields, 
        /// replacing the SQL fields with model fields on a difference between the two and async 
        /// saving the changes. If Model State is not valid, redirects to Edit GET with the id.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(EditReccViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entry = satChilddb.Recommendations.FirstOrDefault(a => a.Id == model.Id);
                if (!entry.Author.Equals(model.Author))
                {
                    entry.Author = model.Author;
                    entry.AuthorId = satChilddb.Employees.FirstOrDefault(a => a.Name.Equals(model.Author)).Id;
                }
                entry.RecommendationAuthor = model.ReccAuthor.Equals(entry.RecommendationAuthor) ? entry.RecommendationAuthor : model.ReccAuthor;
                entry.RecommendationTitle = model.ReccTitle.Equals(entry.RecommendationTitle) ? entry.RecommendationTitle : model.ReccTitle;
                entry.RecommendationLink = model.ReccLink.Equals(entry.RecommendationLink) ? entry.RecommendationLink : model.ReccLink;
                entry.Title = model.Title.Equals(entry.Title) ? entry.Title : model.Title;
                entry.Entry = model.Entry.Equals(entry.Entry) ? entry.Entry : model.Entry;
                entry.Tags = model.Tags.Equals(entry.Tags) ? entry.Tags : model.Tags;

                satChilddb.SaveChangesAsync();
                return RedirectToAction("Index", new { message = RecommendationMessages.EditSuccess });
            }
            return RedirectToAction("Edit", model.Id);
        }

        /// <summary>
        /// Gets a page showing some detail of a Recommendation record. Record fetched via LINQ 
        /// and id, and used to populate DetailReccViewModel. Model is then returned with the page.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Detail(int id)
        {
            var entry = satChilddb.Recommendations.FirstOrDefault(a => a.Id == id);
            var model = new DetailReccViewModel
            {
                Id = entry.Id,
                EntryDate = entry.RecommendationDate,
                ReccAuthor = entry.RecommendationAuthor,
                ReccTitle = entry.RecommendationTitle,
                ReccLink = entry.RecommendationLink,
                Title = entry.Title,
                Entry = entry.Entry,//.Take(30),
                Tags = entry.Tags
            };
            return View(model);
        }

        /// <summary>
        /// Posts the removal of a Recommendation from the SQL database. Given the id,
        /// locates the Recommendation record via LINQ and tries to remove it before saving 
        /// async and redirecting to the Index page with a failure or success message.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            var entry = satChilddb.Recommendations.FirstOrDefault(a => a.Id == id);
            try
            {
                satChilddb.Recommendations.Remove(entry);
                return RedirectToAction("Index", new { message = RecommendationMessages.DeleteSuccess });
            }
            catch
            {
                return RedirectToAction("Index", new { message = RecommendationMessages.Error });
            }
        }

        /// <summary>
        /// Gets a handful of Recommendation entries within the last week for display, for site visitors.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Display(DateTime? end)
        {
            List<Recommendation> list = (end == null) ? satChilddb.Recommendations.Where(a => a.RecommendationDate >= DateTime.Now.AddDays(-7)).ToList() :
                                                        satChilddb.Recommendations.Where(a => a.RecommendationDate > ((DateTime)end).AddDays(-7)).Where(a => a.RecommendationDate < end).ToList();
            var model = new ReccDisplayViewModel();
            
            foreach(var item in list)
            {
                var listItem = new DetailReccViewModel
                {
                    Id = item.Id,
                    EntryDate = item.RecommendationDate,
                    ReccAuthor = item.RecommendationAuthor,
                    ReccTitle = item.RecommendationTitle,
                    ReccLink = item.RecommendationLink,
                    Title = item.Title,
                    Entry = item.Entry,
                    Tags = item.Tags
                };
                model.ReccList.Add(listItem);
            }
            model.End = model.ReccList.OrderBy(a => a.EntryDate).LastOrDefault().EntryDate;
            return View(model);
        }

        /// <summary>
        /// Gets the Index view. Default page for Recommendation folder. Displays 
        /// messages for success/failure from other views in folder. 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "admin")]
        public ActionResult Index(RecommendationMessages? message)
        {
            ViewBag.IndexMessage = message == RecommendationMessages.AddSuccess ? "The recommendation was successfully added." :
                                   message == RecommendationMessages.DeleteSuccess ? "The recommendation was successfully deleted." :
                                   message == RecommendationMessages.EditSuccess ? "The recommendation was successfully edited." :
                                   message == RecommendationMessages.Error ? "An error has occurred. Please try again." :
                                   "";

            return View();
        }

        #region Helpers
        public enum RecommendationMessages
        {
            AddSuccess,
            EditSuccess,
            DeleteSuccess,
            Error
        }
        #endregion
    }
}