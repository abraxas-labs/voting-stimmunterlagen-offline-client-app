//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// This code was generated by XmlSchemaClassGenerator version 2.1.963.0
namespace EVoting.Print
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.1.963.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute("contestType", Namespace="http://www.evoting.ch/xmlns/print/1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ContestType
    {
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("contestIdentification", Order=0)]
        public string ContestIdentification { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<VotingCardType> _votingCard;
        
        [System.Xml.Serialization.XmlElementAttribute("votingCard", Order=1)]
        public System.Collections.Generic.List<VotingCardType> VotingCard
        {
            get
            {
                return _votingCard;
            }
            set
            {
                _votingCard = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the VotingCard collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool VotingCardSpecified
        {
            get
            {
                return ((this.VotingCard != null) 
                            && (this.VotingCard.Count != 0));
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="ContestType" /> class.</para>
        /// </summary>
        public ContestType()
        {
            this._votingCard = new System.Collections.Generic.List<VotingCardType>();
        }
    }
}
