using System;
using System.Threading;
using System.Threading.Tasks;
using HttpDDoS.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HttpDDoS.Master.Controllers
{
    [ApiController]
    [Route("/")]
    public class IndexController : ControllerBase
    {
        static int Requests = 0;

        private readonly IStatusService _statusService;

        public IndexController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        [Route("[action]")]
        [HttpGet]
        public ActionResult<Status> Status() => _statusService.Status;

        [Route("[action]")]
        [HttpPost]
        public ActionResult<Status> Status(Status model)
        {
            _statusService.SetStatus(model);
            return model;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> Test()
        {
            await Task.Delay(100);
            return Ok(++Requests);
        }
    }
}
