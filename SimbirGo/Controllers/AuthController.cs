using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SimbirGo.Models.Db;
using SimbirGo.Models.Dto;
using SimbirGo.Services;
using SimbirGo.Services.Interfaces;

namespace SimbirGo.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    private readonly IJwtService _jwtService;
    private readonly IAccountService _accountService;
    
    public AuthController(IJwtService jwtService, IAccountService accountService)
    {
        this._jwtService = jwtService;
        this._accountService = accountService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto req)
    {
        var account = await _accountService.GetAccountData(req.Email, req.Password);
        
        if (account == null)
            return BadRequest("Bad credentials");

        var claims = Jwt.GetClaims(account.Id, req.Email, account.Username, account.Role);
        var accessToken = _jwtService.CreateToken(claims, 1);
        
        return Ok(new AuthDto()
        {
            AccessToken = accessToken,
            RefreshToken = account.RefreshToken
        });
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto req)
    {
        var refreshToken = _jwtService.CreateToken(new List<Claim>(), 72);

        if (await _jwtService.CheckAccount(req.Email))
            return BadRequest("email is already taken");

        var id = await _accountService.CreateAccount(new DbAccount()
        {
            Email = req.Email,
            Password = req.Password,
            Role = req.Role,
            Username = req.Username,
            RefreshToken = refreshToken,
            RefreshTokenExpiredTime = DateTime.UtcNow.Add(TimeSpan.FromHours(72)),
        });

        var claims = Jwt.GetClaims(id, req.Email, req.Username, req.Role);
        var accessToken = _jwtService.CreateToken(claims, 1);
        
        return Ok(new AuthDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        });
    }

    [HttpPut("expire-token/{token}")]
    public async Task<IActionResult> ExpireToken([FromRoute] string token)
    {
        return Ok(await _jwtService.ExpireToken(token));
    }
} 