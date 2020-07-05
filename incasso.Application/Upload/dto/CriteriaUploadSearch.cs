using System;

namespace Incasso.Upload.Dto
{
    public class CriteriaUploadSearch
    {
        public int RequestedPage { get; set; }
        public int PageSize { get; set; }
        public int SkipCount { get; set; }
        public string Search { get; set; }
        public DateTime? Date { get; set; }
        public int MaxResultCount { get; set; }
    }
}