// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using Didstopia.PDFSharp.Pdf;
using Didstopia.PDFSharp.Pdf.IO;
using Microsoft.Extensions.Logging;

namespace PdfMerger;

public class Application
{
    private readonly ILogger<Application> _logger;
    private readonly Arguments _arguments;


    public Application(ILogger<Application> logger, Arguments arguments)
    {
        _logger = logger;
        _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
    }

    public void Run()
    {
        MergePdfFiles();
    }

    private void MergePdfFiles()
    {
        try
        {
            var mergedPdf = new PdfDocument();

            var pdfFiles = Directory.GetFiles(_arguments.Input, "*.pdf");

            _logger.LogInformation("Found {pdfFiles.Length} pdf files in {_arguments.Input}", pdfFiles.Length, _arguments.Input);

            foreach (var pdfFile in pdfFiles)
            {
                using var streamReader = new StreamReader(pdfFile);
                var doc = PdfReader.Open(streamReader.BaseStream, PdfDocumentOpenMode.Import);

                _logger.LogTrace("Found {doc.PageCount} pages in pdf file {pdfFile}", doc.PageCount, pdfFile);

                foreach (var docPage in doc.Pages)
                {
                    mergedPdf.AddPage(docPage);
                }
            }

            _logger.LogInformation("The merged Pdf file contains {mergedPdf.PageCount} and will be written to {_arguments.Output}", mergedPdf.PageCount, _arguments.Output);
            mergedPdf.Save(_arguments.Output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while merging the pdf files.");
            throw;
        }
    }


}
