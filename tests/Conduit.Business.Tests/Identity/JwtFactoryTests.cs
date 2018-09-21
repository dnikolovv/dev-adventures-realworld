using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Conduit.Business.Identity;
using Conduit.Core.Configuration;
using Xunit;

namespace Conduit.Business.Tests.Identity
{
    public class JwtFactoryTests
    {
        private readonly JwtFactory _jwtFactory;
        private readonly JwtConfiguration _jwtConfiguration;

        public JwtFactoryTests()
        {
            var fixture = new Fixture();

            var signingKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(fixture.Create<string>()));

            _jwtConfiguration = fixture
                .Build<JwtConfiguration>()
                .Without(config => config.NotBefore)
                .With(config => config.SigningCredentials, new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256))
                .Create();

            var configuration = Options.Create(_jwtConfiguration);

            _jwtFactory = new JwtFactory(configuration);
        }

        [Theory]
        [CustomAutoData]
        public void GenerateEncodedToken_Should_Generate_Proper_Token_With_Extra_Claims(string userId, string email, Fixture claimsFixture)
        {
            // Arrange
            // To enable AutoFixture to generate claims
            claimsFixture.Behaviors.Add(new OmitOnRecursionBehavior());
            claimsFixture.Customize<Claim>(
                c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery())));

            var claims = claimsFixture.Create<List<Claim>>();

            // Act
            var result = _jwtFactory.GenerateEncodedToken(userId, email, claims);

            // Assert
            var jwt = new JwtSecurityToken(result);

            jwt.Claims.ShouldContain(c => c.Type == JwtRegisteredClaimNames.Iss && c.Value == _jwtConfiguration.Issuer);
            jwt.Claims.ShouldContain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId);
            jwt.Claims.ShouldContain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
            jwt.Claims.ShouldContain(c => c.Type == JwtRegisteredClaimNames.Aud && c.Value == _jwtConfiguration.Audience);

            // The jwt should contain all of the extra claims
            claims.ShouldAllBe(c => jwt.Claims.Any(x => x.Type == c.Type && x.Value == c.Value));

            jwt.ValidFrom.ShouldBe(_jwtConfiguration.NotBefore, TimeSpan.FromSeconds(10));
            jwt.ValidTo.ShouldBe(_jwtConfiguration.Expiration, TimeSpan.FromSeconds(10));
        }

        [Theory]
        [CustomAutoData]
        public void GenerateEncodedToken_Should_Generate_Proper_Token(string userId, string email)
        {
            // Arrange
            // Act
            var result = _jwtFactory.GenerateEncodedToken(userId, email, new List<Claim>());

            // Assert
            var jwt = new JwtSecurityToken(result);

            jwt.Claims.ShouldContain(c => c.Type == JwtRegisteredClaimNames.Iss && c.Value == _jwtConfiguration.Issuer);
            jwt.Claims.ShouldContain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId);
            jwt.Claims.ShouldContain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
            jwt.Claims.ShouldContain(c => c.Type == JwtRegisteredClaimNames.Aud && c.Value == _jwtConfiguration.Audience);

            jwt.ValidFrom.ShouldBe(_jwtConfiguration.NotBefore, TimeSpan.FromSeconds(10));
            jwt.ValidTo.ShouldBe(_jwtConfiguration.Expiration, TimeSpan.FromSeconds(10));
        }
    }
}
