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
    public class RoleClaimsController : ControllerBase
    {
        private readonly IRoleClaimService _roleClaimService;
        private readonly IRedisService _redisService;

        public RoleClaimsController(IRoleClaimService roleClaimService, IRedisService redisService)
        {
            _roleClaimService = roleClaimService;
            _redisService = redisService;
        }

        [HttpGet]
        [CacheAtribute(1000)]
        public async Task<IActionResult> GetAllRoleClaims(string? searchValue, int index = 1, int pageSize = 10)
        {
            PaginatedList<ResponseRoleClaimModel> paginatedList = await _roleClaimService.GetAllRoleClaims(searchValue, index, pageSize);
            return Ok(BaseResponse<IReadOnlyCollection<ResponseRoleClaimModel>>.OkDataResponse(paginatedList, "Lấy danh sách thành công"));
        }


        [HttpPost]
        public async Task<IActionResult> CreateRoleClaim(CreateRoleClaim model)
        {
            await _roleClaimService.CreateRoleClaim(model);
            await _redisService.RemoveCacheResponseAsync("/api/roleclaims");
            return Ok(BaseResponse<string>.OkMessageResponseModel("Tạo mới thành công"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoleClaim(UpdateRoleClaim model)
        {
            await _roleClaimService.UpdateRoleClaim(model);
            await _redisService.RemoveCacheResponseAsync("/api/roleclaims");
            return Ok(BaseResponse<string>.OkMessageResponseModel("Cập nhật thành công"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleClaim(string id)
        {
            await _roleClaimService.DeleteRoleClaim(id);
            await _redisService.RemoveCacheResponseAsync("/api/roleclaims");
            return Ok(BaseResponse<string>.OkMessageResponseModel("Xóa thành công"));
        }

        [HttpGet("{id}")]
        [CacheAtribute(1000)]
        public async Task<IActionResult> GetRoleClaimById(string id)
        {
            await _roleClaimService.GetRoleClaimsById(id);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Lấy data thành công"));
        }
    }

}
