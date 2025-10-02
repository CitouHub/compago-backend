using Compago.Common;
using Microsoft.AspNetCore.Mvc;

namespace Compago.API.Security
{
    public class AuthorizeRoleAttribute : TypeFilterAttribute
    {
        public AuthorizeRoleAttribute(params Role[] roles) : base(typeof(AuthorizeUserFilter))
        {
            Arguments = [roles];
        }
    }
}
