

using AutoMapper;
using ChatFPT.Application.Interface;
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Core.Models.Feedback;
using ChatFPT.Core.Pagination;
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
            _contextAccessor = httpContextAccessor;
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
            IQueryable<ResponseFeedbackModel> query = from feedback in _unitOfWork.GetRepository<Feedback>().Entities
                                                      join answer in _unitOfWork.GetRepository<Answer>().Entities on feedback.AnswerId equals answer.Id
                                                      join question in _unitOfWork.GetRepository<Question>().Entities on answer.QuestionId equals question.Id
                                                      where !feedback.DeleteTime.HasValue
                                                      select new ResponseFeedbackModel
                                                      {
                                                          AnswerId = feedback.AnswerId,
                                                          QuestionId = question.Id,
                                                          Rate = feedback.Rate,
                                                          Note = feedback.Note,
                                                          CreatedTime = feedback.CreatedTime,
                                                          AnswerContent = answer.Content,
                                                          QuestionContent = question.Content,
                                                      };

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(s => s.Note!.Contains(searchName));
            }

            PaginatedList<ResponseFeedbackModel> paginatedFeedback = await _unitOfWork.GetRepository<ResponseFeedbackModel>().GetPagingAsync(query, index, PageSize);
            return paginatedFeedback;
        }

        public async Task<ResponseFeedbackModel> GetFeedbackId(string id)
        {
            Feedback feedback = await _unitOfWork.GetRepository<Feedback>().Entities.FirstOrDefaultAsync(r => r.Id == id && !r.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy Id");

            Answer? answer = await _unitOfWork.GetRepository<Answer>().Entities.FirstOrDefaultAsync(a => a.Id == feedback.AnswerId);

            Question? question = await _unitOfWork.GetRepository<Question>().Entities.FirstOrDefaultAsync(qt => qt.Id == answer.QuestionId);
            return new ResponseFeedbackModel
            {
                AnswerId = feedback.Id,
                Rate = feedback.Rate,
                Note = feedback.Note,
                AnswerContent = answer!.Content,
                QuestionContent = question!.Content,
                CreatedTime = feedback.CreatedTime,
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
