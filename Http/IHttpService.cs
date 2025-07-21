using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RexStudios.ADO.ContactInformation.BusinessLayer.Http
{
    /// <summary>
    /// Interface for Generic Http Service
    /// </summary>
    public interface IHttpService
    {

        /// <summary>
        /// Make a call to Url to get response as an Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<HttpResult<T>> GetObjectAsync<T>(string url, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false);

        /// <summary>
        /// Make a call to Url to get response as a String
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<HttpResult<string>> GetStringAsync(string url, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false);

        /// <summary>
        /// Make a call to Url to post request and get response as on Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<HttpResult<T>> PostJsonAndGetObject<T>(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false);

        /// <summary>
        /// Make a call to Url to post request and get response as a String
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<HttpResult<string>> PostJsonAndGetString(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false);

        /// <summary>
        /// Make a call to Url to put request and get response as an Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<HttpResult<T>> PutJsonAndGetObject<T>(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false);

        /// <summary>
        /// Make a call to Url to put request and get response as a String
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<HttpResult<string>> PutJsonAndGetString(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false);

        /// <summary>
        /// Make a call to Url to make delete request and get response as an Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<HttpResult<T>> DeleteJsonAndGetObject<T>(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false);

        /// <summary>
        /// Make a call to Url to make delete request and get response as a String
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<HttpResult<string>> DeleteJsonAndGetString(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false);

        /// <summary>
        /// Make a call to Url to make patch request and get response as object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="headers"></param>
        /// <param name="isFormUrlEncodedContent"></param>
        /// <returns></returns>
        Task<HttpResult<T>> PatchJsonAndGetObject<T>(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false);

        /// <summary>
        /// Make a call to Url to make patch request and get response as  string
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="headers"></param>
        /// <param name="isFormUrlEncodedContent"></param>
        /// <returns></returns>
        Task<HttpResult<string>> PatchJsonAndGetString(string url, object requestBody, Dictionary<string, string> headers = null, bool isFormUrlEncodedContent = false);
    }
}
