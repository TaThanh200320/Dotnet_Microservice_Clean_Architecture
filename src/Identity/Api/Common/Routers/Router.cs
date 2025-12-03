using Contracts.Constants;

namespace IdentityApi.Common.Routers;

public static class Router
{
    public static class UserRoute
    {
        public const string Users = nameof(Users);
        public const string GetUpdateDelete = $"{nameof(Users)}/" + "{" + RoutePath.Id + "}";
        public const string GetRouteName = $"{Users}DetailEndpoint";

        public const string Profile = $"{nameof(Users)}/{nameof(Profile)}";

        public const string Tags = $"{nameof(Users)} endpoint";
    }

    public static class AuthRoute
    {
        public const string Auth = nameof(Auth);

        public const string Login = $"{nameof(Auth)}/{nameof(Login)}";
        public const string RefreshToken = $"{nameof(Auth)}/{nameof(RefreshToken)}";
        public const string ChangePassword = $"{nameof(Auth)}/{nameof(ChangePassword)}";
        public const string RequestResetPassword = $"{nameof(Auth)}/{nameof(RequestResetPassword)}";
        public const string ResetPassword = $"{nameof(Auth)}/{nameof(ResetPassword)}";

        public const string Tags = $"{nameof(Auth)} endpoint";
    }

    public static class RoleRoute
    {
        public const string Roles = nameof(Roles);

        public const string GetUpdateDelete = $"{nameof(Roles)}/" + "{" + RoutePath.Id + "}";

        public const string GetRouteName = $"{Roles}DetailEndpoint";

        public const string Tags = $"{nameof(Roles)} endpoint";
    }

    public static class PermissionRoute
    {
        public const string Permissions = nameof(Permissions);

        public const string Tags = $"{nameof(Permissions)} endpoint";
    }

    public static class AuditLogRoute
    {
        public const string AuditLog = nameof(AuditLog);
        public const string Tags = $"{nameof(AuditLog)} endpoint";
    }
}
