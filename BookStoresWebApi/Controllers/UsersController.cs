using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoresWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BookStoresWebApi.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BookStoresDBContext _context;
        private readonly JWTSettings _jwtSettings;

        public UsersController(BookStoresDBContext context, IOptions<JWTSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        ////该方法是针对Basic Authentication的练习
        //// GET: api/Users/Login
        //[Authorize]
        //[HttpGet("Login")]
        //public async Task<ActionResult<User>> Login()
        //{
        //    //通过Identity获取用户邮箱
        //    string email = HttpContext.User.Identity.Name;
        //    User user = await _context.Users.Where(u => u.EmailAddress == email).FirstOrDefaultAsync();

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return user;
        //}

        //该方法是针对JWT的练习
        // GET: api/Users/Login
        [HttpPost("Login")]
        public async Task<ActionResult<Token>> Login([FromBody] dynamic userInfo)
        {
            //这里使用dynamic动态类型传参，属性名必须和传入的json字符串的属性名完全一致，大小写敏感。否则，读取不到参数。
            string email = userInfo.EmailAddress;
            string pwd = userInfo.Password;
            User validUser = await _context.Users
                                        .Where(u => u.EmailAddress == email && u.Password == pwd)
                                        .FirstOrDefaultAsync();

            if (validUser == null)
            {
                return NotFound();
            }
            else
            {
                RefreshToken refreshToken = createRefreshToken();
                validUser.RefreshTokens.Add(refreshToken);
                await _context.SaveChangesAsync();

                Token token = new Token
                {
                    JWT= createJWT(validUser.UserId),
                    RefreshToken =refreshToken.Token,
                };
                return token;
            }
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<Token>> RefreshToken([FromBody] Token token)
        {
            User user = getUserFromJWT(token.JWT);
            if (user!=null && validateRefreshToken(user, token.RefreshToken))
            {
                Token newToken = new Token
                {
                    JWT = createJWT(user.UserId),
                    RefreshToken = token.RefreshToken,
                };
                return newToken;
            }
            else
            {
                return null;
            }
        }

        private RefreshToken createRefreshToken()
        {
            RefreshToken refreshToken = new RefreshToken();
            byte[] randomNumber = new byte[32];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                //生成一组随机数并写入到字节数组中
                random.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
            }
            refreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
            return refreshToken;
        }

        private string createJWT(int userId)
        {
            //TokenHandler负责创建Token
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            //获取密钥(密钥不能少于128bit也就是16bytes)
            byte[] key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            //为了防止密钥过短可对原始密码进行MD5加密（MD5会固定输出128bit数据）
            //key = MD5.Create().ComputeHash(key);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                //定义Claims
                Subject = new ClaimsIdentity(new Claim[]
                {
                        //new Claim(ClaimTypes.Name,email)
                        //此处使用UserId作为Claim名称
                        new Claim(ClaimTypes.Name,Convert.ToString(userId))
                }),
                //定义过期时间
                Expires = DateTime.UtcNow.AddSeconds(30),
                //签署凭证
                //包含指定加密密钥和加密方法
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key)
                                            , SecurityAlgorithms.HmacSha256Signature)
            };
            //执行创建Token
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            //把JWT对象序列化成JWT字符串
            string JWT = tokenHandler.WriteToken(securityToken);

            return JWT;
        }

        //验证JWT AccessToken字符串，并获取对应的User
        //此处再次验证AccessToken貌似没有必要
        private User getUserFromJWT(string jwt)
        {
            //TokenHandler负责创建Token
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            //获取密钥(密钥不能少于128bit也就是16bytes)
            byte[] key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                //ClockSkew=TimeSpan.Zero,
            };
            SecurityToken securityToken;

            //根据验证规则来验证JWT
            ClaimsPrincipal claimsPrincipal= tokenHandler.ValidateToken(jwt, tokenValidationParameters, out securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

            if(jwtSecurityToken!=null && 
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                string userId = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
                User user=_context.Users.Where(u => u.UserId == Convert.ToInt32(userId)).FirstOrDefault();
                return user;
            }
            else
            {
                return null;
            }
        }

        //去数据库中验证RefreshToken是否与User对应，并且没有过期
        private bool validateRefreshToken(User user, string refreshToken)
        {
            RefreshToken rt = _context.RefreshTokens
                                .Where(rt => rt.Token == refreshToken)
                                .OrderByDescending(rt => rt.ExpiryDate)
                                .FirstOrDefault();
            if(rt!=null && rt.UserId==user.UserId && rt.ExpiryDate>DateTime.UtcNow)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost("test")]
        public ActionResult Test([FromBody] string text)
        {
            return Ok(text);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
