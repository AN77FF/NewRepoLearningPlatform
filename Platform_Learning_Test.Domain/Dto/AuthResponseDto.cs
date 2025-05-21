using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform_Learning_Test.Domain.Dto
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public UserResponseDto User { get; set; }


    }
}
