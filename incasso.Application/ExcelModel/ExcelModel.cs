using incasso.Invoices;
using Incasso.Upload;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace incasso.ExcelModel
{
   public class ExcelModel
    {
        private Debtors.Debtor debtor { get; set; }
        private Invoices.Invoice Invoice{ get; set; }
        public ExcelModel(Upload fileDetails)
        {
            debtor = new Debtors.Debtor();
            Invoice = new Invoices.Invoice();
            Invoice.StatusCatalog = new StatusCatalog();
            Invoice.Upload = fileDetails;
            //Invoice.AdministratorId=fileDetails.Administrators.
        }
        public string d_Number { set { debtor.Number = value; }}
        public string d_Name { set { debtor.Name = value; } }
        public string d_Contact { set { debtor.Contact= value; } }
        public string d_Email { set { debtor.Email = value; } }
        public string d_Phone { set { debtor.Phone = value; } }
        public string d_Mobile { set { debtor.Mobile= value; } }
        public string d_Address { set { debtor.Address = value; } }
        public string d_Postal { set { debtor.Postal = value; } }
        public string d_City { set { debtor.City = value; } }
        public string d_Country { set { debtor.Country = value; } }
        public string I_Notes { get; set; }
        public string I_Notes_mm { get; set; }

        public string I_DossierNo { set { Invoice.DossierNo= value;
                debtor.DossierNo = value;
            } }
        public string I_InvoiceNo { set { Invoice.InvoiceNo = value; } }
        public string I_Currency { set { Invoice.Currency = value; } }
        public string I_InvoiceDate
        {
            set {
                    if(DateTime.TryParse(value,out DateTime date))
                    Invoice.InvoiceDate= date;
            }
        }
        public string I_Expired {
            set
            {
                if (DateTime.TryParse(value, out DateTime date))
                    Invoice.ExpiredDate = date;
            }
        }
        public string I_PaymentDate {
            set {
                if (DateTime.TryParse(value, out DateTime parsedValue))
                    Invoice.PaymentDate= parsedValue;
            }
        }
        public string I_Amount {
            set
            {
                if (float.TryParse(value, out float parsedValue))
                    Invoice.Amount = parsedValue;
                else
                    Invoice.Amount = 0;
            }
        }
        public string I_Open
        {
            set
            {
                if (float.TryParse(value, out float parsedValue))
                    Invoice.Open = parsedValue;
            }
        }
        public string I_Paid
        {
            set
            {
                if (float.TryParse(value, out float parsedValue))
                    Invoice.Paid = parsedValue <0? parsedValue*-1:parsedValue;
                else
                    Invoice.Paid = 0;
            }
        }
        public string I_Paidmm
        {
            set
            {
                if (float.TryParse(value, out float parsedValue))
                    Invoice.Paidmm = parsedValue;
                else
                    Invoice.Paidmm = 0;
            }
        }
        public string I_PaidClient
        {
            set
            {
                if (float.TryParse(value, out float parsedValue))
                    Invoice.PaidClient = parsedValue;
                else
                    Invoice.PaidClient = 0;
            }
        }
        public string I_Interest
        {
            set
            {
                if (float.TryParse(value, out float parsedValue))
                    Invoice.Interest = parsedValue;
                else
                    Invoice.Interest = 0;
            }
        }
        public string I_CollectionFee
        {
            set
            {
                if (float.TryParse(value, out float parsedValue))
                    Invoice.CollectionFee = parsedValue;
                else
                    Invoice.CollectionFee = 0;
            }
        }
        public string I_AdminCosts
        {
            set
            {
                if (float.TryParse(value, out float parsedValue))
                    Invoice.AdminCosts = parsedValue;
                else
                    Invoice.AdminCosts = 0;
            }
        }
        public int I_Closed
        {
            set
            {
                    Invoice.Closed = value==1;
            }
        }
        public string I_DisputeAction { set { Invoice.DisputeAction= value; } }
        public string I_ActionDate
        {
            set
            {
                if (DateTime.TryParse(value, out DateTime parsedValue))
                    Invoice.ActionDate= parsedValue;
            }
        }
        public string I_Action { set { Invoice.Action = value; } }
        public string I_Status { set { Invoice.StatusCatalog.Catalog = (value); } }

        public string I_Paractitioner { set { Invoice.Paractitioner = value; } }

        private int GetStatuValue(string value)
        {
            return InvoiceStatusCatalog.ToInt(value);
        }

        public Debtors.Debtor GetDebtor()
        {
            debtor.Invoices = new List<Invoices.Invoice>();

            try
            {
                if (!string.IsNullOrEmpty(I_Notes))
                {
                    var exp = @"\d{1,2}-\d{1,2}-\d{4}";
                    var dates = Regex.Matches(I_Notes, exp);
                    var notes = Regex.Split(I_Notes, $"{exp}");
                    Invoice.Notes = Invoice.Notes ?? new List<InvoiceNotes.InvoiceNote>();
                    int i = 0;
                    notes = notes.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    foreach (var matches in dates)
                    {

                        string[] formats = { "dd-M-yyyy", "d-M-yyyy",  "MM/dd/yyyy hh:mm:ss tt", "yyyy-MM-dd hh:mm:ss" };
                        var flag = DateTime.TryParseExact(matches.ToString(),
                           formats,
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out DateTime date);


                        if (flag)
                        {
                            Invoice.Notes.Add(new InvoiceNotes.InvoiceNote
                            {
                                NoteDate = date,
                                Notes = notes[i]?.Replace(':',' '),
                                Status = "Active",
                            });
                        }
                        i++;
                    }
                }
            }
            catch (Exception e)
            {

            }
            debtor.Invoices.Add(Invoice);

            return debtor;
        }
    }
}
