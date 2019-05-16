using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using SaturdaysChild.Models;
using System.Collections.Generic;
using SaturdaysChild.Models.ViewModels;
using SaturdaysChild.Models.EntityModel;
using SaturdaysChild.Controllers.Lucene;

/// <summary>
/// Class includes basic CRUD operations and view retrieval for blog entries. 
/// Content is separated from articles and recommendations for organization.
/// </summary>
namespace SaturdaysChild.Controllers
{
    public class BlogsController : Controller
    {
        // reference to Entity model
        private SaturdaysChildDbEntities satChilddb;
        public SaturdaysChildDbEntities SatChilddb { get => satChilddb; set => satChilddb = value; }

        /// <summary>
        /// Gets page used to add a new blog entry to the database. Instantiates 
        /// AddReccViewModel, filling Author with the identity of the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin, employee")]
        public ActionResult Add()
        {
            var model = new AddBlogViewModel { Author = HttpContext.User.Identity.Name };
            return View(model);
        }

        /// <summary>
        /// Posts user-entered data from Add page to Blogs table of the database. If model is valid, instantiates
        /// new Blog entry and populates it with user-entered data before asynchorously saving it
        /// to the database and redirecting to Index. If model is not valid, redirects to Add. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, employee")]
        public ActionResult Add(AddBlogViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entry = new Blog
                {
                    EntryDate = DateTime.Now,
                    AuthorId = satChilddb.Employees.FirstOrDefault(a => a.Email.Equals(model.Author)).Id,
                    Author = satChilddb.Employees.FirstOrDefault(a => a.Email.Equals(model.Author)).Name,
                    Title = model.Title,
                    Entry = model.Entry,
                    Tags = model.Tags
                };
                satChilddb.Blogs.Add(entry);
                satChilddb.SaveChangesAsync();
                // finding last entry
                // var lastEntry = satChilddb.Blogs.LastOrDefault(a => a.Author.Equals(model.Author));
                // adding new entry to lucene_index
                // AddToLuceneIndex(new LuceneData { 
                //                 InternalId = lastEntry.Id, 
                //                 PubDate = lastEntry.EntryDate, 
                //                 Author = lastEntry.Author,
                //                 Title = lastEntry.Title,
                //                 Type = "blog",
                //                 Tags = lastEntry.Tags,
                //                 Link = "",
                //                 Description = entry.Entry
                // });
                return RedirectToAction("Index", new { message = BlogMessages.AddSuccess });
            }
            return RedirectToAction("Add");
        }

