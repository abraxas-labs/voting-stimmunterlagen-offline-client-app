//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// This code was generated by XmlSchemaClassGenerator version 2.1.963.0
namespace EVoting.Config
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.1.963.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("electionGroupBallotType", Namespace="http://www.evoting.ch/xmlns/config/5")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ElectionGroupBallotType
    {
        
        /// <summary>
        /// <para xml:lang="en">Minimum length: 1.</para>
        /// <para xml:lang="en">Maximum length: 50.</para>
        /// <para xml:lang="en">Pattern: [\w\-_]{1,50}.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MinLengthAttribute(1)]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.ComponentModel.DataAnnotations.RegularExpressionAttribute("[\\w\\-_]{1,50}")]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("electionGroupIdentification", Order=0)]
        public string ElectionGroupIdentification { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Minimum length: 1.</para>
        /// <para xml:lang="en">Maximum length: 50.</para>
        /// <para xml:lang="en">Pattern: [\w\-_]{1,50}.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MinLengthAttribute(1)]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.ComponentModel.DataAnnotations.RegularExpressionAttribute("[\\w\\-_]{1,50}")]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("domainOfInfluence", Order=1)]
        public string DomainOfInfluence { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<ElectionGroupDescriptionInformationTypeElectionGroupDescriptionInfo> _electionGroupDescription;
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlArrayAttribute("electionGroupDescription", Order=2)]
        [System.Xml.Serialization.XmlArrayItemAttribute("electionGroupDescriptionInfo", Namespace="http://www.evoting.ch/xmlns/config/5")]
        public System.Collections.Generic.List<ElectionGroupDescriptionInformationTypeElectionGroupDescriptionInfo> ElectionGroupDescription
        {
            get
            {
                return _electionGroupDescription;
            }
            set
            {
                _electionGroupDescription = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="ElectionGroupBallotType" /> class.</para>
        /// </summary>
        public ElectionGroupBallotType()
        {
            this._electionGroupDescription = new System.Collections.Generic.List<ElectionGroupDescriptionInformationTypeElectionGroupDescriptionInfo>();
            this._electionInformation = new System.Collections.Generic.List<ElectionInformationType>();
        }
        
        /// <summary>
        /// <para xml:lang="en">Minimum inclusive value: 1.</para>
        /// <para xml:lang="en">Maximum inclusive value: 50.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.RangeAttribute(typeof(decimal), "1", "50", ConvertValueInInvariantCulture=true)]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("electionGroupPosition", Order=3)]
        public byte ElectionGroupPosition { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<ElectionInformationType> _electionInformation;
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("electionInformation", Order=4)]
        public System.Collections.Generic.List<ElectionInformationType> ElectionInformation
        {
            get
            {
                return _electionInformation;
            }
            set
            {
                _electionInformation = value;
            }
        }
    }
}
