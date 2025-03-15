namespace MsClean.Application;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class SignInCommandHandler : IRequestHandler<SignInCommand, string>
{
    private readonly IConfiguration _configuration;
    public SignInCommandHandler(IConfiguration configuration) => _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public async Task<string> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityToken:Key"]));

        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokeOptions = new JwtSecurityToken(
                issuer: _configuration["JwtSecurityToken:Issuer"],
                audience: _configuration["JwtSecurityToken:Audience"],
                claims:
                [
                        new Claim("username", request.UserName),
                        new Claim("rol", "admin")
                ],
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signinCredentials
            );

        var token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

        return await Task.FromResult(token);
    }
}
