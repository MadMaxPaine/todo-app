using Microsoft.AspNetCore.Mvc;

namespace ToDoBackEnd.Middleware;

public class ExceptionMiddleware
{
 private readonly RequestDelegate _next;
 private readonly ILogger<ExceptionMiddleware> _logger;

 public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
 {
  _next = next;
  _logger = logger;
 }

 public async Task Invoke(HttpContext context)
 {
  try
  {
   await _next(context);
  }
  catch (Exception ex)
  {
   _logger.LogError(ex, "Unhandled exception");

   var problem = new ProblemDetails
   {
    Title = "Server error",
    Status = StatusCodes.Status500InternalServerError,
    Detail = "Something went wrong on server"
   };

   context.Response.StatusCode = problem.Status.Value;
   await context.Response.WriteAsJsonAsync(problem);
  }
 }
}