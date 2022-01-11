using PoseidonSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace LoopringSharp
{
    public static class Utils
    {        
        public static string IntervalsEnumToString(Intervals interval)
        {
            var intervals = "";
            switch (interval)
            {
                case Intervals.min1:
                case Intervals.min15:
                case Intervals.min30:
                case Intervals.min5:
                    intervals = interval.ToString().Replace("min", "") + "min";
                    break;
                case Intervals.hr4:
                case Intervals.hr2:
                case Intervals.hr1:
                case Intervals.hr12:
                    intervals = interval.ToString().Replace("hr", "") + "hr";
                    break;
                case Intervals.w1:
                    intervals = "1w";
                    break;
                case Intervals.d1:
                    intervals = "1d";
                    break;
            }
            return intervals;
        }

        public static string UrlEncodeUpperCase(string stringToEncode)
        {
            var reg = new Regex(@"%[a-f0-9]{2}");
            stringToEncode = HttpUtility.UrlEncode(stringToEncode);
            return reg.Replace(stringToEncode, m => m.Value.ToUpperInvariant());
        }       

        public static BigInteger CreateSha256Signature(HttpMethod method, List<(string Key, string Value)> queryParams, string postBody, string apiMethod, string apiUrl)
        {
            var signatureBase = "";
            var parameterString = "";
            if (method == HttpMethod.Post)
            {
                signatureBase += "POST&";
                parameterString = postBody;
            }
            else if (method == HttpMethod.Get)
            {
                signatureBase += "GET&";
                if (queryParams != null)
                {
                    int i = 0;
                    foreach (var parameter in queryParams)
                    {
                        parameterString += parameter.Key + "=" + parameter.Value;
                        if (i < queryParams.Count - 1)
                            parameterString += "&";
                        i++;
                    }
                }
            }
            else if (method == HttpMethod.Delete)
            {
                signatureBase += "DELETE&";
                if (queryParams != null)
                {
                    int i = 0;
                    foreach (var parameter in queryParams)
                    {
                        parameterString += parameter.Key + "=" + parameter.Value;
                        if (i < queryParams.Count - 1)
                            parameterString += "&";
                        i++;
                    }
                }
            }
            else
                throw new Exception("Http method type not supported");

            signatureBase += Utils.UrlEncodeUpperCase(apiUrl + apiMethod) + "&";
            signatureBase += Utils.UrlEncodeUpperCase(parameterString);

            return SHA256Helper.CalculateSHA256HashNumber(signatureBase);
        }

        public static int GetUnixTimestamp() => (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        public static BigInteger ParseHexUnsigned(string toParse)
        {
            toParse = toParse.Replace("0x", "");
            var parsResult = BigInteger.Parse(toParse, System.Globalization.NumberStyles.HexNumber);
            if (parsResult < 0)
                parsResult = BigInteger.Parse("0" + toParse, System.Globalization.NumberStyles.HexNumber);
            return parsResult;
        }

        static ConcurrentQueue<CustomHttpRequest> httpRequests = new ConcurrentQueue<CustomHttpRequest>();
        static ConcurrentDictionary<Guid, string> httpResults = new ConcurrentDictionary<Guid, string>();
      
        public static string Http(string url, (string, string)[] parameters = null, (string, string)[] headers = null, string method = "get", string body = null)
        {
            if (parameters != null && parameters.Length > 0)
            {
                if (parameters.Any(a => !string.IsNullOrWhiteSpace(a.Item2)))
                {
                    url += "?";
                    foreach (var parameter in parameters)
                    {
                        if (!string.IsNullOrWhiteSpace(parameter.Item2))
                        {
                            url += parameter.Item1 + "=" + parameter.Item2 + "&";
                        }
                    }
                    url = url.TrimEnd('&');
                }
            }
            HttpMethod tmethod = HttpMethod.Get;
            if (method == "delete")
                tmethod = HttpMethod.Delete;
            if (method == "post")
                tmethod = HttpMethod.Post;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Method = method;

            if (headers != null && headers.Length > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Item1, header.Item2);
                }
            }
            if (!string.IsNullOrWhiteSpace(body))
            {
                var dataBytes = Encoding.UTF8.GetBytes(body);

                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.ContentLength = dataBytes.Length;
                request.ContentType = "application/json";
                using (Stream requestBody = request.GetRequestStream())
                {
                    requestBody.Write(dataBytes, 0, dataBytes.Length);
                }
            }
           

            string result = null;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    throw new Exception("LOOPRING API ERROR: "+reader.ReadToEnd());
                }
            }

            return result;
        }       
    }
    public class CustomHttpRequest
    {
        public string url { get; set; }
        public (string, string)[] parameters { get; set; }
        public (string, string)[] headers { get; set; }
        public string method { get; set; }
        public string body { get; set; }
        public Guid guid { get; set; }
    }
}
