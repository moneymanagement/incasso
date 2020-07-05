using ParityAccess.Utils.EPPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.Style;

namespace ParityAccess.Application.Utils.EPPlus
{
    public class ExportColumnCustomFormat
    {
        public int ColumnIndex { get; set; }
        public ExportParameterModel Styling { get; set; }
    }
    public class ExportColumnCustomStyle
    {
        public string BorderColor { get; set; } 
        public OfficeOpenXml.Style.ExcelBorderStyle BorderLeftThickness { get; set; } = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        public OfficeOpenXml.Style.ExcelBorderStyle BorderRightThickness { get; set; } = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        public OfficeOpenXml.Style.ExcelBorderStyle BorderTopThickness { get; set; } = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        public OfficeOpenXml.Style.ExcelBorderStyle BorderBottomThickness { get; set; } = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        public string BackgroundColor { get; set; } = "#ffffff";
        public int ColumnIndex { get; set; }
        public int RowIndex { get; set; }
        public ExcelFillStyle PatternType { get; internal set; } = ExcelFillStyle.Solid;
    }
}
