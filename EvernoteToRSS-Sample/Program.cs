using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernoteToRSS;
using System.Xml;

namespace EvernoteToRSS_Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            string sessionToken = "YOUR_EVERNOTE_DEVELOPER_TOKEN";
            string noteStoreUrl = "YOUR EVERNOTE NOTE STORE URL";

            // Initiate the feed creator
            FeedMaker en = new FeedMaker(sessionToken, noteStoreUrl);

            if(en.Authenticate())
            {
                // Name of notebook that you want to generate an RSS feed for
                string noteBookName = "Notes";
                
                // Number of notes to return in the feed
                int maxResults = 30;

                // Generate the RSS feed
                XmlDocument doc = en.NotebookToRSS(noteBookName, maxResults);

                // Print for demo purposes
                Console.WriteLine(doc.OuterXml);

                Console.ReadLine();
            }
        }
    }
}
