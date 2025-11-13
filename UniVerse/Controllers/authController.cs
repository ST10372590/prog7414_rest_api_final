using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universe.Api.Data;
using Universe.Api.Models;
using Universe.Api.Services;
using Google.Apis.Auth;



namespace Universe.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        // POST: auth/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDto request)
        {
            // Check if username or email already exists
            if (_context.Users.Any(u => u.Username == request.Username || u.Email == request.Email))
                return BadRequest(new { message = "Username or email already exists" });

            // Hash the password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create user and include additional fields
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email,
                Role = request.Role,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = hashedPassword,
                LastLogin = null
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "User registered successfully" });
        }


        // POST: auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            // Verify password using BCrypt.Net-Next
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            _context.SaveChanges();

            return Ok(new LoginResponseDto { Token = token });
        }

        // STEP 1: Redirect user to Google Login
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleCallback") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // STEP 2: Callback from Google
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return Unauthorized("Google authentication failed.");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized("Google account does not provide email.");

            // Check if user exists, otherwise create them
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = name?.Split(' ').FirstOrDefault() ?? "Google",
                    LastName = name?.Split(' ').Skip(1).FirstOrDefault() ?? "User",
                    Email = email,
                    Username = email,
                    Role = "Student", // default role (can adjust later)
                    PasswordHash = "", // not used for Google accounts
                    LastLogin = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            // Generate JWT token for the user
            var token = _jwtService.GenerateToken(user);

            return Ok(new { Token = token, Email = email, Name = name });
        }

        [AllowAnonymous]
        [HttpPost("login-with-google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginRequestDto request)
        {
            try
            {
                var payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(request.IdToken);
                var email = payload.Email;

                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    user = new User
                    {
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        Email = email,
                        Username = email,
                        Role = "Student",
                        PasswordHash = "",
                        LastLogin = DateTime.UtcNow
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    user.LastLogin = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                var token = _jwtService.GenerateToken(user);
                return Ok(new { Token = token, Email = email });
            }
            catch
            {
                return Unauthorized("Invalid Google ID token");
            }
        }

    }
}
