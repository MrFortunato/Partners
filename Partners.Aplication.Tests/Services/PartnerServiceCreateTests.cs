using AutoMapper;
using FluentAssertions;
using Moq;
using Partners.Application;
using Partners.Application.Exceptions;
using Partners.Domain;
using Partners.Domain.Interfaces;
using Partners.Aplication.Tests.TestData;
using Xunit;

namespace Partners.Aplication.Tests.Services;

public class PartnerServiceCreateTests
{
    private readonly Mock<IPartnerRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PartnerService _sut;

    public PartnerServiceCreateTests()
    {
        _repositoryMock = new Mock<IPartnerRepository>();
        _mapperMock = new Mock<IMapper>();
        _sut = new PartnerService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldCreatePartnerAndReturnDto()
    {
        // Arrange
        var dto = new CreatePartnerDto
        {
            Name = PartnerTestData.ValidName,
            TaxNumber = PartnerTestData.ValidTaxNumber,
            Type = PartnerType.BOTH,
            Email = PartnerTestData.Email,
            CreditLimit = PartnerTestData.ValidCreditLimit
            
        };

        var partner = PartnerTestData.BuildPartner(name: dto.Name, type: PartnerType.BOTH);
        var expectedDto = new PartnerDetailsDto { Id = partner.Id, Name = dto.Name, Type = PartnerType.BOTH };

        _repositoryMock
            .Setup(r => r.ExistsByTaxNumberAsync(dto.TaxNumber, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mapperMock.Setup(m => m.Map<Partner>(dto)).Returns(partner);
        _mapperMock.Setup(m => m.Map<PartnerDetailsDto>(partner)).Returns(expectedDto);

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(dto.Name);
        result.Type.Should().Be(PartnerType.BOTH);
        result.Version.Should().Be(1);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenTaxNumberAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        var dto = new CreatePartnerDto
        {
            Name = PartnerTestData.ValidName,
            TaxNumber = PartnerTestData.DuplicateTaxNumber,
            Type = PartnerType.CUSTOMER,
            CreditLimit = PartnerTestData.ValidCreditLimit
        };

        _repositoryMock
            .Setup(r => r.ExistsByTaxNumberAsync(dto.TaxNumber, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<ConflictException>();
        _repositoryMock.Verify(r => r.Add(It.IsAny<Partner>()), Times.Never);
    }
}
