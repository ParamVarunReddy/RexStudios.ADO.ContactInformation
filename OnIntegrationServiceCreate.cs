using Microsoft.Xrm.Sdk;
using System;
using RexStudios.ADO.ContactInformation.Interfaces;
using RexStudios.ADO.ContactInformation.BusinessLogic;

namespace ContactInformation.Plugins
{
    /// <summary>
    /// Plugin triggered on Create of rex_integrationservice (PostOperation, Asynchronous).
    /// </summary>
    public class OnIntegrationServiceCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Retrieve plugin context services
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // System user context
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            // Managed identity service (if configured)
            var managedIdentityService = (IManagedIdentityService)serviceProvider.GetService(typeof(IManagedIdentityService));

            try
            {
                // Only proceed on Create of rex_integrationservice
                // Gate conditions
                if (!GateConditions(context))
                    return;

                // Get the created integration entity from InputParameters
                Entity integrationEntity = (Entity)context.InputParameters["Target"];
                if (integrationEntity == null)
                {
                    tracingService.Trace("OnIntegrationServiceCreate: integrationEntity is null");
                    return;
                }
                string accessToken = managedIdentityService.AcquireToken(new[] { "https://org985affd2.api.crm.dynamics.com/.default" });
                tracingService.Trace($"accessToken");
                // Delegate to business logic
                IIntegrationServiceCreate handler = new CreateIntegrationService(tracingService);
                handler.Execute(integrationEntity, service, accessToken);

                // TODO: Implement post-operation asynchronous logic here
            }
            catch (Exception ex)
            {
                //tracingService.Trace($"OnIntegrationServiceCreate plugin error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gate conditions: only post-operation async create for rex_integrationservice
        /// </summary>
        private bool GateConditions(IPluginExecutionContext context)
        {
            return context.Stage == 40 // typically PostOperation
                && context.Mode == 0 // typically Asynchronous
                && context.PrimaryEntityName.Equals("rex_integrationservice", StringComparison.OrdinalIgnoreCase)
                && context.MessageName.Equals("Create", StringComparison.OrdinalIgnoreCase);
        }
    }
}
