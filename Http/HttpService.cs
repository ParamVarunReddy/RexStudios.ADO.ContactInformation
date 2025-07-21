namespace RexStudios.ADO.ContactInformation.BusinessLayer.Http
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using System.Security.Policy;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation For Generic Http Service Class
    /// </summary>
    public class HttpService : IHttpService
    {
        private readonly ITracingService _tracingService;
        /// <summary>
        /// Http Client
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Consturctor
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        public HttpService(ITracingService tracingService)
        {
            _httpClient = HttpClientSingleton.Instance;
            _tracingService = tracingService;
        }

        //#region Client Configuration

        ///// <summary>
        ///// Extension method to add HttpClient using Startup Class
        ///// </summary>
        ///// <param name="services"></param>
        //public static void AddHttpClientToServiceCollection(IServiceCollection services)
        //{
        //    services.AddHttpClient<IHttpService, HttpService>();
        //}

        //#endregion

        #region Request

        public Task<HttpResult<T>> GetObjectAsync<T>(string url, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false)
        {
            return ProcessRequestAsync<T>(HttpMethod.Get, url, null, headers, isFormUrlEncodedContent);
        }

        public Task<HttpResult<string>> GetStringAsync(string url, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false)
        {
            return ProcessRequestAsync<string>(HttpMethod.Get, url, null, headers, isFormUrlEncodedContent);
        }

        public Task<HttpResult<T>> PostJsonAndGetObject<T>(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false)
        {
            return ProcessRequestAsync<T>(HttpMethod.Post, url, requestBody, headers, isFormUrlEncodedContent);
        }

        public Task<HttpResult<string>> PostJsonAndGetString(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false)
        {
            return ProcessRequestAsync<string>(HttpMethod.Post, url, requestBody, headers, isFormUrlEncodedContent);
        }

        public Task<HttpResult<T>> PutJsonAndGetObject<T>(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false)
        {
            return ProcessRequestAsync<T>(HttpMethod.Put, url, requestBody, headers, isFormUrlEncodedContent);
        }

        public Task<HttpResult<string>> PutJsonAndGetString(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false)
        {
            return ProcessRequestAsync<string>(HttpMethod.Put, url, requestBody, headers, isFormUrlEncodedContent);
        }

        public Task<HttpResult<T>> DeleteJsonAndGetObject<T>(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false)
        {
            return ProcessRequestAsync<T>(HttpMethod.Delete, url, requestBody, headers, isFormUrlEncodedContent);
        }

        public Task<HttpResult<string>> DeleteJsonAndGetString(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false)
        {
            return ProcessRequestAsync<string>(HttpMethod.Delete, url, requestBody, headers, isFormUrlEncodedContent);
        }

        public Task<HttpResult<T>> PatchJsonAndGetObject<T>(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false)
        {
            return ProcessRequestAsync<T>(new HttpMethod("PATCH"), url, requestBody, headers, isFormUrlEncodedContent);
        }

        public Task<HttpResult<string>> PatchJsonAndGetString(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false)
        {
            return ProcessRequestAsync<string>(new HttpMethod("PATCH"), url, requestBody, headers, isFormUrlEncodedContent);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Process Request Asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private async Task<HttpResult<T>> ProcessRequestAsync<T>(HttpMethod method, string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false)
        {
            try
            {
                if (isFormUrlEncodedContent)
                {
                    List<KeyValuePair<string, string>> payload = requestBody as List<KeyValuePair<string, string>>;

                    using (HttpRequestMessage request = CreateFormUrlEncodedContentRequest(method, url, headers, payload))
                    {
                        using (HttpResponseMessage response = await _httpClient.SendAsync(request).ConfigureAwait(false))
                        {
                            return await GetHttpResponseMessageFromHttpResponse<T>(response).ConfigureAwait(false);
                        }
                    }
                }

                using (HttpRequestMessage request = CreateRequest(method, url, headers, requestBody))
                {
                    using (HttpResponseMessage response = await _httpClient.SendAsync(request).ConfigureAwait(false))
                    {
                        return await GetHttpResponseMessageFromHttpResponse<T>(response).ConfigureAwait(false);
                    }
                }

                /*HttpRequestMessage request;
                HttpResponseMessage response;
                if (isFormUrlEncodedContent)
                {
                    List<KeyValuePair<string, string>> payload = requestBody as List<KeyValuePair<string, string>>;

                    using ( request = CreateFormUrlEncodedContentRequest(method, url, headers, payload))
                    {
                        using (response = await _httpClient.SendAsync(request).ConfigureAwait(false))
                        {
                            return await GetHttpResponseMessageFromHttpResponse<T>(response).ConfigureAwait(false);
                        }
                    }
                }

                request = CreateRequest(method, url, headers, requestBody);
                response = await _httpClient.SendAsync(request);
                
               // string result = response.Content.ReadAsStringAsync().Result;

                return await GetHttpResponseMessageFromHttpResponse<T>(response);
                // {
                //using (HttpResponseMessage response = await _httpClient.SendAsync(request).ConfigureAwait(false))
                //{
                //response = await _httpClient.SendAsync(request);
                  //  return await GetHttpResponseMessageFromHttpResponse<T>(response);
                   // return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    //return await GetHttpResponseMessageFromHttpResponse<T>(response).ConfigureAwait(false);
                    //}
               // }*/
            }
            catch (HttpRequestException hre)
            {
                _tracingService.Trace($"Http Request Exception triggered for {url} , {hre.Message}");
                _tracingService.Trace($"Stack trace Exception triggered for {url} , {hre.StackTrace}");
                if (hre.InnerException != null)
                    _tracingService.Trace($"Inner Exception Exception triggered for {url} , {hre.InnerException.Message}");


                throw GetException(
                   $"Http Request Exception triggered for {url}",
                   originalException: hre,
                   url,
                   requestBody,
                   typeof(T).FullName);
            }
            catch (HttpException ex)
            {
                _tracingService.Trace($"Http Exception triggered for {url} , {ex.Message}");

                throw GetException(
                 $"Unexpected exeption when calling {url}",
                 originalException: ex,
                 url,
                 requestBody,
                 typeof(T).FullName);
            }
            catch (Exception ex)
            {
                _tracingService.Trace($"Exception triggered for {url}, {ex.Message} ");

                throw GetException(
                  $"Unexpected exeption when calling {url}",
                  originalException: ex,
                  url,
                  requestBody,
                  typeof(T).FullName);
            }
        }

        /// <summary>
        /// Get Http Response Message From Http Response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<HttpResult<T>> GetHttpResponseMessageFromHttpResponse<T>(HttpResponseMessage response)
        {
            if (response is null) return null;
            var requestBody = "";
            var responseBody = "";
            try
            {
              

                //requestBody = response?.RequestMessage?.Content != null ? await response.RequestMessage.Content.ReadAsStringAsync().ConfigureAwait(false) : string.Empty;
                responseBody = response?.Content != null ? await response.Content.ReadAsStringAsync().ConfigureAwait(false) : string.Empty;

                var httpResult = new HttpResult<T>
                {
                    RequestUri = response.RequestMessage.RequestUri.ToString(),
                    StatusCode = response.StatusCode,
                    ResponseBody = responseBody,
                    ReasonPhrase = response.ReasonPhrase,
                    IsSuccessful = response.IsSuccessStatusCode,
                };

                

                if (response.IsSuccessStatusCode)
                {
                    // Only Serialize if return a specific type
                    if (typeof(T) != typeof(string))
                    {
                        httpResult.Result = JsonSerializerExtension.Deserialize<T>(responseBody);
                    }
                    else
                    {
                        httpResult.Result = (T)Convert.ChangeType(responseBody, typeof(T));
                    }
                    httpResult.IsSuccessful = true;
                }
                else
                {
                    httpResult.Message = $"Call to {response.RequestMessage.RequestUri} failed with code {response.StatusCode}. Reason: {response.ReasonPhrase}";
                    httpResult.IsSuccessful = false;
                }

                return httpResult;
            }
            catch (Exception ex)
            {
                throw GetException(
                    $"[GetHttpResponseMessageFromHttpResponse] Error reading response from {response.RequestMessage.RequestUri} into {typeof(T).FullName}",
                    originalException: ex,
                    response.RequestMessage.RequestUri.ToString(),
                    requestBody,
                    typeof(T).FullName,
                    responseBody);
            }
        }
    

        /// <summary>
        /// Get Http Response Message From Exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="additionalContext"></param>
        /// <returns></returns>
        private static HttpResult<T> GetHttpResponseMessageFromException<T>(string requestUri, Exception ex, string message = "", string additionalContext = "")
        {
            var httpResult = new HttpResult<T>
            {
                IsSuccessful = false,
                RequestUri = requestUri,
                Exception = ex,
                Message = message,
                AdditionalContext = additionalContext,
                Result = default(T),
                StatusCode = HttpStatusCode.InternalServerError,
            };

            return httpResult;
        }

        /// <summary>
        /// Get Exception of different parameters
        /// </summary>
        /// <param name="message"></param>
        /// <param name="originalException"></param>
        /// <param name="requestUri"></param>
        /// <param name="requestBody"></param>
        /// <param name="expectedResponseTypeName"></param>
        /// <param name="responseBody"></param>
        /// <returns></returns>
        private static HttpException GetException(string message, Exception originalException, string requestUri, object requestBody, string expectedResponseTypeName, string responseBody = "")
        {
            string requestBodyString;

            try
            {
                requestBodyString = JsonSerializerExtension.Serialize(requestBody);
            }
            catch
            {
                requestBodyString = "Cannot serialize requestBody";
            }

            return new HttpException(message, originalException)
            {
                RequestUri = requestUri,
                RequestBody = requestBodyString,
                ResponseBody = responseBody,
                ExpectedResponseTypeName = expectedResponseTypeName
            };
        }

        /// <summary>
        /// Create Http Request
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static HttpRequestMessage CreateRequest(HttpMethod method, string url, Dictionary<string, string> headers, object body)
        {
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(url)
            };

            if (headers != null)
            {
                foreach (var kvp in headers)
                {
                    request.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            if (body != null)
            {
               

                request.Content = method.Method == "PATCH"
                    ? new StringContent(body.ToString(), Encoding.UTF8, "application/merge-patch+json")
                    : new StringContent(body.ToString(), Encoding.UTF8, "application/json");
            }
            

            return request;
        }

        /// <summary>
        /// Create Form Url Encoded Content Request
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static HttpRequestMessage CreateFormUrlEncodedContentRequest(HttpMethod method, string url, Dictionary<string, string> headers, List<KeyValuePair<string, string>> body)
        {
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(url)
            };

            if (headers != null)
            {
                foreach (var kvp in headers)
                {
                    request.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            if (body != null)
            {
                request.Content = new FormUrlEncodedContent(body);
            }

            return request;
        }
        #endregion
    }

    public class HttpClientSingleton
    {
        private static HttpClient _client;

        private HttpClientSingleton() { }

        public static HttpClient Instance
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                }
                return _client;
            }
        }
    }

    public class JsonSerializerExtension
    {
        /// Deserialize an from json string
        /// </summary>
        public static T Deserialize<T>(string body)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(body);
                writer.Flush();
                stream.Position = 0;
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
            }
        }

        /// <summary>
        /// Serialize an object to json
        /// </summary>
        public static string Serialize<T>(T item)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new DataContractJsonSerializer(typeof(T)).WriteObject(ms, item);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }
    }
}
