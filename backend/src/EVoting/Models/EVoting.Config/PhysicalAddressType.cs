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
    [System.Xml.Serialization.XmlTypeAttribute("physicalAddressType", Namespace="http://www.evoting.ch/xmlns/config/6")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PhysicalAddressType
    {
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("mrMrs", Order=0)]
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
        /// <para xml:lang="en">Maximum length: 20.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
        [System.Xml.Serialization.XmlElementAttribute("title", Order=1)]
        public string Title { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Maximum length: 30.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
        [System.Xml.Serialization.XmlElementAttribute("firstName", Order=2)]
        public string FirstName { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Maximum length: 30.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
        [System.Xml.Serialization.XmlElementAttribute("lastName", Order=3)]
        public string LastName { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Maximum length: 60.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(60)]
        [System.Xml.Serialization.XmlElementAttribute("street", Order=4)]
        public string Street { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Maximum length: 12.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
        [System.Xml.Serialization.XmlElementAttribute("houseNumber", Order=5)]
        public string HouseNumber { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Maximum length: 10.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
        [System.Xml.Serialization.XmlElementAttribute("dwellingNumber", Order=6)]
        public string DwellingNumber { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Maximum length: 15.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(15)]
        [System.Xml.Serialization.XmlElementAttribute("postOfficeBoxText", Order=7)]
        public string PostOfficeBoxText { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Maximum inclusive value: 99999999.</para>
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("postOfficeBoxNumber", Order=8)]
        public uint PostOfficeBoxNumberValue { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Gets or sets a value indicating whether the PostOfficeBoxNumber property is specified.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool PostOfficeBoxNumberValueSpecified { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Maximum inclusive value: 99999999.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<uint> PostOfficeBoxNumber
        {
            get
            {
                if (this.PostOfficeBoxNumberValueSpecified)
                {
                    return this.PostOfficeBoxNumberValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.PostOfficeBoxNumberValue = value.GetValueOrDefault();
                this.PostOfficeBoxNumberValueSpecified = value.HasValue;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Minimum length: 1.</para>
        /// <para xml:lang="en">Maximum length: 15.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MinLengthAttribute(1)]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(15)]
        [System.Xml.Serialization.XmlElementAttribute("zipCode", Order=9)]
        public string ZipCode { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Maximum length: 40.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
        [System.Xml.Serialization.XmlElementAttribute("town", Order=10)]
        public string Town { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Minimum length: 2.</para>
        /// <para xml:lang="en">Maximum length: 2.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MinLengthAttribute(2)]
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
        [System.Xml.Serialization.XmlElementAttribute("country", Order=11)]
        public string Country { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Maximum length: 50.</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("countryNameShort", Order=12)]
        public string CountryNameShort { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<string> _belowTitleLine;
        
        [System.Xml.Serialization.XmlElementAttribute("belowTitleLine", Order=13)]
        public System.Collections.Generic.List<string> BelowTitleLine
        {
            get
            {
                return _belowTitleLine;
            }
            set
            {
                _belowTitleLine = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the BelowTitleLine collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BelowTitleLineSpecified
        {
            get
            {
                return ((this.BelowTitleLine != null) 
                            && (this.BelowTitleLine.Count != 0));
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="PhysicalAddressType" /> class.</para>
        /// </summary>
        public PhysicalAddressType()
        {
            this._belowTitleLine = new System.Collections.Generic.List<string>();
            this._belowNameLine = new System.Collections.Generic.List<string>();
            this._belowStreetLine = new System.Collections.Generic.List<string>();
            this._belowPostOfficeBoxLine = new System.Collections.Generic.List<string>();
            this._belowTownLine = new System.Collections.Generic.List<string>();
            this._belowCountryLine = new System.Collections.Generic.List<string>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<string> _belowNameLine;
        
        [System.Xml.Serialization.XmlElementAttribute("belowNameLine", Order=14)]
        public System.Collections.Generic.List<string> BelowNameLine
        {
            get
            {
                return _belowNameLine;
            }
            set
            {
                _belowNameLine = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the BelowNameLine collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BelowNameLineSpecified
        {
            get
            {
                return ((this.BelowNameLine != null) 
                            && (this.BelowNameLine.Count != 0));
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<string> _belowStreetLine;
        
        [System.Xml.Serialization.XmlElementAttribute("belowStreetLine", Order=15)]
        public System.Collections.Generic.List<string> BelowStreetLine
        {
            get
            {
                return _belowStreetLine;
            }
            set
            {
                _belowStreetLine = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the BelowStreetLine collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BelowStreetLineSpecified
        {
            get
            {
                return ((this.BelowStreetLine != null) 
                            && (this.BelowStreetLine.Count != 0));
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<string> _belowPostOfficeBoxLine;
        
        [System.Xml.Serialization.XmlElementAttribute("belowPostOfficeBoxLine", Order=16)]
        public System.Collections.Generic.List<string> BelowPostOfficeBoxLine
        {
            get
            {
                return _belowPostOfficeBoxLine;
            }
            set
            {
                _belowPostOfficeBoxLine = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the BelowPostOfficeBoxLine collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BelowPostOfficeBoxLineSpecified
        {
            get
            {
                return ((this.BelowPostOfficeBoxLine != null) 
                            && (this.BelowPostOfficeBoxLine.Count != 0));
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<string> _belowTownLine;
        
        [System.Xml.Serialization.XmlElementAttribute("belowTownLine", Order=17)]
        public System.Collections.Generic.List<string> BelowTownLine
        {
            get
            {
                return _belowTownLine;
            }
            set
            {
                _belowTownLine = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the BelowTownLine collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BelowTownLineSpecified
        {
            get
            {
                return ((this.BelowTownLine != null) 
                            && (this.BelowTownLine.Count != 0));
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<string> _belowCountryLine;
        
        [System.Xml.Serialization.XmlElementAttribute("belowCountryLine", Order=18)]
        public System.Collections.Generic.List<string> BelowCountryLine
        {
            get
            {
                return _belowCountryLine;
            }
            set
            {
                _belowCountryLine = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the BelowCountryLine collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BelowCountryLineSpecified
        {
            get
            {
                return ((this.BelowCountryLine != null) 
                            && (this.BelowCountryLine.Count != 0));
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlElementAttribute("frankingArea", Order=19)]
        public FrankingAreaType FrankingAreaValue { get; set; }
        
        /// <summary>
        /// <para xml:lang="en">Gets or sets a value indicating whether the FrankingArea property is specified.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public bool FrankingAreaValueSpecified { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<FrankingAreaType> FrankingArea
        {
            get
            {
                if (this.FrankingAreaValueSpecified)
                {
                    return this.FrankingAreaValue;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.FrankingAreaValue = value.GetValueOrDefault();
                this.FrankingAreaValueSpecified = value.HasValue;
            }
        }
    }
}
