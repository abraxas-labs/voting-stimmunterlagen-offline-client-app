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
    [System.Xml.Serialization.XmlTypeAttribute("extendedAuthenticationKeysType", Namespace="http://www.evoting.ch/xmlns/config/5")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ExtendedAuthenticationKeysType
    {
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private System.Collections.Generic.List<ExtendedAuthenticationKeyType> _extendedAuthenticationKey;
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("extendedAuthenticationKey", Order=0)]
        public System.Collections.Generic.List<ExtendedAuthenticationKeyType> ExtendedAuthenticationKey
        {
            get
            {
                return _extendedAuthenticationKey;
            }
            set
            {
                _extendedAuthenticationKey = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="ExtendedAuthenticationKeysType" /> class.</para>
        /// </summary>
        public ExtendedAuthenticationKeysType()
        {
            this._extendedAuthenticationKey = new System.Collections.Generic.List<ExtendedAuthenticationKeyType>();
        }
    }
}
