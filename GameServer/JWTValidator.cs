using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer
{
    static class JWTValidator
    {
        // API 서버랑 동일해야함
        private static readonly string Secret = "dj248120fhasv94h1oa7d73489vay760ddjfvhakjhg3498fhsuij3120v98wsa8";

        public static JwtPayload? Validate(string token)
        {
            byte[] key = Encoding.UTF8.GetBytes(Secret);

            TokenValidationParameters parameters = new TokenValidationParameters
            {
                ValidateIssuer = false,                             // 발급자 검증 여부
                ValidateAudience = false,                           // 수신자 검증 여부
                ValidateLifetime = true,                            // 토큰 만료 검증 여부
                ValidateIssuerSigningKey = true,                    // 서명 키 검증 여부
                IssuerSigningKey = new SymmetricSecurityKey(key),   // 서명 키 설정
                ClockSkew = TimeSpan.Zero                           // 시간 허용 오차 범위, 0초
            };

            try
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, parameters, out SecurityToken validatedToken);

                JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken.Payload;
            }
            catch
            {
                return null;
            }
        }
    }
}
