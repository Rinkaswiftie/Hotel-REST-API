using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HotelAPI.Filters
{

    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateFileAttribute : Attribute, IActionFilter
    {
        public ValidateFileAttribute()
        {

        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var file = context.HttpContext.Request.Form.Files.FirstOrDefault();
            if (file == null)
            {
                context.Result = new JsonResult(new { message = "No file found" }) { StatusCode = StatusCodes.Status400BadRequest };
                return;
            }

            var extention = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);
            var validExtensions = new List<string>() { "png", "jpg", "jpeg" };
            if (!validExtensions.Contains(extention, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new JsonResult(new { message = "File format invalid (Allowed: png, jpg, jpeg)" }) { StatusCode = StatusCodes.Status400BadRequest };
                return;
            }

            if (file.Length > (250 * 1000)) //should be file less than 250kb
            {
                context.Result = new JsonResult(new { message = "File size must be less than 250kb" }) { StatusCode = StatusCodes.Status400BadRequest };
                return;
            }

        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}