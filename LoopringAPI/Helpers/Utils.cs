using PoseidonSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

        static ConcurrentQueue<CustomHttpRequest> httpRequests = new ConcurrentQueue<CustomHttpRequest>();
        static ConcurrentDictionary<Guid, string> httpResults = new ConcurrentDictionary<Guid, string>();

        static bool HttpProcessorRunning = false;
        static int thortleRequests = 200;

        public static void StartHttpProcessor(int requestThrottle = 200)
        {
            thortleRequests = requestThrottle;
            if (requestThrottle < 10)
                thortleRequests = 10;
            if (!HttpProcessorRunning)
            {
                HttpProcessorRunning = true;
                Task.Run(async () =>
                {
                    HttpProcessorRunning = true;
                    while (true)
                    {
                        if (httpRequests.TryDequeue(out CustomHttpRequest request))
                        {
                            try
                            {
                                var result = await InterlanHttp(request);
                                httpResults.TryAdd(request.guid, result);
                            }
                            catch (Exception ex)
                            {
                                httpResults.TryAdd(request.guid, ex.Message);
                            }
                            System.Threading.Thread.Sleep(thortleRequests);
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(10);
                        }
                        
                    }
                });
            }
        }

        static DateTime lastRequest;

        public static async Task<string> Http(string url, (string, string)[] parameters = null, (string, string)[] headers = null, string method = "get", string body = null)
        {
            var reqId = Guid.NewGuid();

            httpRequests.Enqueue(new CustomHttpRequest()
            {
                body = body,
                method = method,
                url = url,
                parameters = parameters,
                headers = headers,
                guid = reqId
            });

            string result = null;

            while (!httpResults.TryRemove(reqId, out result))
            {
                await Task.Delay(1);
            }
            if(DateTime.UtcNow.Subtract(lastRequest).TotalMilliseconds<1000)
            {
                Debug.WriteLine("Waited between calls: " + DateTime.UtcNow.Subtract(lastRequest).TotalMilliseconds + " ms");
            }

            lastRequest = DateTime.UtcNow;
            return result;


        }

        public static async Task<string> InterlanHttp(CustomHttpRequest request)
        {
            if (_client == null)
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };
                _client = new HttpClient(httpClientHandler);
            }

            if (request.parameters != null && request.parameters.Length > 0)
            {
                if (request.parameters.Any(a => !string.IsNullOrWhiteSpace(a.Item2)))
                {
                    request.url += "?";
                    foreach (var parameter in request.parameters)
                    {
                        if (!string.IsNullOrWhiteSpace(parameter.Item2))
                        {
                            request.url += parameter.Item1 + "=" + parameter.Item2 + "&";
                        }
                    }
                    request.url = request.url.TrimEnd('&');
                }
            }
            HttpMethod tmethod = HttpMethod.Get;
            if (request.method == "delete")
                tmethod = HttpMethod.Delete;
            if (request.method == "post")
                tmethod = HttpMethod.Post;

            using (var httpRequest = new HttpRequestMessage(tmethod, request.url))
            {
                if (request.headers != null && request.headers.Length > 0)
                {
                    foreach (var header in request.headers)
                    {
                        httpRequest.Headers.Add(header.Item1, header.Item2);
                    }
                }
                if (!string.IsNullOrWhiteSpace(request.body))
                {
                    httpRequest.Content = new StringContent(request.body, Encoding.UTF8, "application/json");
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
