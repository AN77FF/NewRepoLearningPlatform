using AutoMapper;
using Platform_Learning_Test.Domain.Dto;
using Platform_Learning_Test.Domain.Entities;

namespace Platform_Learning_Test.Common.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponseDto>();
            CreateMap<Role, RoleDto>();

            CreateMap<Test, TestDto>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));
            CreateMap<Test, TestDetailDto>();
           
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
            CreateMap<User, UserProfileDto>();

        }
    }

}
