using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using RexStudios.ADO.ContactInformation.Interfaces;
using RexStudios.ADO.ContactInformation.BusinessLayer.Http;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace RexStudios.ADO.ContactInformation.BusinessLogic
{
    /// <summary>
    /// Implementation of IAzureDevOpsWorkItemService using Azure DevOps REST API.
    /// </summary>
    public class AzureDevOpsWorkItemService : IAzureDevOpsWorkItemService
    {
    private readonly HttpClient _httpClient;
    private readonly IHttpService _httpService;
    private readonly ITracingService _tracingService;

        /// <summary>
        /// <summary>
        /// Default constructor initializes HttpClient and HttpService for HTTP calls.
        /// </summary>
        public AzureDevOpsWorkItemService(IHttpService httpService, ITracingService tracingService)
        {
            _httpClient = new HttpClient();
            _httpService = httpService;
            _tracingService = tracingService;
        }

        private void SetAuth(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentException("Access token must be provided", nameof(accessToken));
            // Use Bearer token for authorization
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public string CreateWorkItem(string organizationUrl, string projectName, string workItemType, string jsonDocument, string accessToken)
        {
            var requestUri = $"{organizationUrl}/{projectName}/_apis/wit/workitems/${workItemType}?api-version=6.0";
            // Use HttpService to send POST request for creation
            var headers = new Dictionary<string, string> { { "Authorization", $"Bearer {accessToken}" } };
            // Trace request details
            _tracingService.Trace($"CreateWorkItem: POST {requestUri}");
            _tracingService.Trace($"CreateWorkItem: Payload={jsonDocument}");
            var result = _httpService.PostJsonAndGetString(requestUri, jsonDocument, headers).Result;
            // Trace response
            _tracingService.Trace($"CreateWorkItem: Response={result.Result}");
            return result.Result;
        }

        public string UpdateWorkItem(string organizationUrl, int workItemId, string jsonDocument, string accessToken)
        {
            SetAuth(accessToken);
            var requestUri = $"{organizationUrl}/_apis/wit/workitems/{workItemId}?api-version=6.0";
            using (var content = new StringContent(jsonDocument, Encoding.UTF8, "application/json-patch+json"))
            using (var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri) { Content = content })
            {
                var headersUpd = new Dictionary<string, string> { { "Authorization", $"Bearer {accessToken}" } };
                // Trace request details
                _tracingService.Trace($"UpdateWorkItem: PATCH {requestUri}");
                _tracingService.Trace($"UpdateWorkItem: Payload={jsonDocument}");
                var resultUpd = _httpService.PatchJsonAndGetString(requestUri, jsonDocument, headersUpd).Result;
                // Trace response
                _tracingService.Trace($"UpdateWorkItem: Response={resultUpd.Result}");
                return resultUpd.Result;
            }
        }

        public string GetWorkItem(string organizationUrl, int workItemId, string accessToken)
        {
            SetAuth(accessToken);
            var requestUri = $"{organizationUrl}/_apis/wit/workitems/{workItemId}?api-version=6.0";
            // Use HttpService to get work item
            var headersGet = new Dictionary<string, string> { { "Authorization", $"Bearer {accessToken}" } };
            // Trace request details
            _tracingService.Trace($"GetWorkItem: GET {requestUri}");
            var resultGet = _httpService.GetStringAsync(requestUri, headersGet).Result;
            // Trace response
            _tracingService.Trace($"GetWorkItem: Response={resultGet.Result}");
            return resultGet.Result;
        }
    }
}
