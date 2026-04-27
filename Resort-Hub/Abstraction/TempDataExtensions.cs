using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Resort_Hub.Abstraction;

public static class TempDataExtensions
{
    public static void SetError(this ITempDataDictionary tempData, Error error)
    {
        tempData["ErrorCode"] = error.Code;
        tempData["ErrorDescription"] = error.Description;
        tempData["ErrorType"] = error.Type.ToString();
    }

    public static void SetMessageModal(this ITempDataDictionary tempData, ModalMessage msg)
    {
        tempData["ModalTitle"] = msg.Title;
        tempData["ModalMessage"] = msg.Message;
        tempData["ModalType"] = msg.Type.ToString();
    }
}