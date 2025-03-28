
using ChatFPT.API.MiddleWare.Attributes;
using ChatFPT.Core.Base;
using ChatFPT.Core.Models.Role;
using ChatFPT.Core.Pagination;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IRedisService _redisService;
        public RolesController(IRoleService roleService, IRedisService redisService)
        {
            _roleService = roleService;
            _redisService = redisService;
        }

        [HttpGet]
        [CacheAtribute(1000)]
        public async Task<IActionResult> GetAllRoleAsync(string? searchName, int index = 1, int PageSize = 10) {
            PaginatedList<ResponseRoleModel> list = await _roleService.GetAllRole(searchName, index, PageSize);
            return Ok(BaseResponse<IReadOnlyCollection<ResponseRoleModel>>.OkDataResponse(list, "Lấy danh sách thành công"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoleAsync(CreateRoleModel model) {
            await _roleService.CreateRole(model);
            await _redisService.RemoveCacheResponseAsync("/api/roles");
            return Ok(BaseResponse<string>.OkMessageResponseModel("Tạo mới Role thành công"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoleAsync(UpdateRoleModel model) { 
            await _roleService.UpdateRole(model);
            await _redisService.RemoveCacheResponseAsync("/api/roles");
            return Ok(BaseResponse<string>.OkMessageResponseModel("Cập nhật Role thành công"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleAsync (Guid id)
        {
            await _roleService.DeleteRole(id);
            await _redisService.RemoveCacheResponseAsync("/api/roles");
            return Ok(BaseResponse<string>.OkMessageResponseModel("Xóa Role thành công"));
        }

        [HttpGet("{id}")]
        [CacheAtribute(1000)]
        public async Task<IActionResult> GetRoleByIdAsync(Guid id)
        {
            ResponseRoleModel model = await _roleService.GetRoleById(id);
            return Ok(BaseResponse<string>.OkDataResponse(model, "Lấy Role thành công"));
        }
    }
}
