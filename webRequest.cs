using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace callKeeper
{
    ///json список номеров
    public class ListNumbers
    {
        public Dictionary<string, Value> Value { get; set; }
        public bool Success { get; set; }
        public object Error { get; set; }
        public bool Failure { get; set; }
    }
    public class Value
    {
        public int TZ { get; set; }
        public int AllowChange { get; set; }
        public bool InCall { get; set; }
        public bool OutCall { get; set; }
        public string BeginDT { get; set; }
        public string EndDT { get; set; }
        public int Days { get; set; }
    }

    /// json список вызовов и ссылки на записи
    public class ListCalls
    {
        public List<CallsValue> Value { get; set; }
        public bool Success { get; set; }
        public object Error { get; set; }
        public bool Failure { get; set; }
    }
    public class CallsValue
    {
        public string Number { get; set; }
        public int CallType { get; set; }
        public string CallTime { get; set; }
        public string Duration { get; set; }
        public string FileName { get; set; }
    }

    class cLog
    {
        private string description { get; set; }

        public cLog(string description)
        {
            logWrite(description);
        }
        
        private void logWrite(string description)
        {
            using (FileStream fstream = new FileStream(@"log.log", FileMode.Append))
            {
                string log = string.Format("[{1}] {0}"+'\n', description, DateTime.Now);
                byte[] array = System.Text.Encoding.Default.GetBytes(log);
                fstream.Write(array, 0, array.Length);
            }
        }

    }

    class cWebRequest
    {
        static private List<string> getListNumber()
        {
            List<string> listNumber = new List<string>();
            string rawList = LoadHttpPageWithBasicAuthentication(@"https://mrecord.mts.ru/api/v2/numbers", "9122307669", "vhnoWWlPHA");
            ListNumbers ListNumbers = JsonConvert.DeserializeObject<ListNumbers>(rawList);
            if (ListNumbers.Value != null)
            {
                foreach (var i in ListNumbers.Value)
                {
                    listNumber.Add(i.Key);
                }
            }
            return listNumber;
        }

        static public bool saveRecordCall(string disk)
        {
            DateTime now = DateTime.Now.AddDays(0);
            int year = now.Year;
            int month = now.Month;
            int day =  now.Day;
            string phoneNumber = "";
            try {
                foreach (var j in getListNumber())
                {
                    phoneNumber = j;
                    string conStr = String.Format(@"https://mrecord.mts.ru/api/v2/recs/{0}/{1}-{2:00}-{3:00}T00:00:00/{1}-{2:00}-{3:00}T23:59:59", j, year, month, day);
                    var rawListCalls = LoadHttpPageWithBasicAuthentication(conStr, "9122307669", "vhnoWWlPHA");
                    ListCalls ListCalls = JsonConvert.DeserializeObject<ListCalls>(rawListCalls);
                    //string path = String.Format(@"F:\CallsLibrary\{0}\{1:00}\{2}\", year, month, phoneNumber);
                    string path = $@"{disk}CallsLibrary\{year}\{month:00}\{phoneNumber}\";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    if (ListCalls.Value != null)
                    {
                        path += String.Format(@"{0:00}\", day);
                        Directory.CreateDirectory(path);
                        foreach (var i in ListCalls.Value)
                        {
                            saveFile(path, i.FileName, phoneNumber);
                        }
                    }
                }
                cLog log = new cLog("Cохранено");
                return true;
            }
            catch(Exception e)
            {
                cLog log = new cLog(e.ToString());
                return false;
            }
        }

        static private void saveFile(string path, string fileName, string phoneNumber)
        {
            using (FileStream fstream = new FileStream(path+fileName+".mp3", FileMode.OpenOrCreate))
            {
                string mp3 = LoadHttpPageWithBasicAuthentication(@"https://mrecord.mts.ru/api/v2/file/" + phoneNumber + "/" + fileName,"9122307669", "vhnoWWlPHA");
                if (!File.Exists(mp3))
                {
                    byte[] array = System.Text.Encoding.Default.GetBytes(mp3);
                    fstream.Write(array, 0, array.Length);
                }
            }
        }

        static private string LoadHttpPageWithBasicAuthentication(string url, string username, string password)
        {
            try {
                Uri myUri = new Uri(url);
                WebRequest myWebRequest = HttpWebRequest.Create(myUri);

                HttpWebRequest myHttpWebRequest = (HttpWebRequest)myWebRequest;

                NetworkCredential myNetworkCredential = new NetworkCredential(username, password);

                CredentialCache myCredentialCache = new CredentialCache();
                myCredentialCache.Add(myUri, "Basic", myNetworkCredential);

                myHttpWebRequest.PreAuthenticate = true;
                myHttpWebRequest.Credentials = myCredentialCache;

                WebResponse myWebResponse = myWebRequest.GetResponse();

                Stream responseStream = myWebResponse.GetResponseStream();

                StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

                string pageContent = myStreamReader.ReadToEnd();

                responseStream.Close();

                myWebResponse.Close();

                return pageContent;
            }
            catch (Exception e)
            {

                return null;
            }
        }
         
    }
}
