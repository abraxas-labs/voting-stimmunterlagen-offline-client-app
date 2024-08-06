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
    [System.Xml.Serialization.XmlTypeAttribute("candidatePositionType", Namespace="http://www.evoting.ch/xmlns/config/6")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CandidatePositionType
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
        [System.Xml.Serialization.XmlElementAttribute("candidateListIdentification", Order=0)]
        public string CandidateListIdentification { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Minimum inclusive value: 1.</para>
        /// <para xml:lang="en">Maximum inclusive value: 120.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.RangeAttribute(typeof(decimal), "1", "120", ConvertValueInInvariantCulture=true)]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("positionOnList", Order=1)]
        public byte PositionOnList { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Minimum length: 1.</para>
        /// <para xml:lang="en">Maximum length: 10.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MinLengthAttribute(1)]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("candidateReferenceOnPosition", Order=2)]
        public string CandidateReferenceOnPosition { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Minimum length: 1.</para>
        /// <para xml:lang="en">Maximum length: 50.</para>
        /// <para xml:lang="en">Pattern: [\w\-_]{1,50}.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MinLengthAttribute(1)]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.ComponentModel.DataAnnotations.RegularExpressionAttribute("[\\w\\-_]{1,50}")]
        [System.Xml.Serialization.XmlElementAttribute("candidateIdentification", Order=3)]
        public string CandidateIdentification { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<CandidateTextInformationTypeCandidateTextInfo> _candidateTextOnPosition;
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlArrayAttribute("candidateTextOnPosition", Order=4)]
        [System.Xml.Serialization.XmlArrayItemAttribute("candidateTextInfo", Namespace="http://www.evoting.ch/xmlns/config/6")]
        public System.Collections.Generic.List<CandidateTextInformationTypeCandidateTextInfo> CandidateTextOnPosition
        {
            get
            {
                return _candidateTextOnPosition;
            }
            set
            {
                _candidateTextOnPosition = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="CandidatePositionType" /> class.</para>
        /// </summary>
        public CandidatePositionType()
        {
            this._candidateTextOnPosition = new System.Collections.Generic.List<CandidateTextInformationTypeCandidateTextInfo>();
        }
    }
}
