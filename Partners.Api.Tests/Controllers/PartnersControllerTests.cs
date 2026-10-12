using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Partners.Api.Controllers;
using Partners.Application;
using Xunit;

namespace Partners.Api.Tests.Controllers;

public class PartnersControllerTests
{
    private readonly Mock<IPartnerService> _serviceMock;
    private readonly PartnersController _sut;

    public PartnersControllerTests()
    {
        _serviceMock = new Mock<IPartnerService>();
        _sut = new PartnersController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetById_WhenPartnerExists_ShouldReturn200Ok()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new PartnerDetailsDto { Id = id, Name = "Acme Lda" };

        _serviceMock
            .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _sut.GetById(id);

        // Assert
        var okResult = result.Result.As<OkObjectResult>();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.As<PartnerDetailsDto>().Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_WhenPartnerDoesNotExist_ShouldReturn404NotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _serviceMock
            .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PartnerDetailsDto?)null);

        // Act
        var result = await _sut.GetById(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }
}
