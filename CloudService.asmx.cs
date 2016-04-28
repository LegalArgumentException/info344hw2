using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace WebRole1
{
    /// <summary>
    /// This service provides the the functionality for a query auto-complete by downloading
    /// a txt file full of wikipedia titles, creating a trie out of those titles, and allowing
    /// the created trie to be queried and return likely results.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CloudService : System.Web.Services.WebService
    {
        //Temporary path used to write wiki txt file
        string dictPath = System.IO.Path.GetTempPath() + @"\TitleLibrary.txt";

        /// <summary>
        /// This web method downloads a text file from a blob full of Wikipedia Titles
        /// on the local machine in a temporary file. 
        /// </summary>
        /// <returns>
        /// Returns a string "pass" if successful and "fail" if not
        /// </returns>
        [WebMethod]
        public string DownloadWiki()
        {
            //Blob Account and client details
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("pa2blob");
            if (container.Exists())
            {
                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        using (var fileStream = System.IO.File.OpenWrite(@dictPath))
                        {
                            CloudBlockBlob blob = container.GetBlockBlobReference("trimmedtitles.txt");
                            dictPath = Path.GetTempPath() + "\\TitleLibrary.txt";
                            blob.DownloadToStream(fileStream);
                            return "pass";
                        }
                    }
                }
            }
            return "fail";
        }

        /// <summary>
        /// This web method builds a trie from the wiki txt file
        /// </summary>
        /// <returns>
        /// A JSON file with information about the number of titles inserted in the trie, 
        /// the startingmemory amount and ending memory amounts as floats, 
        /// and the last line read in as a string
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string BuildTrie()
        {
            PerformanceCounter memProcess = new PerformanceCounter("Memory", "Available MBytes");
            float counter = 0;
            bool lowMem = false;
            string line = "";
            float startMemory = memProcess.NextValue();
            // Reads from the file at the temporary path
            using (StreamReader sr = new StreamReader(dictPath))
            {
                while (sr.EndOfStream == false && !lowMem)
                {
                    counter++;
                    line = sr.ReadLine().ToLower();
                    Trie.addTitle(line);
                    if (counter % 2500 == 0)
                    {
                        //Quits process once there is only 50Mb of memory left on the machine
                        if (memProcess.NextValue() <= 50)
                        {
                            lowMem = true;
                        }
                    }
                }
            }
            jsonTest testReturn = new jsonTest();
            testReturn.counter = counter;
            testReturn.startMemory = startMemory;
            testReturn.memory = memProcess.NextValue();
            testReturn.lastLine = line;
            return new JavaScriptSerializer().Serialize(testReturn);

        }

        /// <summary>
        /// This web method traverses through the trie using the given input string, and finds
        /// the first 10 full titles that contain the input string.
        /// </summary>
        /// <param name="query"></param>
        /// <returns>
        /// A JSON file full of the first 10 queries found while traversing through the trie as 
        /// strings.
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Query(string query)
        {
            List<string> queries = Trie.getPrefix(query);
            string[] queryArray = new string[10];
            return new JavaScriptSerializer().Serialize(queries);
        }
    }
}
