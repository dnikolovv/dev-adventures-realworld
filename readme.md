
# ![RealWorld Example App](logo.png)

> ### A functionally written ASP.NET Core codebase containing real world examples (CRUD, auth, advanced patterns, etc) that adheres to the [RealWorld](https://github.com/gothinkster/realworld) spec and API.


### [Demo RealWorld](https://github.com/gothinkster/realworld)


This codebase was created to demonstrate a fully fledged backend application built with **ASP.NET Core** including CRUD operations, authentication, routing, pagination, and more.

We've gone to great lengths to adhere to the **ASP.NET Core** community styleguides & best practices.

For more information on how to this works with other frontends/backends, head over to the [RealWorld](https://github.com/gothinkster/realworld) repo.

# How it works

What's special about this specific implementation is that it employs different functional programming principles. You can read more about these [here](https://devadventures.net/2018/04/17/forget-object-reference-not-set-to-an-instance-of-an-object-functional-adventures-in-c/) and [here](https://devadventures.net/2018/09/20/real-life-examples-of-functional-c-sharp-either/).

This application has been written using the [Dev Adventures .NET Core template](https://marketplace.visualstudio.com/items?itemName=dnikolovv.dev-adventures-project-setup), therefore it has all of the features that the template provides.

- [x] AutoMapper
- [x] EntityFramework Core with SQL Server and ASP.NET Identity
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


- [x] Swagger UI + Fully Documented Controllers <br>

![swagger-ui](https://devadventures.net/wp-content/uploads/2018/09/swagger.png)

- [x] Global Model Errors Handler <br>

![model-errors](https://devadventures.net/wp-content/uploads/2018/05/model-errors.png)
- [x] Global Environment-Dependent Exception Handler <br>
Development <br>
![exception-development](https://devadventures.net/wp-content/uploads/2018/06/exception-development.png)<br> 
Production <br>
![enter image description here](https://devadventures.net/wp-content/uploads/2018/05/exception-production.png)
- [x] Neatly organized solution structure <br>
![solution-structure](https://devadventures.net/wp-content/uploads/2018/09/solution-structure.png)
- [x] Thin Controllers <br>
![thin-controllers](https://devadventures.net/wp-content/uploads/2018/09/thin-controllers.png) <br>
- [x] Robust service layer using the [Either](http://optional-github.com) monad. <br>
![either-monad](https://devadventures.net/wp-content/uploads/2018/09/robust-service-layer.png)<br>
![service-example](https://devadventures.net/wp-content/uploads/2018/09/service-layer-article-comments.png)
- [x] Safe query string parameter model binding using the [Option](http://optional-github.com) monad.<br>
![optional-binding](https://devadventures.net/wp-content/uploads/2018/09/get-articles-model.png)<br>

### Test Suite
- [x] xUnit
- [x] Autofixture
- [x] Moq
- [x] Shouldly
- [x] Arrange Act Assert Pattern

![enter image description here](https://devadventures.net/wp-content/uploads/2018/09/tests.png)

# Getting started

> npm install, npm start, etc.

