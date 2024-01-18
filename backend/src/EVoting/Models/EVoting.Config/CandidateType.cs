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
    [System.Xml.Serialization.XmlTypeAttribute("candidateType", Namespace="http://www.evoting.ch/xmlns/config/5")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CandidateType
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
        [System.Xml.Serialization.XmlElementAttribute("candidateIdentification", Order=0)]
        public string CandidateIdentification { get; set; }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("mrMrs", Order=1)]
        public MrMrsType MrMrsValue { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Gets or sets a value indicating whether the MrMrs property is specified.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool MrMrsValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<MrMrsType> MrMrs
        {
            get
            {
                if (this.MrMrsValueSpecified)
                {
                    return this.MrMrsValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.MrMrsValue = value.GetValueOrDefault();
                this.MrMrsValueSpecified = value.HasValue;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Minimum length: 1.</para>
        /// <para xml:lang="en">Maximum length: 50.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MinLengthAttribute(1)]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.Xml.Serialization.XmlElementAttribute("title", Order=2)]
        public string Title { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Minimum length: 1.</para>
        /// <para xml:lang="en">Maximum length: 100.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MinLengthAttribute(1)]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("familyName", Order=3)]
        public string FamilyName { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Minimum length: 1.</para>
        /// <para xml:lang="en">Maximum length: 100.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MinLengthAttribute(1)]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.Xml.Serialization.XmlElementAttribute("firstName", Order=4)]
        public string FirstName { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Minimum length: 1.</para>
        /// <para xml:lang="en">Maximum length: 100.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MinLengthAttribute(1)]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("callName", Order=5)]
        public string CallName { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<CandidateTextInformationTypeCandidateTextInfo> _candidateText;
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlArrayAttribute("candidateText", Order=6)]
        [System.Xml.Serialization.XmlArrayItemAttribute("candidateTextInfo", Namespace="http://www.evoting.ch/xmlns/config/5")]
        public System.Collections.Generic.List<CandidateTextInformationTypeCandidateTextInfo> CandidateText
        {
            get
            {
                return _candidateText;
            }
            set
            {
                _candidateText = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="CandidateType" /> class.</para>
        /// </summary>
        public CandidateType()
        {
            this._candidateText = new System.Collections.Generic.List<CandidateTextInformationTypeCandidateTextInfo>();
            this._swiss = new System.Collections.Generic.List<string>();
            this._occupationalTitle = new System.Collections.Generic.List<OccupationalTitleInformationTypeOccupationalTitleInfo>();
            this._partyAffiliation = new System.Collections.Generic.List<PartyAffiliationformationTypePartyAffiliationInfo>();
        }
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("dateOfBirth", Order=7, DataType="date")]
        public System.DateTime DateOfBirth { get; set; }
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("sex", Order=8)]
        public CandidateTypeSex Sex { get; set; }
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("incumbent", Order=9)]
        public IncumbentType Incumbent { get; set; }
        
        [System.Xml.Serialization.XmlElementAttribute("dwellingAddress", Order=10)]
        public DwellingAddressType DwellingAddress { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<string> _swiss;
        
        /// <summary>
        /// <para xml:lang="en">Minimum length: 1.</para>
        /// <para xml:lang="en">Maximum length: 80.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MinLengthAttribute(1)]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(80)]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlArrayAttribute("swiss", Order=11)]
        [System.Xml.Serialization.XmlArrayItemAttribute("origin", Namespace="http://www.evoting.ch/xmlns/config/5")]
        public System.Collections.Generic.List<string> Swiss
        {
            get
            {
                return _swiss;
            }
            set
            {
                _swiss = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<OccupationalTitleInformationTypeOccupationalTitleInfo> _occupationalTitle;
        
        [System.Xml.Serialization.XmlArrayAttribute("occupationalTitle", Order=12)]
        [System.Xml.Serialization.XmlArrayItemAttribute("occupationalTitleInfo", Namespace="http://www.evoting.ch/xmlns/config/5")]
        public System.Collections.Generic.List<OccupationalTitleInformationTypeOccupationalTitleInfo> OccupationalTitle
        {
            get
            {
                return _occupationalTitle;
            }
            set
            {
                _occupationalTitle = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the OccupationalTitle collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool OccupationalTitleSpecified
        {
            get
            {
                return ((this.OccupationalTitle != null) 
                            && (this.OccupationalTitle.Count != 0));
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Minimum inclusive value: 1.</para>
        /// <para xml:lang="en">Maximum inclusive value: 120.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.RangeAttribute(typeof(decimal), "1", "120", ConvertValueInInvariantCulture=true)]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("position", Order=13)]
        public byte PositionValue { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Gets or sets a value indicating whether the Position property is specified.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PositionValueSpecified { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Minimum inclusive value: 1.</para>
        /// <para xml:lang="en">Maximum inclusive value: 120.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<byte> Position
        {
            get
            {
                if (this.PositionValueSpecified)
                {
                    return this.PositionValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PositionValue = value.GetValueOrDefault();
                this.PositionValueSpecified = value.HasValue;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Minimum length: 1.</para>
        /// <para xml:lang="en">Maximum length: 10.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MinLengthAttribute(1)]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
        [System.Xml.Serialization.XmlElementAttribute("referenceOnPosition", Order=14)]
        public string ReferenceOnPosition { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<PartyAffiliationformationTypePartyAffiliationInfo> _partyAffiliation;
        
        [System.Xml.Serialization.XmlArrayAttribute("partyAffiliation", Order=15)]
        [System.Xml.Serialization.XmlArrayItemAttribute("partyAffiliationInfo", Namespace="http://www.evoting.ch/xmlns/config/5")]
        public System.Collections.Generic.List<PartyAffiliationformationTypePartyAffiliationInfo> PartyAffiliation
        {
            get
            {
                return _partyAffiliation;
            }
            set
            {
                _partyAffiliation = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the PartyAffiliation collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PartyAffiliationSpecified
        {
            get
            {
                return ((this.PartyAffiliation != null) 
                            && (this.PartyAffiliation.Count != 0));
            }
        }
    }
}