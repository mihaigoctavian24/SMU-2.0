using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Infrastructure.Services;

public class PdfGenerationService : IPdfGenerationService
{
    public PdfGenerationService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateRequestDocument(RequestResponse request)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text("University Management System")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Item().Text($"Document Type: {request.Type}").FontSize(16).SemiBold();
                        x.Item().Text($"Date: {DateTime.Now.ToShortDateString()}");
                        x.Item().PaddingVertical(1, Unit.Centimetre).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                        x.Item().Text($"This is to certify that student {request.StudentName} has requested a {request.Type}.");
                        x.Item().PaddingTop(0.5f, Unit.Centimetre).Text($"Details: {request.Content}");
                        
                        x.Item().PaddingTop(2, Unit.Centimetre).Text("Approved by Secretariat");
                        x.Item().Text("Signature: ______________________");
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
            });
        })
        .GeneratePdf();
    }
}
