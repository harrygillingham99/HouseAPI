namespace House.API.Controllers
{
    using System;
    using HLL.News.Interfaces;
    using HLL.News.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using HLL.ServerStats;

    [Route("status")]
    public class StatusController : BaseController
    {
        private readonly IMemoryStatusProvider _memoryStatusProvider;

        public StatusController(IMemoryStatusProvider memoryStatusProvider)
        {
            _memoryStatusProvider = memoryStatusProvider;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(List<Status>), (int)HttpStatusCode.OK)]
        public IActionResult GetStats()
        {
            return ExecuteAndMapToActionResultSync(() =>
            {
                var result = _memoryStatusProvider.GetMemoryInfo().ToList();
                return result;
            });
        }
    }
}