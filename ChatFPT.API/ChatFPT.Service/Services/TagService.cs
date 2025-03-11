

using AutoMapper;
using ChatFPT.Application.Interface;
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Core.Models.Tag;
using ChatFPT.Core.Pagination;
using ChatFPT.Domain.Entities;
using ChatFPT.Service.Insfracstructure;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ChatFPT.Service.Services
{
    public class TagService : ITagService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public TagService(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _contextAccessor = httpContextAccessor;
        }
        public async Task CreateTag(CreateTagModel model)
        {
            if (await _unitOfWork.GetRepository<Category>().Entities.FirstOrDefaultAsync(c => c.Id == model.CategoryId && !c.DeleteTime.HasValue) == null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "CategoryId không tồn tại");
            }

            Tag tag = _mapper.Map<Tag>(model);
            tag.CreatedBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            tag.CreatedTime = DateTime.Now;

            await _unitOfWork.GetRepository<Tag>().AddAsync(tag);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteTag(string id)
        {
            Tag tag = await _unitOfWork.GetRepository<Tag>().Entities.FirstOrDefaultAsync(t => t.Id == id && !t.DeleteTime.HasValue)
                 ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy TagId");

            tag.DeleteTime = DateTime.Now;
            tag.DeleteBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            await _unitOfWork.GetRepository<Tag>().UpdateAsync(tag);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<ResponseTagModel>> GetAllTag(string? searchName, int index, int PageSize)
        {
            IQueryable<ResponseTagModel> query = from t in _unitOfWork.GetRepository<Tag>().Entities
                                                 where !t.DeleteTime.HasValue
                                                 select new ResponseTagModel()
                                                 {
                                                     Id = t.Id,
                                                     Name = t.Name,
                                                     CategoryId = t.CategoryId,
                                                     CreatedTime = t.CreatedTime
                                                 };

            if(!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(t => t.Name!.Contains(searchName));
            }

            PaginatedList<ResponseTagModel> paginatedTag = await _unitOfWork.GetRepository<ResponseTagModel>().GetPagingAsync(query, index, PageSize);
            return paginatedTag;

        }

        public async Task<ResponseTagModel> GetTagById(string id)
        {
            Tag tag = await _unitOfWork.GetRepository<Tag>().Entities.FirstOrDefaultAsync(t => t.Id == id)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy TagId");

            ResponseTagModel model = _mapper.Map<ResponseTagModel>(tag);
            return model;
        }

        public async Task UpdateTag(UpdateTagModel model)
        {
            Tag tag = await _unitOfWork.GetRepository<Tag>().Entities.FirstOrDefaultAsync(t => t.Id == model.Id && !t.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy Tag");

            if (await _unitOfWork.GetRepository<Category>().Entities.FirstOrDefaultAsync(c => c.Id == model.CategoryId && !c.DeleteTime.HasValue) == null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "CategoryId không tồn tại");
            }

            _mapper.Map(model, tag);

            tag.LastUpdateBy =Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            tag.LastUpdateTime = DateTime.Now;

            await _unitOfWork.GetRepository<Tag>().UpdateAsync(tag);
            await _unitOfWork.SaveAsync();
        }
    }
}
