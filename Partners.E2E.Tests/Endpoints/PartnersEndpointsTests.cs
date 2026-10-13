using FluentAssertions;
using Partners.Application;
using Partners.Domain;
using Partners.E2E.Tests.Infrastructure;
using Partners.E2E.Tests.Models;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Partners.E2E.Tests.Endpoints;

/// <summary>
/// Testes E2E essenciais — fluxos completos HTTP de ponta a ponta.
/// </summary>
public class PartnersEndpointsTests : IClassFixture<PartnersWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly PartnersWebAppFactory _factory;

    public PartnersEndpointsTests(PartnersWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_CreatePartner_WithValidPayload_ShouldReturn201Created()
    {
        // Arrange
        CreatePartnerDto request = new()
        {
            Name = "Acme Lda",
            TaxNumber = "123456789",
            Type = PartnerType.CUSTOMER,
            Email = "contact@acme.ao",
            CreditLimit = 5000m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/partners", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var created = await response.Content.ReadFromJsonAsync<PartnerResponse>();
        created!.Name.Should().Be("Acme Lda");
        created.Type.Should().Be("CUSTOMER");
        created.Version.Should().Be(1);
    }

    [Fact]
    public async Task GET_PartnerById_WhenExists_ShouldReturn200Ok()
    {
        // Arrange
        var id = Guid.NewGuid();
        await _factory.SeedAsync(async db =>
        {
            db.Partners.Add(new Partners.Domain.Partner
            {
                Id = id,
                Name = "Acme Lda",
                TaxNumber = "333333333",
                Type = "BOTH",
                CreditLimit = 5000m,
                IsActive = true,
                Version = 1,
                CreatedAt = DateTime.UtcNow
            });
        });

        // Act
        var response = await _client.GetAsync($"/api/partners/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var partner = await response.Content.ReadFromJsonAsync<PartnerResponse>();
        partner!.Id.Should().Be(id);
        partner.Type.Should().Be("BOTH");
    }

    [Fact]
    public async Task FullLifecycle_Create_Update_Get_ShouldPersistChanges()
    {
        // 1. CREATE
        CreatePartnerDto createRequest = new()
        {
            Name = "Lifecycle Partner",
            TaxNumber = "777777777",
            Type = PartnerType.CUSTOMER, 
            CreditLimit = 3000m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/partners", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<PartnerResponse>();

        // 2. UPDATE
        UpdatePartnerDto updateRequest = new()
        {
            Name = "Lifecycle Updated",
            TaxNumber = "888888888",
            Type = PartnerType.SUPPLIER,
            CreditLimit = 6000m,
            IsActive = false,
            Version = created!.Version
        };
        var updateResponse = await _client.PutAsJsonAsync($"/api/partners/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3. GET (confirma persistência)
        var getResponse = await _client.GetAsync($"/api/partners/{created.Id}");
        var final = await getResponse.Content.ReadFromJsonAsync<PartnerResponse>();
        final!.Name.Should().Be("Lifecycle Updated");
        final.Type.Should().Be("SUPPLIER");
        final.IsActive.Should().BeFalse();
        final.Version.Should().Be(2);
    }
}
