namespace TimeKeeper.Application.Common.Models
{
    //TODO: probably its even domain objects
    public static class Roles
    {
        public const string User = "User";
        public const string UserManager = "UserManager";
        public const string Admin = "Admin";

        public const string CanManageUsers = Admin + "," + UserManager;
        public static readonly string[] All = { User, UserManager, Admin };
    }
}
