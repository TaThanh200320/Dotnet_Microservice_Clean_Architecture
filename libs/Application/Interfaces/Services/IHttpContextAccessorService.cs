using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services;

public interface IHttpContextAccessorService
{
    HttpContext? HttpContext { get; }
    string? GetRouteValue(string key);
    string? GetHttpMethod();
    string? GetRequestPath();
    string? GetId();
}
