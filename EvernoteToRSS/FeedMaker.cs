using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernoteSDK;
using System.Xml;
using System.Xml.Linq;

namespace EvernoteToRSS
{
    public class FeedMaker
    {
        string developerToken;
        string noteStoreUrl;

        public FeedMaker(string developerToken, string noteStoreUrl)
        {
            this.developerToken = developerToken;
            this.noteStoreUrl = noteStoreUrl;
        }

        public bool Authenticate()
        {
            try {
                ENSession.SetSharedSessionDeveloperToken(developerToken, noteStoreUrl);
                return ENSession.SharedSession.IsAuthenticated;
            }
            catch
            {
                return false;
            }
        }

        public XmlDocument NotebookToRSS(string notebookName, int maxResults, string feedTitle = "", string feedLink = "", string feedDescription = "", string feedLanguage = "en-us", string feedEditor = "", string feedWebmaster = "")
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                List<ENSessionFindNotesResult> notesResult = ENSession.SharedSession.FindNotes(ENNoteSearch.NoteSearch("notebook:\"" + notebookName + "\""), null, ENSession.SearchScope.All, ENSession.SortOrder.RecentlyUpdated | ENSession.SortOrder.RecentlyCreated | ENSession.SortOrder.Normal, maxResults);

                StringBuilder strFeed = new StringBuilder();
                strFeed.Append("<rss version=\"2.0\">");
                strFeed.Append("<channel>");
                strFeed.Append("<title>" + feedTitle + "</title>");
                strFeed.Append("<link>" + feedLink + "</link>");
                strFeed.Append("<description>" + feedDescription + "</description>");
                strFeed.Append("<language>" + feedLanguage + "</language>");
                strFeed.Append("<pubDate>" + DateToRFS822(DateTime.Now) + "</pubDate>");
                strFeed.Append("<lastBuildDate>" + DateToRFS822(DateTime.Now) + "</lastBuildDate>");
                strFeed.Append("<docs></docs>");
                strFeed.Append("<generator></generator>");
                strFeed.Append("<managingEditor>" + feedEditor + "</managingEditor>");
                strFeed.Append("<webMaster>" + feedWebmaster + "</webMaster>");

                if (notesResult.Count > 0)
                {
                    foreach (ENSessionFindNotesResult result in notesResult)
                    {
                        ENNote note = ENSession.SharedSession.DownloadNote(result.NoteRef);

                        string sourceUrl = "";

                        if (note.SourceUrl != null)
                        {
                            sourceUrl = note.SourceUrl.ToString();
                        }

                        strFeed.Append("<item>");
                        strFeed.Append("<title>" + XMLEscape(result.Title) + "</title>");
                        strFeed.Append("<link>" + XMLEscape(sourceUrl) + "</link>");
                        strFeed.Append("<description>" + XMLEscape(note.TextContent) + "</description>");
                        strFeed.Append("<pubdate>" + XMLEscape(DateToRFS822(result.Created)) + "</pubdate>");
                        strFeed.Append("<guid>" + XMLEscape(result.NoteRef.Guid) + "</guid>");
                        strFeed.Append("</item>");
                    }
                }

                strFeed.Append("</channel>");
                strFeed.Append("</rss>");

                doc.LoadXml(strFeed.ToString());
            }
            catch
            {
            }

            return doc;
        }

        string XMLEscape(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                return new XText(content).ToString();
            }
            return "";
        }

        string DateToRFS822(DateTime date)
        {
            var value = date.ToString("ddd',' d MMM yyyy HH':'mm':'ss", new System.Globalization.CultureInfo("en-US")) + " " + date.ToString("zzzz").Replace(":", "");
            return value;
        }

    }
}
