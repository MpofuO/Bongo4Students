using Bongo.Models;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class MyAuthorize : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!IsAuthorized(context))
        {
            context.Result = context.Result;
        }
    }
    private bool IsAuthorized(AuthorizationFilterContext context)
    {
        return true;
    }
}
