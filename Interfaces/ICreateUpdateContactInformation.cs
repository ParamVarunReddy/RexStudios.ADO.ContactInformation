using Microsoft.Xrm.Sdk;

namespace RexStudios.ADO.ContactInformation.Interfaces
{
    public interface ICreateUpdateContactInformation
    {
        /// <summary>
        /// Executes create or update logic for a contact record.
        /// </summary>
        /// <param name="contactEntity">The contact entity to create or update.</param>
        void Execute(Entity contactEntity, IOrganizationService service);

        /// <summary>
        /// Executes update logic for an existing contact record.
        /// </summary>
        /// <param name="contactEntity">The contact entity to update.</param>
        void Update(Entity contactEntity, IOrganizationService service);
    }
}
