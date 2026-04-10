using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Resort_Hub.Handlers.ErrorHandler;

public static class TempDataExtensions
{
    public static void SetError(this ITempDataDictionary tempData, Error error)
    {
        tempData["ErrorCode"] = error.Code;
        tempData["ErrorDescription"] = error.Description;
        tempData["ErrorType"] = error.Type.ToString();
    }
}
