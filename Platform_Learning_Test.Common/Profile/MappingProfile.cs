using AutoMapper;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;

namespace LearningPlatformTast.Common.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponseDto>();
            CreateMap<Role, RoleDto>();

            CreateMap<Test, TestDto>();
            CreateMap<Test, TestDetailDto>();
            CreateMap<CreateTestDto, Test>();
            CreateMap<UpdateTestDto, Test>();

            CreateMap<Question, QuestionDto>();
            CreateMap<CreateQuestionDto, Question>();
            CreateMap<UpdateQuestionDto, Question>();

            CreateMap<AnswerOption, AnswerOptionDto>();
            CreateMap<CreateAnswerOptionDto, AnswerOption>();
            CreateMap<UpdateAnswerOptionDto, AnswerOption>();

            CreateMap<UserTestHistory, TestResultDto>();
            CreateMap<UserTestHistory, TestResultDetailsDto>();
            CreateMap<TestReview, TestReviewDto>();
        }
    }

}
