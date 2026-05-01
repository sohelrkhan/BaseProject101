namespace SadaqaAccounting.Api.AuthorizeExtensions
{
    public class CheckAuthorizeAttribute : TypeFilterAttribute
    {
        public CheckAuthorizeAttribute(string controllerName, string actionName) : base(typeof(AuthorizationFilter))
        {
            Arguments = new object[] { controllerName, actionName };
        }
    }
}