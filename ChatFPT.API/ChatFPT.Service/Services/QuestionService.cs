using AutoMapper;
using ChatFPT.Application.Interface;
using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using ChatFPT.Core.Models.Question;
using ChatFPT.Core.Pagination;
using ChatFPT.Domain.Entities;
using ChatFPT.Service.Insfracstructure;
using ChatFPT.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ChatFPT.Service.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public QuestionService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = httpContextAccessor;
        }
        public async Task CreateQuestion(RequestQuestionModel model)
        {
            
            Question question = _mapper.Map<Question>(model);
            question.UserId = Guid.Parse(Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor));
            question.CreatedTime = DateTime.Now;
            question.CreatedBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            await _unitOfWork.GetRepository<Question>().AddAsync(question);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteQuestion(string id)
        {
            Question question = await _unitOfWork.GetRepository<Question>().Entities.FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy id");

            question.DeleteTime = DateTime.Now;
            question.DeleteBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            await _unitOfWork.GetRepository<Question>().UpdateAsync(question);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<ResponseQuestionModel>> GetAllQuestion(string? searchName, int index = 1, int PageSize = 10)
        {
            IQueryable<ResponseQuestionModel> query = from question in _unitOfWork.GetRepository<Question>().Entities
                                                      where !question.DeleteTime.HasValue
                                                      select new ResponseQuestionModel()
                                                      {
                                                          UserId = question.UserId.ToString(),
                                                          Content = question.Content,
                                                          IsResolve = question.IsResolve,
                                                      };

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(q => q.Content!.Contains(searchName));
            }
            PaginatedList<ResponseQuestionModel> paginatedQuestion = await _unitOfWork.GetRepository<ResponseQuestionModel>().GetPagingAsync(query, index, PageSize);
            return paginatedQuestion;
        }

        public async Task<ResponseQuestionModel> GetQuestionById(string id)
        {
            Question model = await _unitOfWork.GetRepository<Question>().Entities
                .FirstOrDefaultAsync(q => q.Id == id && !q.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy question id");

            ResponseQuestionModel responseQuestionModel = _mapper.Map<ResponseQuestionModel>(model);

            return responseQuestionModel;
        }

        public async Task UpdateQuestion(UpdateQuestionModel model)
        {           
            Question question = await _unitOfWork.GetRepository<Question>().Entities.FirstOrDefaultAsync(q => q.Id == model.Id)
              ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "CategoryId không tồn tại");

            _mapper.Map(model, question);
            question.LastUpdateBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            question.LastUpdateTime = DateTime.Now;
            await _unitOfWork.GetRepository<Question>().UpdateAsync(question);
            await _unitOfWork.SaveAsync();
        }
    }
}
