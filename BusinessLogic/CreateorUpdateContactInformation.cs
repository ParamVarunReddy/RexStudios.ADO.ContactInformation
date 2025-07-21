using Microsoft.Xrm.Sdk;
using RexStudios.ADO.ContactInformation.Data;
using RexStudios.ADO.ContactInformation.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace RexStudios.ADO.ContactInformation.BusinessLogic
{
    public class CreateorUpdateContactInformation : ICreateUpdateContactInformation
    {
        private BeverageConfigurationReader beverageConfigurationReader;
        public CreateorUpdateContactInformation()
        { 
        }
        /// <summary>
        /// Placeholder for create logic.
        /// </summary>
        public void Execute(Entity contactEntity, IOrganizationService service)
        {
            beverageConfigurationReader = new BeverageConfigurationReader();
            if (contactEntity == null || service == null)
            {
                Trace.TraceWarning("CreateorUpdateContactInformation.Execute: contactEntity or service is null");
                return;
            }

            string jsonResult = SerializeAttributesToJson(contactEntity);
            // Map CRM attribute names to ADO field names
            if (!string.IsNullOrEmpty(jsonResult))
            {
                string mappingJson = beverageConfigurationReader.GetValue(service, "ADO_ContactDetail");
                if (string.IsNullOrEmpty(mappingJson))
                {
                    Trace.TraceWarning("Field mapping JSON is empty or not found");
                    return;
                }
                else
                {
                    var mappings = new FieldMappingReader().LoadFromJson(mappingJson);
                    jsonResult = MapJsonFields(jsonResult, mappings);
                }
            }
            // Create integration record
            CreateIntegrationRecord(contactEntity, service, jsonResult);
        }

        /// <summary>
        /// Placeholder for update logic.
        /// </summary>
        public void Update(Entity contactEntity, IOrganizationService service)
        {
            beverageConfigurationReader = new BeverageConfigurationReader();
            if (contactEntity == null || service == null)
            {
                Trace.TraceWarning("CreateorUpdateContactInformation.Update: contactEntity or service is null");
                return;
            }

            string jsonResult = SerializeAttributesToJson(contactEntity);
            // Map CRM attribute names to ADO field names
            if (!string.IsNullOrEmpty(jsonResult))
            {
                string mappingJson = beverageConfigurationReader.GetValue(service, "ADO_ContactDetail");
                if (string.IsNullOrEmpty(mappingJson))
                {
                    Trace.TraceWarning("Field mapping JSON is empty or not found");
                    return;
                }
                else
                {
                    var mappings = new FieldMappingReader().LoadFromJson(mappingJson);
                    jsonResult = MapJsonFields(jsonResult, mappings);
                }
            }
            // Create integration record for update
            CreateIntegrationRecord(contactEntity, service, jsonResult);
        }

        /// <summary>
        /// Common helper to serialize contact attributes to JSON
        /// </summary>
        private string SerializeAttributesToJson(Entity contactEntity)
        {
            if (contactEntity == null || contactEntity.Attributes == null)
            {
                Trace.TraceWarning("CreateorUpdateContactInformation.SerializeAttributesToJson: contactEntity or Attributes is null");
                return string.Empty;
            }
            var attributeDataList = contactEntity.Attributes
                .Where(kvp => kvp.Value != null)
                .Select(kvp => new ContactAttributeData
                {
                    AttributeName = kvp.Key,
                    Value = kvp.Value is OptionSetValue osVal ? osVal.Value.ToString()
                        : kvp.Value is EntityReference er ? er.Name
                        : kvp.Value.ToString()
                })
                .ToList();
            var serializer = new DataContractJsonSerializer(typeof(List<ContactAttributeData>));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, attributeDataList);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        
        /// <summary>
        /// Creates a rex_integrationservice record for the contact.
        /// </summary>
        private void CreateIntegrationRecord(Entity contactEntity, IOrganizationService service, string jsonString)
        {
            if (contactEntity == null || service == null || string.IsNullOrEmpty(jsonString))
            {
                Trace.TraceWarning("CreateorUpdateContactInformation.CreateIntegrationRecord: invalid parameters");
                return;
            }
            // Title from contact's full name
            string title = contactEntity.Contains("rex_regardingobject") ? contactEntity.GetAttributeValue<EntityReference>("rex_regardingobject").Name : string.Empty;
            // ADO Unique Number
            string adoUniqueNumber = contactEntity.Contains("rex_adouniquenumber") ? contactEntity.GetAttributeValue<string>("rex_adouniquenumber") : string.Empty;
            // Target GUID
            string targetGuid = contactEntity.Contains("rex_adoguid") ? contactEntity.GetAttributeValue<string>("rex_adoguid") : string.Empty;
            // Target Link placeholder
            string targetLink = contactEntity.Contains("rex_adolink") ? contactEntity.GetAttributeValue<string>("rex_adolink") : string.Empty;

            var integrationEntity = new Entity("rex_integrationservice");
            integrationEntity["rex_title"] = title;
            integrationEntity["rex_request"] = jsonString;
            integrationEntity["rex_targetuniquenumber"] = adoUniqueNumber;
            integrationEntity["rex_targetguid"] = targetGuid;
            integrationEntity["rex_targetlink"] = targetLink;
            // Set regarding to contact
            integrationEntity["rex_regardingobject"] = new EntityReference("contact", contactEntity.Id);

            service.Create(integrationEntity);
        }

        /// <summary>
        /// Maps JSON array fields from CRM attribute names to ADO field names based on provided mappings.
        /// Unmapped CRM fields are logged and removed.
        /// </summary>
        private string MapJsonFields(string jsonContent, List<FieldMappingItem> mappings)
        {
            if (string.IsNullOrEmpty(jsonContent) || mappings == null)
                return jsonContent;

            try
            {
                // Deserialize to ContactAttributeData list
                var serializer = new DataContractJsonSerializer(typeof(List<ContactAttributeData>));
                List<ContactAttributeData> list;
                using (var msRead = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent)))
                {
                    list = (List<ContactAttributeData>)serializer.ReadObject(msRead);
                }

                // Map fields and filter
                var mapped = new List<ContactAttributeData>();
                foreach (var item in list)
                {
                    var mapping = mappings.FirstOrDefault(m => m.CrmFieldName.Equals(item.AttributeName, StringComparison.OrdinalIgnoreCase));
                    if (mapping == null)
                    {
                        Trace.TraceWarning($"No ADO mapping for CRM field '{item.AttributeName}'");
                        continue;
                    }
                    mapped.Add(new ContactAttributeData { AttributeName = mapping.AdoFieldName, Value = item.Value });
                }

                // Serialize back to JSON
                using (var msWrite = new MemoryStream())
                {
                    serializer.WriteObject(msWrite, mapped);
                    return Encoding.UTF8.GetString(msWrite.ToArray());
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"MapJsonFields failed: {ex.Message}");
                return jsonContent;
            }
        }
    }
}