using ERPLite.Services.DTOs.Reports;
using ERPLite.Services.Interfaces.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;

namespace ERPLite.Services.Services.Reports
{
    public class PdfGenerator : IPdfGenerator
    {
        public byte[] GenerateReportPdf<T>(string title, T data)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    page.Header().Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text(title).FontSize(18).Bold().FontColor(Colors.Blue.Darken3);

                            row.ConstantItem(150).Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}")
                                .FontSize(8).Light();
                        });

                        column.Item().PaddingTop(5).PaddingBottom(15).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        if (data is EmployeesReportDto empReport)
                        {
                            BuildEmployeesTable(col, empReport);
                        }
                        else if (data is AttendanceReportDto attReport)
                        {
                            BuildAttendanceTable(col, attReport);
                        }
                        else if (data is InventoryReportDto invReport)
                        {
                            BuildInventoryTable(col, invReport);
                        }
                        else if (data is SalesReportDto salesReport)
                        {
                            BuildSalesTable(col, salesReport);
                        }
                        else if (data is FinancialReportDto finReport)
                        {
                            BuildFinancialSummary(col, finReport);
                        }
                        else
                        {
                            col.Item().Text("No dynamic layout defined for this data model.").Italic();
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }


        private void BuildEmployeesTable(ColumnDescriptor col, EmployeesReportDto report)
        {
            col.Item().PaddingBottom(10).Text($"Total Employees: {report.TotalEmployees}  |  Active: {report.ActiveEmployees}  |  Total Budget: ${report.TotalSalaries:N2}").Bold();

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40);  // ID
                    columns.RelativeColumn(3);   // Full Name
                    columns.RelativeColumn(3);   // Email
                    columns.RelativeColumn(2);   // Salary
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("#").Bold();
                    header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("Full Name").Bold();
                    header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("Email").Bold();
                    header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("Salary").Bold();
                });

                // Rows
                foreach (var emp in report.Employees)
                {
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(emp.Id.ToString());
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(emp.FullName);
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(emp.Email);
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text($"${emp.Salary:N2}");
                }
            });
        }

        private void BuildAttendanceTable(ColumnDescriptor col, AttendanceReportDto report)
        {
            col.Item().PaddingBottom(10).Text($"Period: {report.From:yyyy-MM-dd} to {report.To:yyyy-MM-dd}  |  Total Sheets: {report.TotalRecords}  |  Present: {report.PresentCount}  |  Late: {report.LateCount}").Bold();

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);   // Date
                    columns.RelativeColumn(3);   // Employee Name
                    columns.RelativeColumn(2);   // Status
                    columns.RelativeColumn(2);   // Note/Time
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Indigo.Lighten4).Padding(5).Text("Date").Bold();
                    header.Cell().Background(Colors.Indigo.Lighten4).Padding(5).Text("Employee").Bold();
                    header.Cell().Background(Colors.Indigo.Lighten4).Padding(5).Text("Status").Bold();
                    header.Cell().Background(Colors.Indigo.Lighten4).Padding(5).Text("Remarks").Bold();
                });

                foreach (var rec in report.Records)
                {
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(rec.Date.ToString("yyyy-MM-dd"));
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(rec.EmployeeName ?? $"ID: {rec.EmployeeId}");

                    var statusCell = table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);
                    statusCell.Text(rec.Status.ToString());

                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text("-");
                }
            });
        }

        private void BuildInventoryTable(ColumnDescriptor col, InventoryReportDto report)
        {
            col.Item().PaddingBottom(10).Text($"Total Products: {report.TotalProducts}  |  Low Stock Alert: {report.LowStockProducts}  |  Total Value: ${report.InventoryValue:N2}").Bold();

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40);
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("ID").Bold();
                    header.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Product Name").Bold();
                    header.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Price").Bold();
                    header.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Stock").Bold();
                });

                foreach (var prod in report.Products)
                {
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(prod.Id.ToString());
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(prod.Name);
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text($"${prod.Price:N2}");

                    var stockCell = table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);
                    if (prod.QuantityInStock <= prod.MinStockLevel)
                        stockCell.Text($"{prod.QuantityInStock} (Low)").Bold().FontColor(Colors.Red.Medium);
                    else
                        stockCell.Text(prod.QuantityInStock.ToString());
                }
            });
        }

        private void BuildSalesTable(ColumnDescriptor col, SalesReportDto report)
        {
            col.Item().PaddingBottom(10).Text($"Period: {report.From:yyyy-MM-dd} to {report.To:yyyy-MM-dd}  |  Orders: {report.TotalOrders}  |  Total Income: ${report.TotalSales:N2}").Bold();

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(50);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Orange.Lighten4).Padding(5).Text("Order #").Bold();
                    header.Cell().Background(Colors.Orange.Lighten4).Padding(5).Text("Date").Bold();
                    header.Cell().Background(Colors.Orange.Lighten4).Padding(5).Text("Customer").Bold();
                    header.Cell().Background(Colors.Orange.Lighten4).Padding(5).Text("Total").Bold();
                });

                foreach (var order in report.Orders)
                {
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(order.Id.ToString());
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(order.OrderDate.ToString("yyyy-MM-dd HH:mm"));
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(order.CustomerName ?? "Walking Customer");
                    table.Cell().Padding(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text($"${order.TotalPrice:N2}");
                }
            });
        }

        private void BuildFinancialSummary(ColumnDescriptor col, FinancialReportDto report)
        {
            col.Item().PaddingBottom(15).Text("Executive Financial Balance Summary").FontSize(14).Bold();

            col.Item().Background(Colors.Grey.Lighten4).Padding(15).Column(innerCol =>
            {
                innerCol.Item().Row(r => { r.RelativeItem().Text("Total System Revenue:"); r.ConstantItem(100).Text($"${report.TotalRevenue:N2}").Bold(); });

                innerCol.Item().PaddingVertical(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);

                innerCol.Item().Row(r => { r.RelativeItem().Text("Total Collected (Paid) Cash:"); r.ConstantItem(100).Text($"${report.TotalPaid:N2}").Bold().FontColor(Colors.Green.Darken2); });
                innerCol.Item().PaddingVertical(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);

                innerCol.Item().Row(r => { r.RelativeItem().Text("Outstanding Market Balance:"); r.ConstantItem(100).Text($"${report.OutstandingBalance:N2}").Bold().FontColor(Colors.Red.Darken2); });
                innerCol.Item().PaddingVertical(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);

                innerCol.Item().Row(r => { r.RelativeItem().Text("Settled Paid Invoices:"); r.ConstantItem(100).Text($"{report.PaidOrders} Orders"); });
                innerCol.Item().PaddingVertical(5).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);

                innerCol.Item().Row(r => { r.RelativeItem().Text("Unpaid Pending Invoices:"); r.ConstantItem(100).Text($"{report.UnpaidOrders} Orders"); });
            });
        }
    }
}