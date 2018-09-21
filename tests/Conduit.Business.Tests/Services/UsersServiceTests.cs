using AutoMapper;
using Conduit.Business.Services;
using Conduit.Core.Identity;
using Conduit.Core.Models;
using Conduit.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Conduit.Business.Tests.Services
{
    public class UsersServiceTests
    {
        private readonly UsersService _usersService;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IJwtFactory> _jwtFactoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public UsersServiceTests()
        {
            _userManagerMock = IdentityMocksProvider.GetMockUserManager();
            _jwtFactoryMock = new Mock<IJwtFactory>();
            _mapperMock = new Mock<IMapper>();

            _usersService = new UsersService(
                _userManagerMock.Object,
                _jwtFactoryMock.Object,
                _mapperMock.Object);
        }

        [Theory]
        [CustomAutoData]
        public async Task Login_Should_Return_Jwt(CredentialsModel model, User expectedUser, string expectedJwt)
        {
            // Arrange
            MockFindByEmail(model.Email, expectedUser);

            MockCheckPassword(expectedUser, model.Password, true);

            _jwtFactoryMock.Setup(jwtFactory => jwtFactory
                .GenerateEncodedToken(expectedUser.Id, expectedUser.Email, new List<Claim>()))
                .Returns(expectedJwt);

            _mapperMock.Setup(mapper => mapper
                .Map<UserModel>(expectedUser))
                .Returns(new UserModel());

            // Act
            var result = await _usersService.LoginAsync(model);

            // Assert
            result.Exists(jwt => jwt.Token == expectedJwt).ShouldBeTrue();
        }

        [Theory]
        [CustomAutoData]
        public async Task Login_Should_Return_Exception_When_Credentials_Are_Invalid(CredentialsModel model, User expectedUser)
        {
            // Arrange
            MockFindByEmail(model.Email, expectedUser);

            MockCheckPassword(expectedUser, model.Password, false);

            // Act
            var result = await _usersService.LoginAsync(model);

            // Assert
            result.HasValue.ShouldBeFalse();
            result.MatchNone(error => error.Messages?.Count.ShouldBeGreaterThan(0));
        }

        [Theory]
        [CustomAutoData]
        public async Task Register_Should_Register_User(
            RegisterUserModel model,
            User userToRegister,
            UserModel userToReturn)
        {
            // Arrange
            MockMapper(model, userToRegister);

            MockMapper(userToRegister, userToReturn);

            _userManagerMock.Setup(userManager => userManager
                .CreateAsync(userToRegister, model.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _usersService.RegisterAsync(model);

            // Assert
            result.Exists(u =>
                u.Email == userToReturn.Email &&
                u.Username == userToReturn.Username &&
                u.Bio == userToReturn.Bio &&
                u.Image == userToReturn.Image &&
                u.Token == userToReturn.Token)
                .ShouldBeTrue();
        }

        [Theory]
        [CustomAutoData]
        public async Task Register_Should_Return_Validation_Errors(
            RegisterUserModel model,
            User userToRegister,
            IdentityError[] expectedErrors)
        {
            // Arrange
            MockMapper(model, userToRegister);

            _userManagerMock.Setup(userManager => userManager
                .CreateAsync(userToRegister, model.Password))
                .ReturnsAsync(IdentityResult.Failed(expectedErrors));

            // Act
            var result = await _usersService.RegisterAsync(model);

            // Assert
            result.HasValue.ShouldBeFalse();
            result.MatchNone(error => error.Messages
                .ShouldAllBe(message => expectedErrors
                    .Any(expectedError => expectedError.Description == message)));
        }

        private void MockMapper<T, TExpected>(T model, TExpected expected) =>
            _mapperMock.Setup(mapper => mapper
                .Map<TExpected>(model))
                .Returns(expected);

        private void MockCheckPassword(User user, string password, bool result) =>
            _userManagerMock.Setup(userManager => userManager
                .CheckPasswordAsync(user, password))
                .ReturnsAsync(result);

        private void MockFindByEmail(string email, User expected) =>
            _userManagerMock.Setup(userManager => userManager
                .FindByEmailAsync(email))
                .ReturnsAsync(expected);
    }
}
