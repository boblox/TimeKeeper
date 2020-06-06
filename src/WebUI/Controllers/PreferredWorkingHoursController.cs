using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeKeeper.Application.PreferredWorkingHours.UpdatePreferredWorkingHours;

namespace TimeKeeper.WebUI.Controllers
{
    [Route("api/preferred-working-hours")]
    public class PreferredWorkingHoursController : ApiController
    {
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Update([FromBody]UpdatePreferredWorkingHoursCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
