using AutoMapper;
using ChatFPT.Application.Interface;
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Core.Models.Answer;
using ChatFPT.Core.Models.Category;
using ChatFPT.Core.Pagination;
using ChatFPT.Domain.Entities;
using ChatFPT.Service.Insfracstructure;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace ChatFPT.Service.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AnswerService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task CreateAnswer(CreateAnswerModel model)
        {
            Question question = await _unitOfWork.GetRepository<Question>().Entities.FirstOrDefaultAsync(q => q.Id == model.QuestionId && !q.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy QuestionId");
            Answer answer = _mapper.Map<Answer>(model);
            answer.CreatedBy = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
            answer.CreatedTime = DateTime.Now;
            await _unitOfWork.GetRepository<Answer>().AddAsync(answer);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAnswer(string? answerId)
        {
            Answer answer = await _unitOfWork.GetRepository<Answer>().Entities.FirstOrDefaultAsync(a => a.Id == answerId && !a.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm AnswerId");

            answer.DeleteTime = DateTime.Now;
            answer.DeleteBy = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            await _unitOfWork.GetRepository<Answer>().UpdateAsync(answer);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<ResponseAnswerModel>> GetAllAnswers(string? searchName, int index = 1, int pageSize = 10)
        {
            IQueryable<ResponseAnswerModel> query = from a in _unitOfWork.GetRepository<Answer>().Entities
                                                    where !a.DeleteTime.HasValue
                                                    select new ResponseAnswerModel()
                                                    {
                                                        Id = a.Id,
                                                        Content = a.Content,
                                                        QuestionId = a.QuestionId,

                                                    };

            if(!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(q => q.Content!.Contains(searchName));
            }

            PaginatedList<ResponseAnswerModel> paginatedAnswer = await _unitOfWork.GetRepository<ResponseAnswerModel>().GetPagingAsync(query, index, pageSize);
            return paginatedAnswer;
        }

        public async Task UpdateAnswer(UpdateAnswerModel model)
        {
            Question question = await _unitOfWork.GetRepository<Question>().Entities.FirstOrDefaultAsync(q => q.Id == model.QuestionId && !q.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy QuestionId");

            Answer answer = await _unitOfWork.GetRepository<Answer>().Entities.FirstOrDefaultAsync(a => a.Id == model.Id && !a.DeleteTime.HasValue)
               ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm AnswerId");

            _mapper.Map(model, answer);
            answer.LastUpdateBy = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
            answer.LastUpdateTime = DateTime.Now;

            await _unitOfWork.GetRepository<Answer>().UpdateAsync(answer);
            await _unitOfWork.SaveAsync();
        }
    }
}
