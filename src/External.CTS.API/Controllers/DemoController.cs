using External.CTS.API.Services;
using External.CTS.Core.Models.Dto;
using External.CTS.Core.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace External.CTS.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class DemoController(IBaseService baseService) : ControllerBase
    {
        public readonly IBaseService _baseService = baseService;

        [HttpPost("login")]
        public async Task<LoginResponseDto> Login()
        {
            var responseDto = await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = new LoginRequestDto("admin", "123"),
                Url = SD.GatewayAPIBase + SD.Api_Login
            }, withBearer: false);

            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto =
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

                //await SignInUser(loginResponseDto);
                //_tokenProvider.SetToken(loginResponseDto.Token);
                return loginResponseDto;
            }

            return new LoginResponseDto();
        }
    }
}
