using Microsoft.AspNetCore.Mvc;

namespace Task_Management_System_API.Attributes
{
    public class AuthAttribute : TypeFilterAttribute
    {
        public AuthAttribute(string actionName, string controller) : base(typeof(AuthorizeAction))
        {
            Arguments = new object[] {
            actionName,
            controller
            };
        }
    }
}
