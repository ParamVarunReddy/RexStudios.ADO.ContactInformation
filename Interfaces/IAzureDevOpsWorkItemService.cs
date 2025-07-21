using System;

namespace RexStudios.ADO.ContactInformation.Interfaces
{
    /// <summary>
    /// Provides operations for Azure DevOps work item creation, update, and retrieval.
    /// </summary>
    public interface IAzureDevOpsWorkItemService
    {
        /// <summary>
        /// Creates a new work item in Azure DevOps.
        /// </summary>
        /// <param name="organizationUrl">The Azure DevOps organization URL (e.g., https://dev.azure.com/{organization}).</param>
        /// <param name="projectName">The name of the Azure DevOps project.</param>
        /// <param name="workItemType">The type of work item (e.g., "Task", "Bug").</param>
        /// <param name="jsonDocument">The JSON patch document for the work item fields.</param>
        /// <param name="personalAccessToken">The PAT used for authentication.</param>
        /// <returns>The response JSON string of the created work item.</returns>
        string CreateWorkItem(string organizationUrl, string projectName, string workItemType, string jsonDocument, string accessToken);

        /// <summary>
        /// Updates an existing work item in Azure DevOps.
        /// </summary>
        /// <param name="organizationUrl">The Azure DevOps organization URL.</param>
        /// <param name="workItemId">The ID of the work item to update.</param>
        /// <param name="jsonDocument">The JSON patch document for the updated fields.</param>
        /// <param name="personalAccessToken">The PAT used for authentication.</param>
        /// <returns>The response JSON string of the updated work item.</returns>
        string UpdateWorkItem(string organizationUrl, int workItemId, string jsonDocument, string accessToken);

        /// <summary>
        /// Retrieves a work item from Azure DevOps.
        /// </summary>
        /// <param name="organizationUrl">The Azure DevOps organization URL.</param>
        /// <param name="workItemId">The ID of the work item to retrieve.</param>
        /// <param name="personalAccessToken">The PAT used for authentication.</param>
        /// <returns>The response JSON string of the work item.</returns>
        string GetWorkItem(string organizationUrl, int workItemId, string accessToken);
    }
}
