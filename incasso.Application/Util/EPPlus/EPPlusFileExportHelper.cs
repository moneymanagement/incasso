using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using System.Text.RegularExpressions;
using Abp.Extensions;
using System.Drawing;
using System.Globalization;
using System.Linq.Dynamic;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Utilities;
using ParityAccess.Application.Utils.EPPlus;
using OfficeOpenXml.Table.PivotTable;
using System.Web.Mvc;
using Incasso.MultiTenancy.Dto;
using OfficeOpenXml.Drawing;
using incasso.Debtors;
using incasso.Helper;

namespace ParityAccess.Utils.EPPlus
{


    public static partial class FileExportHelper
    {
        private const string PrintHeaderTemplate = "&\"Calibri,Bold\"&12Valid Claims\n%%ReportTitle%%&\"Calibri,Regular\"&11\n&\"Calibri,Italic\"&9Search Criteria: %%SearchCriteria%%";
        private const string PrintFooterTemplate = "Page %%PageNumber%% of %%NumberOfPages%%\n%%DateTime%%";
        #region Private Methods
        private static ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName, DataTable table, string tableStartFrom = "A2")
        {
            if (p.Workbook.Worksheets.Any(x => x.Name == sheetName))
            {
                throw new Exception("Work sheet (" + sheetName + ") already exists.");
            }
            p.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet ws = p.Workbook.Worksheets[sheetName];
            ws.Name = sheetName; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
            ws.Cells.Style.WrapText = true;
            //ws.Cells.IsRichText = true;
            ws.Cells[tableStartFrom].LoadFromDataTable(table, true);
            return ws;
        }
         
