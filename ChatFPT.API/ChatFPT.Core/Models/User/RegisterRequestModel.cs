using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace ChatFPT.Core.Models.User
{
    public class RegisterRequestModel
    {
        public required string UserName { get; set; }
        public required string PasswordHash { get; set; }
        public required string RoleId { get; set; }
        public string? FullName { get; set; }

        public void CheckValid()
        {
            if (string.IsNullOrWhiteSpace(UserName) || !Regex.IsMatch(UserName, @"^[a-zA-Z0-9]+$"))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "UserName chỉ được chứa chữ cái và số.");
            }
            if (string.IsNullOrWhiteSpace(PasswordHash) || PasswordHash.Length < 6 ||
                !Regex.IsMatch(PasswordHash, @"[A-Z]") || !Regex.IsMatch(PasswordHash, @"\W"))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Password phải có tối thiểu 6 kí tự, 1 kí tự viết hoa, 1 kí tự đặc biệt.");
            }
            if (!string.IsNullOrWhiteSpace(FullName) && !Regex.IsMatch(FullName, @"^[a-zA-ZÀ-ỹ\s]+$"))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "FullName không được chứa kí tự đặc biệt và số.");
            }
        }
    }
}
