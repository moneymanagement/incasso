namespace Incasso.MultiTenancy.Dto
{
    public class CriteriaAdministratorSearch
    {
        public int RequestedPage { get; set; }
        public int PageSize { get; set; }
        public int SkipCount { get; set; }
        public string Search { get; set; }
        public int MaxResultCount { get; set; }
    }
}