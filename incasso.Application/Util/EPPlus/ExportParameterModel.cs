using ParityAccess.Application.Utils.EPPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParityAccess.Utils.EPPlus
{
    public class ExportParameterModel
    {
        public string DataField { get; set; }
        public string HeaderText { get; set; }
        public string FooterText { get; set; }
        public string BackgroundColor { get; set; }
        public ExportDataFormatCatalog DataFormat { get; set; }        
        public int ColumnWidth { get; set; }
        public System.Web.UI.WebControls.HorizontalAlign TextHorizontalAlign { get; set; }
        public bool IsCheckboxField { get; set; }
        public bool WrapText { get; set; }
        public bool AutoAdjustWidth { get; set; }
        public bool ParseValue { get; set; }
        public ExportColumnCustomStyle CustomStyle;

    }
}
