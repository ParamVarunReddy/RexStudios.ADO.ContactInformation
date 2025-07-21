using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;

namespace RexStudios.ADO.ContactInformation.Data
{
    /// <summary>
    /// Helper to read rex_beverageconfiguration records.
    /// </summary>
    public class BeverageConfigurationReader
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BeverageConfigurationReader() { }
        /// <summary>
        /// Retrieves the rex_value for the given rex_name from rex_beverageconfiguration.
        /// </summary>
        /// <param name="service">Organization service to use for the query.</param>
        /// <param name="name">The configuration name (rex_name) to look up.</param>
        /// <returns>The rex_value as a string, or null if not found.</returns>
        public string GetValue(IOrganizationService service, string name)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Configuration name must be provided", nameof(name));

            var query = new QueryExpression("rex_beverageconfiguration")
            {
                ColumnSet = new ColumnSet("rex_value"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("rex_name", ConditionOperator.Equal, name)
                    }
                },
                TopCount = 1
            };

            var result = service.RetrieveMultiple(query);
            if (result.Entities.Count > 0)
            {
                var entity = result.Entities[0];
                return entity.GetAttributeValue<string>("rex_value");
            }

            return null;
        }
    
    /// <summary>
    /// Retrieves all rex_value for configuration names starting with the given prefix.
    /// </summary>
    /// <param name="service">Organization service to use for the query.</param>
    /// <param name="prefix">The prefix of rex_name to look up (e.g., "ADO_").</param>
    /// <returns>Dictionary mapping configuration names to their values.</returns>
    public Dictionary<string, string> GetValuesByPrefix(IOrganizationService service, string prefix)
    {
        if (service == null) throw new ArgumentNullException(nameof(service));
        if (string.IsNullOrEmpty(prefix)) throw new ArgumentException("Prefix must be provided", nameof(prefix));

        var query = new QueryExpression("rex_beverageconfiguration")
        {
            ColumnSet = new ColumnSet("rex_name", "rex_value"),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression("rex_name", ConditionOperator.Like, $"{prefix}%")
                }
            }
        };

        var result = service.RetrieveMultiple(query);
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entity in result.Entities)
        {
            var key = entity.GetAttributeValue<string>("rex_name");
            var value = entity.GetAttributeValue<string>("rex_value");
            if (key != null)
            {
                dict[key] = value;
            }
        }
        return dict;
    }
    }
}
