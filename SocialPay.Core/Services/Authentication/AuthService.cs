using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Authentication
{
    public class AuthRepoService : BaseService<ClientAuthentication>
    {
        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        public AuthRepoService(SocialPayDbContext context, IOptions<AppSettings> appSettings) : base(context)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public async Task<LoginAPIResponse> Authenticate(LoginRequestDto loginRequestDto)
        {
            try
            {
                if (string.IsNullOrEmpty(loginRequestDto.Email) || string.IsNullOrEmpty(loginRequestDto.Password))
                    return new LoginAPIResponse { ResponseCode = AppResponseCodes.Failed};


                var validateuserInfo = await _context.ClientAuthentication.SingleOrDefaultAsync(x => x.Email == loginRequestDto.Email);

                // check if username exists
                if (validateuserInfo == null)
                    return new LoginAPIResponse { ResponseCode = AppResponseCodes.InvalidLogin };

                // check if password is correct
                if (!VerifyPasswordHash(loginRequestDto.Password.Encrypt(_appSettings.appKey), validateuserInfo.ClientSecretHash, validateuserInfo.ClientSecretSalt))
                    return new LoginAPIResponse { ResponseCode = AppResponseCodes.InvalidLogin };

                var tokenResult = new LoginAPIResponse();
                // var getName = checkIfUserExist.TblRole.RoleName.ToString();
                var key = Encoding.ASCII.GetBytes(_appSettings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor();
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                   {
                    new Claim(ClaimTypes.Name, validateuserInfo.Email),
                    new Claim(ClaimTypes.Role, validateuserInfo.RoleName),
                    new Claim(ClaimTypes.Email, validateuserInfo.Email),
                    new Claim("UserStatus",  validateuserInfo.StatusCode),
                    new Claim(ClaimTypes.NameIdentifier,  Convert.ToString(validateuserInfo.ClientAuthenticationId)),

                   }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                tokenResult.AccessToken = tokenString;
                tokenResult.ClientId = validateuserInfo.Email;
                tokenResult.Role = validateuserInfo.RoleName;
                tokenResult.UserStatus = validateuserInfo.StatusCode;
                tokenResult.ResponseCode = AppResponseCodes.Success;
                return tokenResult;
            }
            catch (Exception ex)
            {
                return new LoginAPIResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

    }
}
