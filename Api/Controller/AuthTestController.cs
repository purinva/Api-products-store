using Api.Common;
using Api.Data;
using Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controller
{
    public class AuthTestController : StoreController
    {
        public AuthTestController(AppDbContext dbContext) : base(dbContext)
        {
        }

        [HttpGet]
        public IActionResult Test1()
        {
            return Ok(new ResponseServer
            {
                StatusCode = HttpStatusCode.OK,
                Result = "Test1: для всех"
            });
        }

        [HttpGet]
        [Authorize]
        public IActionResult Test2()
        {
            return Ok(new ResponseServer
            {
                StatusCode = HttpStatusCode.OK,
                Result = "Test2: для авторизованных пользователей"
            });
        }

        [HttpGet]
        [Authorize(Roles = SharedData.Roles.Consumer)]
        public IActionResult Test3()
        {
            return Ok(new ResponseServer
            {
                StatusCode = HttpStatusCode.OK,
                Result = "Test2: для авторизованных пользователей с правами Consumer"
            });
        }

        [HttpGet]
        [Authorize(Roles = SharedData.Roles.Admin)]
        public IActionResult Test4()
        {
            return Ok(new ResponseServer
            {
                StatusCode = HttpStatusCode.OK,
                Result = "Test2: для авторизованных пользователей с правами Admin"
            });
        }
    }
}
