using AutoMapper;
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
    private readonly Mock<IMapper> _mapperMock;
    private readonly PartnerService _sut;

    public PartnerServiceGetTests()
    {
        _repositoryMock = new Mock<IPartnerRepository>();
        _mapperMock = new Mock<IMapper>();

        _sut = new PartnerService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPartnerExists_ShouldReturnDto()
    {
        // Arrange
        var partner = PartnerTestData.BuildPartner();
        var expectedDto = new PartnerDetailsDto
        {
            Id = partner.Id,
            Name = partner.Name,
            Type = PartnerType.CUSTOMER
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(PartnerTestData.ExistingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _mapperMock
            .Setup(m => m.Map<PartnerDetailsDto>(partner))
            .Returns(expectedDto);

        // Act
        var result = await _sut.GetByIdAsync(PartnerTestData.ExistingId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(partner.Id);
        result.Name.Should().Be(partner.Name);
        result.Type.Should().Be(PartnerType.CUSTOMER);
    }
}