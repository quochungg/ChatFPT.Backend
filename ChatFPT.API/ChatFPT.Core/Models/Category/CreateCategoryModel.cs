
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace ChatFPT.Core.Models.Category
{
    public class CreateCategoryModel
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }

        public void ValidateFields()
        {
            ValidateField(CategoryName!, "Tên phân loại");
            ValidateField(Description!, "Chi tiết");
        }

        public void ValidateField(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, $"{fieldName} không thể để trống hoặc chỉ chứa khoảng trắng.");
            }
            value = value.Trim();
            // Kiểm tra ký tự đặc biệt
            if (value.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch)))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, $"{fieldName} không được chứa ký tự đặc biệt.");
            }
            else if (value != value.Trim())
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, $"{fieldName} không được chứa khoảng trắng đầu hoặc cuối.");
            }
            else if (Regex.IsMatch(value, @"\s{2,}"))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, $"{fieldName} không được chứa nhiều khoảng trắng liên tiếp giữa các từ.");
            }
            else if (value.Any(char.IsDigit))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, $"{fieldName} không được chứa số.");
            }
        }

    }

}
