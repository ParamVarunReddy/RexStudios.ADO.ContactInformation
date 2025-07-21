using Microsoft.Xrm.Sdk;

namespace RexStudios.ADO.ContactInformation.Interfaces
{
    /// <summary>
    /// Handler interface for rex_integrationservice create plugin.
    /// </summary>
    public interface IIntegrationServiceCreate
    {
        /// <summary>
        /// Executes logic for a post-operation, asynchronous create of rex_integrationservice.
        /// </summary>
        /// <param name="integrationEntity">The created rex_integrationservice entity.</param>
        /// <param name="service">Organization service for data operations.</param>
        /// <param name="accessToken">Access token for Azure DevOps authentication.</param>
        void Execute(Entity integrationEntity, IOrganizationService service, string accessToken);
    }
}
