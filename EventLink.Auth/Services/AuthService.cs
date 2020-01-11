using EventLink.Auth.Model;
using EventLink.DataAccess;
using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace EventLink.Auth.Services
{
    public interface IAuthService
    {
        AuthResponseModel Authenticate(AuthRequestModel authRequest);
        void ForgotPassword(EmailModel emailModel);
    }

    public class AuthService : IAuthService
    {
        private static readonly IConfiguration Config = ConfigurationManager.AppSetting;

        private static readonly UserService UserService = UserService.Instance;

        private static readonly Lazy<AuthService> instance =
            new Lazy<AuthService>(() => new AuthService());

        public static AuthService Instance => instance.Value;

        private AuthService()
        {

        }

        private User AuthenticateUser(AuthRequestModel authRequest)
        {
            if (authRequest == null)
            {
                throw new AuthException("AuthRequestModel is null!");
            }

            if (string.IsNullOrEmpty(authRequest.Email))
            {
                throw new AuthException("AuthRequestModel Email is null!");
            }

            if (string.IsNullOrEmpty(authRequest.Password))
            {
                throw new AuthException("AuthRequest Password is null!");
            }

            try
            {
                var user = UserService.GetUserByEmail(authRequest.Email);

                if (!user.IsActive)
                {
                    throw new AuthException("User Email (" + authRequest.Email + ") is deactactivated.");
                }

                var authHash = HashingUtils.SHA512(authRequest.Password);

                if (!user.HashedPassword.ToUpper().Equals(authHash))
                {
                    throw new AuthBadCredentialsException("Incorrect credentials!");
                }

                return user;
            }
            catch (DADocNotFoundException e)
            {
                throw new AuthUserNotFoundException(e.Message, e);
            }
            catch (DAException e)
            {
                throw new AuthException(e.Message, e);
            }
        }

        public AuthResponseModel Authenticate(AuthRequestModel authRequest)
        {
            User user;

            try
            {
                user = AuthenticateUser(authRequest);
            }
            catch (AuthBadCredentialsException e)
            {
                throw new AuthBadCredentialsException(e.Message, e);
            }
            catch (AuthUserNotFoundException e)
            {
                throw new AuthUserNotFoundException(e.Message, e);
            }
            catch (AuthException e)
            {
                throw new AuthException(e.Message, e);
            }

            if (user == null)
            {
                throw new AuthUserNotFoundException("User Email (" + authRequest.Email + ") was not found!");
            }

            var token = GenerateJsonWebToken(authRequest);
            var refreshToken = GenerateRefreshToken();

            user.LastActivityDate = DateTime.Now;
            UserService.UpdateUser(user);

            var message = "User (" + user.Id + ") has been authenticated successfully.";
            return new AuthResponseModel(token, refreshToken, message);
        }

        public void ForgotPassword(EmailModel emailModel)
        {
            try
            {
                var elMail = Config["Email:Mail"];
                var elMailPassword = Config["Email:Password"];

                var user = UserService.GetUserByEmail(emailModel.Email);

                var newPassword = ResetPassword(emailModel.Email);

                using (var reader = File.OpenText("Assets/forgotPassword.html"))
                {
                    var builder = new StringBuilder();
                    builder.Append(reader.ReadToEnd());
                    builder.Replace("{{full-name}}", user.FullName);
                    builder.Replace("{{new-password}}", newPassword);
                    var html = builder.ToString();

                    var message = new MailMessage();
                    var smtp = new SmtpClient();
                    message.From = new MailAddress(elMail);
                    message.To.Add(new MailAddress(emailModel.Email));
                    message.Subject = "EventLink - Forgot Password";
                    message.IsBodyHtml = true;
                    message.Body = html;
                    smtp.Port = 587;
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(elMail, elMailPassword);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);
                }
            }
            catch (AuthException e)
            {
                throw new AuthException(e.Message);
            }
        }

        private string GenerateJsonWebToken(AuthRequestModel authModel)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(Config["Jwt:Issuer"], Config["Jwt:Issuer"],
                null, expires: DateTime.Now.AddMinutes(120), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private string ResetPassword(string email)
        {
            // 1. find user.
            // 2. generate new password.
            // 3. hash new password.
            // 4. update user with new password
            // 5. return new password
            try
            {
                var user = UserService.GetUserByEmail(email);

                var newPassword = RandomPassword(10);

                var hashedPassword = HashingUtils.SHA512(newPassword);

                user.HashedPassword = hashedPassword;

                UserService.UpdateUser(user);

                return newPassword;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        private string RandomPassword(int size = 0)
        {
            var builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        private int RandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }

        private string RandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random();
            char ch;

            for (var i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            if (lowerCase)
                return builder.ToString().ToLower();

            return builder.ToString();
        }

    }

}