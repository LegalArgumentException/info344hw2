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
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        public static string dictPath;

        [WebMethod]
        public void DownloadWiki()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("pa2blob");
            if(container.Exists())
            {
                foreach(IListBlobItem item in container.ListBlobs(null, false))
                {
                    if(item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = container.GetBlockBlobReference("trimmedtitles.txt");
                        dictPath = Path.GetTempPath() + "\\TitleLibrary.txt";
                        blob.DownloadToFile(dictPath, FileMode.Create);
                    }
                }
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string BuildTrie()
        {
            PerformanceCounter memProcess = new PerformanceCounter("Memory", "Available MBytes");
            float counter = 0;
            bool lowMem = false;
            string line = "";
            float startMemory = memProcess.NextValue();
            using (StreamReader sr = new StreamReader(dictPath))
            {
                while (sr.EndOfStream == false && !lowMem)
                {
                    counter++;
                    line = sr.ReadLine().ToLower();
                    Trie.addTitle(line);
                    if (counter % 5000 == 0)
                    {
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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Query(string query)
        {
            List<string> queries = Trie.getPrefix(query);
            string[] queryArray = new string[10];
            /*for(int i = 0; i < 10; i++)
            {
                queryArray[i] = queries[i];
            }*/
            return new JavaScriptSerializer().Serialize(queries);
        }
    }
}
