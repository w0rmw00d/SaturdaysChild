using System;
using System.IO;
using System.Web;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using SaturdaysChild.Models;
using Lucene.Net.QueryParsers;
using System.Collections.Generic;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;
using System.Text.RegularExpressions;

namespace SaturdaysChild.Controllers.Lucene
{
    public class LuceneSearch
    {
        // full physical path to the lucene_index folder at application root
        public static string LuceneDirectory = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index");
        // local copy of lucene_index for operations in Directory
        private static FSDirectory TempDirectory;
        // used by all search methods to access the lucene search index
        public static FSDirectory Directory
        {
            get
            {
                // assigning TempDirectory to current LuceneDirectory
                if (TempDirectory == null) TempDirectory = FSDirectory.Open(new DirectoryInfo(LuceneDirectory));
                // if TempDirectory is locked, unlock it
                if (IndexWriter.IsLocked(TempDirectory)) IndexWriter.Unlock(TempDirectory);
                // assigning a path to check if main directory is locked
                var lockFilePath = Path.Combine(LuceneDirectory, "write.lock");
                // if path is valid, delete locked file
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                // return temp copy of directory (TempDirectory)
                return TempDirectory;
            }
        }

        /// <summary>
        /// Maps database fields from incoming data source to the Lucene class Document
        /// and adds it to the search index. String or text properties are marked as
        /// ANALYZED, and int or single value properties are marked as NON_ANALYZED.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="writer"></param>
        private static void AddToLuceneIndex(LuceneData data, IndexWriter writer)
        {
            if (data == null) return;
            // removing old index query
            var query = new TermQuery(new Term("Id", data.Id.ToString()));
            writer.DeleteDocuments(query);
            // adding new index entry
            var document = new Document();
            // adding lucene fields and mapping them to db fields
            document.Add(new Field("Id", data.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("InternalId", data.InternalId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("PubDate", data.PubDate.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Author", data.Author, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Title", data.Title, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Type", data.Type, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Tags", data.Tags, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Link", data.Link, Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("Description", data.Description, Field.Store.YES, Field.Index.ANALYZED));
            // adding entry to index
            writer.AddDocument(document);
        }

        /// <summary>
        /// Adds list of records to the Lucene search index.
        /// </summary>
        /// <param name="datas"></param>
        public static void AddUpdateLuceneIndex(IEnumerable<LuceneData> datas)
        {
            // initializing lucene analyzer
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);

            using (var writer = new IndexWriter(Directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // adding data to index
                foreach (var data in datas) AddToLuceneIndex(data, writer);
                // closing handlers
                analyzer.Close();
                writer.Dispose();
            }
        }

        /// <summary>
        /// Adds a single record to the Lucene search index.
        /// </summary>
        /// <param name="data"></param>
        public static void AddUpdateLuceneIndex(LuceneData data)
        {
            AddUpdateLuceneIndex(new List<LuceneData> { data });
        }

        /// <summary>
        /// Clears current Lucene index record.
        /// </summary>
        /// <param name="recordId"></param>
        public static void ClearLuceneIndexRecord(int recordId)
        {
            // initializing Lucene analyzer
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(Directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // removing older index entry
                var query = new TermQuery(new Term("Id", recordId.ToString()));
                writer.DeleteDocuments(query);
                // closing handlers
                analyzer.Close();
                writer.Dispose();
            }
        }

        /// <summary>
        /// Clears the Lucene Index record.
        /// </summary>
        /// <returns></returns>
        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                using (var writer = new IndexWriter(Directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    //removing older index entries
                    writer.DeleteAll();
                    // close handlers
                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Optimizes the contents of the current Lucene directory.
        /// </summary>
        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(Directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // closing handlers and optimizing
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }

        /// <summary>
        /// Maps Lucene document to an instance of LuceneData.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private static LuceneData MapLuceneDocumentToData(Document document)
        {
            return new LuceneData
            {
                Id = Convert.ToInt32(document.Get("Id")),
                InternalId = Convert.ToInt32(document.Get("InternalId")),
                PubDate = document.Get("PubDate"),
                Author = document.Get("Author"),
                Title = document.Get("Title"),
                Type = document.Get("Type"),
                Tags = document.Get("Tags"),
                Link = document.Get("Link"),
                Description = document.Get("Description")
            };
        }

        /// <summary>
        /// Maps list of Documents to list of LuceneData.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static IEnumerable<LuceneData> MapLuceneToDataList(IEnumerable<Document> list)
        {
            // NOTE: I'm not sure wtf this line is doing.
            return list.Select(MapLuceneDocumentToData).ToList();
        }

        /// <summary>
        /// Maps list of ScoreDoc and instance of IndexSearcher to list of LuceneData.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="searcher"></param>
        /// <returns></returns>
        private static IEnumerable<LuceneData> MapLuceneToDataList(IEnumerable<ScoreDoc> list, IndexSearcher searcher)
        {
            return list.Select(hit => MapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

        /// <summary>
        /// Formats and assigns search query to Lucene Query object.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Query ParseQuery(string searchstring, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchstring.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchstring.Trim()));
            }
            return query;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private static IEnumerable<LuceneData> _Search(string searchstring, string fields = "")
        {
            // validating searchstring
            if (string.IsNullOrEmpty(searchstring.Replace("*", "").Replace("?", ""))) return new List<LuceneData>();
            // TEMP: adding index records
            AddUpdateLuceneIndex(SampleLuceneRepository.GetAll());
            // setting up and using Lucene searcher
            using (var searcher = new IndexSearcher(Directory, false))
            {
                var hitLimit = 1000;
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                var parser = new MultiFieldQueryParser(Version.LUCENE_30, new[] { "PubDate", "Author", "Title", "Type", "Tags", "Description" }, analyzer);
                var query = ParseQuery(searchstring, parser);
                var hits = searcher.Search(query, hitLimit).ScoreDocs;
                var results = MapLuceneToDataList(hits, searcher);

                // closing handlers
                analyzer.Close();
                searcher.Dispose();
                return results;
            }
        }

        /// <summary>
        /// Public search method. Cleans incoming search string and calls private search method.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static IEnumerable<LuceneData> Search(string searchstring, string fields = "")
        {
            // validating searchstring
            if (string.IsNullOrEmpty(searchstring)) return new List<LuceneData>();
            // trimming and replacing '-' with a space, splitting incoming
            // search terms into array, and filtering out invalid terms
            var terms = searchstring.Trim().Replace("-", " ").Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            // joining cleaned terms into single search string
            searchstring = string.Join(" ", terms).Trim();
            // calls Lucene private search method
            return _Search(searchstring, fields);
        }

        /// <summary>
        /// Gets all records in index. Performance will be impacted by 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<LuceneData> GetAllIndexRecords()
        {
            // validating search index
            if (!System.IO.Directory.EnumerateFiles(LuceneDirectory).Any()) return new List<LuceneData>();
            // setting up Lucene searcher
            var searcher = new IndexSearcher(Directory, false);
            var reader = IndexReader.Open(Directory, false);
            var docs = new List<Document>();
            var term = reader.TermDocs();
            // adding docs to list
            while (term.Next()) docs.Add(searcher.Doc(term.Doc));
            // disposing of handlers
            reader.Dispose();
            searcher.Dispose();
            // mapping Lucene doc list to data list
            return MapLuceneToDataList(docs);
        }
    }
}