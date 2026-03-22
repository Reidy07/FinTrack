using FinTrack.Core.DTOs.Expenses;
using FinTrack.Core.DTOs.Incomes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Claims;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly HttpClient _httpClient;

        // Inyectamos HttpClient
        public ReportsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FinTrackAPI");
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> ExportPdf()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var startDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            var endDate = DateTime.Now.ToString("yyyy-MM-dd");

            // Obtenemos los datos desde la API
            var expenses = await _httpClient.GetFromJsonAsync<IEnumerable<ExpenseDto>>($"api/expenses/user/{userId}?startDate={startDate}&endDate={endDate}");
            var incomes = await _httpClient.GetFromJsonAsync<IEnumerable<IncomeDto>>($"api/incomes/user/{userId}?startDate={startDate}&endDate={endDate}");

            // Validar nulos por seguridad
            expenses ??= new List<ExpenseDto>();
            incomes ??= new List<IncomeDto>();

            var totalExpenses = expenses.Sum(e => e.Amount);
            var totalIncomes = incomes.Sum(i => i.Amount);
            var balance = totalIncomes - totalExpenses;

            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor("#0f172a");
                    page.DefaultTextStyle(x => x.FontColor("#f8fafc").FontSize(10));

                    // Encabezado
                    page.Header().PaddingBottom(20).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("FINTRACK").FontSize(22).ExtraBold().FontColor("#10b981");
                            col.Item().Text("REPORTE FINANCIERO").FontSize(10).SemiBold().FontColor("#94a3b8");
                        });

                        row.ConstantItem(150)
                           .AlignRight()
                           .Text(DateTime.Now.ToString("dd/MM/yyyy"))
                           .SemiBold();
                    });


                    page.Content().Column(col =>
                    {
                        col.Spacing(20);


                        col.Item().Row(row =>
                        {
                            row.Spacing(15);

                            row.RelativeItem().Element(CardStyle).Column(c =>
                            {
                                c.Item().Text("INGRESOS").FontSize(8).ExtraBold().FontColor("#94a3b8");
                                c.Item().Text($"{totalIncomes:C}").FontSize(16).Bold().FontColor("#10b981");
                            });

                            row.RelativeItem().Element(CardStyle).Column(c =>
                            {
                                c.Item().Text("GASTOS").FontSize(8).ExtraBold().FontColor("#94a3b8");
                                c.Item().Text($"{totalExpenses:C}").FontSize(16).Bold().FontColor("#f43f5e");
                            });

                            row.RelativeItem().Element(CardStyle).Column(c =>
                            {
                                c.Item().Text("BALANCE").FontSize(8).ExtraBold().FontColor("#94a3b8");
                                c.Item().Text($"{balance:C}")
                                    .FontSize(16)
                                    .Bold()
                                    .FontColor(balance >= 0 ? "#10b981" : "#f43f5e");
                            });
                        });

                        // Titulo de tabla 
                        col.Item()
                           .PaddingTop(10)
                           .Text("DETALLE DE TRANSACCIONES")
                           .FontSize(12)
                           .Bold();

                        // Tabla
                        col.Item().Background("#1e293b").Padding(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("FECHA").FontColor("#94a3b8");
                                header.Cell().Element(CellStyle).Text("TIPO").FontColor("#94a3b8");
                                header.Cell().Element(CellStyle).AlignRight().Text("MONTO").FontColor("#94a3b8");

                                static IContainer CellStyle(IContainer container) =>
                                    container.PaddingVertical(5)
                                             .BorderBottom(1)
                                             .BorderColor("#334155");
                            });

                            var allData = incomes
                                .Select(i => new { i.Date, Tipo = "Ingreso", i.Amount, Color = "#10b981" })
                                .Concat(expenses.Select(e => new { e.Date, Tipo = "Gasto", e.Amount, Color = "#f43f5e" }))
                                .OrderByDescending(x => x.Date);

                            foreach (var item in allData)
                            {
                                table.Cell().Element(RowStyle).Text(item.Date.ToString("dd/MM/yyyy"));
                                table.Cell().Element(RowStyle).Text(item.Tipo);
                                table.Cell().Element(RowStyle)
                                    .AlignRight()
                                    .Text($"{item.Amount:C}")
                                    .FontColor(item.Color)
                                    .Bold();

                                static IContainer RowStyle(IContainer container) =>
                                    container.PaddingVertical(8)
                                             .BorderBottom(0.5f)
                                             .BorderColor("#334155");
                            }
                        });
                    });
                    page.Footer().PaddingTop(15).Column(col =>
                    {
                        col.Spacing(5);
                        col.Item().LineHorizontal(1).LineColor("#334155");

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text(text =>
                            {
                                text.Span("FinTrack ").Bold().FontColor("#10b981");
                                text.Span("• Sistema de Finanzas")
                                    .FontColor("#94a3b8")
                                    .FontSize(9);
                            });

                            row.ConstantItem(170).AlignRight().Text(text =>
                            {
                                text.Span("Generado: ")
                                    .FontColor("#475569")
                                    .FontSize(9);

                                text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                                    .FontColor("#94a3b8")
                                    .FontSize(9);
                            });
                        });

                        col.Item().AlignCenter().Text(text =>
                        {
                            text.Span("Página ").FontColor("#475569").FontSize(9);
                            text.CurrentPageNumber().FontColor("#10b981").FontSize(9).Bold();
                        });
                    });
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", "Reporte_FinTrack.pdf");
        }

        private IContainer CardStyle(IContainer container)
        {
            return container
                .Background("#1e293b")
                .Padding(15);
        }
    }
}