using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TranscriptAnalyzer.Api.Tests.Common;
using TranscriptAnalyzer.Api.Tests.Fixtures;
using TranscriptAnalyzer.Infrastructure.Persistence;

namespace TranscriptAnalyzer.Api.Tests.Clients;

[Collection("Api")]
public class GetClientTests
{
    private readonly ApiTestFixture _fixture;

    public GetClientTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region T063: Contract test for GET /clients/{id}

    [Fact]
    public async Task GetClient_WithValidId_ReturnsClient()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        // Create a client first
        var createRequest = new
        {
            clientType = "Individual",
            firstName = "John",
            lastName = "Doe",
            taxIdentifier = "123-45-6789",
            email = "john.doe@example.com",
            phone = "555-123-4567",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            },
            notes = "Test notes"
        };

        var createResponse = await client.PostAsJsonAsync("/api/v1/clients", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act
        var response = await client.GetAsync($"/api/v1/clients/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ClientDetailDto>(TestJsonOptions.Default);
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.ClientType.Should().Be("Individual");
        result.DisplayName.Should().Be("Doe, John");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john.doe@example.com");
        result.Phone.Should().Be("555-123-4567");
        result.TaxIdentifierLast4.Should().Be("6789");
        result.TaxIdentifierMasked.Should().Be("***-**-6789");
        result.Address.Should().NotBeNull();
        result.Address!.Street1.Should().Be("123 Main St");
        result.Address.City.Should().Be("New York");
        result.Version.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetClient_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);
        var invalidId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/v1/clients/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetClient_FromOtherTenant_ReturnsNotFound()
    {
        // Arrange
        await CleanupClientsAsync();
        using var clientA = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);
        using var clientB = _fixture.CreateClientWithTenant(_fixture.TenantBId, _fixture.UserBId);

        // Create a client in tenant A
        var createRequest = new
        {
            clientType = "Individual",
            firstName = "John",
            lastName = "Doe",
            taxIdentifier = "123-45-6789",
            email = "john@example.com",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        var createResponse = await clientA.PostAsJsonAsync("/api/v1/clients", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act - Try to access from tenant B
        var response = await clientB.GetAsync($"/api/v1/clients/{created!.Id}");

        // Assert - Should return 404 to prevent enumeration attacks
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region T068: Contract test verifying full SSN/EIN never returned

    [Fact]
    public async Task GetClient_NeverReturnsFullSsn()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var createRequest = new
        {
            clientType = "Individual",
            firstName = "Jane",
            lastName = "Smith",
            taxIdentifier = "456-78-9012",
            email = "jane@example.com",
            address = new
            {
                street1 = "456 Oak Ave",
                city = "Boston",
                state = "MA",
                postalCode = "02101",
                country = "US"
            }
        };

        var createResponse = await client.PostAsJsonAsync("/api/v1/clients", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act
        var response = await client.GetAsync($"/api/v1/clients/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();

        // Verify full SSN is NOT in response
        content.Should().NotContain("456-78-9012");
        content.Should().NotContain("456789012");

        // Verify only masked version is present
        content.Should().Contain("***-**-9012");
        content.Should().Contain("9012");
    }

    [Fact]
    public async Task GetClient_NeverReturnsFullEin()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var createRequest = new
        {
            clientType = "Business",
            businessName = "Test Corp",
            entityType = "LLC",
            taxIdentifier = "12-3456789",
            email = "info@testcorp.com",
            address = new
            {
                street1 = "789 Business Blvd",
                city = "Chicago",
                state = "IL",
                postalCode = "60601",
                country = "US"
            }
        };

        var createResponse = await client.PostAsJsonAsync("/api/v1/clients", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act
        var response = await client.GetAsync($"/api/v1/clients/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();

        // Verify full EIN is NOT in response
        content.Should().NotContain("12-3456789");
        content.Should().NotContain("123456789");

        // Verify only masked version is present
        content.Should().Contain("**-***6789");
        content.Should().Contain("6789");
    }

    #endregion

    #region Role-based access (all roles can view)

    [Theory]
    [InlineData("Admin")]
    [InlineData("TaxProfessional")]
    [InlineData("ReadOnly")]
    public async Task GetClient_AllRolesCanView(string role)
    {
        // Arrange
        await CleanupClientsAsync();
        using var adminClient = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, "Admin");
        using var roleClient = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, role);

        // Create a client as admin
        var createRequest = new
        {
            clientType = "Individual",
            firstName = "Test",
            lastName = "User",
            taxIdentifier = "111-22-3333",
            email = $"test-{Guid.NewGuid():N}@example.com",
            address = new
            {
                street1 = "123 Test St",
                city = "Test City",
                state = "TS",
                postalCode = "12345",
                country = "US"
            }
        };

        var createResponse = await adminClient.PostAsJsonAsync("/api/v1/clients", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act - Access with specified role
        var response = await roleClient.GetAsync($"/api/v1/clients/{created!.Id}");

        // Assert - All roles should be able to view
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Helper Methods

    private async Task CleanupClientsAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.ExecuteSqlRawAsync("DELETE FROM clients");
    }

    #endregion
}

// DTOs for test deserialization
public record ClientDetailDto
{
    public Guid Id { get; init; }
    public string ClientType { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? BusinessName { get; init; }
    public string? EntityType { get; init; }
    public string? ResponsibleParty { get; init; }
    public string TaxIdentifierLast4 { get; init; } = string.Empty;
    public string TaxIdentifierMasked { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public AddressDto? Address { get; init; }
    public string? Notes { get; init; }
    public bool IsArchived { get; init; }
    public int Version { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public int ActiveAuthorizationCount { get; init; }
    public int TranscriptCount { get; init; }
}
