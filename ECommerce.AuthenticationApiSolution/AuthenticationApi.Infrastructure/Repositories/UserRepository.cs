using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration configuration) : IUser
    {
        private async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user is null ? null! : user!;
        }
        public async Task<GetUserDTO> GetUser(int userId)
        {
            var user = await context.Users.FindAsync(userId);
            return user is not null ? new GetUserDTO(user.Id, user.Name!, user.TelephoneNumber!, user.Address!, user.Email!, user.Role!) : null!;
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var existingUser = await GetUserByEmail(loginDTO.Email);  
            if (existingUser is null)
                return new Response(false, "User not found.");
            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, existingUser.Password))
                return new Response(false, "Invalid password.");

            var token = GenerateToken(existingUser);
            return new Response(true, token);
        }

        private string GenerateToken(AppUser user)
        {
            // Token generation logic here (e.g., JWT token generation)
            // This is a placeholder; implement your token generation logic.

            var key = Encoding.UTF8.GetBytes(configuration.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Email, user.Email!)
            };
            if(!string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role))
                claims.Add(new Claim(ClaimTypes.Role, user.Role!));

            var token = new JwtSecurityToken(
                    issuer: configuration["Authentication:Issuer"],
                    audience: configuration["Authentication:Audience"],
                    claims: claims, 
                    expires: null,
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var userExists = await GetUserByEmail(appUserDTO.Email);
            if (userExists is not null)
                return new Response(false, "User already exists with this email address.");

            var result = context.Users.Add(new AppUser
            {
                Name = appUserDTO.Name,
                TelephoneNumber = appUserDTO.TelephoneNumber,
                Address = appUserDTO.Address,
                Email = appUserDTO.Email,
                Role = appUserDTO.Role,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
            });

            await context.SaveChangesAsync();
            return result.Entity.Id > 0
                ? new Response(true, "User registered successfully.")
                : new Response(false, "User registration failed.");
        }
    }
}
