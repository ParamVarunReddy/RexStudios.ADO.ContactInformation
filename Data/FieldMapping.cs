using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace RexStudios.ADO.ContactInformation.Data
{
    [DataContract]
    public class FieldMappingItem
    {
        [DataMember(Name = "crmfieldname")]
        public string CrmFieldName { get; set; }

        [DataMember(Name = "adofieldname")]
        public string AdoFieldName { get; set; }
    }

    public class FieldMappingReader
    {

        /// <summary>
        /// Loads field mapping items from a JSON string.
        /// </summary>
        /// <param name="jsonContent">JSON content representing an array of FieldMappingItem.</param>
        /// <returns>List of FieldMappingItem parsed from JSON string.</returns>
        public List<FieldMappingItem> LoadFromJson(string jsonContent)
        {
            if (string.IsNullOrEmpty(jsonContent))
                throw new ArgumentException("jsonContent must be provided", nameof(jsonContent));
            
            var bytes = Encoding.UTF8.GetBytes(jsonContent);
            using (var ms = new MemoryStream(bytes))
            {
                var serializer = new DataContractJsonSerializer(typeof(List<FieldMappingItem>));
                return (List<FieldMappingItem>)serializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// Returns the ADO field name corresponding to the given CRM field name from the mapping list.
        /// </summary>
        public string GetAdoFieldName(string crmFieldName, List<FieldMappingItem> mappings)
        {
            if (string.IsNullOrEmpty(crmFieldName) || mappings == null)
                return null;

            var mapping = mappings.FirstOrDefault(m => m.CrmFieldName.Equals(crmFieldName, StringComparison.OrdinalIgnoreCase));
            return mapping?.AdoFieldName;
        }

        /// <summary>
        /// Returns the CRM field name corresponding to the given ADO field name from the mapping list.
        /// </summary>
        public string GetCrmFieldName(string adoFieldName, List<FieldMappingItem> mappings)
        {
            if (string.IsNullOrEmpty(adoFieldName) || mappings == null)
                return null;

            var mapping = mappings.FirstOrDefault(m => m.AdoFieldName.Equals(adoFieldName, StringComparison.OrdinalIgnoreCase));
            return mapping?.CrmFieldName;
        }
    }
}
