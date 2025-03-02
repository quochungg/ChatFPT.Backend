
using ChatFPT.Core.Constaints;
using Microsoft.AspNetCore.Http;

namespace ChatFPT.Core.Base
{
    public class BaseResponseModel<T>
    {
        public T? Data { get; set; }
        public object? AdditionalData { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public string Code { get; set; }

        public BaseResponseModel(int statusCode, string code, T? data, object? additionalData = null, string? message = null)
        {
            this.StatusCode = statusCode;
            this.Code = code;
            this.Data = data;
            this.AdditionalData = additionalData;
            this.Message = message;
        }
        public BaseResponseModel(int statusCode, string code, string? message)
        {
            this.StatusCode = statusCode;
            this.Code = code;
            this.Message = message;
        }


        public static BaseResponseModel<T> OkResponseModel(T data, object? additionalData = null, string code = ResponseCodeConstaints.SUCCESS)
        {
            return new BaseResponseModel<T>(StatusCodes.Status200OK, code, data, additionalData);
        }

        public static BaseResponseModel<T> OkDataResponse<T>(T data, string message, string code = ResponseCodeConstaints.SUCCESS)
        {
            return new BaseResponseModel<T>(StatusCodes.Status200OK, code, data, null, message);
        }

        public static BaseResponseModel<T> OkMessageResponseModel(string message, string code = ResponseCodeConstaints.SUCCESS)
        {
            return new BaseResponseModel<T>(StatusCodes.Status200OK, code, message);
        }

        public static BaseResponseModel<T> NotFoundResponseModel(T? data, object? additionalData = null, string code = ResponseCodeConstaints.NOT_FOUND)
        {
            return new BaseResponseModel<T>(StatusCodes.Status404NotFound, code, data, additionalData);
        }

        public static BaseResponseModel<T> BadRequestResponseModel(T? data, object? additionalData = null, string code = ResponseCodeConstaints.FAILED)
        {
            return new BaseResponseModel<T>(StatusCodes.Status400BadRequest, code, data, additionalData);
        }

        public static BaseResponseModel<T> InternalErrorResponseModel(T? data, object? additionalData = null, string code = ResponseCodeConstaints.FAILED)
        {
            return new BaseResponseModel<T>(StatusCodes.Status500InternalServerError, code, data, additionalData);
        }
    }
}
