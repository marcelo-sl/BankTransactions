using System.Net;

namespace BankTransactions.Api.Models;

public class HttpErrorResult(HttpStatusCode code, string message)
{
    public HttpStatusCode Code { get; set; } = code;
    public string Message { get; set; } = message;

    public static IResult BadRequest(string message)
    {
        var error = new HttpErrorResult(HttpStatusCode.BadRequest, message);
        return Results.BadRequest(error);
    }

    public static IResult NotFound(string message = "Resource not found")
    {
        var error = new HttpErrorResult(HttpStatusCode.BadRequest, message);
        return Results.NotFound(error);
    }
}
