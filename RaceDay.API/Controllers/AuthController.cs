using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    private readonly PasswordService _passwordService;
    private readonly TokenService _tokenService;
    private TokenService? tokenService;
    private object token;
    private object email;
    private object role;

    public AuthController(RaceDayDbContext context,
                          PasswordService passwordService,
                          TokenService tokenService)
    {
        _context = context;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    [HttpPost("register/participant")]
    public async Task<IActionResult> RegisterParticipant(
        [FromBody] RegisterRequest request)
    {
        if (await _context.Participants.AnyAsync(
            p => p.Email == request.Email))
        {
            return Conflict(new
            {
                message = "This email address is already registered."
            });
        }

        var participant = new Participant
        {
            ParticipantName = request.Name,
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(request.Password)
        };

        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(
            participant.ParticipantID,
            participant.Email,
            "Participant");

        return Ok(new
        {
            message = "Participant login successful.",
            participant.ParticipantID,
            participant.Email,
            role = "Participant",
            token
        });

    }
    [HttpPost("register/organiser")]
    public async Task<IActionResult> RegisterOrganiser(
    [FromBody] RegisterRequest request)
    {
        if (await _context.Organisers.AnyAsync(
            o => o.Email == request.Email))
        {
            return Conflict(new
            {
                message = "This email address is already registered."
            });
        }

        var organiser = new Organiser
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(request.Password),
            CreatedAT = DateTime.Now
        };

        _context.Organisers.Add(organiser);
        await _context.SaveChangesAsync();


        var token = GenerateJwtToken(
            organiser.OrganiserID,
            organiser.Email,
            "Organiser");

        return Ok(new
        {
            message = "Organiser login successful.",
            organiser.OrganiserID,
            organiser.Email,
            role = "Organiser",
            token
        });


    }


    private string GenerateJwtToken(int userId, string email, string role)
    {
        var configuration = HttpContext.RequestServices
            .GetRequiredService<IConfiguration>();

        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is missing.");

        var issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is missing.");

        var audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT Audience is missing.");

        var expiryMinutes = int.TryParse(
            configuration["Jwt:ExpiryMinutes"], out var minutes)
            ? minutes
            : 60;

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim(ClaimTypes.Role, role)
    };

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        if (request.Role.Equals(
            "Participant",
            StringComparison.OrdinalIgnoreCase))
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.Email == request.Email);

            if (participant == null ||
                string.IsNullOrEmpty(participant.PasswordHash) ||
                !_passwordService.VerifyPassword(
                    participant.PasswordHash,
                    request.Password))
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var token = _tokenService.GenerateToken(
                participant.ParticipantID.ToString(),
                participant.Email,
                "Participant");

            return Ok(new
            {
                message = "Participant login successful.",
                participant.ParticipantID,
                participant.Email,
                role = "Participant",
                token
            });
        }

        if (request.Role.Equals(
            "Organiser",
            StringComparison.OrdinalIgnoreCase))
        {
            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.Email == request.Email);

            if (organiser == null ||
                string.IsNullOrEmpty(organiser.PasswordHash) ||
                !_passwordService.VerifyPassword(
                    organiser.PasswordHash,
                    request.Password))
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var token = _tokenService.GenerateToken(
                organiser.OrganiserID.ToString(),
                organiser.Email,
                "Organiser");

            return Ok(new
            {
                message = "Organiser login successful.",
                organiser.OrganiserID,
                organiser.Email,
                role = "Organiser",
                token
            });
        }
        return Ok(new
        {
            token = token,
            email = email,
            role = role
        });
        return BadRequest(new
        {
            message = "Role must be Participant or Organiser."
        });
    }

}