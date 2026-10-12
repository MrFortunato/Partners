using FluentAssertions;
using Moq;
using Partners.Application;
using Partners.Domain;
using Partners.Domain.Interfaces;
using Partners.Aplication.Tests.TestData;

namespace Partners.Aplication.Tests.Services;

public class PartnerServiceUpdateTests
{
    private readonly Mock<IPartnerRepository> _repositoryMock;
    private readonly PartnerService _sut;

    public PartnerServiceUpdateTests()
    {
        _repositoryMock = new Mock<IPartnerRepository>();
        _sut = new PartnerService(_repositoryMock.Object);
    }

    [Fact]
    public async Task UpdateAsync_WhenPartnerExistsAndVersionMatches_ShouldUpdateAndIncrementVersion()
    {
        // Arrange
        var existing = PartnerTestData.BuildPartner(version: 1);

        var dto = new UpdatePartnerDto
        {
            Name = "Acme Updated Lda",
            TaxNumber = "987654321",
            Type = PartnerType.SUPPLIER,
            Email = "new@acme.pt",
            CreditLimit = 7500m,
            IsActive = false,
            Version = 1
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(PartnerTestData.ExistingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _repositoryMock
            .Setup(r => r.ExistsByTaxNumberAsync(dto.TaxNumber, PartnerTestData.ExistingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.UpdateAsync(PartnerTestData.ExistingId, dto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Acme Updated Lda");
        result.Type.Should().Be(PartnerType.SUPPLIER);
        result.IsActive.Should().BeFalse();
        result.Version.Should().Be(2);
    }

    [Fact]
    public async Task UpdateAsync_WhenPartnerDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var dto = new UpdatePartnerDto
        {
            Name = PartnerTestData.ValidName,
            TaxNumber = PartnerTestData.ValidTaxNumber,
            Type = PartnerType.CUSTOMER,
            CreditLimit = PartnerTestData.ValidCreditLimit,
            Version = 1
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(PartnerTestData.NonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        // Act
        var act = async () => await _sut.UpdateAsync(PartnerTestData.NonExistentId, dto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_WhenVersionDoesNotMatch_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var existing = PartnerTestData.BuildPartner(version: 2);

        var dto = new UpdatePartnerDto
        {
            Name = PartnerTestData.ValidName,
            TaxNumber = PartnerTestData.ValidTaxNumber,
            Type = PartnerType.CUSTOMER,
            CreditLimit = PartnerTestData.ValidCreditLimit,
            Version = 1 // desatualizada
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(PartnerTestData.ExistingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var act = async () => await _sut.UpdateAsync(PartnerTestData.ExistingId, dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("O registo foi modificado por outro utilizador. Por favor, atualize os dados.");
    }
}
