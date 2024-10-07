using External.CTS.Core.Models.Dto;

namespace External.CTS.AuthAPI.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
    }

    public class AuthService : IAuthService
    {
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            return new LoginResponseDto
            {
                User = new UserDto
                {
                    ID = 1,
                    Name = "Admin"
                },
                Token = ""
            };
        }
    }
}
