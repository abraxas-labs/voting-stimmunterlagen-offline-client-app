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
    [System.Xml.Serialization.XmlTypeAttribute("electionInformationType", Namespace="http://www.evoting.ch/xmlns/config/6")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ElectionInformationType
    {
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("election", Order=0)]
        public ElectionType Election { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<CandidateType> _candidate;
        
        [System.Xml.Serialization.XmlElementAttribute("candidate", Order=1)]
        public System.Collections.Generic.List<CandidateType> Candidate
        {
            get
            {
                return _candidate;
            }
            set
            {
                _candidate = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Candidate collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CandidateSpecified
        {
            get
            {
                return ((this.Candidate != null) 
                            && (this.Candidate.Count != 0));
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="ElectionInformationType" /> class.</para>
        /// </summary>
        public ElectionInformationType()
        {
            this._candidate = new System.Collections.Generic.List<CandidateType>();
            this._list = new System.Collections.Generic.List<ListType>();
            this._listUnion = new System.Collections.Generic.List<ListUnionType>();
            this._writeInCandidate = new System.Collections.Generic.List<WriteInCandidateType>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<ListType> _list;
        
        [System.Xml.Serialization.XmlElementAttribute("list", Order=2)]
        public System.Collections.Generic.List<ListType> List
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the List collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ListSpecified
        {
            get
            {
                return ((this.List != null) 
                            && (this.List.Count != 0));
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<ListUnionType> _listUnion;
        
        [System.Xml.Serialization.XmlElementAttribute("listUnion", Order=3)]
        public System.Collections.Generic.List<ListUnionType> ListUnion
        {
            get
            {
                return _listUnion;
            }
            set
            {
                _listUnion = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the ListUnion collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ListUnionSpecified
        {
            get
            {
                return ((this.ListUnion != null) 
                            && (this.ListUnion.Count != 0));
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<WriteInCandidateType> _writeInCandidate;
        
        [System.Xml.Serialization.XmlElementAttribute("writeInCandidate", Order=4)]
        public System.Collections.Generic.List<WriteInCandidateType> WriteInCandidate
        {
            get
            {
                return _writeInCandidate;
            }
            set
            {
                _writeInCandidate = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the WriteInCandidate collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool WriteInCandidateSpecified
        {
            get
            {
                return ((this.WriteInCandidate != null) 
                            && (this.WriteInCandidate.Count != 0));
            }
        }
    }
}
