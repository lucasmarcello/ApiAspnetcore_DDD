using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Api.Domain.DTO;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Repository;
using Api.Domain.Interfaces.Services.User;
using Api.Domain.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Service.Services
{
    public class LoginService : ILoginService
    {

        private IUserRepository _repository;
        private SigningConfigurations _signingConfigurations;
        private TokenConfiguration _tokenConfiguration;
        private IConfiguration _configuration { get; }

        public LoginService(
            IUserRepository repository, SigningConfigurations SigningConfigurations, 
            TokenConfiguration tokenConfiguration, IConfiguration configuration)
        {
            _repository = repository;
            _signingConfigurations = SigningConfigurations;
            _tokenConfiguration = tokenConfiguration;
            _configuration = configuration;
        }

        private string CreateToken(ClaimsIdentity identity, DateTime createDate, DateTime expirationDate, JwtSecurityTokenHandler handler)
        {
            var securityToken = handler.CreateToken(
                new SecurityTokenDescriptor
                {
                    Issuer = _tokenConfiguration.Issuer,
                    Audience = _tokenConfiguration.Audience,
                    SigningCredentials = _signingConfigurations.SigningCredentials,
                    Subject = identity,
                    NotBefore = createDate,
                    Expires = expirationDate,
                }
            );

            return handler.WriteToken(securityToken);
        }

        private object SuccessObject(DateTime createDate, DateTime expirationDate, string token, LoginDTO user)
        {
            return new
            {
                authenticated = true,
                created = createDate.ToString("yyyy-MM-dd HH:mm:ss"),
                expiration = expirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                acessToken = token,
                userName = user.Email,
                message = "Usuario logado com sucesso"
            };
        }

        private object AutenticacaoFalha(string mensagem)
        {
            return new
            {
                authenticated = false,
                message = string.Format("Falha ao autenticar, {0}", mensagem)
            };
        }
        public async Task<object> FindByLogin(LoginDTO loginInfo)
        {
            UserEntity user = new UserEntity();
            if (loginInfo != null && !string.IsNullOrWhiteSpace(loginInfo.Email))
            {
                user = await _repository.FindByLogin(loginInfo.Email);
                if (user == null)
                {
                    return AutenticacaoFalha("usuario nao encontrado");
                }
                else
                {
                    ClaimsIdentity identity = new ClaimsIdentity(
                        new GenericIdentity(user.Email),
                        new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.Name)
                        }
                    );

                    DateTime createDate = DateTime.Now;
                    DateTime expirationDate = createDate.AddSeconds(_tokenConfiguration.Seconds);

                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    string token = CreateToken(identity, createDate, expirationDate, handler);

                    return SuccessObject(createDate, expirationDate, token, loginInfo);
                }

            }
            else
            {
                return AutenticacaoFalha("email vazio");
            }
        }

    }
}
