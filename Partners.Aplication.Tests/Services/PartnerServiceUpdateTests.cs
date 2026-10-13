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

public class PartnerServiceUpdateTests
{
    private readonly Mock<IPartnerRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PartnerService _sut;

    public PartnerServiceUpdateTests()
    {
        _repositoryMock = new Mock<IPartnerRepository>();
        _mapperMock = new Mock<IMapper>();
        _sut = new PartnerService(_repositoryMock.Object, _mapperMock.Object);
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
            Email = "new@acme.ao",
            CreditLimit = 7500m,
            IsActive = false,
            Version = 1
        };

        var expectedDto = new PartnerDetailsDto
        {
            Id = existing.Id,
            Name = "Acme Updated Lda",
            Type = PartnerType.SUPPLIER,
            IsActive = false,
            Version = 2
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(PartnerTestData.ExistingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _repositoryMock
            .Setup(r => r.ExistsByTaxNumberAsync(dto.TaxNumber, PartnerTestData.ExistingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mapperMock
            .Setup(m => m.Map(dto, existing))
            .Returns(existing);

        _mapperMock
            .Setup(m => m.Map<PartnerDetailsDto>(existing))
            .Returns(expectedDto);

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
    public async Task UpdateAsync_WhenVersionDoesNotMatch_ShouldThrowConflictException()
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

        // Assert — mantém ConflictException conforme pedido
        await act.Should().ThrowAsync<ConflictException>();
    }
}
