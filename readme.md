
[![Build Status](https://travis-ci.com/dnikolovv/dev-adventures-realworld.svg?branch=master)](https://travis-ci.com/dnikolovv/dev-adventures-realworld)

# ![RealWorld Example App](logo.png)

> ### A functionally written ASP.NET Core codebase containing real world examples (CRUD, auth, advanced patterns, etc) that adheres to the [RealWorld](https://github.com/gothinkster/realworld) spec and API.


### [RealWorld Repo](https://github.com/gothinkster/realworld)


This codebase was created to demonstrate a fully fledged backend application built with **ASP.NET Core** and **[Optional](https://github.com/nlkl/Optional)**. It includes CRUD operations, authentication, routing, pagination, and more.

It completely adheres to the **ASP.NET Core** community styleguides & best practices.

For more information on how to this works with other frontends/backends, head over to the [RealWorld](https://github.com/gothinkster/realworld) repo.

# Features

What's special about this specific implementation is that it employs a different approach on error handling and propagation. It uses the **Maybe** and **Either** monads to enable very explicit function declarations and allow us to abstract the conditionals and validations into the type itself.

This allows you to do cool stuff like:

```csharp
public Task<Option<UserModel, Error>> LoginAsync(CredentialsModel model) =>
    GetUser(u => u.Email == model.Email)
        .FilterAsync<User, Error>(user => UserManager.CheckPasswordAsync(user, model.Password), "Invalid credentials.")
        .MapAsync(async user =>
        {
            var result = Mapper.Map<UserModel>(user);

            result.Token = GenerateToken(user.Id, user.Email);

            return result;
        });
```

You can read more about **Maybe** and **Either** [here](https://devadventures.net/2018/04/17/forget-object-reference-not-set-to-an-instance-of-an-object-functional-adventures-in-c/) and [here](https://devadventures.net/2018/09/20/real-life-examples-of-functional-c-sharp-either/).

# Architecture

This application has been made using the [Dev Adventures .NET Core template](https://marketplace.visualstudio.com/items?itemName=dnikolovv.dev-adventures-project-setup), therefore it follows the architecture of and has all of the features that the template provides.

- [x] Swagger UI + Fully Documented Controllers

![swagger-ui](https://devadventures.net/wp-content/uploads/2018/09/swagger-1.png)

- [x] Thin Controllers

```csharp
/// <summary>
/// Retreives a user's profile by username.
/// </summary>
/// <param name="username">The username to look for.</param>
/// <returns>A user profile or not found.</returns>
/// <response code="200">Returns the user's profile.</response>
/// <response code="404">No user with tha given username was found.</response>
[HttpGet("{username}")]
[ProducesResponseType(typeof(UserProfileModel), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> Get(string username) =>
    (await _profilesService.ViewProfileAsync(CurrentUserId.SomeNotNull(), username))
    .Match<IActionResult>(profile => Ok(new { profile }), Error);
```

- [x] Robust service layer using the [Either](https://devadventures.net/2018/09/20/real-life-examples-of-functional-c-sharp-either/) monad.

```csharp
public interface IProfilesService
{
    Task<Option<UserProfileModel, Error>> FollowAsync(string followerId, string userToFollowUsername);

    Task<Option<UserProfileModel, Error>> UnfollowAsync(string followerId, string userToUnfollowUsername);

    Task<Option<UserProfileModel, Error>> ViewProfileAsync(Option<string> viewingUserId, string profileUsername);
}
```

```csharp
public Task<Option<UserProfileModel, Error>> FollowAsync(string followerId, string userToFollowUsername) =>
    GetUserByIdOrError(followerId).FlatMapAsync(user =>
    GetUserByNameOrError(userToFollowUsername)
        .FilterAsync(async u => u.Id != followerId, "A user cannot follow himself.")
        .FilterAsync(async u => user.Following.All(fu => fu.FollowingId != u.Id), "You are already following this user")
        .FlatMapAsync(async userToFollow =>
        {
            DbContext.FollowedUsers.Add(new FollowedUser
            {
                FollowerId = followerId,
                FollowingId = userToFollow.Id
            });

            await DbContext.SaveChangesAsync();

            return await ViewProfileAsync(followerId.Some(), userToFollow.UserName);
        }));
```

- [x] Safe query string parameter model binding using the [Option](https://github.com/nlkl/Optional) monad.

```csharp
public class GetArticlesModel
{
  public Option<string> Tag { get; set; }

  public Option<string> Author { get; set; }

  public Option<string> Favorited { get; set; }

  public int Limit { get; set; } = 20;

  public int Offset { get; set; } = 0;
}
```

- [x] AutoMapper
- [x] EntityFramework Core with ~~SQL Server~~ Postgres and ASP.NET Identity
- [x] JWT authentication/authorization
- [x] File logging with Serilog
- [x] Stylecop
- [x] Neat folder structure
```
├───src
│   ├───configuration
│   └───server
│       ├───Conduit.Api
│       ├───Conduit.Business
│       ├───Conduit.Core
│       ├───Conduit.Data
│       └───Conduit.Data.EntityFramework
└───tests
    └───Conduit.Business.Tests
```

- [x] Global Model Errors Handler

```json
{
  "messages": [
    "The Email field is not a valid email.",
    "The LastName field is required.",
    "The FirstName field is required."
  ]
}
```

- [x] Global Environment-Dependent Exception Handler

```json
// Development
{
    "ClassName": "System.Exception",
    "Message": null,
    "Data": null,
    "InnerException": null,
    "HelpURL": null,
    "StackTraceString": "...",
    "RemoteStackTraceString": null,
    "RemoteStackIndex": 0,
    "ExceptionMethod": null,
    "HResult": -2146233088,
    "Source": "Conduit.Api",
    "WatsonBuckets": null
}

// Production
{
  "messages": [
    "An unexpected internal server error has occurred."
  ]
}
```

- [x] Neatly organized solution structure <br>
![solution-structure](https://devadventures.net/wp-content/uploads/2018/09/solution-structure.png)

### Test Suite
- [x] xUnit
- [x] Autofixture
- [x] Moq
- [x] Shouldly
- [x] Arrange Act Assert Pattern

```csharp
[Theory]
[AutoData]
public async Task Login_Should_Return_Exception_When_Credentials_Are_Invalid(CredentialsModel model, User expectedUser)
{
    // Arrange
    AddUserWithEmail(model.Email, expectedUser);

    MockCheckPassword(model.Password, false);

    // Act
    var result = await _usersService.LoginAsync(model);

    // Assert
    result.HasValue.ShouldBeFalse();
    result.MatchNone(error => error.Messages?.Count.ShouldBeGreaterThan(0));
}
```

# Getting started

1. Set the connection string in `src/server/Conduit.Api/appsettings.json` to a running Postgres instance. Set the database name to an unexisting one.
2. Execute `dotnet restore`
3. Execute `dotnet build`
4. Execute `dotnet ef database update`
5. Execute `dotnet run`
