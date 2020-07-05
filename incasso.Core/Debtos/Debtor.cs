using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using incasso.Invoices;
using Incasso.Administrators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace incasso.Debtors
{
    public class Debtor : FullAuditedEntity, ISoftDelete,ICloneable
    {
       public virtual Administrator Administrator { get; set; }
        public string Number{get;set;}
        public string DossierNo { get; set; }
        public string Name{get;set;}
        public string Contact{get;set;}
        public string Email{get;set;}
        public string Phone{get;set;}
        public string Mobile{get;set;}
        public string Address{get;set;}
        public string Postal{get;set;}
        public string City{get;set;}
        public string Country{get;set;}
        public string Notes{get;set;}
        public string Notes_mm{get;set;}
        public int Status{get;set;}
        [ForeignKey("Administrator")]
        public int AdministratorId { get; set; }
        public List<Invoice> Invoices { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
