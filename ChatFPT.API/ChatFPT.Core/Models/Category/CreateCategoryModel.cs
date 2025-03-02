
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using Microsoft.AspNetCore.Http;

namespace ChatFPT.Core.Models.Category
{
    public class CreateCategoryModel
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }

        public void checkValid()
        {
            if(string.IsNullOrEmpty(CategoryName))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không được để trống CategoryName");
            }
        }
        
    }
    
}
