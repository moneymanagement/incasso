using ParityAccess.Application.Utils.EPPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParityAccess.Utils.EPPlus
{
    public class ExportParameterRowStyle
    {
        public List<ExportColumnCustomStyle> CustomColumnStyle;

        public int RowIndex { get; set; }
        public bool Bold { get; set; }
        public int FontSize { get; set; }  
        public bool MergeRow { get; set; }
        public System.Web.UI.WebControls.HorizontalAlign TextHorizontalAlign { get; set; }
        public string BorderColor { get; set; }
        public OfficeOpenXml.Style.ExcelBorderStyle BorderThickness { get; set; } = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        public string BackgroundColor { get; set; }
        public List<ExportColumnCustomFormat> CustomColumnFormat { get; set; }
        public List<ExportRowSpanParameter> RowSpanParameter { get; set; }

    }
}
