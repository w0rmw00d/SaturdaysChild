using System;
using System.Linq;
using System.Collections.Generic;
using SaturdaysChild.Models.EntityModel;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Class includes template and scaffolding for interface between Lucene 
/// search engine and the ASP.NET Entity model for the SQL database.
/// </summary>
namespace SaturdaysChild.Models
{
    /// <summary>
    /// Class contains template for records in the Lucene Index.
    /// NOTE: InternalId corresponds to the SQL table id for the 
    /// indexed entry. Id refers to the Lucene index. Description 
    /// refers to the Entry field of tables in the SQL database. 
    /// Link refers to the link text to be used by the results page.
    /// </summary>
    public class LuceneData
    {
        public int Id { get; set; }
        public int InternalId { get; set; }
        public string PubDate { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Tags { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
    }

    public static class SampleLuceneRepository
    {
        public static LuceneData Get(int id)
        {
            return GetAll().SingleOrDefault(a => a.Id.Equals(id));
        }
        public static List<LuceneData> GetAll()
        {
            return new List<LuceneData>
            {
                new LuceneData { Id = 0, InternalId = 1, PubDate = DateTime.Now.ToUniversalTime().ToShortDateString(), Author = "Bob Generic", Title = "Bob Hates Vests", Type = "blog", Tags = "opinion, clothing", Link = "#", Description = "Generic Bob." },
                new LuceneData { Id = 1, InternalId = 2, PubDate = DateTime.Now.AddDays(-1).ToUniversalTime().ToShortDateString(), Author = "Sally Shrug", Title = "Sally Gives No Shits", Type = "blog", Tags = "opinion, angst", Link = "#", Description = "Generic Sally." },
                new LuceneData { Id = 2, InternalId = 3, PubDate = DateTime.Now.AddDays(-2).ToUniversalTime().ToShortDateString(), Author = "Sally Jesse Raphael", Title = "Sally Is A Host", Type = "blog", Tags = "opinion, occupation", Link = "#", Description = "Generic Jenny." },
                new LuceneData { Id = 3, InternalId = 56, PubDate = DateTime.Now.AddDays(-4).ToUniversalTime().ToShortDateString(), Author = "the Rickiest Rick", Title = "It Took Way too Long For this Show to Come Back", Type = "recommendation", Tags = "tv shows", Link = "#", Description = "Goddamn it, Rick and Morty."},
                new LuceneData { Id = 4, InternalId = 34, PubDate = DateTime.Now.AddDays(-3).ToUniversalTime().ToShortDateString(), Author = "Robbie the Robot", Title = "Beep Boop", Type = "recommendation", Tags = "processing", Link = "#", Description = "Beep, beep. BOOP."}
            };
        }
    }
    
    /// <summary>
    /// Class provides scaffolding and pulls SQL entries from tables for the Lucene 
    /// index. Using structure from Lucene Data, creates indexed pages for each entry 
    /// to expose the Lucene engine to the contents of the SQL tables.
    /// </summary>
    public class LuceneDataRepository
    {
    //    // reference to entity model for internal data
    //    private SaturdaysChildDbEntities satChilddb;
    //    public SaturdaysChildDbEntities SatChilddb { get => satChilddb; set => satChilddb = value; }

    //    public LuceneData Get(int id)
    //    {
    //        return GetAll().SingleOrDefault(a => a.Id.Equals(id));
    //    }

    //    /// <summary>
    //    /// Gets all available data records from the tables Blogs and 
    //    /// ReadingRecommendations in SaturdaysChildEntities, all Views 
    //    /// in Home and Login from Account for a Lucene search.
    //    /// </summary>
    //    /// <returns></returns>
    //    public List<LuceneData> GetAll()
    //    {
    //        var list = new List<LuceneData>();
    //        // grabbing blog entries
    //        foreach (var entry in GetBlogs())
    //        {
    //            list.Add(entry);
    //        }
    //        // grabbing book reviews
    //        foreach (var entry in GetReviews())
    //        {
    //            list.Add(entry);
    //        }
    //        // grabbing views from Home folder + login from Account
    //        foreach (var entry in GetViews())
    //        {
    //            list.Add(entry);
    //        }
    //        // assigning unique id for Lucene index
    //        var current = 0;
    //        foreach (var item in list)
    //        {
    //            item.Id = current;
    //            current++;
    //        }
    //        return list;
    //    }

    //    /// <summary>
    //    /// Gets all blog entries from SaturdaysChildEntities and converts to LuceneData type.
    //    /// </summary>
    //    /// <returns></returns>
    //    public List<LuceneData> GetBlogs()
    //    {
    //        var list = new List<LuceneData>();
    //        foreach (var entry in SatChilddb.Blogs)
    //        {
    //            list.Add(new LuceneData { Id = entry.Id, Title = entry.Title, Type = "blog", Description = entry.Entry.Split().Take(20).ToString(), Link = "@Url.Action('Detail', 'Blog', new { id = " + entry.Id + "})" });
    //        }
    //        return list;
    //    }

    //    /// <summary>
    //    /// Gets all book review entries from SaturdaysChildEntities and converts to LuceneData type.
    //    /// </summary>
    //    /// <returns></returns>
    //    public List<LuceneData> GetReviews()
    //    {
    //        var list = new List<LuceneData>();
    //        foreach (var review in SatChilddb.Recommendations)
    //        {
    //            list.Add(new LuceneData { Id = review.Id, Title = review.Title, Type = "review", Description = review.Entry.Split().Take(20).ToString(), Link = "@Url.Action('Detail', 'ReadingRecommendations', new { id = " + review.Id + "})" });
    //        }
    //        return list;
    //    }

    //    /// <summary>
    //    /// Gets all pages from Home folder in Views and Login from Account folder in Views.
    //    /// </summary>
    //    /// <returns></returns>
    //    /// TODO: create site map and map Lucene Data to elements marked able to be accessed by anonymous users.
    //    public List<LuceneData> GetViews()
    //    {
    //        var list = new List<LuceneData>();


    //        return list;
    //    }
    }
}