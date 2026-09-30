using MC.Control.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static MC.Control.Util.Constants;

namespace MC.Control.Security
{
    public class TokenControl : ITokenControl
    {
        public ResponseContainerModel GenerateJwtToken(Dictionary<string, string> dictTokenParam, Dictionary<string, string> dictClaims)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var passwordToken = dictTokenParam.FirstOrDefault(x => x.Key == ConstantesToken.Key).Value;
            var minutesToken = int.Parse(dictTokenParam.FirstOrDefault(x => x.Key == ConstantesToken.Minutes).Value);

            List<Claim> claims = new();
            foreach (var item in dictClaims)
            {
                claims.Add(new Claim(item.Key, item.Value));
            }

            var key = Encoding.ASCII.GetBytes(passwordToken);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(minutesToken),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);


            var responseContainerModel = new ResponseContainerModel
            {
                Token = tokenHandler.WriteToken(token),
                FechaInicioVigencia = DateTime.Now,
                FechaFinVigencia = tokenDescriptor.Expires ?? DateTime.UtcNow.AddMinutes(minutesToken)
            };

            return responseContainerModel;
        }

        public string GetClaimValueByToken(string paramkeytoken, string tipoclaim, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(paramkeytoken);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var claim = jwtToken.Claims.First(x => x.Type == tipoclaim).Value;

            return claim;

        }

        public string IsTokenJwtValid(string paramkeytoken, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(paramkeytoken);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken _);
            return "";
        }
    }
}
