using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IO;
using HotelAPI.Filters;

namespace HotelAPI.Tests.Filters;

public class ValidateFileAttributeTests
{


    [Fact]
    public void OnActionExecuting_ShouldNotAllowInValidFileFormats()
    {
        //Arrange
        //Setup mock file using a memory stream
        var content = "Hello World from a Fake File";
        var fileName = "test.pdf";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        var files = new FormFileCollection() { file };
        var formData = new FormCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues> { { "test", "value" } },
            files
            );
        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(x => x.Form).Returns(formData);

        var contextMock = new Mock<HttpContext>();
        contextMock.SetupGet(x => x.Request).Returns(requestMock.Object);

        var actionContext = new ActionContext(

            contextMock.Object,
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            Mock.Of<ModelStateDictionary>()
        );

        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            Mock.Of<Controller>()
        );

        //Act
        new ValidateFileAttribute().OnActionExecuting(actionExecutingContext);

        //Assert
        Assert.IsType<JsonResult>(actionExecutingContext.Result);
        var result = actionExecutingContext.Result as JsonResult;
        Assert.Equal(result.StatusCode,StatusCodes.Status400BadRequest);
    }

    
    [Fact]
    public void OnActionExecuting_ShouldNotAllowFilesGreaterThanLimitSize()
    {
        //Arrange
        //Setup mock file using a memory stream
        var content = "Hello World from a Fake File";
        var fileName = "test.png";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, 500*1000, "id_from_form", fileName);
        var files = new FormFileCollection() { file };
        var formData = new FormCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues> { { "test", "value" } },
            files
            );
        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(x => x.Form).Returns(formData);

        var contextMock = new Mock<HttpContext>();
        contextMock.SetupGet(x => x.Request).Returns(requestMock.Object);

        var actionContext = new ActionContext(

            contextMock.Object,
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            Mock.Of<ModelStateDictionary>()
        );

        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            Mock.Of<Controller>()
        );

        //Act
        new ValidateFileAttribute().OnActionExecuting(actionExecutingContext);

        //Assert
        Assert.IsType<JsonResult>(actionExecutingContext.Result);
        var result = actionExecutingContext.Result as JsonResult;
        Assert.Equal(result.StatusCode,StatusCodes.Status400BadRequest);
    }

    
    [Fact]
    public void OnActionExecuting_ShouldAllowValidFiles()
    {
        //Arrange
        //Setup mock file using a memory stream
        var content = "Hello World from a Fake File";
        var fileName = "test.png";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, 100, "id_from_form", fileName);
        var files = new FormFileCollection() { file };
        var formData = new FormCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues> { { "test", "value" } },
            files
            );
        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(x => x.Form).Returns(formData);

        var contextMock = new Mock<HttpContext>();
        contextMock.SetupGet(x => x.Request).Returns(requestMock.Object);

        var actionContext = new ActionContext(

            contextMock.Object,
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            Mock.Of<ModelStateDictionary>()
        );

        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            Mock.Of<Controller>()
        );

        //Act
        new ValidateFileAttribute().OnActionExecuting(actionExecutingContext);

        //Assert
        Assert.IsNotType<JsonResult>(actionExecutingContext.Result);
    }
}