using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Catalogs
{
    
    public class PortalType : StringCatalogBase
    {
        public const string Outsourcing = "Outsourcing";
        public const string Collection = "Collection";
        
        private Dictionary<string, string> _catalog;

        public PortalType() : base()
        {
        }
        public PortalType(string initialValue) : base(initialValue)
        {
        }

        static public implicit operator string(PortalType catalog)
        {
            return catalog.Value;
        }
        static public implicit operator PortalType(string stringValue)
        {
            PortalType catalog = new PortalType(stringValue);
            return catalog;
        }
        public override string ToString()
        {
            string result = this;
            return result;
        }

    }
}
