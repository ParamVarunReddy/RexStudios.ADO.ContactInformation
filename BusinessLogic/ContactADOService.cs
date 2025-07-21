using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xrm.Sdk;
using RexStudios.ADO.ContactInformation.BusinessLayer.Http;

namespace RexStudios.ADO.ContactInformation.BusinessLogic
{
    /// <summary>
    /// Handles integration work for Contact entities.
    /// </summary>
    public class ContactADOService
    {
        private readonly IHttpService _httpService;
        private readonly ITracingService _tracingService;
        private readonly AzureDevOpsWorkItemService _adoService;

        public ContactADOService(IHttpService httpService, ITracingService tracingService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            _tracingService = tracingService ?? throw new ArgumentNullException(nameof(tracingService));
            _adoService = new AzureDevOpsWorkItemService(_httpService, _tracingService);
        }
        
        /// <summary>
        /// Entry point for processing contact integration work.
        /// </summary>
        public void Process(EntityReference regardingRef, Entity integrationEntity, IOrganizationService crmService, string accessToken,
            string organizationUrl, string projectName, string workItemType)
        {
            if (regardingRef == null)
            {
                Trace.TraceWarning("ContactADOService.Process: regardingRef is null");
                return;
            }
            if (integrationEntity == null)
            {
                Trace.TraceWarning("ContactADOService.Process: integrationEntity is null");
                return;
            }
            if (crmService == null)
            {
                Trace.TraceWarning("ContactADOService.Process: IOrganizationService is null");
                return;
            }
            if (string.IsNullOrEmpty(accessToken))
            {
                Trace.TraceWarning("ContactADOService.Process: accessToken is null or empty");
                return;
            }

            // Log entry parameters
            _tracingService.Trace($"ContactADOService.Process: regardingRef.Id={regardingRef.Id}, integrationEntity.Id={integrationEntity.Id}, accessToken=[REDACTED], orgUrl={organizationUrl}, projectName={projectName}, workItemType={workItemType}");
            // Extract existing ADO fields
            var adoUniqueNumber = integrationEntity.GetAttributeValue<string>("rex_targetuniquenumber");
            // ...existing checks...
            if (!string.IsNullOrEmpty(adoUniqueNumber))
            {
                ExecuteWorkItem(integrationEntity, crmService, accessToken, organizationUrl, projectName, workItemType, adoUniqueNumber);
                return;
            }
            ExecuteWorkItem(integrationEntity, crmService, accessToken, organizationUrl, projectName, workItemType, null);
        }

        // Helper: perform create or update based on presence of workItemId
        private void ExecuteWorkItem(Entity integrationEntity, IOrganizationService crmService, string accessToken,
            string orgUrl, string projectName, string workItemType, string adoUniqueNumber)
        {
            var requestJson = integrationEntity.GetAttributeValue<string>("rex_request");
            _tracingService.Trace($"ContactADOService.ExecuteWorkItem: requestJson={requestJson}");
            if (string.IsNullOrEmpty(requestJson))
            {
                _tracingService.Trace("ContactADOService: Missing request JSON");
                return;
            }
            // parse attribute data
            var attributes = ParseAttributes(requestJson);
            // build payload
            var payload = BuildPayload(attributes, projectName);
            _tracingService.Trace($"ContactADOService.ExecuteWorkItem: payload={System.Text.Json.JsonSerializer.Serialize(payload)}");
            string jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            // Execute create or update and capture success for status reason
            bool success = false;
            try
            {
            _tracingService.Trace($"ContactADOService.ExecuteWorkItem: adoUniqueNumber={adoUniqueNumber}");
            if (string.IsNullOrEmpty(adoUniqueNumber))
                {
                    // create
                    var result = _adoService.CreateWorkItem(orgUrl, projectName, workItemType, jsonPayload, accessToken);
                    _tracingService.Trace($"ContactADOService.ExecuteWorkItem: CreateWorkItem resultJson={result}");
                    UpdateCrmReferences(integrationEntity, crmService, result);
                }
                else
                {
                    // update
                    if (!int.TryParse(adoUniqueNumber, out int id)) throw new InvalidOperationException("Invalid work item ID");
                    _tracingService.Trace($"ContactADOService.ExecuteWorkItem: UpdateWorkItem id={id}, jsonPayload={jsonPayload}");
                    _adoService.UpdateWorkItem(orgUrl, id, jsonPayload, accessToken);
                }
                success = true;
            }
            catch (Exception ex)
            {
                //_tracingService.Trace($"ContactADOService: Work item operation error: {ex.Message}");
                SetIntegrationMessage(integrationEntity, crmService, ex.Message);
                success = false;
            }
            finally
            {
                try
                {
                    var statusUpdate = new Entity(integrationEntity.LogicalName, integrationEntity.Id)
                    {
                        ["statuscode"] = new OptionSetValue(success ? 910290002 : 910290003)
                    };
                    crmService.Update(statusUpdate);
                }
                catch (Exception exStatus)
                {
                    _tracingService.Trace($"ContactADOService: Failed to update status reason: {exStatus.Message}");
                }
            }
        }

        private List<RexStudios.ADO.ContactInformation.Data.ContactAttributeData> ParseAttributes(string requestJson)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(
                typeof(List<RexStudios.ADO.ContactInformation.Data.ContactAttributeData>));
            using (var ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestJson)))
            {
                return (List<RexStudios.ADO.ContactInformation.Data.ContactAttributeData>)serializer.ReadObject(ms);
            }
        }

        private object BuildPayload(IEnumerable<RexStudios.ADO.ContactInformation.Data.ContactAttributeData> attributes, string projectName)
        {
            var userFields = attributes.ToDictionary(a => a.AttributeName, a => a.Value);
            // Determine title based on fullname field
            string title;
            if (!userFields.TryGetValue("fullname", out title) || title == null)
            {
                title = string.Empty;
            }
            return new
            {
                body = new
                {
                    title = title,
                    iteration = projectName,
                    area = projectName,
                    userEnteredFields = userFields,
                    dynamicFields = new Dictionary<string, string> { { "System.State", "Active" } }
                }
            };
        }

        private void UpdateCrmReferences(Entity integrationEntity, IOrganizationService crmService, string resultJson)
        {
            using (var doc = System.Text.Json.JsonDocument.Parse(resultJson))
            {
                int id = doc.RootElement.GetProperty("id").GetInt32();
                string url = doc.RootElement.GetProperty("url").GetString();
                _tracingService.Trace($"ContactADOService.UpdateCrmReferences: parsed id={id}, url={url}");
            // update integration record
            var update = new Entity(integrationEntity.LogicalName, integrationEntity.Id)
            {
                ["rex_targetuniquenumber"] = id.ToString(), ["rex_targetguid"] = id.ToString(), ["rex_targetlink"] = url
            };
            crmService.Update(update);
            // update contact record
            var refer = (EntityReference)integrationEntity["rex_regardingobject"];
            var contact = new Entity(refer.LogicalName, refer.Id)
            {
                ["rex_adouniquenumber"] = id.ToString(), ["rex_adoguid"] = id.ToString(), ["rex_adolink"] = url
            };
            crmService.Update(contact);
            }
        }

        // Updates the integration service record with error message
        private void SetIntegrationMessage(Entity integrationEntity, IOrganizationService crmService, string message)
        {
            try
            {
                var update = new Entity(integrationEntity.LogicalName, integrationEntity.Id)
                {
                    ["rex_exception"] = message
                };
                crmService.Update(update);
            }
            catch (Exception ex)
            {
                _tracingService.Trace($"ContactADOService: Failed to set integration message: {ex.Message}");
            }
        }
    }
}