    private static ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName, DataTable table, int fromCol, int fromRow)
        {
            if (p.Workbook.Worksheets.Any(x => x.Name == sheetName))
            {
                throw new Exception("Work sheet (" + sheetName + ") already exists.");
            }
            p.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet ws = p.Workbook.Worksheets[sheetName];
            ws.Name = sheetName; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
            ws.Cells.Style.WrapText = true;
            ws.PrinterSettings.HeaderMargin = 0.5M;//Default Header Margin
            ws.PrinterSettings.FooterMargin = 0.5M;//Default Footer Margin
            ws.PrinterSettings.Orientation = eOrientation.Landscape;//Default Print Orientation            
            ws.PrinterSettings.FitToPage = true;
            ws.PrinterSettings.FitToWidth = 1;
            ws.PrinterSettings.FitToHeight = 0;
            ws.PrinterSettings.VerticalCentered = true;//Default Vertical Allginment
            LoadFromDataTable(table, fromCol, fromRow, ws);
            return ws;
        }
        private static ExcelWorksheet LoadSheet(ExcelPackage p, string sheetName, DataTable table, string tableStartFrom = "A2")
        {
            if (!p.Workbook.Worksheets.Any(x => x.Name == sheetName))
            {
                throw new Exception("Work sheet (" + sheetName + ") doesn't exists.");
            }
            ExcelWorksheet ws = p.Workbook.Worksheets[sheetName];
            ws.Cells[tableStartFrom].LoadFromDataTable(table, true);
            return ws;
        }
        private static void CreateHeader(this ExcelWorksheet ws, int totalColumns, string headerText)
        {
            ws.Cells[1, 1].Value = headerText;
            ws.Cells[1, 1, 1, totalColumns].Merge = true;
            ws.Cells[1, 1, 1, totalColumns].Style.Font.Bold = true;
            ws.Cells[1, 1, 1, totalColumns].Style.Font.Size = 14;
            ws.Cells[1, 1, 1, totalColumns].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1, 1, totalColumns].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
        }
        private static void CreateRepeatingHeader(this ExcelWorksheet ws, int totalColumns, int startRow, int startCol, List<ExportParameterGroupedHeaderModel> headersText)
        {
            int colNum = startCol;
            foreach (ExportParameterGroupedHeaderModel hTxt in headersText)
            {
                ws.Cells[startRow, colNum].Value = hTxt.HeaderText;
                ws.Cells[startRow, colNum, startRow, (colNum + hTxt.ColSpan) - 1].Merge = true;
                ws.Cells[startRow, colNum, startRow, (colNum + hTxt.ColSpan) - 1].Style.Font.Bold = hTxt.IsBold;
                ws.Cells[startRow, colNum, startRow, (colNum + hTxt.ColSpan) - 1].Style.Font.Size = 12;
                ws.Cells[startRow, colNum, startRow, (colNum + hTxt.ColSpan) - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[startRow, colNum, startRow, (colNum + hTxt.ColSpan) - 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                if (hTxt.CustomStyle != null)
                {
                    ws.Cells[startRow, colNum, startRow, (colNum + hTxt.ColSpan) - 1].Style.Border.Top.Style = hTxt.CustomStyle.BorderTopThickness;
                    ws.Cells[startRow, colNum, startRow, (colNum + hTxt.ColSpan) - 1].Style.Border.Bottom.Style = hTxt.CustomStyle.BorderBottomThickness;
                    ws.Cells[startRow, colNum, startRow, (colNum + hTxt.ColSpan) - 1].Style.Border.Right.Style = hTxt.CustomStyle.BorderRightThickness;
                    ws.Cells[startRow, colNum, startRow, (colNum + hTxt.ColSpan) - 1].Style.Border.Left.Style = hTxt.CustomStyle.BorderLeftThickness;

                    ws.Cells[startRow, colNum, startRow, (colNum + hTxt.ColSpan) - 1].Style.Fill.PatternType = hTxt.CustomStyle.PatternType;
                    ws.Cells[startRow, colNum, startRow, (colNum + hTxt.ColSpan) - 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(hTxt.CustomStyle.BackgroundColor));
                }
                colNum += hTxt.ColSpan;
            }
        }
        private static void ApplyExcelStyling(this ExcelWorksheet ws, int totalRecords, List<ExportParameterModel> parameters, List<ExportParameterRowStyle> rowStylingParameters, bool hasFooter, int startRow = 2, bool applyBordersOnSheet = false)
        {
            ws.Cells[startRow, 1, startRow, parameters.Count()].Style.Border.BorderAround(ExcelBorderStyle.Thick, System.Drawing.Color.Black);
            for (int i = 0; i < parameters.Count(); i++)
            {
                ws.Cells[startRow, i + 1].Value = parameters[i].HeaderText;
                ws.Cells[startRow, i + 1].Style.Font.Bold = true;
                ws.Cells[startRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[startRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#E7E6E6"));
                ws.Cells[startRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                FormatColumn(ws.Column(i + 1), parameters[i]);
                FormatCellRange(ws.Cells[startRow + 1, i + 1, totalRecords + startRow, i + 1], parameters[i]);
                ws.Cells[startRow + 1, i + 1, totalRecords + startRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                if (hasFooter)
                {
                    ws.Cells[startRow + totalRecords, 1, startRow + totalRecords, parameters.Count].Style.Font.Bold = true;
                    ws.Cells[startRow + totalRecords, 1, startRow + totalRecords, parameters.Count].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                }
                ws.Cells[startRow, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                if (parameters[i].CustomStyle != null)
                { 
                    ws.Cells[startRow, i + 1].Style.Border.Top.Style = parameters[i].CustomStyle.BorderTopThickness;
                    ws.Cells[startRow, i + 1].Style.Border.Bottom.Style = parameters[i].CustomStyle.BorderBottomThickness;
                    ws.Cells[startRow, i + 1].Style.Border.Right.Style = parameters[i].CustomStyle.BorderRightThickness;
                    ws.Cells[startRow, i + 1].Style.Border.Left.Style = parameters[i].CustomStyle.BorderLeftThickness;
                     
                    ws.Cells[startRow, i + 1].Style.Fill.PatternType = parameters[i].CustomStyle.PatternType;
                    ws.Cells[startRow, i + 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(parameters[i].CustomStyle.BackgroundColor));}
            }
             
            if(applyBordersOnSheet)
            {

                ws.Cells[startRow + 1,1,totalRecords + startRow,parameters.Count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[startRow + 1,1,totalRecords + startRow,parameters.Count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells[startRow + 1,1,totalRecords + startRow,parameters.Count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells[startRow + 1,1,totalRecords + startRow,parameters.Count].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells[startRow + 1,1,totalRecords + startRow,parameters.Count].Style.Border.BorderAround(ExcelBorderStyle.Thick, System.Drawing.Color.Black);
            }
            if (rowStylingParameters != null)
            {
                for(int c = 0; c < rowStylingParameters.Count; c++)
                {
                    var rowStyle = rowStylingParameters[c];
                    if (rowStyle.RowSpanParameter != null && rowStyle.RowSpanParameter.Count > 0)
                    {
                        for (int k = 0; k < rowStyle.RowSpanParameter.Count; k++)
                        {
                            ws.Cells[rowStyle.RowIndex, rowStyle.RowSpanParameter[k].ColumnIndex, rowStyle.RowIndex + rowStyle.RowSpanParameter[k].RowSpan, rowStyle.RowSpanParameter[k].ColumnIndex].Merge = true;
                            ws.Cells[rowStyle.RowIndex, rowStyle.RowSpanParameter[k].ColumnIndex, rowStyle.RowIndex + rowStyle.RowSpanParameter[k].RowSpan, rowStyle.RowSpanParameter[k].ColumnIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }
                    }

                    ws.Cells[rowStyle.RowIndex + startRow + 1, 1, rowStyle.RowIndex + startRow + 1, parameters.Count].Style.Font.Bold = rowStyle.Bold;
                    if (rowStyle.FontSize > 0)
                        ws.Cells[rowStyle.RowIndex + startRow + 1, 1, rowStyle.RowIndex + startRow + 1, parameters.Count].Style.Font.Size = rowStyle.FontSize;
                    if (rowStyle.MergeRow)
                        ws.Cells[rowStyle.RowIndex + startRow + 1, 1, rowStyle.RowIndex + startRow + 1, parameters.Count].Merge = true;
                    if (rowStyle.TextHorizontalAlign != System.Web.UI.WebControls.HorizontalAlign.NotSet)
                        ws.Cells[rowStyle.RowIndex + startRow + 1, 1, rowStyle.RowIndex + startRow + 1, parameters.Count].Style.HorizontalAlignment = (ExcelHorizontalAlignment)(Enum.Parse(typeof(ExcelHorizontalAlignment), rowStyle.TextHorizontalAlign.ToString())); ;
                    if (!string.IsNullOrEmpty(rowStyle.BorderColor))
                        ws.Cells[rowStyle.RowIndex + startRow + 1, 1, rowStyle.RowIndex + startRow + 1, parameters.Count].Style.Border.BorderAround(rowStyle.BorderThickness, System.Drawing.ColorTranslator.FromHtml(rowStyle.BorderColor));
                    if (!string.IsNullOrEmpty(rowStyle.BackgroundColor))
                    {
                        ws.Cells[rowStyle.RowIndex + startRow + 1, 1, rowStyle.RowIndex + startRow + 1, parameters.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[rowStyle.RowIndex + startRow + 1, 1, rowStyle.RowIndex + startRow + 1, parameters.Count].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(rowStyle.BackgroundColor));
                    }
                    if (rowStyle.CustomColumnFormat != null && rowStyle.CustomColumnFormat.Count > 0)
                    {
                        for (int j = 0; j < rowStyle.CustomColumnFormat.Count; j++)
                        {
                            FormatExcelRange(ws.Cells[rowStyle.RowIndex + startRow + 1, rowStyle.CustomColumnFormat[j].ColumnIndex], rowStyle.CustomColumnFormat[j].Styling);
                        }
                    }
                    if(rowStyle.CustomColumnStyle!=null && rowStyle.CustomColumnStyle.Count > 0)
                    {
                        for (int j = 0; j < rowStyle.CustomColumnStyle.Count; j++)
                        {
                            ws.Cells[rowStyle.CustomColumnStyle[j].RowIndex, rowStyle.CustomColumnStyle[j].ColumnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[rowStyle.CustomColumnStyle[j].RowIndex, rowStyle.CustomColumnStyle[j].ColumnIndex].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(rowStyle.CustomColumnStyle[j].BackgroundColor));
                            ws.Cells[rowStyle.CustomColumnStyle[j].RowIndex, rowStyle.CustomColumnStyle[j].ColumnIndex].Style.Border.Top.Style   = rowStyle.CustomColumnStyle[j].BorderTopThickness;
                            ws.Cells[rowStyle.CustomColumnStyle[j].RowIndex, rowStyle.CustomColumnStyle[j].ColumnIndex].Style.Border.Bottom.Style = rowStyle.CustomColumnStyle[j].BorderBottomThickness;
                            ws.Cells[rowStyle.CustomColumnStyle[j].RowIndex, rowStyle.CustomColumnStyle[j].ColumnIndex].Style.Border.Left.Style = rowStyle.CustomColumnStyle[j].BorderLeftThickness;
                            ws.Cells[rowStyle.CustomColumnStyle[j].RowIndex, rowStyle.CustomColumnStyle[j].ColumnIndex].Style.Border.Right.Style = rowStyle.CustomColumnStyle[j].BorderRightThickness;
                        }
                    }
                }
            }
        }
        private static void ApplyExcelStylingForNestedTable(this ExcelWorksheet ws, DataTable table, int totalRecords, List<ExportParameterModel> parameters, List<ExportParameterModel> nestedParameters, List<ExportParameterRowStyle> rowStylingParameters, int startRow = 2, List<ExportFooterParameters> footerStyling = null, List<ExportParameterRowStyle> nestedRowStylingParameters = null)
        {
            for (int i = 0; i < parameters.Count(); i++)
            {
                if (parameters[i].DataFormat != ExportDataFormatCatalog.NestedGrid)
                {
                    ws.Cells[startRow, i + 1].Value = parameters[i].HeaderText;
                    ws.Cells[startRow, i + 1].Style.Font.Bold = true;
                    ws.Cells[startRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[startRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    ws.Cells[startRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    FormatColumn(ws.Column(i + 1), parameters[i]);
                    FormatCellRange(ws.Cells[startRow + 1, i + 1, totalRecords + 1, i + 1], parameters[i]);
                    ws.Cells[startRow + 1, i + 1, totalRecords + 2, i + 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[startRow + 1, i + 1, totalRecords + 2, i + 1].Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);

                    ws.Cells[startRow + 1, i + 1, totalRecords + 2, i + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells[startRow + 1, i + 1, totalRecords + 2, i + 1].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);

                    ws.Cells[startRow + 1, i + 1, totalRecords + 2, i + 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[startRow + 1, i + 1, totalRecords + 2, i + 1].Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);

                    ws.Cells[startRow + 1, i + 1, totalRecords + 2, i + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[startRow + 1, i + 1, totalRecords + 2, i + 1].Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    ws.Cells[startRow, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                else
                {
                    ws.Cells[startRow, i + 1].Value = "";
                }
            }
            if (rowStylingParameters != null)
            {
                foreach (ExportParameterRowStyle rowStyle in rowStylingParameters)
                {
                    ws.Cells[rowStyle.RowIndex + startRow + 1, 1, rowStyle.RowIndex + startRow + 1, parameters.Count - 1].Style.Font.Bold = rowStyle.Bold;
                    ws.Cells[rowStyle.RowIndex + startRow + 1, 1, rowStyle.RowIndex + startRow + 1, parameters.Count - 1].Style.Border.BorderAround(rowStyle.BorderThickness, System.Drawing.ColorTranslator.FromHtml(rowStyle.BorderColor));
                    if (!string.IsNullOrEmpty(rowStyle.BackgroundColor))
                    {
                        ws.Cells[rowStyle.RowIndex + startRow + 1, 1, rowStyle.RowIndex + startRow + 1, parameters.Count - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[rowStyle.RowIndex + startRow + 1, 1, rowStyle.RowIndex + startRow + 1, parameters.Count - 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(rowStyle.BackgroundColor));
                    }
                    if (rowStyle.CustomColumnFormat != null && rowStyle.CustomColumnFormat.Count > 0)
                    {
                        for (int j = 0; j < rowStyle.CustomColumnFormat.Count; j++)
                        {
                            FormatExcelRange(ws.Cells[rowStyle.RowIndex + startRow + 1, rowStyle.CustomColumnFormat[j].ColumnIndex], rowStyle.CustomColumnFormat[j].Styling);
                        }
                    }
                }
            }
            if (nestedParameters != null && nestedParameters.Count > 0)
            {
                int parentTableRows = totalRecords;
                int currentRow = startRow + 1;
                int totalCount = (parentTableRows + startRow + 1);
                if (footerStyling != null && footerStyling.Count > 0)
                    totalCount = (parentTableRows + startRow + 1) - footerStyling.Count;

                for (int i = startRow + 1; i < totalCount ; i++)
                {
                    string temp = parameters.Where(x=>x.DataFormat == ExportDataFormatCatalog.NestedGrid).Select(x => x.DataField).FirstOrDefault();
                    DataTable nestedTable = table.Rows[i - startRow - 1][temp] as DataTable;
                    if (nestedTable != null)
                    {
                        totalRecords += nestedTable.Rows.Count + 1;
                        ws.InsertRow(currentRow + 1, nestedTable.Rows.Count + 1);
                        for (int j = 0; j < nestedParameters.Count(); j++)
                        {
                            ws.Cells[currentRow + 1, j + 2].Value = nestedParameters[j].HeaderText;
                            ws.Cells[currentRow + 1, j + 2].Style.Font.Bold = true;
                            ws.Cells[currentRow + 1, j + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[currentRow + 1, j + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Snow);
                            ws.Cells[currentRow + 1, j + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                            ws.Cells[currentRow + 1, j + 2, currentRow + nestedTable.Rows.Count + 1, j + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                            ws.Cells[currentRow + 1, j + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                        ws.Cells[currentRow + 2, 2].LoadFromDataTable(nestedTable, false);
                        for (int j = 0; j < nestedParameters.Count(); j++)
                        {
                            FormatExcelRange(ws.Cells[currentRow + 2, j + 2, currentRow + 1 + nestedTable.Rows.Count, j + 2], nestedParameters[j]);
                        }

                        currentRow += nestedTable.Rows.Count + 2;
                    }
                }
            }
            if (nestedRowStylingParameters != null)
            {
                foreach (ExportParameterRowStyle rowStyle in nestedRowStylingParameters)
                {
                    ws.Cells[rowStyle.RowIndex, 1, rowStyle.RowIndex, nestedParameters.Count + 1].Style.Font.Bold = rowStyle.Bold;
                    ws.Cells[rowStyle.RowIndex, 1, rowStyle.RowIndex, nestedParameters.Count + 1].Style.Border.BorderAround(rowStyle.BorderThickness, System.Drawing.ColorTranslator.FromHtml(rowStyle.BorderColor));
                    if (!string.IsNullOrEmpty(rowStyle.BackgroundColor))
                    {
                        ws.Cells[rowStyle.RowIndex, 1, rowStyle.RowIndex, nestedParameters.Count + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[rowStyle.RowIndex, 1, rowStyle.RowIndex, nestedParameters.Count + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(rowStyle.BackgroundColor));
                    }
                    if (rowStyle.CustomColumnFormat != null && rowStyle.CustomColumnFormat.Count > 0)
                    {
                        for (int j = 0; j < rowStyle.CustomColumnFormat.Count; j++)
                        {
                            FormatExcelRange(ws.Cells[rowStyle.RowIndex, rowStyle.CustomColumnFormat[j].ColumnIndex], rowStyle.CustomColumnFormat[j].Styling);
                        }
                    }
                }
            }
        }
        private static void FormatColumn(ExcelColumn col, ExportParameterModel parameter)
        {
            if (parameter.DataFormat != ExportDataFormatCatalog.None)
            {
                string format = string.Empty;
                //format = "_($* #,##0.00_);_($* (#,##0.00);_($* \\\" - \\\"??_);_(@_)";
                if (parameter.DataFormat == ExportDataFormatCatalog.Accounting)
                    format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
                else if (parameter.DataFormat == ExportDataFormatCatalog.DateDMY)
                    format = "dd/mm/yyyy";
                else if (parameter.DataFormat == ExportDataFormatCatalog.DateMDY)
                    format = "MM/DD/YYYY";
                else if (parameter.DataFormat == ExportDataFormatCatalog.DateDMYHMST)
                    format = "mm/dd/yyyy h:mm AM/PM;";
                else if (parameter.DataFormat == ExportDataFormatCatalog.Currency || parameter.DataFormat == ExportDataFormatCatalog.Amount)
                    format = "_-$ #,##0.00_-;$ (#,##0.00);_-$ \\0.00\\ ;_-@_-";
                else if (parameter.DataFormat == ExportDataFormatCatalog.Number)
                    format = "0";
                else if (parameter.DataFormat == ExportDataFormatCatalog.Text)
                    format = "@";
                else if (parameter.DataFormat == ExportDataFormatCatalog.Text)
                    format = "@";
                else if (parameter.DataFormat == ExportDataFormatCatalog.Percentage)
                    format = "0.00%";
                if (!string.IsNullOrEmpty(format))
                    col.Style.Numberformat.Format = format;
            }
            if (parameter.TextHorizontalAlign != System.Web.UI.WebControls.HorizontalAlign.NotSet)
            {
                col.Style.HorizontalAlignment = (ExcelHorizontalAlignment)(Enum.Parse(typeof(ExcelHorizontalAlignment), parameter.TextHorizontalAlign.ToString()));
            }
            if (!parameter.AutoAdjustWidth && parameter.ColumnWidth > 0)
                col.Width = parameter.ColumnWidth / 6;
            else
                col.AutoFit(16, 500);
            col.Style.WrapText = parameter.WrapText;
        }

        private static void FormatExcelRange(ExcelRange excelRange, ExportParameterModel parameter)
        {
            if (parameter.DataFormat != ExportDataFormatCatalog.None)
            {
                string format = string.Empty;
                if (parameter.DataFormat == ExportDataFormatCatalog.Accounting)
                    format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
                else if (parameter.DataFormat == ExportDataFormatCatalog.DateDMY)
                    format = "dd/mm/yyyy";
                else if (parameter.DataFormat == ExportDataFormatCatalog.DateMDY)
                    format = "MM/DD/YYYY";
                else if (parameter.DataFormat == ExportDataFormatCatalog.DateDMYHMST)
                    format = "mm/dd/yyyy h:mm AM/PM;";
                else if (parameter.DataFormat == ExportDataFormatCatalog.Currency || parameter.DataFormat == ExportDataFormatCatalog.Amount)
                    format = "_-$ #,##0.00_-;$ (#,##0.00);_-$ \\0.00\\ ;_-@_-";
                else if (parameter.DataFormat == ExportDataFormatCatalog.Number)
                    format = "0";
                else if (parameter.DataFormat == ExportDataFormatCatalog.Text)
                    format = "@";
                else if (parameter.DataFormat == ExportDataFormatCatalog.Percentage)
                    format = "0.00%";
                if (!string.IsNullOrEmpty(format))
                {

                    excelRange.Style.Numberformat.Format = format;

                    if (parameter.ParseValue && excelRange.Value != null && (parameter.DataFormat == ExportDataFormatCatalog.Accounting || parameter.DataFormat == ExportDataFormatCatalog.Percentage))
                        excelRange.Value = decimal.Parse(excelRange.Value.ToString());
                }
            }

            if (parameter.TextHorizontalAlign != System.Web.UI.WebControls.HorizontalAlign.NotSet)
            {
                excelRange.Style.HorizontalAlignment = (ExcelHorizontalAlignment)(Enum.Parse(typeof(ExcelHorizontalAlignment), parameter.TextHorizontalAlign.ToString()));
            }

            if(parameter.AutoAdjustWidth)
                excelRange.AutoFitColumns(25, 1000);
        }
        private static void FormatCellRange(ExcelRange colRange, ExportParameterModel parameter)
        {
            if (!string.IsNullOrEmpty(parameter.BackgroundColor))
            {
                colRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                colRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(parameter.BackgroundColor));
            }
        }
        private static DataTable ToDataTable<T>(this IReadOnlyList<T> list, List<ExportParameterModel> parameters)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < parameters.Count; i++)
            {
                DataColumn dc = new DataColumn();
                if (!table.Columns.Contains(parameters[i].DataField))
                    dc.ColumnName = parameters[i].DataField;
                else
                    dc.ColumnName = parameters[i].DataField + Guid.NewGuid().ToString().Substring(0, 5);
                if (parameters[i].DataFormat == ExportDataFormatCatalog.Accounting || parameters[i].DataFormat == ExportDataFormatCatalog.Amount || parameters[i].DataFormat == ExportDataFormatCatalog.Currency)
                    dc.DataType = typeof(decimal);
                else if (parameters[i].DataFormat == ExportDataFormatCatalog.DateMDY)
                    dc.DataType = typeof(DateTime);
                else if (parameters[i].DataFormat == ExportDataFormatCatalog.Number)
                    dc.DataType = typeof(int);
                table.Columns.Add(dc);
            }
            object[] values = new object[parameters.Count];
            foreach (T item in list)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (parameters[i].DataFormat == ExportDataFormatCatalog.KeyValueDetails)
                    {
                        var obj = props["KeyValueDetails"].GetValue(item) ?? DBNull.Value;
                        bool isValueSet = false;
                        if (obj != DBNull.Value)
                        {
                            foreach (var kv in (System.Collections.IList)obj)
                            {
                                var type = kv.GetType();
                                var key = type.GetProperty("vName");
                                var value = type.GetProperty("vValue");
                                var keyObj = key.GetValue(kv, null);
                                var valueObj = value.GetValue(kv, null);
                                if (keyObj != null && keyObj.ToString() == parameters[i].DataField)
                                {
                                    values[i] = valueObj == null ? "" : valueObj.ToString();
                                    isValueSet = true;
                                    break;
                                }
                            }
                        }
                        if (!isValueSet)
                        {
                            values[i] = "";
                        }
                    }
                    else
                    {
                        object val = null;
                        if (parameters[i].DataField.Contains(":"))
                            parameters[i].DataField = parameters[i].DataField.Split(':')[1];
                        else if (parameters[i].DataField.Contains("."))
                        {
                            string[] complexObj= parameters[i].DataField.Split('.');
                            object complexVal = props[complexObj[0]].GetValue(item) ?? DBNull.Value;
                            if(complexVal != null)
                            {
                                PropertyDescriptorCollection complexProps = TypeDescriptor.GetProperties(complexVal.GetType());
                                val = complexProps[complexObj[1]].GetValue(complexVal) ?? DBNull.Value;
                            }
                            else
                            {
                                val = "";
                            }
                        }
                        else if (parameters[i].DataField.Contains("[")&& parameters[i].DataField.Contains("]"))
                        {
                            object complexVal = props[parameters[i].DataField.Split('[')[0]].GetValue(item) ?? DBNull.Value;

                            List<object> arrayValues = (complexVal as IEnumerable).Cast<object>().ToList();

                            if (complexVal != null)
                            {
                                var index= parameters[i].DataField.Split('[', ']')[1];
                                val = arrayValues[int.Parse(index)];
                            }
                            else
                            {
                                val = "";
                            }
                        }
                        if (val == null)
                            val = props[parameters[i].DataField].GetValue(item) ?? DBNull.Value;

                        if (val != null && val.ToString().Contains("<br/>"))
                            val = val.ToString().Replace("<br/>", Environment.NewLine);
                        if (val != null && val.ToString().Contains("<br>"))
                            val = val.ToString().Replace("<br>", Environment.NewLine);
                        values[i] = val;
                    }
                }
                table.Rows.Add(values);
            }
            return table;
        }
        private static DataTable ToDataTable<T, T2>(this IList<T> list, List<ExportParameterModel> parameters, List<ExportParameterModel> nestedParameters)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < parameters.Count; i++)
            {
                DataColumn dc = new DataColumn();
                if (!table.Columns.Contains(parameters[i].DataField))
                    dc.ColumnName = parameters[i].DataField;
                else
                    dc.ColumnName = parameters[i].DataField + Guid.NewGuid().ToString().Substring(0, 5);
                if (parameters[i].DataFormat == ExportDataFormatCatalog.Accounting || parameters[i].DataFormat == ExportDataFormatCatalog.Amount || parameters[i].DataFormat == ExportDataFormatCatalog.Currency)
                    dc.DataType = typeof(decimal);
                else if (parameters[i].DataFormat == ExportDataFormatCatalog.DateMDY)
                    dc.DataType = typeof(DateTime);
                else if (parameters[i].DataFormat == ExportDataFormatCatalog.Number)
                    dc.DataType = typeof(int);
                else if (parameters[i].DataFormat == ExportDataFormatCatalog.NestedGrid)
                    dc.DataType = typeof(DataTable);
                table.Columns.Add(dc);
            }
            object[] values = new object[parameters.Count];
            foreach (T item in list)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (parameters[i].DataFormat == ExportDataFormatCatalog.KeyValueDetails)
                    {
                        var obj = props["KeyValueDetails"].GetValue(item) ?? DBNull.Value;
                        bool isValueSet = false;
                        if (obj != DBNull.Value)
                        {
                            foreach (var kv in (System.Collections.IList)obj)
                            {
                                var type = kv.GetType();
                                var key = type.GetProperty("vName");
                                var value = type.GetProperty("vValue");
                                var keyObj = key.GetValue(kv, null);
                                var valueObj = value.GetValue(kv, null);
                                if (keyObj != null && keyObj.ToString() == parameters[i].DataField)
                                {
                                    values[i] = valueObj == null ? "" : valueObj.ToString();
                                    isValueSet = true;
                                    break;
                                }
                            }
                        }
                        if (!isValueSet)
                        {
                            values[i] = "";
                        }
                    }
                    else if (parameters[i].DataFormat == ExportDataFormatCatalog.NestedGrid)
                    {
                        List<T2> nestedList = (item.GetType().GetProperty(parameters[i].DataField).GetValue(item, null) as List<T2>);
                        DataTable nestedTable = (nestedList == null ? new List<T2>() : item.GetType().GetProperty(parameters[i].DataField).GetValue(item, null) as List<T2>).ToDataTable(nestedParameters);
                        values[i] = nestedTable;
                    }
                    else
                    {
                        object val = null;
                        if (parameters[i].DataField.Contains(":"))
                            parameters[i].DataField = parameters[i].DataField.Split(':')[1];
                        else if (parameters[i].DataField.Contains("."))
                        {
                            string[] complexObj= parameters[i].DataField.Split('.');
                            object complexVal = props[complexObj[0]].GetValue(item) ?? DBNull.Value;
                            if(complexVal != null)
                            {
                                PropertyDescriptorCollection complexProps = TypeDescriptor.GetProperties(complexVal.GetType());
                                val = complexProps[complexObj[1]].GetValue(complexVal) ?? DBNull.Value;
                            }
                            else
                            {
                                val = "";
                            }
                        }
                        if(val == null)
                            val = props[parameters[i].DataField].GetValue(item) ?? DBNull.Value;

                        if (val != null && val.ToString().Contains("<br/>"))
                            val = val.ToString().Replace("<br/>", Environment.NewLine);
                        if (val != null && val.ToString().Contains("<br>"))
                            val = val.ToString().Replace("<br>", Environment.NewLine);
                        values[i] = val;
                    }
                }
                table.Rows.Add(values);
            }
            return table;
        }
        private static void LoadFromDataTable(DataTable table, int _fromCol, int _fromRow, ExcelWorksheet worksheet)
        {
            if (table == null)
            {
                throw (new ArgumentNullException("Table can't be null"));
            }
            int col = _fromCol, row = _fromRow;
            foreach (DataRow dr in table.Rows)
            {
                foreach (DataColumn dc in table.Columns)
                {
                    if (dc.DataType == typeof(decimal))
                        worksheet.Cells[row, col++].Value = string.IsNullOrEmpty(dr[dc.ColumnName].ToString()) ? (decimal)0.00 : Math.Round((decimal)dr[dc.ColumnName], 2);
                    else if (dc.DataType == typeof(int))
                        worksheet.Cells[row, col++].Value = string.IsNullOrEmpty(dr[dc.ColumnName].ToString()) ? 0 : (int?)dr[dc.ColumnName];
                    else if (dc.DataType == typeof(DateTime))
                    {
                        DateTime dt = string.IsNullOrEmpty(dr[dc.ColumnName].ToString()) ? DateTime.MinValue : DateTime.Parse(dr[dc.ColumnName].ToString());
                        worksheet.Cells[row, col++].Value = string.IsNullOrEmpty(dr[dc.ColumnName].ToString()) || dt == DateTime.MinValue ? "" : dt.ToString("MM/dd/yyyy");
                    }
                    else
                    {
                        if (dr[dc.ColumnName] != null && dr[dc.ColumnName].ToString().Contains("<br/>"))
                            dr[dc.ColumnName] = dr[dc.ColumnName].ToString().Replace("<br/>", Environment.NewLine);
                        if (dr[dc.ColumnName] != null && dr[dc.ColumnName].ToString().Contains("<br>"))
                            dr[dc.ColumnName] = dr[dc.ColumnName].ToString().Replace("<br>", Environment.NewLine);
                        worksheet.Cells[row, col++].Value = dr[dc.ColumnName];
                    }
                }
                row++;
                col = _fromCol;
            }
        }
        private static void SetPrinterSettings(this ExcelWorksheet ws, ExcelAddress headerAddress = null)
        {
            ws.PrinterSettings.HeaderMargin = 0.25M;//Default Header Margin
            ws.PrinterSettings.FooterMargin = 0.25M;//Default Footer Margin
            ws.PrinterSettings.Orientation = eOrientation.Landscape;//Default Print Orientation
            ws.PrinterSettings.VerticalCentered = false;//Default Vertical Allginment
            ws.PrinterSettings.FitToPage = true;
            ws.PrinterSettings.FitToWidth = 1;
            ws.PrinterSettings.FitToHeight = 0;
            ws.PrinterSettings.TopMargin = 1;
            ws.PrinterSettings.BottomMargin = 1;
            ws.PrinterSettings.LeftMargin = 0.5M;
            ws.PrinterSettings.RightMargin = 0.5M;
            if(headerAddress != null)
                ws.PrinterSettings.RepeatRows = headerAddress;
        }
        #endregion
        public static FileStreamResult ExportToExcel<T>(IReadOnlyList<T> data, List<ExportParameterModel> parameters, string fileName, string editionName, bool hasFooter = false, string headerText = "", List<ExportParameterRowStyle> rowStylingParameters = null, string searchCriteria = null, ILogger logger = null, List<ExportParameterGroupedHeaderModel> groupedHeaders = null, int groupedHeadersStartRow = 1, int groupedHeadersStartCol = 1, bool applyBorderOnSheet = false, EnumReportNameForPieChart enumReportNameForPieChart = EnumReportNameForPieChart.None)
        {
            //response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //response.Headers.Add("content-disposition", "attachment;filename=" + fileName);
            try
            {
                using (ExcelPackage pack = new ExcelPackage())
                {
                    DataTable table = new DataTable();
                    string startRow = groupedHeaders == null || groupedHeaders.Count == 0 ? "A1" : "A2";
                    int rowStartFrom = 1;
                    if (groupedHeaders != null && groupedHeaders.Count > 0)
                        rowStartFrom += 1;
                    table = data.ToDataTable(parameters);
                    ExcelWorksheet ws = CreateSheet(pack, headerText, table, startRow);
                    ws.SetPrinterSettings(new ExcelAddress("3:3"));


                    string printHeader = PrintHeaderTemplate.Replace("%%ReportTitle%%", headerText);
                    string printFooter = PrintFooterTemplate.Replace("%%PageNumber%%", ExcelHeaderFooter.PageNumber).Replace("%%NumberOfPages%%", ExcelHeaderFooter.NumberOfPages).Replace("%%DateTime%%", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));

                    if (!string.IsNullOrEmpty(searchCriteria))
                        printHeader = printHeader.Replace("%%SearchCriteria%%", searchCriteria);
                    else
                        printHeader = printHeader.Replace("Search Criteria: %%SearchCriteria%%", string.Empty);

                    if (!string.IsNullOrEmpty(editionName))
                    {
                        printHeader = printHeader.Replace("Valid Claims", editionName);
                    }

                    ws.HeaderFooter.OddHeader.CenteredText = printHeader;
                    ws.HeaderFooter.OddFooter.RightAlignedText = printFooter;


                    ws.ApplyExcelStyling(table.Rows.Count>0? table.Rows.Count:1, parameters, rowStylingParameters, hasFooter, rowStartFrom, applyBorderOnSheet);
                    if (groupedHeaders != null && groupedHeaders.Count > 0)
                        ws.CreateRepeatingHeader(table.Columns.Count, groupedHeadersStartRow, groupedHeadersStartCol, groupedHeaders);

                    if (enumReportNameForPieChart == EnumReportNameForPieChart.PaymentForthComingDetail)
                         AddPieChartForPaymentForthCommingReport(pack,headerText);

                    var ms = new System.IO.MemoryStream();
                        pack.SaveAs(ms);
                    ms.Seek(0, 0);
                    return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = fileName };
                    //ms.WriteTo(response.Body);
                
                }
            }
            catch (Exception ex)
            {
                logger?.Error("Exception in ExportToExcel<T>", ex);
            }

            return null;
        }

        public static int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }
        public static FileStreamResult ExportToExcelFromDataTableWithGroupedHeaders(DataTable table, List<ExportParameterModel> parameters, string fileName, string imagePath="",  bool hasFooter = false,string debtorName="", string titleHeader = "", List<ExportParameterGroupedHeaderModel> groupedHeaders = null, int groupedHeadersStartRow = 1, int groupedHeadersStartCol = 1, List<ExportParameterRowStyle> rowStylingParameters = null, string templateName = null, string searchCriteria = null, ILogger logger = null, string tenantName = "", bool removeTitle = false, bool deleteStartingRow = false, string printerRepeatRow = "", List<ExportParameterGroupedHeaderModel> grouped2_Headers = null, int grouped2_HeadersStartRow = 2, int grouped2_HeadersStartCol = 1,bool applyBordersOnSheet=false)
        {
            //response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //response.Headers.Add("content-disposition", "attachment;filename=" + fileName);
            try
            {
                ExcelPackage pack = null;
                ExcelWorksheet ws = null;
                Bitmap image = new Bitmap(imagePath);
                ExcelPicture excelImage = null;

                int startRowIndex = 0;
                string startRow = groupedHeaders == null || groupedHeaders.Count == 0 ? "A10" : "A10";
                if (startRow == "A2" && grouped2_Headers != null && grouped2_Headers.Count > 0) //if groupedHeaders, check if there are 2nd Level groupedHeaders as well
                    startRow = "A3";
                int.TryParse(Regex.Match(startRow, @"\d+").Value, out startRowIndex );
                string sheetName = titleHeader;
                if (!string.IsNullOrEmpty(templateName))
                {
                    System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ParityAccess.Application.ReportTemplates." + templateName + ".xltx");
                    pack = new ExcelPackage(stream);
                    ws = LoadSheet(pack, sheetName, table, startRow);


                }
                else
                {
                    pack = new ExcelPackage();
                    ws = CreateSheet(pack, sheetName, table, startRow);
                    ws.SetPrinterSettings(!string.IsNullOrEmpty(printerRepeatRow) ? new ExcelAddress(printerRepeatRow) : (groupedHeaders != null ? new ExcelAddress("2:2") : new ExcelAddress("1:1")));

                    string printHeader = PrintHeaderTemplate.Replace("%%ReportTitle%%", titleHeader);
                    if (removeTitle || !string.IsNullOrWhiteSpace(tenantName))
                        printHeader = printHeader.Replace("Valid Claims", tenantName);

                    string printFooter = PrintFooterTemplate.Replace("%%PageNumber%%",ExcelHeaderFooter.PageNumber).Replace("%%NumberOfPages%%",ExcelHeaderFooter.NumberOfPages).Replace("%%DateTime%%",DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));

                    if(!string.IsNullOrEmpty(searchCriteria))
                        printHeader = printHeader.Replace("%%SearchCriteria%%", searchCriteria);
                    else
                        printHeader = printHeader.Replace("Search Criteria: %%SearchCriteria%%",string.Empty);

                    ws.HeaderFooter.OddHeader.CenteredText = printHeader;
                    ws.HeaderFooter.OddFooter.RightAlignedText = printFooter;
                }

                if (!string.IsNullOrEmpty(debtorName)) {
                    ws.Cells[8, 1].Value = $"Debiteur: {debtorName}";
                    ws.Cells[8, 1].Style.Font.Bold = true;
                }

                if (image != null)
                {
                    excelImage = ws.Drawings.AddPicture("Logo", image);
                    excelImage.From.Column = 0;
                    excelImage.From.Row = 0;
                    excelImage.SetSize(350, 100);
                    // 2x2 px space for better alignment
                    excelImage.From.ColumnOff = Pixel2MTU(0);
                    excelImage.From.RowOff = Pixel2MTU(0);
                }


                ws.ApplyExcelStyling(table.Rows.Count>0? table.Rows.Count:1, parameters, rowStylingParameters, hasFooter, startRowIndex, applyBordersOnSheet: applyBordersOnSheet);
                  if (groupedHeaders != null && groupedHeaders.Count > 0)
                    ws.CreateRepeatingHeader(table.Columns.Count, groupedHeadersStartRow, groupedHeadersStartCol, groupedHeaders);

                if (grouped2_Headers != null && grouped2_Headers.Count > 0)
                    ws.CreateRepeatingHeader(table.Columns.Count, grouped2_HeadersStartRow, grouped2_HeadersStartCol, grouped2_Headers);

                if (deleteStartingRow)
                    ws.DeleteRow(startRowIndex); 

                var ms = new System.IO.MemoryStream();
                pack.SaveAs(ms);
                ms.Seek(0, 0);
                return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = fileName };
            }
            catch (Exception ex)
            {
                logger?.Error("Exception in ExportToExcelFromDataTableWithGroupedHeaders", ex);
            }
            return null;
        }

        private static void AddPieChartForPaymentForthCommingReport(ExcelPackage pack,string  sheetName)
        {
            var ws = pack.Workbook.Worksheets[sheetName]; 
            var dataRange = ws.Cells["A1:J"+(ws.Dimension.Rows-1)]; 
            var wsPivot = pack.Workbook.Worksheets.Add("Chart");
            var pivotTable1 = wsPivot.PivotTables.Add(wsPivot.Cells["B5"], dataRange, "Forthcoming");

            pivotTable1.RowFields.Add(pivotTable1.Fields["Employee Number"]);
            pivotTable1.DataOnRows = false;

            pivotTable1.RowHeaderCaption = "Employee Number";
            var field =    pivotTable1.DataFields.Add(pivotTable1.Fields["Revenue Forthcoming"]);
            field.Name = "Forthcoming Summarize Report";
            field.Function = DataFieldFunctions.Sum;
            field.Format = "$#,##0.00"; ;


            var chart = wsPivot.Drawings.AddChart("PivotChart", eChartType.Pie, pivotTable1) as ExcelPieChart;
            //size of the chart
            chart.SetSize(500, 400);
            chart.DataLabel.ShowValue = true;
          
            chart.DataLabel.ShowSeriesName = false;
            chart.ShowDataLabelsOverMaximum = true;
            chart.Legend.Position = eLegendPosition.Bottom;
            chart.Title.Text = "Forthcoming Payment Report";
            chart.SetPosition(4, 0, 6, 0);
        }

        public static FileStreamResult ExportToExcelWithGroupedHeaders<T>(List<T> data, List<ExportParameterModel> parameters, string fileName, bool hasFooter = false, string titleHeader = "", List<ExportParameterGroupedHeaderModel> groupedHeaders = null, int groupedHeadersStartRow = 1, int groupedHeadersStartCol = 1, List<ExportParameterRowStyle> rowStylingParameters = null, string searchCriteria = null,ILogger logger=null)
        {
            //response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //response.Headers.Add("content-disposition", "attachment;filename=" + fileName);
            try
            {
                using (ExcelPackage pack = new ExcelPackage())
                {
                    DataTable table = new DataTable();
                    table = data.ToDataTable(parameters);
                    ExcelWorksheet ws = CreateSheet(pack, "Sheet 1", table, "A2");
                    ws.SetPrinterSettings(new ExcelAddress("2:2"));

                    string printHeader = PrintHeaderTemplate.Replace("%%ReportTitle%%", titleHeader);
                    string printFooter = PrintFooterTemplate.Replace("%%PageNumber%%",ExcelHeaderFooter.PageNumber).Replace("%%NumberOfPages%%",ExcelHeaderFooter.NumberOfPages).Replace("%%DateTime%%",DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));

                    if(!string.IsNullOrEmpty(searchCriteria))
                        printHeader = printHeader.Replace("%%SearchCriteria%%", searchCriteria);
                    else
                        printHeader = printHeader.Replace("Search Criteria: %%SearchCriteria%%",string.Empty);

                    ws.HeaderFooter.OddHeader.CenteredText = printHeader;
                    ws.HeaderFooter.OddFooter.RightAlignedText = printFooter;

                    ws.ApplyExcelStyling(table.Rows.Count, parameters, rowStylingParameters, hasFooter, 3);
                    if (groupedHeaders != null)
                        ws.CreateRepeatingHeader(table.Columns.Count, groupedHeadersStartRow, groupedHeadersStartCol, groupedHeaders);
                    var ms = new System.IO.MemoryStream();
                    pack.SaveAs(ms);
                    ms.Seek(0, 0);
                    return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = fileName };
                    //ms.WriteTo(response.Body);
                }
            }
            catch (Exception ex)
            {
                logger?.Error("Exception in ExportToExcelWithGroupedHeaders<T>", ex);
            }

            return null;
        }

        public static FileStreamResult ExportToExcel<T, T2>(List<T> data, List<ExportParameterModel> parameters, List<ExportParameterModel> nestedParameters, string fileName, string headerText = "", List<ExportParameterRowStyle> rowStylingParameters = null, string searchCriteria = null, ILogger logger = null, List<ExportParameterGroupedHeaderModel> groupedHeaders = null, int groupedHeadersStartRow = 1, int groupedHeadersStartCol = 1, List<ExportFooterParameters> footerStyling = null, List<ExportParameterRowStyle> nestedRowStylingParameters = null)
        {
            //response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //response.Headers.Add("content-disposition", "attachment;filename=" + fileName);
            try
            {
                using (ExcelPackage pack = new ExcelPackage())
                {
                    string startRow = groupedHeaders == null || groupedHeaders.Count == 0 ? "A1" : "A2";
                    int rowStartFrom = 1;
                    if (groupedHeaders != null && groupedHeaders.Count > 0)
                        rowStartFrom += 1;
                    DataTable table = new DataTable();
                    table = data.ToDataTable<T, T2>(parameters, nestedParameters);
                    ExcelWorksheet ws = CreateSheet(pack, headerText, table, startRow);
                    ws.SetPrinterSettings(new ExcelAddress("3:3"));

                    string printHeader = PrintHeaderTemplate.Replace("%%ReportTitle%%", headerText);
                    string printFooter = PrintFooterTemplate.Replace("%%PageNumber%%",ExcelHeaderFooter.PageNumber).Replace("%%NumberOfPages%%",ExcelHeaderFooter.NumberOfPages).Replace("%%DateTime%%",DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));

                    if(!string.IsNullOrEmpty(searchCriteria))
                        printHeader = printHeader.Replace("%%SearchCriteria%%", searchCriteria);
                    else
                        printHeader = printHeader.Replace("Search Criteria: %%SearchCriteria%%",string.Empty);

                    ws.HeaderFooter.OddHeader.CenteredText = printHeader;
                    ws.HeaderFooter.OddFooter.RightAlignedText = printFooter;

                    ws.ApplyExcelStylingForNestedTable(table, table.Rows.Count, parameters, nestedParameters, rowStylingParameters, rowStartFrom, footerStyling: footerStyling, nestedRowStylingParameters: nestedRowStylingParameters);
                    if (groupedHeaders != null && groupedHeaders.Count > 0)
                        ws.CreateRepeatingHeader(table.Columns.Count, groupedHeadersStartRow, groupedHeadersStartCol, groupedHeaders);
                    int spanVal = nestedParameters.Count + 1 > parameters.Count ? (parameters.Count + (nestedParameters.Count - parameters.Count) + 1) : (parameters.Count < nestedParameters.Count ? parameters.Count : parameters.Count - 1);
                    var ms = new System.IO.MemoryStream();
                    pack.SaveAs(ms);
                    ms.Seek(0, 0);
                    return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = fileName };
                }
            }
            catch (Exception ex)
            {
                logger?.Error("Exception in ExportToExcel<T, T2>", ex);
            }

            return null;
        }
    }
}
