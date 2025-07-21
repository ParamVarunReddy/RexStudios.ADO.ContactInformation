namespace RexStudios.ADO.ContactInformation.BusinessLayer.Http
{
    using System;
    using System.Net;

    /// <summary>
    /// Http Result Class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HttpResult<T>
    {
        /// <summary>
        /// URI For Request
        /// </summary>
        public string RequestUri { get; set; }
        /// <summary>
        /// Reason Phrase
        /// </summary>
        public string ReasonPhrase { get; set; }
        /// <summary>
        /// Response Body
        /// </summary>
        public string ResponseBody { get; set; }
        /// <summary>
        /// Check whether call is successful or not
        /// </summary>
        public bool IsSuccessful { get; set; }
        /// <summary>
        /// HTTP Status Code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Any Additional Context
        /// </summary>
        public string AdditionalContext { get; set; }
        /// <summary>
        /// Error Message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Exception
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// Exception Message
        /// </summary>
        public string ExceptionMessage { get { return Exception?.Message; } }
        /// <summary>
        /// Result
        /// </summary>
        public T Result { get; set; }
    }
}
