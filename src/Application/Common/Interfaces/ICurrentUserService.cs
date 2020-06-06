using TimeKeeper.Application.Common.Models;

namespace TimeKeeper.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        public UserDto User { get; }
    }
}
