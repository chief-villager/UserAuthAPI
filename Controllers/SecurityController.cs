using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using SecurityQuestionAuthAPI.Data;
using SecurityQuestionAuthAPI.Services;

namespace SecurityQuestionAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly UserAuthServices _userAuthServices;


        public SecurityController(UserAuthServices userAuthServices,IBackgroundJobClient backgroundJobClient)
        {
            _userAuthServices = userAuthServices;

        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateUser([FromBody] UserRecieveDto userRecieveDto)
        {
            var result = await _userAuthServices.CreateUserAsync(userRecieveDto.UserName, userRecieveDto.Password, userRecieveDto.Email);
            if (result == null)
            {
                return BadRequest("Unable to create user");
            }

            return Ok("User creates successfully");


        }
        [HttpPost("SecurityQ&A")]
        public async Task<ActionResult> CreatSecurityQuestionAndAnswer(SecurityQuestionRequestDto securityQuestionRequestDto)
        {
            var result = await _userAuthServices.CreateSecurityQuestionAndAnswerAsync(securityQuestionRequestDto.QuestionText, securityQuestionRequestDto.Answer, securityQuestionRequestDto.UserEmail);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok("successfull");


        }

        [HttpGet("AllUsers")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUSer()
        {
            var result = await _userAuthServices.GetAllUsersAsync();
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpGet("GetallQuestionsByEmail")]
        public async Task<ActionResult<IEnumerable<SecurityQuestionResponseDto>>> GetAllSecurityQuestionsByEmail(string email)
        {
            var result = await _userAuthServices.GetallSecurityQuestionsAsyncByemail(email);
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpPut("ResetPassword")]
        public async Task<ActionResult<SecurityQuestionResponseDto>> ResetPassword(string email, string question, string answer, string newpassword)
        {

            try
            {
                var result = await _userAuthServices.ResetPasswordByQuestionAsync(email, question, answer, newpassword);
                if (result == null)
                {
                    return BadRequest();
                }
                return Ok();
            }
            catch (System.Exception)
            {

                throw new Exception("error");
            }

        }

        [HttpGet("getPhrases")]
        public async Task<ActionResult<string>> GetPhrases(string email)
        {
            var result = await _userAuthServices.GetRecoveryPhrase(email);
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }

        [HttpPut("ResetPasswordWithPhrase")]
        public async Task<ActionResult<UserResponseDto>> ResetPasswordWithRecoveryPhrase(string newpassword, string phrase, string email)
        {
            var result = await _userAuthServices.ResetPasswordUsingPhrase(newpassword, phrase, email);
            if (result == null)
            {
                return NoContent();
            }
            return Ok();
        }

        [HttpGet("GetOtp")]
        public async Task<IActionResult> SendOtpToUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Invalid email address");
            }

            try
            {
                var result = await _userAuthServices.SendOtpToUserEmailAsync(email);
                if (result > 0)
                {
                    return Ok("OTP sent to email");
                }
                else
                {
                    return BadRequest("Failed to send OTP");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
        }

        [HttpPut("ResetPasswordWithOTP")]
        public async Task<ActionResult> ResetPasswordWithOTP(string NewPasword, int Otp)
        {
          var result =  await _userAuthServices.ResetPasswordWithOTP(NewPasword, Otp);
          if (result == null)
          {
            return BadRequest("Code expired");
          }
          return Ok();
        }

    }
}
