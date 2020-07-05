using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace incasso.Invoices
{
    public class InvoiceStatusCatalog
    {
        public static int ToInt(string status)
        {
            switch (status.Trim())
            {

                case "Minnelijke Incasso":
                    return 0;
                case "gesloten - Creditering":
                    return 01;
                case "Gesloten - Deel Vordering":
                    return 02;
                case "Gesloten - Faillissement":
                    return 03;
                case "Gesloten - WSNP":
                    return 04;
                case "Gesloten - Finale Kwijting":
                    return 05;
                case "Gesloten - I.O.M. Cliënte":
                    return 06;
                case "Gesloten - I.O.V. Cliënte":
                    return 07;
                case "Gesloten - Incl. R & I":
                    return 08;
                case "Gesloten - Kostendekkend":
                    return 09;
                case "Gesloten - Zonder Provisie":
                    return 10;
                case "Lopend - Eerste Aanmaning":
                    return 11;
                case "Lopend - Laatste Aanmaning":
                    return 12;
                case "Lopend - Conceptdagvaarding":
                    return 13;
                case "Lopend - Betalingsregeling":
                    return 14;
                case "Lopend - Betalingstoezegging":
                    return 15;
                case "Lopend - Deelbetaling":
                    return 16;
                case "Lopend - Hoofdsom Voldaan (R & I)":
                    return 17;
                case "Lopend - Afwachting Klant":
                    return 18;
                case "Executie":
                    return 19;
                case "Gerechtelijke incasso":
                    return 20;
                default:
                    return 0;
            }
        }

        public static string ParseToString(int status)
        {
            switch (status)
            {

                case 0:
                    return "Minnelijke Incasso";
                case 01:
                    return "gesloten - Creditering";
                case 02:
                    return "Gesloten - Deel Vordering";
                case 03:
                    return "Gesloten - Faillissement";
                case 04 :
                    return "Gesloten - WSNP";
                case 05:
                    return "Gesloten - Finale Kwijting";
                case 06:
                    return "Gesloten - I.O.M. Cliënte";
                case 07 :
                    return "Gesloten - I.O.V. Cliënte";
                case 08:
                    return "Gesloten - Incl. R & I";
                case 09:
                    return "Gesloten - Kostendekkend";
                case 10:
                    return "Gesloten - Zonder Provisie";
                case 11:
                    return "Lopend - Eerste Aanmaning";
                case 12:
                    return "Lopend - Laatste Aanmaning";
                case 13:
                    return "Lopend - Conceptdagvaarding";
                case 14:
                    return "Lopend - Betalingsregeling";
                case 15:
                    return "Lopend - Betalingstoezegging";
                case 16:
                    return "Lopend - Deelbetaling";
                case 17:
                    return "Lopend - Hoofdsom Voldaan (R & I)";
                case 18:
                    return "Lopend - Afwachting Klant";
                case 19:
                    return "Executie";
                case 20:
                    return "Gerechtelijke incasso";
                default:
                    return "";
            }
        }
    }
}
