using Microsoft.Xrm.Sdk;
using RexStudios.ADO.ContactInformation.BusinessLayer.Http;
using RexStudios.ADO.ContactInformation.Data;
using RexStudios.ADO.ContactInformation.Interfaces;
using System;

namespace RexStudios.ADO.ContactInformation.BusinessLogic
{
    /// <summary>
    /// Business logic for handling creation of rex_integrationservice work.
    /// </summary>
    public class CreateIntegrationService : IIntegrationServiceCreate
    {
        private readonly BeverageConfigurationReader _configReader;
        private readonly IHttpService _httpService;
        private readonly ITracingService _tracingService;

        public CreateIntegrationService(ITracingService tracingService)
        {
            if (tracingService == null) throw new ArgumentNullException(nameof(tracingService));
            _configReader = new BeverageConfigurationReader();
            _tracingService = tracingService;
            // Initialize HttpService using tracing service
            _httpService = new HttpService(_tracingService);
        }
        /// <summary>
        /// Executes integration service create logic.
        /// </summary>
        /// <param name="integrationEntity">The created integration service entity.</param>
        /// <param name="service">Organization service.</param>
        /// <param name="accessToken">Access token for Azure DevOps.</param>
        public void Execute(Entity integrationEntity, IOrganizationService service, string accessToken)
        {
            // Guard clauses for null parameters
            if (integrationEntity == null)
            {
                _tracingService.Trace("CreateIntegrationService: integrationEntity is null");
                return;
            }
            if (service == null)
            {
                _tracingService.Trace("CreateIntegrationService: IOrganizationService is null");
                return;
            }
            if (string.IsNullOrEmpty(accessToken))
            {
                _tracingService.Trace("CreateIntegrationService: accessToken is null or empty");
                return;
            }
            // Retrieve all ADO_* configuration values
            var adoConfigs = _configReader.GetValuesByPrefix(service, "ADO_");
            if (adoConfigs == null || adoConfigs.Count == 0)
            {
                _tracingService.Trace("CreateIntegrationService: No ADO configuration values found with prefix 'ADO_'.");
                return;
            }
            // Extract ADO configuration values
            adoConfigs.TryGetValue("ADO_OrgName", out var orgUrl);
            adoConfigs.TryGetValue("ADO_ProjectName", out var projName);
            adoConfigs.TryGetValue("ADO_WorkItemType", out var wiType);
            // Determine regarding object entity type
            if (integrationEntity.Attributes.TryGetValue("rex_regardingobject", out var regardingObj) && regardingObj is EntityReference er)
            {
                if (er.LogicalName.Equals("contact", StringComparison.OrdinalIgnoreCase))
                {
                    // Use ContactADOService for contacts
                    var contactService = new ContactADOService(_httpService, _tracingService);
                    contactService.Process(
                        er,
                        integrationEntity,
                        service,
                        accessToken,
                        orgUrl,
                        projName,
                        wiType);
                    return;
                }
            }
            // TODO: Handle other entity types or default behavior
        }
    }
}
