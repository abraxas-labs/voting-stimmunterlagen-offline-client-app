using System;

namespace HtmlGeneration.RazorLight.Exceptions;

public class RazorLightRenderException : Exception
{
    public RazorLightRenderException(Exception ex) : base("cshtml Template Error: " + ex.Message)
    {
    }
}
