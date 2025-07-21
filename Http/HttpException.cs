namespace RexStudios.ADO.ContactInformation.BusinessLayer.Http
{
    using System;

    ///// <summary>
    /// Http Exception
    /// </summary>
    public class HttpException : Exception
    {
        public HttpException() { }
        public HttpException(string message) : base(message) { }
        public HttpException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Request Uri
        /// </summary>
        public string RequestUri { get; set; }

        /// <summary>
        /// Request Body
        /// </summary>
        public string RequestBody { get; set; }

        /// <summary>
        /// Response Body
        /// </summary>
        public string ResponseBody { get; set; }

        /// <summary>
        /// Expected Response Type Name
        /// </summary>
        public string ExpectedResponseTypeName { get; set; }
    }
}