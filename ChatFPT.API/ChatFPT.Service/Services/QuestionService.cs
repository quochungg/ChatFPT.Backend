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
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core;

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
            Tag tag = await _unitOfWork.GetRepository<Tag>().Entities.FirstOrDefaultAsync(t => t.Id == model.TagId && !t.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Tag không tồn tại");
            Question question = _mapper.Map<Question>(model);
            
            question.CreatedTime = DateTime.Now;
            question.CreatedBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            await _unitOfWork.GetRepository<Question>().AddAsync(question);
            await _unitOfWork.SaveAsync();

            QuestionTag questionTag = new QuestionTag()
            {
                TagId = model.TagId,
                QuestionId = question.Id,
            };
            await _unitOfWork.GetRepository<QuestionTag>().AddAsync(questionTag);
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

        public async Task<PaginatedList<ResponseQuestionModel>> GetAllQuestion(string? searchName, int index, int PageSize, string orderBy, string sortBy)
        {
            IQueryable<ResponseQuestionModel> query = from question in _unitOfWork.GetRepository<Question>().Entities
                                                      join questionTag in _unitOfWork.GetRepository<QuestionTag>().Entities on question.Id equals questionTag.QuestionId
                                                      join tag in _unitOfWork.GetRepository<Tag>().Entities on questionTag.TagId equals tag.Id
                                                      where !question.DeleteTime.HasValue
                                                      select new ResponseQuestionModel()
                                                      {
                                                          Id = question.Id,
                                                          Content = question.Content,
                                                          TagId = questionTag.TagId,
                                                          TagName = tag.Name,
                                                          IsResolve = question.IsResolve,
                                                      };

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(q => q.Content!.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                string sortDirection = (sortBy?.ToLower() == "desc") ? "descending" : "ascending";
                try
                {
                    query = query.OrderBy($"{orderBy} {sortDirection}");
                }
                catch (ParseException)
                {
                    query = query.OrderBy("Id");
                }
            }

            PaginatedList<ResponseQuestionModel> paginatedQuestion = await _unitOfWork.GetRepository<ResponseQuestionModel>().GetPagingAsync(query, index, PageSize);
            return paginatedQuestion;
        }

        public async Task<ResponseQuestionModel> GetQuestionById(string id)
        {
            Question model = await _unitOfWork.GetRepository<Question>().Entities
                .FirstOrDefaultAsync(q => q.Id == id && !q.DeleteTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "Không tìm thấy question id");

            QuestionTag? questionTag = await _unitOfWork.GetRepository<QuestionTag>().Entities.FirstOrDefaultAsync(qt => qt.QuestionId == model.Id);
            Tag? tag = await _unitOfWork.GetRepository<Tag>().Entities.FirstOrDefaultAsync(t => t.Id == questionTag!.TagId);

            ResponseQuestionModel responseQuestionModel = _mapper.Map<ResponseQuestionModel>(model);
            responseQuestionModel.TagName = tag!.Name;

            return responseQuestionModel;
        }

        public async Task UpdateQuestion(UpdateQuestionModel model)
        {           
            Question question = await _unitOfWork.GetRepository<Question>().Entities.FirstOrDefaultAsync(q => q.Id == model.Id)
              ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstaints.BADREQUEST, "QuestionId không tồn tại");

            QuestionTag? questionTag = await _unitOfWork.GetRepository<QuestionTag>().Entities.FirstOrDefaultAsync(qt => qt.QuestionId == model.Id);
            questionTag!.TagId = model.TagId;
            _mapper.Map(model, question);
            question.LastUpdateBy = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            question.LastUpdateTime = DateTime.Now;
            await _unitOfWork.GetRepository<Question>().UpdateAsync(question);
            await _unitOfWork.GetRepository<QuestionTag>().UpdateAsync(questionTag);
            await _unitOfWork.SaveAsync();
        }
    }
}
