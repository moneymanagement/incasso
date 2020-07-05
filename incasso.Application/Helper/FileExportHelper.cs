using Incasso.MultiTenancy.Dto;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace incasso.Helper
{
    //public static partial class FileExportHelper
    //{
    //    private static ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName, DataTable table, string tableStartFrom = "A2")
    //    {
    //        if (p.Workbook.Worksheets.Any(x => x.Name == sheetName))
    //        {
    //            throw new Exception("Work sheet (" + sheetName + ") already exists.");
    //        }
    //        p.Workbook.Worksheets.Add(sheetName);
    //        ExcelWorksheet ws = p.Workbook.Worksheets[sheetName];
    //        ws.Name = sheetName; //Setting Sheet's name
    //        ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
    //        ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
    //        ws.Cells.Style.WrapText = true;
    //        ws.Cells[tableStartFrom].LoadFromDataTable(table, true);
    //        return ws;
    //    }

    //    public static FileStreamResult ExportToExcel (DataTable data, List<ExportParameterModel> parameters, string fileName  )
    //    {
    //        try
    //        {
    //            using (ExcelPackage pack = new ExcelPackage())
    //            {
    //                DataTable table = new DataTable();
    //                table = data;
    //                ExcelWorksheet ws = CreateSheet(pack, fileName, table );
    //              ws.ApplyExcelStyling(data.Rows.Count>0? table.Rows.Count:1, parameters);

    //                var ms = new System.IO.MemoryStream();
    //                pack.SaveAs(ms);
    //                ms.Seek(0, 0);
    //                return  new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = fileName };
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //        }
    //        return null;
    //    }
    //    public async static Task<FileStreamResult> Export(List<InvoiceDto> invoices)
    //    {
    //        var dt = new DataTable();
    //        List<ExportParameterModel> parameters = new List<ExportParameterModel>();
    //        parameters.Add(new ExportParameterModel() { ColumnWidth = 250, DataField = "InvoiceNo", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = "Factuurnr." });
    //        parameters.Add(new ExportParameterModel() { ColumnWidth = 250, DataField = "InvoiceDate", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = "Factuurdatum" });
    //        parameters.Add(new ExportParameterModel() { ColumnWidth = 250, DataField = "ExpiredDate", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = "Vervaldatum" });
    //        parameters.Add(new ExportParameterModel() { ColumnWidth = 250, DataField = "Amount", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = "Fact.bedrag" });
    //        parameters.Add(new ExportParameterModel() { ColumnWidth = 250, DataField = "Open", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = "Bedrag open" });
    //        parameters.Add(new ExportParameterModel() { ColumnWidth = 250, DataField = "AdminCosts", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = "Rente" });
    //        parameters.Add(new ExportParameterModel() { ColumnWidth = 250, DataField = "Open", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = "Incassokosten" });
    //        parameters.Add(new ExportParameterModel() { ColumnWidth = 250, DataField = "Paidmm", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = "Betaald MM" });
    //        parameters.Add(new ExportParameterModel() { ColumnWidth = 250, DataField = "PaidClient", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = "Betaald cliënt" });
    //        parameters.Add(new ExportParameterModel() { ColumnWidth = 250, DataField = "TotalOpenInc", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = " Totaal betaald" });
    //        parameters.Add(new ExportParameterModel() { ColumnWidth = 250, DataField = "Status", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = "Status" });
    //        parameters.Add(new ExportParameterModel() { ColumnWidth = 250, DataField = "Notes", TextHorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left, HeaderText = "Notities" });

     

    //        dt.Columns.Add(new DataColumn() { ColumnName = "InvoiceNo" });
    //        dt.Columns.Add(new DataColumn() { ColumnName = "InvoiceDate" });
    //        dt.Columns.Add(new DataColumn() { ColumnName = "ExpiredDate" });
    //        dt.Columns.Add(new DataColumn() { ColumnName = "Amount" });
    //        dt.Columns.Add(new DataColumn() { ColumnName = "Open" });
    //        dt.Columns.Add(new DataColumn() { ColumnName = "AdminCosts" });
    //        dt.Columns.Add(new DataColumn() { ColumnName = "Paidmm" });
    //        dt.Columns.Add(new DataColumn() { ColumnName = "PaidClient" });
    //        dt.Columns.Add(new DataColumn() { ColumnName = "TotalOpenInc" });
    //        dt.Columns.Add(new DataColumn() { ColumnName = "Status" });
    //        dt.Columns.Add(new DataColumn() { ColumnName = "Notes" });

    //        foreach (var invoice in invoices)
    //        {
    //            DataRow dr = dt.NewRow();
    //            dr["InvoiceNo"] = invoice.InvoiceNo;
    //            dr["InvoiceDate"] = invoice.InvoiceDate;
    //            dr["ExpiredDate"] = invoice.ExpiredDate;
    //            dr["Amount"] = invoice.Amount;
    //            dr["Open"] = invoice.Open;
    //            dr["AdminCosts"] = invoice.AdminCosts;
    //            dr["Paidmm"] = invoice.Paidmm;
    //            dr["PaidClient"] = invoice.PaidClient;
    //            dr["TotalOpenInc"] = invoice.TotalOpenInc;
    //            dr["Status"] = invoice.Status;
    //            dr["Notes"] = invoice.Status;
    //            dt.Rows.Add(dr);
    //            //rowStyling.Add(new ExportParameterRowStyle()
    //            //{
    //            //    BorderThickness = invoice.IsTotalRow ? ExcelBorderStyle.Thick : ExcelBorderStyle.Thin,
    //            //    BorderColor = "#000000",
    //            //    Bold = invoice.IsTotalRow,
    //            //    RowIndex = dt.Rows.Count - 1
    //            //});
    //        }

    //        return ExportToExcel(dt,  parameters, DateTime.Now.Millisecond.ToString()+ ".XLSX");
    //    }
    //}


    //public class ExportParameterModel
    //{
    //    public string DataField { get; set; }
    //    public string HeaderText { get; set; }
    //    public string FooterText { get; set; }
    //    public string BackgroundColor { get; set; } 
    //    public int ColumnWidth { get; set; }
    //    public System.Web.UI.WebControls.HorizontalAlign TextHorizontalAlign { get; set; }
    //    public bool IsCheckboxField { get; set; }
    //    public bool WrapText { get; set; }
    //    public bool AutoAdjustWidth { get; set; }
    //    public bool ParseValue { get; set; } 
    //}
}
