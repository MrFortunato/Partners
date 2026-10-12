using FluentAssertions;
using Moq;
using Partners.Application;
using Partners.Domain;
using Partners.Domain.Interfaces;
using Partners.Aplication.Tests.TestData;

namespace Partners.Aplication.Tests.Services;

public class PartnerServiceCreateTests
{
    private readonly Mock<IPartnerRepository> _repositoryMock;
    private readonly PartnerService _sut;

    public PartnerServiceCreateTests()
    {
        _repositoryMock = new Mock<IPartnerRepository>();
        _sut = new PartnerService(_repositoryMock.Object);
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

        _repositoryMock
            .Setup(r => r.ExistsByTaxNumberAsync(dto.TaxNumber, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

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
    public async Task CreateAsync_WhenTaxNumberAlreadyExists_ShouldThrowInvalidOperationException()
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

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Já existe um parceiro com este número fiscal.");
        _repositoryMock.Verify(r => r.Add(It.IsAny<Partner>()), Times.Never);
    }
}
