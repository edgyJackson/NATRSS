using SpillTracker.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SpillTracker.Services
{
    public class APICall : IAPICall
    {
        public string DoAPICall(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            string jsonString = null;
            // TODO: You should handle exceptions here
            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                jsonString = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }

            return jsonString;
        }
    }
}
