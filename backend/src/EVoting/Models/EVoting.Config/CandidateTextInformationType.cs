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
    [System.Xml.Serialization.XmlTypeAttribute("candidateTextInformationType", Namespace="http://www.evoting.ch/xmlns/config/6")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CandidateTextInformationType
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<CandidateTextInformationTypeCandidateTextInfo> _candidateTextInfo;
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("candidateTextInfo", Order=0)]
        public System.Collections.Generic.List<CandidateTextInformationTypeCandidateTextInfo> CandidateTextInfo
        {
            get
            {
                return _candidateTextInfo;
            }
            set
            {
                _candidateTextInfo = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="CandidateTextInformationType" /> class.</para>
        /// </summary>
        public CandidateTextInformationType()
        {
            this._candidateTextInfo = new System.Collections.Generic.List<CandidateTextInformationTypeCandidateTextInfo>();
        }
    }
}
