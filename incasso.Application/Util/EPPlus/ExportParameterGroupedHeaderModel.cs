using ParityAccess.Application.Utils.EPPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParityAccess.Utils.EPPlus
{
    public class ExportParameterGroupedHeaderModel
    {
        public string HeaderText { get; set; }
        public int ColSpan { get; set; }
        public bool IsBold { get; set; }
        public ExportColumnCustomStyle CustomStyle;
    }
}
