using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;


namespace ChatFPT.Core.Models.Role
{
    public class UpdateRoleModel
    {
        public Guid? Id  { get; set; }
        public string? Name { get; set; }
        public string? FullName { get; set; }
        public void ValidateFields()
        {
            ValidateField(Name!, "Tên vai trò");
            ValidateField(FullName!, "FullName");
        }

        // Phương thức riêng để xác thực từng trường
        private void ValidateField(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, $"{fieldName} không thể để trống hoặc chỉ chứa khoảng trắng.");
            }
            value = value.Trim();

            // Kiểm tra ký tự đặc biệt
            if (value.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch)))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, $"{fieldName} không được chứa ký tự đặc biệt.");
            }
            else if (fieldName.Equals("FullName", StringComparison.OrdinalIgnoreCase))
            {
                if (value != value.Trim())
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, $"{fieldName} không được chứa khoảng trắng đầu hoặc cuối.");
                }
                if (Regex.IsMatch(value, @"\s{2,}"))
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, $"{fieldName} không được chứa nhiều khoảng trắng liên tiếp giữa các từ.");
                }
                if (value.Any(char.IsDigit))
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, $"{fieldName} không được chứa số.");
                }
            }
            else if (fieldName.Equals("Tên vai trò", StringComparison.OrdinalIgnoreCase))
            {
                if (value.Contains(" "))
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, $"{fieldName} không được chứa khoảng trắng.");
                }
            }
            else
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, $"Trường không xác định: {fieldName}");
            }
        }
    }
}
