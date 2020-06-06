using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeKeeper.Application.WorkingHours.CreateWorkingHours;
using TimeKeeper.Application.WorkingHours.DeleteWorkingHours;
using TimeKeeper.Application.WorkingHours.ExportWorkingHours;
using TimeKeeper.Application.WorkingHours.GetWorkingHours;
using TimeKeeper.Application.WorkingHours.UpdateWorkingHours;

namespace TimeKeeper.WebUI.Controllers
{
    [Route("api/working-hours")]
    public class WorkingHoursController : ApiController
    {
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<int>> Create([FromBody] CreateWorkingHoursCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateWorkingHoursCommand command)
        {
            command.Id = id;
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteWorkingHoursCommand(id));
            return NoContent();
        }

        [HttpGet]
        [Authorize]
        public async Task<WorkingHoursSetDto> GetList(string userName, DateTime? start, DateTime? end)
        {
            return await Mediator.Send(new GetWorkingHoursQuery(userName, start, end));
        }

        [HttpGet("export")]
        [Authorize]
        public async Task<IEnumerable<ExportWorkingHoursDto>> Export(string userName, DateTime? start, DateTime? end)
        {
            return await Mediator.Send(new ExportWorkingHoursQuery(userName, start, end));
        }
    }
}
