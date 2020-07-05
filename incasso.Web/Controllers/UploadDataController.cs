using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;
using incasso.Jobs;
using Incasso.Upload;
using Incasso.Upload.Dto;
using Incasso.Users;

namespace Incasso.Web.Controllers
{
    [AbpMvcAuthorize]
    public class UploadDataController : ControllerBase
    {
        private IUploadDataAppService UploadAppService;
        private IUsersAppService usersAppService;

        public UploadDataController( IUploadDataAppService UploadAppService, IUsersAppService usersAppService)
        {
            this.UploadAppService = UploadAppService;
            this.usersAppService = usersAppService;
        }
        public async Task<ActionResult> Index()
        {
            int? currentPage = 0; int? pageSize = 50;
            var model = await UploadAppService.GetGrid(new CriteriaUploadSearch { PageSize = pageSize.Value, MaxResultCount = pageSize.Value, RequestedPage = currentPage.Value });
            return View(model);
        }
        public async Task<ActionResult> GetGrid(string search = "", int? requestedPage = 0, int? pageSize = 50)
        {
            var model = await UploadAppService.GetGrid(new CriteriaUploadSearch { Search = search, PageSize = pageSize.Value, MaxResultCount = pageSize.Value, RequestedPage = requestedPage.Value });
            return View("_GetGrid", model);
        }


        [HttpGet]
        public async Task<ActionResult> Download(int? Id)
        {
            var fileDetals = await UploadAppService.Get(new Abp.Application.Services.Dto.EntityDto<int> { Id = Id.Value });
            var virtualPath = $"~/UploadImages/{DateTime.Now.ToString("yy-MM-dd")}/";

            byte[] fileBytes = System.IO.File.ReadAllBytes(fileDetals.PhysicalFilePath);
            return File(fileBytes, "application/vnd.ms-excel", fileDetals.FileName);

        }
        [HttpPost]
        public ActionResult Upload()
        {
            try
            {
                var fileName = string.Empty;
                var physicalPath = string.Empty;
                var mappedPath = string.Empty;
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        fileName = Path.GetFileName(file.FileName);
                        var virtualPath = $"~/UploadImages/{DateTime.Now.ToString("yy-MM-dd")}/";
                        var root = Server.MapPath(virtualPath);
                        if (!Directory.Exists(root))
                            Directory.CreateDirectory(root);
                        mappedPath = Guid.NewGuid() + fileName;
                        var path = Path.Combine(root, mappedPath);
                        file.SaveAs(path);
                        physicalPath = path;

                        //jobService.ProcessUploadFile(physicalPath);
                    }
                }
                return Json(new { PhysicalFilePath = physicalPath, FileName = fileName, PhysicalFileName = mappedPath });
            }
            catch (System.Exception ex)
            {
                return Json(new { Message = L("Somethingwentwrong") });
            }
        }
    }
}