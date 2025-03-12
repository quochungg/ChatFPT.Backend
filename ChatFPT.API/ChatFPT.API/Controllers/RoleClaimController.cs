using ChatFPT.Core.Base;
using ChatFPT.Core.Models.Role;
using ChatFPT.Core.Pagination;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatFPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleClaimController : ControllerBase
    {
        private readonly IRoleClaimService _roleClaimService;

        public RoleClaimController(IRoleClaimService roleClaimService)
        {
            _roleClaimService = roleClaimService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoleClaims(string? searchValue, int index = 1, int pageSize = 10)
        {
            PaginatedList<ResponseRoleClaimModel> paginatedList = await _roleClaimService.GetAllRoleClaims(searchValue, index, pageSize);
            return Ok(BaseResponse<IReadOnlyCollection<ResponseRoleClaimModel>>.OkDataResponse(paginatedList, "Lấy danh sách thành công"));
        }


        [HttpPost]
        public async Task<IActionResult> CreateRoleClaim(CreateRoleClaim modek)
        {
            await _roleClaimService.CreateRoleClaim(modek);
            return Ok(BaseResponse<string>.OkMessageResponseModel("tạo mới thành công"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoleClaim(UpdateRoleClaim modek)
        {
            await _roleClaimService.UpdateRoleClaim(modek);
            return Ok(BaseResponse<string>.OkMessageResponseModel("cập nhật thành công"));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRoleClaim(string? roleClaimId)
        {
            await _roleClaimService.DeleteRoleClaim(roleClaimId);
            return Ok(BaseResponse<string>.OkMessageResponseModel("Xóa thành công"));
        }
    }

}
