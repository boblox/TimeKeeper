namespace TimeKeeper.Application.Common.Models
{
    public class UserDto
    {
        public virtual string UserName { get; }
        public virtual string UserId { get; }
        public virtual string Role { get; }

        public UserDto() { }

        public UserDto(string userName, string userId, string role)
        {
            UserName = userName;
            UserId = userId;
            Role = role;
        }
    }
}
