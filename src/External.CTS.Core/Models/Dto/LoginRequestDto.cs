using System;

namespace External.CTS.Core.Models.Dto
{
    public class LoginRequestDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public LoginRequestDto(string userName, string password)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }
    }
}
