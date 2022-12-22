using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]//api/Users
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        
        private readonly DataContext _context;

        public UsersController(ILogger<UsersController> logger,DataContext context)
        {
            this._context = context;
           
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        { 
           var users=await _context.Users.ToListAsync();

           return users;
        }


        [HttpGet("{id}")]
        public  async Task<ActionResult<AppUser>> GetUser(int id)
        {
           return  await _context.Users.FindAsync(id);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}