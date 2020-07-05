using incasso.AppDto;

namespace Incasso.MultiTenancy.Dto
{
    public class CriteriaDebtorSearch:BaseCriteria
    {
        public int? AdminId { get; set; }
    }
}