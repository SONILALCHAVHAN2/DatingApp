using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers
{
    public class AccountController:BaseApiController
    {
        private readonly DataContext _datacontex;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext datacontex,ITokenService tokenService)
        {
            _datacontex = datacontex;
            this._tokenService = tokenService;
        }

        [HttpPost("register")]//Pos:api/Account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExist(registerDto.Username))return BadRequest("Username is taken");

            using var hmac =new HMACSHA512();
            var user=new AppUser(){
                UserName=registerDto.Username.ToLower(),
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt=hmac.Key
            };

            _datacontex.Users.Add(user);
            await _datacontex.SaveChangesAsync();
            return new UserDto{
                Username=user.UserName,
                Token=_tokenService.CreateToken(user)
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user=await _datacontex.Users.SingleOrDefaultAsync(x=>x.UserName==loginDto.Username.ToLower());
            if(user==null)return Unauthorized("Invalid Username"); 

            using var hmac =new HMACSHA512(user.PasswordSalt);

            var computeHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)) ;
            for(int i=0;i<computeHash.Length;i++)
            {
                if(computeHash[i]!=user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto{
                Username=user.UserName,
                Token=_tokenService.CreateToken(user)
            };
        }
        private async Task<bool>  UserExist(string username){

            return await _datacontex.Users.AnyAsync(x=>x.UserName==username.ToLower());
        }
    }
}