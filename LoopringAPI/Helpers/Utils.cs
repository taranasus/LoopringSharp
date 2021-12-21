using PoseidonSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace LoopringAPI
{    
    public static class Utils
    {
        static HttpClient _client;
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

        public static async Task<bool> ThrowIfHttpFail(HttpResponseMessage httpResult)
        {
            if (httpResult.IsSuccessStatusCode)
                return true;
            if (httpResult.Content != null)
            {
                var exString = "Error from Loopring API: " + httpResult.StatusCode.ToString() + " | " + (await httpResult.Content.ReadAsStringAsync().ConfigureAwait(false));
                throw new System.Exception(exString);
            }
            throw new System.Exception("Error from Loopring API: " + httpResult.StatusCode.ToString());
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

        public static async Task<string> Http(string url, (string, string)[] parameters = null, (string, string)[] headers = null, string method = "get", string body = null)
        {            
            if(_client==null)
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };
                _client = new HttpClient(httpClientHandler);
            }

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

            using (var httpRequest = new HttpRequestMessage(tmethod, url))
            {
                if (headers != null && headers.Length > 0)
                {
                    foreach (var header in headers)
                    {
                        httpRequest.Headers.Add(header.Item1, header.Item2);
                    }
                }
                if (!string.IsNullOrWhiteSpace(body))
                {
                    httpRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }
                using (var httpResult = await _client.SendAsync(httpRequest).ConfigureAwait(continueOnCapturedContext: false))
                {
                    _ = await Utils.ThrowIfHttpFail(httpResult).ConfigureAwait(false);
                    var result = await httpResult.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return result;
                }
            }
        }
    }
}
