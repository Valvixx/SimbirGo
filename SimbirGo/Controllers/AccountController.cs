using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirGo.Models.Dto;

namespace SimbirGo.Controllers;
[ApiController]
[Route("account")]
public class AccountController: BaseController
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        return Ok(new UserDataDto()
        {
            Id = Id,
            Email = Email,
            Username = Username,
            Role = Role
        });
    }
}