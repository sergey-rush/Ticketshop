using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using TicketShop.Core;

namespace TicketShop.Data
{
    public class HttpFactory
    {
        private readonly Uri _uri;
        private readonly HttpMethod _method;
        private static CookieContainer _cookieJar = new CookieContainer();
        public readonly JsonSerializerSettings DateFormatSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
        };
        private static string _token = String.Empty;
        internal static string Token
        {
            private get { return _token; }
            set
            {
                _token = String.Format("Basic {0}", Convert.ToBase64String(Encoding.ASCII.GetBytes(value)));
            }
        }

        private HttpFactory(Uri uri, HttpMethod method)
        {
            _uri = uri;
            _method = method;
        }

        /// <summary>
        /// Processes Http Request on remote server
        /// </summary>
        /// <typeparam name="TOutput">Output type</typeparam>
        /// <typeparam name="TInput">Input type</typeparam>
        /// <param name="uri">Destination url</param>
        /// <param name="method">Method to be executed</param>
        /// <param name="t">Input type instance</param>
        /// <returns>Output type instance</returns>
        public static TOutput ProcessRequest<TOutput, TInput>(Uri uri, HttpMethod method, TInput t)where TOutput:class
        {
            HttpFactory factory = new HttpFactory(uri, method);
            return factory.CreateRequest<TOutput, TInput>(t);
        }

        private TOutput CreateRequest<TOutput, TInput>(TInput tInput)
        {
            TOutput tOutput = default(TOutput);
            try
            {
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_uri);
                request.Accept = "application/json";
                request.ContentType = "application/json";
                request.Headers["Accept-Language"] = "ru-RU";
                request.Headers.Add("Authorization", Token);
                request.UserAgent = "Mozilla/5.0 (compatible; Ticketshop/1.0; +http://www.main.zirk.ru/ticketshop.html)";
                request.KeepAlive = false;
                request.AllowAutoRedirect = true;
                request.Timeout = 60000;
                request.CachePolicy = policy;
                request.Method = _method.ToString();
                request.CookieContainer = _cookieJar;
                ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;

                if (_method != HttpMethod.GET)
                {
                    string payload = JsonConvert.SerializeObject(tInput);
                    byte[] byteArray = Encoding.UTF8.GetBytes(payload);
                    request.ContentLength = byteArray.Length;
                    using (var dataStream = request.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        //_cookieJar = new CookieContainer();
                        _cookieJar.Add(response.Cookies);
                        using (var streamReader = new StreamReader(stream))
                        {
                            string stringResponse = streamReader.ReadToEnd();
                            tOutput = JsonConvert.DeserializeObject<TOutput>(stringResponse, DateFormatSettings);
                        }
                    }
                }
            }
            catch (WebException we)
            {
                if (we.Status == WebExceptionStatus.TrustFailure)
                {

                }
                    //MessageBox.Show(String.Format("Приложение будет закрыто. {0}", we.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw new Exception("Exception Request Data. Fail to verify server: " + we.Message);
                //Application.Exit();
            }
            catch 
            {
                //MessageBox.Show(String.Format("Приложение будет закрыто. {0}", e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw new Exception("Exception Request Data: " + e.Message);
                //Application.Exit();
            }
            return tOutput;
        }

        public static byte[] ProcessRequest(Uri uri, HttpMethod method)
        {
            HttpFactory factory = new HttpFactory(uri, method);
            return factory.RequestImage();
        }

        private byte[] RequestImage()
        {
            byte[] byteArray = null;

            try
            {
                CookieContainer cookieJar = new CookieContainer();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_uri);
                request.Accept = "image/*";
                request.ContentType = "application/json";
                request.Headers["Accept-Language"] = "ru-RU";
                request.UserAgent = "Mozilla/5.0 (compatible; Ticketshop/1.0; +http://www.main.zirk.ru/ticketshop.html)";
                request.Headers.Add("Authorization", Token);
                request.KeepAlive = false;
                request.AllowAutoRedirect = true;
                request.Timeout = 60000;
                request.Method = _method.ToString();
                request.CookieContainer = cookieJar;
                ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                var buffer = new byte[4096];
                                long totalBytesRead = 0;
                                int bytesRead;

                                while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    totalBytesRead += bytesRead;
                                    ms.Write(buffer, 0, bytesRead);
                                }

                                byteArray = new byte[totalBytesRead];
                                byteArray = ms.ToArray();
                            }
                        }
                    }
                }
            }
            catch (WebException we)
            {
                if (we.Status == WebExceptionStatus.TrustFailure)
                    throw new Exception("Exception Request Data. Fail to verify server: " + we.Message);
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return byteArray;
        }
        
        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            bool result = true;
            if (policyErrors != SslPolicyErrors.None)
            {
                foreach (X509ChainStatus t in chain.ChainStatus)
                {
                    if (t.Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)cert);
                        if (!chainIsValid)
                        {
                            result = false;
                        }
                    }
                }
            }
            return result;
        }
    }
}