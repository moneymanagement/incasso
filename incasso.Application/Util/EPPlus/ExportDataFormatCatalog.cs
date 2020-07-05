using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParityAccess.Utils.EPPlus
{
    public enum ExportDataFormatCatalog : int
    {
        
        None = 0,        
        Amount = 1,        
        DateDMY = 2,                
        DateMDY = 3,        
        Accounting = 4,        
        Text,
        CheckBox,        
        DateDMYHMST,
        KeyValueDetails,
        NestedGrid,
        Currency,
        Number,
        Percentage
    }
}
