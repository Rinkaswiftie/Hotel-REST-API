# Hotel REST API
 A small hotel REST API built to practice .NET Core fundamentals with Authentication and Authorization, Swagger Doc integration and Unit tests.

[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/built-with-love.svg)](https://forthebadge.com)


Features
- A RESTful Web api with .NET CORE 6 using an In-Memory Database provider
- Documented with Swagger with Open API Specification V3 using Swashbuckle
- Authentication and Authorization enabled using JWT Tokens for Access and refresh tokens
- Unit tests with code coverage reporter using XUnit and Cobertura.

![](header.png)

## Image screenshots

![APIs](https://user-images.githubusercontent.com/45427686/204125818-250711a6-f561-43be-81e4-73aad4f3e36c.png)
```sh
Calculating coverage result...
+----------+--------+--------+--------+
| Module   | Line   | Branch | Method |
+----------+--------+--------+--------+
| HotelAPI | 63.28% | 57.44% | 80.73% |
+----------+--------+--------+--------+

+---------+--------+--------+--------+
|         | Line   | Branch | Method |
+---------+--------+--------+--------+
| Total   | 63.28% | 57.44% | 80.73% |
+---------+--------+--------+--------+
| Average | 63.28% | 57.44% | 80.73% |
+---------+--------+--------+--------+
```

## Development setup

Install .NET Core 6 and Navigate to the project directory and use the following command to install dependant packages

```sh
dotnet restore
```

Launch the application from Visual Studio or run from the command line using the following command.

```sh
dotnet run
```
To run unit tests, use the following command

```sh
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

## Author

Rinka Viswathirupathi

Distributed under the MIT license. See ``LICENSE`` for more information.

[https://github.com/Rinkaswiftie/Hotel-REST-API]([https://github.com/Rinkaswiftie/](https://github.com/Rinkaswiftie/))

## Contribute

1. Fork it (<https://github.com/Rinkaswiftie/Hotel-REST-API/fork>)
2. Create your feature branch (`git checkout -b feature/fooBar`)
3. Commit your changes (`git commit -am 'Add some fooBar'`)
4. Push to the branch (`git push origin feature/fooBar`)
5. Create a new Pull Request

