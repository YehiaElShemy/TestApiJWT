using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TestApiJWT.Helper;
using TestApiJWT.Models;

namespace TestApiJWT.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly JWT jWT;
        public AuthService(UserManager<ApplicationUser> _userManager,
            RoleManager<IdentityRole> _RoleManager, IOptions<JWT> _jwt)
        {
            userManager = _userManager;
            roleManager = _RoleManager;
            jWT = _jwt.Value;
        }
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "This Email is already Exist " };
            if (await userManager.FindByNameAsync(model.UserName) is not null)
                return new AuthModel { Message = "This UserName is already Exist " };
            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                string errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthModel() { Message = errors };
            }
            await userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Message = "Registertion Successfully",
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName

            };

        }
        //login
        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            AuthModel authModel = new AuthModel();
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password incorrect!";
                return authModel;

            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await userManager.GetRolesAsync(user);
            authModel.Message = "login Successfully";
            authModel.Email = user.Email;
            authModel.IsAuthenticated = true;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.UserName = user.UserName;
            authModel.Roles = rolesList.ToList();
            if (user.RefreshTokens.Any(t => t.IsAcive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsAcive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await userManager.UpdateAsync(user);
            }
            return authModel;
        }

        public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var rolesClaims = new List<Claim>();
            foreach (var role in roles)
                rolesClaims.Add(new Claim("roles", role));
            var calims = new Claim[]
            {
               new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub,user.UserName),
               new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
               new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email,user.Email),
               new Claim("uid",user.Id)
           }.Union(userClaims).Union(rolesClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWT.Key));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jWT.Issuer,
                audience: jWT.Audience,
                claims: calims,
                expires: DateTime.UtcNow.AddMinutes(jWT.DurationInMinutes),
                signingCredentials: signingCredentials
                );
            return jwtSecurityToken;

        }
        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
            {
                authModel.IsAuthenticated = false;
                authModel.Message = "Invaild token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
            if (!refreshToken.IsAcive)
            {
                authModel.IsAuthenticated = true;
                authModel.Message = "InActive token";
            }
            //refresh token used only one time 
            refreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);


            var jwtToken = await CreateJwtToken(user);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            var roles = await userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
            if (!refreshToken.IsAcive)
                return false;
            refreshToken.RevokedOn = DateTime.UtcNow;
            await userManager.UpdateAsync(user);
            return true;
        }


        private RefreshToken GenerateRefreshToken()
        {
            var randomNumer = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumer);
            return new RefreshToken()
            {
                Token = Convert.ToBase64String(randomNumer),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow,

            };
        }

        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user is null || !await roleManager.RoleExistsAsync(model.Role))
                return "userid or role in valid";
            if (await userManager.IsInRoleAsync(user, model.Role))
                return "this user already in this role";
            var result = await userManager.AddToRoleAsync(user, model.Role);
            return result.Succeeded ? string.Empty : "some thing went error";
        }

    }
}
