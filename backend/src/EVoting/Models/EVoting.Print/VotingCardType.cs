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
    [System.Xml.Serialization.XmlTypeAttribute("votingCardType", Namespace="http://www.evoting.ch/xmlns/print/1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class VotingCardType
    {
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("voterIdentification", Order=0)]
        public string VoterIdentification { get; set; }
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("votingCardId", Order=1)]
        public string VotingCardId { get; set; }
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("startVotingKey", Order=2)]
        public string StartVotingKey { get; set; }
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("ballotCastingKey", Order=3)]
        public string BallotCastingKey { get; set; }
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("voteCastReturnCode", Order=4)]
        public string VoteCastReturnCode { get; set; }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<ElectionType> _election;
        
        [System.Xml.Serialization.XmlElementAttribute("election", Order=5)]
        public System.Collections.Generic.List<ElectionType> Election
        {
            get
            {
                return _election;
            }
            set
            {
                _election = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Election collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ElectionSpecified
        {
            get
            {
                return ((this.Election != null) 
                            && (this.Election.Count != 0));
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="VotingCardType" /> class.</para>
        /// </summary>
        public VotingCardType()
        {
            this._election = new System.Collections.Generic.List<ElectionType>();
            this._vote = new System.Collections.Generic.List<VoteType>();
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<VoteType> _vote;
        
        [System.Xml.Serialization.XmlElementAttribute("vote", Order=6)]
        public System.Collections.Generic.List<VoteType> Vote
        {
            get
            {
                return _vote;
            }
            set
            {
                _vote = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Vote collection is empty.</para>
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool VoteSpecified
        {
            get
            {
                return ((this.Vote != null) 
                            && (this.Vote.Count != 0));
            }
        }
    }
}
