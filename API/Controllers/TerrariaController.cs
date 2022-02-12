namespace House.API.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using HLL.TerrariaRunner.Interfaces;

    [Route("Terraria")]
    public class TerrariaController : BaseController
    {
        private readonly ITerrariaRunner _terrariaRunner;
        public TerrariaController(ITerrariaRunner terrariaRunner)
        {
            _terrariaRunner = terrariaRunner;
        }

        [HttpGet("Start")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public IActionResult Start()
        {
            return ExecuteAndMapToActionResultSync(() => _terrariaRunner.Start());
        }

        [HttpGet("GetLogsTail/{secondsToWait}")]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
        public IActionResult GetLogsTail([FromRoute] int secondsToWait)
        {
            return ExecuteAndMapToActionResultSync(() => _terrariaRunner.GetCurrentLogsTail(secondsToWait));
        }

        [HttpPost("Command")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public Task<IActionResult> Command([FromBody] string command)
        {
            return ExecuteAndMapToActionResult<Task>(() => _terrariaRunner.InputCommand(command));
        }
    }
}