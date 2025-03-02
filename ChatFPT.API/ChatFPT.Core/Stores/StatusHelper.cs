

using ChatFPT.Core.Utils;

namespace ChatFPT.Core.Stores
{
    public enum StatusHelper
    {
        [CustomName("Success")]
        OK = 200,

        [CustomName("Bad Request")]
        BadRequest = 400,

        [CustomName("Unauthorized")]
        Unauthorized = 401,

        [CustomName("Internal Server Error")]
        ServerError = 500,

        [CustomName("Not found Error")]
        NotFound = 404
    }
}
