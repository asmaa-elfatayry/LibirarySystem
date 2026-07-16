using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LibrarySystem.web.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class ReportController(
        IBookService _bookService,
        ILoanService _loanService) : Controller
    {
        public IActionResult Index() => View();

        // ===== الكتب - Excel =====
        public async Task<IActionResult> ExportBooksExcel()
        {
            var books = await _bookService.GetAllAsync();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("الكتب");

            sheet.Cell(1, 1).Value = "العنوان";
            sheet.Cell(1, 2).Value = "المؤلف";
            sheet.Cell(1, 3).Value = "التصنيف";
            sheet.Cell(1, 4).Value = "الناشر";
            sheet.Cell(1, 5).Value = "ISBN";
            sheet.Range(1, 1, 1, 5).Style.Font.Bold = true;
            sheet.Range(1, 1, 1, 5).Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var b in books)
            {
                sheet.Cell(row, 1).Value = b.Title;
                sheet.Cell(row, 2).Value = b.AuthorName;
                sheet.Cell(row, 3).Value = b.CategoryName;
                sheet.Cell(row, 4).Value = b.PublisherName;
                sheet.Cell(row, 5).Value = b.ISBN;
                row++;
            }

            sheet.Columns().AdjustToContents();
            sheet.RightToLeft = true; // ترتيب الأعمدة يناسب العربي

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Books_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        // ===== الكتب - PDF =====
        public async Task<IActionResult> ExportBooksPdf()
        {
            var books = await _bookService.GetAllAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.ContentFromRightToLeft();
                    page.DefaultTextStyle(x => x.FontFamily("Cairo").FontSize(11));

                    // ===== Header =====
                    page.Header().Column(col =>
                    {
                        col.Item().Text("تقرير الكتب المسجّلة")
                            .FontSize(20).Bold().FontColor(Colors.Blue.Darken2);

                        col.Item().PaddingTop(3).Text($"تاريخ الإصدار: {DateTime.Now:dd/MM/yyyy}")
                            .FontSize(10).FontColor(Colors.Grey.Darken1);

                        col.Item().PaddingTop(3).Text($"إجمالي عدد الكتب: {books.Count}")
                            .FontSize(10).FontColor(Colors.Grey.Darken1);

                        col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    // ===== Table =====
                    page.Content().PaddingTop(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);   // العنوان
                            columns.RelativeColumn(2);   // المؤلف
                            columns.RelativeColumn(2);   // التصنيف
                            columns.RelativeColumn(2);   // الناشر
                            columns.RelativeColumn(2);   // ISBN
                        });

                        // Header Row
                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("العنوان");
                            header.Cell().Element(HeaderCellStyle).Text("المؤلف");
                            header.Cell().Element(HeaderCellStyle).Text("التصنيف");
                            header.Cell().Element(HeaderCellStyle).Text("الناشر");
                            header.Cell().Element(HeaderCellStyle).Text("ISBN");

                            static IContainer HeaderCellStyle(IContainer c) => c
                                .Background(Colors.Blue.Darken2)
                                .Padding(6)
                                .DefaultTextStyle(x => x.FontColor(Colors.White).Bold());
                        });

                        // Data Rows
                        int index = 0;
                        foreach (var b in books)
                        {
                            var bg = index % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                            table.Cell().Element(c => DataCellStyle(c, bg)).Text(b.Title ?? "—");
                            table.Cell().Element(c => DataCellStyle(c, bg)).Text(b.AuthorName ?? "—");
                            table.Cell().Element(c => DataCellStyle(c, bg)).Text(b.CategoryName ?? "—");
                            table.Cell().Element(c => DataCellStyle(c, bg)).Text(b.PublisherName ?? "—");
                            table.Cell().Element(c => DataCellStyle(c, bg)).Text(b.ISBN ?? "—");

                            index++;
                        }

                        static IContainer DataCellStyle(IContainer c, string bg) => c
                            .Background(bg)
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(6);
                    });

                    // ===== Footer =====
                    page.Footer().AlignCenter().PaddingTop(10).Text(x =>
                    {
                        x.Span("صفحة ").FontSize(9).FontColor(Colors.Grey.Darken1);
                        x.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Darken1);
                        x.Span(" من ").FontSize(9).FontColor(Colors.Grey.Darken1);
                        x.TotalPages().FontSize(9).FontColor(Colors.Grey.Darken1);
                    });
                });
            });

            var pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Books_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // ===== الاستعارات - Excel =====
        public async Task<IActionResult> ExportLoansExcel()
        {
            var loans = await _loanService.GetAllAsync();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("الاستعارات");

            sheet.Cell(1, 1).Value = "الكتاب";
            sheet.Cell(1, 2).Value = "العضو";
            sheet.Cell(1, 3).Value = "تاريخ الاستعارة";
            sheet.Cell(1, 4).Value = "موعد الإرجاع";
            sheet.Cell(1, 5).Value = "تاريخ الإرجاع الفعلي";
            sheet.Cell(1, 6).Value = "الحالة";
            sheet.Range(1, 1, 1, 6).Style.Font.Bold = true;
            sheet.Range(1, 1, 1, 6).Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var l in loans)
            {
                sheet.Cell(row, 1).Value = l.BookTitle;
                sheet.Cell(row, 2).Value = l.MemberName;
                sheet.Cell(row, 3).Value = l.BorrowDate.ToString("dd/MM/yyyy");
                sheet.Cell(row, 4).Value = l.DueDate.ToString("dd/MM/yyyy");
                sheet.Cell(row, 5).Value = l.ReturnDate?.ToString("dd/MM/yyyy") ?? "—";
                sheet.Cell(row, 6).Value = l.Status;
                row++;
            }

            sheet.Columns().AdjustToContents();
            sheet.RightToLeft = true;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Loans_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}