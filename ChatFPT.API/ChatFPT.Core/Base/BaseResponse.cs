using ChatFPT.Core.Constaints;
using ChatFPT.Core.Stores;
using ChatFPT.Core.Utils;
using Microsoft.AspNetCore.Http;

namespace ChatFPT.Core.Base
{
    public class BaseResponse<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public StatusHelper StatusCode { get; set; }
        public string? Code { get; set; }
        public BaseResponse(StatusHelper statusCode, string code, T? data, string? message)
        {
            Data = data;
            Message = message;
            StatusCode = statusCode;
            Code = code;
        }

        public BaseResponse(StatusHelper statusCode, string code, T? data)
        {
            Data = data;
            StatusCode = statusCode;
            Code = code;
        }

        public BaseResponse(StatusHelper statusCode, string code, string? message)
        {
            Message = message;
            StatusCode = statusCode;
            Code = code;
        }

        public static BaseResponse<T> OkResponse(T? data)
        {
            return new BaseResponse<T>(StatusHelper.OK, StatusHelper.OK.Name(), data);
        }
        public static BaseResponse<T> OkResponse(string? mess)
        {
            return new BaseResponse<T>(StatusHelper.OK, StatusHelper.OK.Name(), mess);
        }
        public static BaseResponseModel<T> OkMessageResponseModel(string message, string code = ResponseCodeConstaints.SUCCESS)
        {
            return new BaseResponseModel<T>(StatusCodes.Status200OK, code, message);
        }
        public static BaseResponseModel<T> OkDataResponse<T>(T data, string code = ResponseCodeConstaints.SUCCESS)
        {
            return new BaseResponseModel<T>(StatusCodes.Status200OK, code, data);
        }
    }
}
