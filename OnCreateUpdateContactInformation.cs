using Microsoft.Xrm.Sdk;
using RexStudios.ADO.ContactInformation.BusinessLogic;
using RexStudios.ADO.ContactInformation.Interfaces;
using System;

namespace ContactInformation
{

    public class OnCreateUpdateContactInformation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            if (!GateConditions(context))
                return;

            // Get the target entity
            Entity targetEntity = (Entity)context.InputParameters["Target"];
            if (context.MessageName.Equals("Create", StringComparison.OrdinalIgnoreCase))
            {
                // For Create, the targetEntity in post-operation contains all created attributes
                ICreateUpdateContactInformation handler = new CreateorUpdateContactInformation();
                handler.Execute(targetEntity,service);
            }
            else if (context.MessageName.Equals("Update", StringComparison.OrdinalIgnoreCase))
            {
                ICreateUpdateContactInformation handler = new CreateorUpdateContactInformation();
                handler.Update(targetEntity,service);
            }
        }

        private bool GateConditions(IPluginExecutionContext context)
        {
            try
            {
                // Prevent recursive execution
                if (context.Depth > 1)
                    return false;
                return context.InputParameters.Contains("Target")
                    && context.InputParameters["Target"] is Entity entity
                    && entity.LogicalName.Equals("contact", StringComparison.OrdinalIgnoreCase)
                    && (context.MessageName.Equals("Create", StringComparison.OrdinalIgnoreCase)
                        || context.MessageName.Equals("Update", StringComparison.OrdinalIgnoreCase))
                    && context.Stage == 40
                    && context.Mode == 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

