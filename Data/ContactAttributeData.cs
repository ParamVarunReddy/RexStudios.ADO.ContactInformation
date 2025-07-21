using System.Runtime.Serialization;

namespace RexStudios.ADO.ContactInformation.Data
{
    /// <summary>
    /// Represents contact attribute data for serialization.
    /// </summary>
    [DataContract]
    public class ContactAttributeData
    {
        [DataMember(Name = "AttributeName")]
        public string AttributeName { get; set; }

        [DataMember(Name = "Value")]
        public string Value { get; set; }
    }
}
