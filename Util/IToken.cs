using ApiWikiTech.Models;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiWikiTech.Util
{
    public class IToken
    {
        /// <summary>
        /// Verifica algun campo de una  lista de string viene vacío o es null
        /// </summary>
        /// <param name="fields"></param>
        /// <returns>True si un campo está vacío o es null</returns>
        public static bool ValidateFields(List<string> fields)
        {
            foreach (string field in fields)
            {
                if (String.IsNullOrEmpty(field.Trim()))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a == null || b == null || a.Length != b.Length)
                return false;

            bool areEqual = true;
            for (int i = 0; i < a.Length; i++)
            {
                areEqual &= (a[i] == b[i]);
            }

            return areEqual;
        }
        public string GenerateToken(User user,JwtConfig config)
        {
            try
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Key));
                SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                JwtSecurityToken token = new JwtSecurityToken(config.Issuer,
                  config.Audience,
                  claims,
                  expires: DateTime.Now.AddMinutes(30),
                  signingCredentials: creds);

                string strToken = new JwtSecurityTokenHandler().WriteToken(token);

                return strToken;
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }
    }
}
