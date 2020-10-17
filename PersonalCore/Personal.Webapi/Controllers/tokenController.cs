using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Personal.Webapi.Entity;

namespace Personal.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class tokenController : ControllerBase
    {
        private IConfiguration _config;
        readonly PersonalContext _personalContext;
        public tokenController(IConfiguration config, PersonalContext personalContext)
        {
            _config = config;
            _personalContext = personalContext ?? throw new ArgumentNullException(nameof(personalContext));
        }
        [AllowAnonymous]
        [HttpPost(template: "login")]
        public IActionResult CreateToken([FromBody]LoginModel login)
        {
            IActionResult response = Unauthorized();
            UserModel user = Authenticate(login); 
            if (user != null)
            {
                TokenUser tokenUser = _personalContext.TokenUser.FirstOrDefaultAsync(d => d.UserId.ToString() == user.UserId).Result;
                if (_personalContext.TokenUser.Contains(tokenUser))
                {
                    _personalContext.TokenUser.Remove(tokenUser);
                }
                string tokenString = BuildToken(user);
                string refreshToken = GenerateRefreshToken();

                TokenUser newtoken = new TokenUser();
                newtoken.ExpriedTime = DateTime.Now.AddDays(1);
                newtoken.UserId = new Guid(user.UserId);
                newtoken.RefreshToken = refreshToken;
                _personalContext.TokenUser.Add(newtoken);
                _personalContext.SaveChanges();

                response = Ok(new { access_token = tokenString, refresh_token= refreshToken });
            }
            return response;
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <returns></returns>
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RequestModel request)
        {

            var principal = GetPrincipalFromAccessToken(request.AccessToken);

            if (principal is null)
            {
                return Ok(false);
            }

            var id = principal.Claims.First(c => c.Type == "userid")?.Value;
            var name = principal.Claims.First(c => c.Type == "username")?.Value;

            if (string.IsNullOrEmpty(id))
            {
                return Ok(false);
            }

            var usertoken = await _personalContext.TokenUser.FirstOrDefaultAsync(d => d.UserId.ToString() == id & d.RefreshToken == request.RefreshToken);

            if (usertoken is null)
            {
                return Ok(false);
            }
            else if (usertoken.ExpriedTime<=DateTime.Now)
            {
                return Ok(false);
            }


            var usermodel = new UserModel()
            {
                UserId = usertoken.UserId.ToString(),
                Name = name
            };

            return Ok(new
            {
                access_token = BuildToken(usermodel),
                refresh_token = request.RefreshToken
            });
        }


        #region 辅助方法
        private UserModel Authenticate(LoginModel login)
        {
            Users LoginUser=(from user in _personalContext.Users
                       where user.LoginName == login.Username
                       select user).FirstOrDefault();
            if (LoginUser==null)
            {
                return null;
            }
            if (GenerateMD5(LoginUser.Password)!=login.Password)
            {
                return null;
            }
            UserModel model = new UserModel
            {
                UserId = LoginUser.UserId.ToString(),
                Name = LoginUser.Name
            };
            return model;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string GenerateMD5(string txt)
        {
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(txt);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }


        private string BuildToken(UserModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
                new Claim("userid",user.UserId),
                 new Claim("username",user.Name)
            };
            var token = new JwtSecurityToken(
               _config["issuer"],
              _config["audience"],
              claims: claims,
              expires: DateTime.Now.AddMinutes(20),
              notBefore: DateTime.Now,
              signingCredentials: creds
              ); ;
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 生成刷新Token
        /// </summary>
        /// <returns></returns>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// 从Token中获取用户身份
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ClaimsPrincipal GetPrincipalFromAccessToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                return handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SecurityKey"])),
                    ValidateLifetime = false
                }, out SecurityToken validatedToken);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion


        #region model
        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        private class UserModel
        {
            public string Name { get; set; }
            public string UserId { get; set; }
            public DateTime Birthdate { get; set; }
        }

        public class UserRefreshToken
        {
            public string Id { get; private set; } = Guid.NewGuid().ToString();
            public string Token { get; set; }
            public DateTime Expires { get; set; }
            public string UserId { get; set; }
            public bool Active => DateTime.Now <= Expires;
        }

        public class RequestModel
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }
        #endregion
    }
}