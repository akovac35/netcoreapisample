using System.Globalization;

namespace Domain
{
    public static class Constants
    {
        public const string ApplicationAcronym = "SMPL";
        public const string ApplicationName = "Sample";

        public const string AdminRoleKey = "AdminRole";
        public const string UserRoleKey = "UserRole";
        public const string RequireAdminRolePolicyKey = "RequireAdminRole";
        public const string JwtAudience = "public";

        public static CultureInfo InvariantCulture { get; } = CultureInfo.InvariantCulture;
    }
}
