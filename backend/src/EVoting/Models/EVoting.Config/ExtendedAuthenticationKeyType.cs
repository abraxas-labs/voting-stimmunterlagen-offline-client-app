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
    [System.Xml.Serialization.XmlTypeAttribute("extendedAuthenticationKeyType", Namespace="http://www.evoting.ch/xmlns/config/6")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ExtendedAuthenticationKeyType
    {
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("name", Order=0)]
        public string Name { get; set; }
        
        [System.ComponentModel.DataAnnotations.RequiredAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("value", Order=1)]
        public string Value { get; set; }
    }
}
