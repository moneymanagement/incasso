using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Incasso.Authorization.Users
{
    public class UsersAdministrators:FullAuditedEntity
    {
        [ForeignKey("Users")]
        public long User_Id { get; set; }
        [ForeignKey("Administrator")]
       public int Administrator_Id { get; set; }

        public Administrators.Administrator Administrator { get; set; }
        public User Users{ get; set; }
 
    }
}