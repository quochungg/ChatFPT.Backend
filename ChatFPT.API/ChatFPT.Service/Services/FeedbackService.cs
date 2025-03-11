

using AutoMapper;
using ChatFPT.Application.Interface;
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Models.Feedback;
using ChatFPT.Core.Models.Role;
using ChatFPT.Core.Pagination;
using ChatFPT.Core.Utils;
using ChatFPT.Domain.Entities;
using ChatFPT.Service.Insfracstructure;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ChatFPT.Service.Services
{
    public class FeedbackService : IFeedBackService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public FeedbackService(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task CreateFeedbackAsync(CreateFeedbackModel model)
        {
            Feedback feedback = _mapper.Map<Feedback>(model);
            feedback.CreatedBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            feedback.CreatedTime = DateTime.Now;
            await _unitOfWork.GetRepository<Feedback>().AddAsync(feedback);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteFeedbackAsync(string id)
        {
            Feedback feedback = await _unitOfWork.GetRepository<Feedback>().Entities.FirstOrDefaultAsync(r => r.Id == id && !r.DeleteTime.HasValue)
                   ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy ID");

            feedback.DeleteTime = DateTime.Now;
            feedback.DeleteBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            await _unitOfWork.GetRepository<Feedback>().UpdateAsync(feedback);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<ResponseFeedbackModel>> GetFeedbacksAsync(string? searchName, int index, int PageSize)
        {
            IQueryable<ResponseFeedbackModel> query = from feedback in
                                              _unitOfWork.GetRepository<Feedback>().Entities
                                                      where !feedback.DeleteTime.HasValue
                                                      select new ResponseFeedbackModel
                                                      {
                                                          AnswerId = feedback.AnswerId,
                                                          Rate = feedback.Rate,
                                                          CreatedTime = feedback.CreatedTime,
                                                          CreatedBy = feedback.CreatedBy,
                                                          LastUpdatedBy = feedback.LastUpdateBy,
                                                          LastUpdatedTime = feedback.LastUpdateTime
                                                      };

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(s => s.CreatedBy!.Contains(searchName));
            }

            PaginatedList<ResponseFeedbackModel> paginatedFeedback = await _unitOfWork.GetRepository<ResponseFeedbackModel>().GetPagingAsync(query, index, PageSize);
            return paginatedFeedback;
        }

        public async Task<ResponseFeedbackModel> GetFeedbackId(string id)
        {
            Feedback feedback = await _unitOfWork.GetRepository<Feedback>().Entities.FirstOrDefaultAsync(r => r.Id == id && !r.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy Id");
            return new ResponseFeedbackModel
            {
                AnswerId = feedback.Id,
                CreatedBy = feedback.CreatedBy,
                Rate = feedback.Rate,
                LastUpdatedBy = feedback.LastUpdateBy,
                LastUpdatedTime = feedback.LastUpdateTime
            };
        }

        public async Task UpdateFeedbackAsync(UpdateFeedbackModel model)
        {
            Feedback feedback = await _unitOfWork.GetRepository<Feedback>().Entities.FirstOrDefaultAsync(r => r.AnswerId == model.AnswerId && !r.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy Id");

            _mapper.Map(model, feedback);
            feedback.LastUpdateTime = DateTime.Now;
            feedback.LastUpdateBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            
                 await _unitOfWork.GetRepository<Feedback>().UpdateAsync(feedback);
            await _unitOfWork.SaveAsync();
        }
    }
}
