using FluentAssertions;
using Moq;
using Partners.Application;
using Partners.Domain;
using Partners.Domain.Interfaces;
using Partners.Aplication.Tests.TestData;
using Xunit;

namespace Partners.Aplication.Tests.Services;

public class PartnerServiceGetTests
{
    private readonly Mock<IPartnerRepository> _repositoryMock;
    private readonly PartnerService _sut;

    public PartnerServiceGetTests()
    {
        _repositoryMock = new Mock<IPartnerRepository>();
        _sut = new PartnerService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPartnerExists_ShouldReturnDto()
    {
        // Arrange
        var partner = PartnerTestData.BuildPartner();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(PartnerTestData.ExistingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        // Act
        var result = await _sut.GetByIdAsync(PartnerTestData.ExistingId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(partner.Id);
        result.Name.Should().Be(partner.Name);
        result.Type.Should().Be(PartnerType.CUSTOMER);
    }
}
