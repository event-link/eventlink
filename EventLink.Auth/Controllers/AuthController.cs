using EventLink.Auth.Model;
using EventLink.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLink.Auth.Controllers
{
    [Route("/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static readonly AuthService AuthService = AuthService.Instance;

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Auth([FromBody]AuthRequestModel authRequest)
        {
            IActionResult response;

            try
            {
                var authResponse = AuthService.Authenticate(authRequest);
                response = Ok(authResponse);
            }
            catch (AuthBadCredentialsException e)
            {
                response = Unauthorized(e.Message);
            }
            catch (AuthException e)
            {
                response = BadRequest(e.Message);
            }

            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword([FromBody]EmailModel emailModel)
        {
            IActionResult response;

            try
            {
                AuthService.ForgotPassword(emailModel);
                response = Ok("Forgot Password Email sent to \"" + emailModel.Email + "\" successfully.");
            }
            catch (AuthException e)
            {
                response = BadRequest(e.Message);
            }

            return response;
        }

    }

}