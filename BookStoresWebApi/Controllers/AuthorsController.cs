using BookStoresWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoresWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly BookStoresDBContext _context;
        public AuthorsController(BookStoresDBContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public IEnumerable<Author> Get()
        {


            //返回所有作者
            return _context.Authors.ToList();

        }
    }
}