        /// <summary>
        /// Posts delete to the Blog table of the database. Takes id from Detail or Edit pages, 
        /// retrieves record from Blog table, tries to remove record. If successful, redirects 
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
                satChilddb.Blogs.Remove(satChilddb.Blogs.FirstOrDefault(a => a.Id == id));
                satChilddb.SaveChangesAsync();
                // removing entry from lucene_index
                // var indexId = LuceneSearch.Search("", "");
                return RedirectToAction("Index", new { message = BlogMessages.DeleteSuccess });
            }
            catch
            {
                return RedirectToAction("Index", new { message = BlogMessages.Error });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Display()
        {
            return View();
            //var entries = satChilddb.Blogs.Where(a => a.EntryDate <= DateTime.Now).Where(a => a.EntryDate >= DateTime.Now.AddDays(-7)).ToList();
            //var list = new List<DetailBlogViewModel>();
            //foreach(var entry in entries)
            //{
            //    var item = new DetailBlogViewModel
            //    {
            //        Id = entry.Id,
            //        EntryDate = entry.EntryDate,
            //        Author = entry.Author,
            //        Tags = entry.Tags,
            //        Entry = entry.Entry,
            //        Title = entry.Title
            //    };
            //    list.Add(item);
            //}
            //return View(new BlogDisplayViewModel { BlogList = list });
        }


        /// <summary>
        /// Gets a page displaying a blog entry for it to be edited. Instantiates EditBlogViewModel 
        /// and fills model fields with record fetched via LINQ search using record id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            var entry = satChilddb.Blogs.FirstOrDefault(a => a.Id == id);
            var model = new EditBlogViewModel {
                Id = entry.Id,
                EntryDate = entry.EntryDate,
                Author = entry.Author,
                Tags = entry.Tags,
                Title = entry.Title,
                Entry = entry.Entry
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
        public ActionResult Edit(EditBlogViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entry = satChilddb.Blogs.FirstOrDefault(a => a.Id == model.Id);
                if(!entry.Author.Equals(model.Author))
                {
                    entry.Author = model.Author;
                    entry.AuthorId = satChilddb.Employees.FirstOrDefault(a => a.Name.Equals(model.Author)).Id;
                }
                entry.Title = model.Title.Equals(entry.Title) ? entry.Title : model.Title;
                entry.Entry = model.Entry.Equals(entry.Entry) ? entry.Entry : model.Entry;
                entry.Tags = model.Tags.Equals(entry.Tags) ? entry.Tags : model.Tags;
                satChilddb.SaveChangesAsync();
                return RedirectToAction("Index", new { message = BlogMessages.EditSuccess });
            }
            return RedirectToAction("Edit", model.Id);
        }

        /// <summary>
        /// Index page. Displays success, failure, or error messages. Default page redirected to for all other pages in the Blog folder.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin, employee")]
        public ActionResult Index(BlogMessages? message)
        {
            // reports success, failure, or error on actions in blog controller
            ViewBag.IndexMessage = message == BlogMessages.AddSuccess ? "The blog entry was successfully added." :
                                   message == BlogMessages.EditSuccess ? "The blog entry was successfully edited." :
                                   message == BlogMessages.DeleteSuccess ? "The blog entry was successfully deleted." :
                                   message == BlogMessages.Error ? "An error has occurred. Please try again." :
                                   "";
            return View();
        }
        
        /// <summary>
        /// Displays results of Lucene search on the Blogs table. Method takes a formatted search string and the fields to
        /// be searched, then calls the Lucene Search, instantiating ResultsViewModel and populating it with the results.
        /// </summary>
        /// <param name="searchstring"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Results(string searchstring, string fields)
        {
            List<LuceneData> results = LuceneSearch.Search(searchstring, fields).ToList();
            var list = new List<ResultsViewModel.ResultData>();
            foreach(var result in results)
            {
                var item = new ResultsViewModel.ResultData
                {
                    Id = result.InternalId,
                    PubDate = DateTime.Parse(result.PubDate),
                    Author = result.Author,
                    Title = result.Title,
                    Entry = result.Description,//.Take(30), // take the first 30 words
                    Tags = result.Tags
                };
                list.Add(item);
            }
            var model = new ResultsViewModel { Results = list };
            return View(model);
        }

        /// <summary>
        /// Gets the search page for the Blogs folder.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Search()
        {
            return View(new SearchViewModel());
        }

        /// <summary>
        /// If Model State is valid, formats search string and fields, and redirects to Results page with 
        /// search string and fields. If Model State is not valid, redirects to Search page with model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Search(SearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                var searchstring = model.SearchString;
                if (!string.IsNullOrEmpty(model.Author)) searchstring = string.Concat(searchstring, "AND Author: ", model.Author, ", ");
                if (!string.IsNullOrEmpty(model.Tags)) searchstring = string.Concat(searchstring, "AND Topics: ", model.Tags, ", ");
                if (!string.IsNullOrEmpty(model.Type)) searchstring = string.Concat(searchstring, "AND Type: ", model.Type);
                return RedirectToAction("Results", new { searchstring = searchstring, fields = "blog" });
            }
            return RedirectToAction("Search");
        }

        #region Helpers
        public enum BlogMessages
        {
            AddSuccess,
            EditSuccess,
            DeleteSuccess,
            Error
        } 
        #endregion
    }
}