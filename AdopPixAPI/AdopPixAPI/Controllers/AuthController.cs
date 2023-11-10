using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DTOs;
using AdopPixAPI.Services.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AdopPixAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IServiceUnitOfWork serviceUnitOfWork;
        private readonly IConfiguration configuration;

        public AuthController(IServiceUnitOfWork serviceUnitOfWork, IConfiguration configuration)
        {
            this.serviceUnitOfWork = serviceUnitOfWork;
            this.configuration = configuration;
        }
        
        [HttpPost("[action]")]
        public async Task<ActionResult<ResponseDto<string>>> Login(UserLoginDto userLoginDto)
        {
            var user = await serviceUnitOfWork.UserService.Verify(userLoginDto);
            if (user == null)
            {
                return BadRequest(new ResponseDto<string>()
                {
                    Successful = false,
                    Message = "email or password invalid",
                    Data = string.Empty
                });
            }
            if (!user.IsConfirmEmail) return BadRequest(new ResponseDto<string>()
            {
                Successful = false,
                Message = "please confirm email",
                Data = string.Empty
            });

            var token = serviceUnitOfWork.UserService.CreateAccessToken(user);
            return Ok(new ResponseDto<string>()
            {
                Successful = true,
                Message = "Success",
                Data = token
            });
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<ResponseDto<bool>>> Register(UserRegisterDto userRegisterDto)
        {
            if (userRegisterDto != null)
            {
                if (await serviceUnitOfWork.UserService.FindByEmail(userRegisterDto.Email) != null)
                {
                    return BadRequest(new ResponseDto<string>()
                    {
                        Successful = false,
                        Message = "Email is aleady to use",
                        Data = string.Empty
                    });
                }

                if (await serviceUnitOfWork.UserService.FindProfileByUsername(userRegisterDto.Username) != null)
                {
                    return BadRequest(new ResponseDto<string>()
                    {
                        Successful = false,
                        Message = "Username is aleady to use",
                        Data = string.Empty
                    });
                }

                var user = await serviceUnitOfWork.UserService.Create(userRegisterDto);
                string token = serviceUnitOfWork.UserService.CreateConfirmEmailToken(user);

                string confirmLinkUrl = $"{configuration.GetSection("FrontUrl").Value}verify/emailAddress/{token}";

                string template = serviceUnitOfWork.EmailService.CreateTemplate("ConfirmEmail",
                                                                                "Last Step !",
                                                                                "Confirm your email, Just click this link",
                                                                                confirmLinkUrl);
                await serviceUnitOfWork.EmailService.SendAsync(user.Email, "Confirm your email", template);

                return Ok(new ResponseDto<string>()
                {
                    Successful = true,
                    Message = "Succesful",
                    Data = string.Empty
                });
            }

            return BadRequest(new ResponseDto<string>()
            {
                Successful = false,
                Message = "Bad Request",
                Data = string.Empty
            });
        }

        [HttpGet("[action]/{token}")]
        public async Task<ActionResult<ResponseDto<bool>>> ConfirmEmail(string token)
        {
            if (token != null)
            {
                var plainTexts = serviceUnitOfWork.TokenService.DecodeBase64(token);
                string[] plainTextInfo = plainTexts.Split(',');
                string userId = plainTextInfo[0];
                string email = plainTextInfo[1];

                var user = await serviceUnitOfWork.UserService.FindById(userId);

                if (user != null)
                {
                    if (user.Email == email)
                    {
                        user.IsConfirmEmail = true;
                        await serviceUnitOfWork.UserService.ConfirmEmail(user);

                        return Ok(new ResponseDto<bool>()
                        {
                            Successful = true,
                            Message = "Successful",
                            Data = true
                        });
                    }
                }
            }

            return BadRequest(new ResponseDto<bool>()
            {
                Successful = false,
                Message = "Bad Request",
                Data = false
            });
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<ActionResult<ResponseDto<string>>> RequestChangeEmail(string newEmail)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                var currentUser = await serviceUnitOfWork.UserService.FindById(userId);
                string token = serviceUnitOfWork.UserService.CreateChangeEmailToken(currentUser, newEmail);

                string linkUrl = $"{configuration.GetSection("UrlAPI").Value}Auth/ConfirmChangeEmail?token={token}";

                string template = serviceUnitOfWork.EmailService.CreateTemplate("ConfirmEmail",
                                                                                "Last Step !",
                                                                                "Click this button for confirm change email",
                                                                                linkUrl);
                await serviceUnitOfWork.EmailService.SendAsync(currentUser.Email, "Confirm your email", template);

                return Ok(new ResponseDto<bool>()
                {
                    Successful = true,
                    Message = "Successful",
                    Data = true
                });
            }

            return Unauthorized();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> ConfirmChangeEmail(string token)
        {
            if(token != null)
            {
                var plainTexts = serviceUnitOfWork.TokenService.DecodeBase64(token);
                string[] plainTextInfo = plainTexts.Split(',');
                string userId = plainTextInfo[0];
                string currentEmail = plainTextInfo[1];
                string newEmail = plainTextInfo[2];

                var user = await serviceUnitOfWork.UserService.FindById(userId);
                if(user != null)
                {
                    if(user.Email == currentEmail)
                    {
                        user.Email = newEmail;
                        user.IsConfirmEmail = false;
                        await serviceUnitOfWork.UserService.ConfirmChangeEmail(user);

                        string confirmToken = serviceUnitOfWork.UserService.CreateConfirmEmailToken(user);

                        string confirmLinkUrl = $"{configuration.GetSection("UrlAPI").Value}Auth/ConfirmEmail/{confirmToken}";

                        string template = serviceUnitOfWork.EmailService.CreateTemplate("ConfirmEmail",
                                                                                        "Last Step !",
                                                                                        "Confirm your email, Just click this link",
                                                                                        confirmLinkUrl);
                        await serviceUnitOfWork.EmailService.SendAsync(newEmail, "Confirm your email", template);
                        return Ok(new ResponseDto<bool>()
                        {
                            Successful = true,
                            Message = "Successful",
                            Data = true
                        });

                    }
                }
            }
            return BadRequest(new ResponseDto<bool>()
            {
                Successful = true,
                Message = "token invalid.",
                Data = true
            });
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<ActionResult<ResponseDto<string>>> ConfirmChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId != null)
            {
                if(dto.NewPassword == dto.ConfirmNewPassword)
                {
                    var user = await serviceUnitOfWork.UserService.FindById(userId);
                    if (user != null)
                    {
                        UserLoginDto userLoginDto = new UserLoginDto()
                        {
                            Email = user.Email,
                            Password = dto.CurrentPassword
                        };

                        var currentUser = await serviceUnitOfWork.UserService.Verify(userLoginDto);
                        if(currentUser != null)
                        {
                            await serviceUnitOfWork.UserService.ConfirmChangePassword(currentUser, dto.NewPassword);
                            return Ok(new ResponseDto<bool>()
                            {
                                Successful = true,
                                Message = "Successful",
                                Data = true
                            });
                        }
                        else
                        {
                            return BadRequest(new ResponseDto<bool>()
                            {
                                Successful = false,
                                Message = "password invalid",
                                Data = false
                            });
                        }
                    }
                }
            }
            return BadRequest(new ResponseDto<bool>()
            {
                Successful = true,
                Message = "bad request",
                Data = true
            });
        }
    }
}
