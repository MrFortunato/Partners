using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Partners.Api.Middlewares;
using Xunit;

namespace Partners.Api.Tests.Middlewares;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock = new();

    [Theory]
    [InlineData(typeof(ArgumentException), HttpStatusCode.BadRequest)]
    [InlineData(typeof(InvalidOperationException), HttpStatusCode.BadRequest)]
    [InlineData(typeof(KeyNotFoundException), HttpStatusCode.NotFound)]
    public async Task InvokeAsync_WhenExceptionIsThrown_ShouldReturnExpectedStatusCode(Type exceptionType, HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
        var exception = (Exception)Activator.CreateInstance(exceptionType, "Erro")!;

        var middleware = new ExceptionHandlingMiddleware(_ => throw exception, _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)expectedStatusCode);
    }
}
