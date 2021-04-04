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
        public async Task<ActionResult<RefreshToken>> Login([FromBody]dynamic userInfo)
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
                //TokenHandler负责创建Token
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                //获取密钥(密钥不能少于128bit也就是16bytes)
                byte[] key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
                //为了防止密钥过短可对原始密码进行MD5加密（MD5会固定输出128bit数据）
                //key = MD5.Create().ComputeHash(key);

                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                {
                    //定义Claims
                    Subject =new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name,email)
                    }),
                    //定义过期时间
                    Expires=DateTime.UtcNow.AddDays(1),
                    //签署凭证
                    //包含指定加密密钥和加密方法
                    SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key)
                                                ,SecurityAlgorithms.HmacSha256Signature)
                };
                //执行创建Token
                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

                RefreshToken refreshToken = new RefreshToken()
                {
                    TokenId = 888,
                    UserId = validUser.UserId,
                    //把token序列化成字符串
                    Token = tokenHandler.WriteToken(token),
                    ExpiryDate =tokenDescriptor.Expires??DateTime.UtcNow
                };
                return refreshToken;
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
