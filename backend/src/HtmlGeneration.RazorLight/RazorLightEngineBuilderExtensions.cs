﻿// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace RazorLight;

using System;
using HtmlGeneration.RazorLight.Razor;

public static class RazorLightEngineBuilderExtensions
{
    public static RazorLightEngineBuilder UseStreamProject(this RazorLightEngineBuilder builder, StreamProject project)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));
        if (project == null)
            throw new ArgumentNullException(nameof(project));

        return builder.UseProject(project);
    }
}
