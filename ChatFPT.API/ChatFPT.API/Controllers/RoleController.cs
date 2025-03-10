
using ChatFPT.Core.Base;
using ChatFPT.Core.Models.Role;
using ChatFPT.Core.Pagination;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoleAsync(string? searchName, int index = 1, int PageSize = 10) {
            PaginatedList<ResponseRoleModel> list = await _roleService.GetAllRole(searchName, index, PageSize);
            return Ok(BaseResponse<IReadOnlyCollection<ResponseRoleModel>>.OkDataResponse(list, "Lấy danh sách thành công"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoleAsync(CreateRoleModel model) {
            await _roleService.CreateRole(model);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Tạo mới Role thành công"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoleAsync(UpdateRoleModel model) { 
            await _roleService.UpdateRole(model);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Cập nhật Role thành công"));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRoleAsync (Guid RoleId)
        {
            await _roleService.DeleteRole(RoleId);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Xóa Role thành công"));
        }

    }
}
