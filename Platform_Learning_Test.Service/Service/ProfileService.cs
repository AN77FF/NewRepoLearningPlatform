using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Platform_Learning_Test.Data.Context;

using Platform_Learning_Test.Domain.Dto;

namespace Platform_Learning_Test.Service.Service
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public ProfileService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");
            return _mapper.Map<UserProfileDto>(user);
        }

        public async Task UpdateUserProfileAsync(int userId, UpdateUserProfileDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            _mapper.Map(dto, user);
            await _context.SaveChangesAsync();
        }
    }
}
