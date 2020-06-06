using MediatR;

namespace TimeKeeper.Application.Identity.UpdateRole
{
    public class UpdateRoleCommand : IRequest
    {
        public string UserName { get; set; }
        public string DesiredRole { get; set; }
    }
}
