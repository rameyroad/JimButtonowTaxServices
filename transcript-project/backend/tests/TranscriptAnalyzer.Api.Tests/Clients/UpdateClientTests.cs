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
public class UpdateClientTests
{
    private readonly ApiTestFixture _fixture;

    public UpdateClientTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region T064: Contract test for PATCH /clients/{id}

    [Fact]
    public async Task UpdateClient_WithValidData_ReturnsOk()
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
            }
        };

        var createResponse = await client.PostAsJsonAsync("/api/v1/clients", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act - Update email and phone
        var updateRequest = new
        {
            email = "john.updated@example.com",
            phone = "555-999-8888",
            version = created!.Version
        };

        var response = await client.PatchAsJsonAsync($"/api/v1/clients/{created.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ClientDto>();
        result.Should().NotBeNull();
        result!.Email.Should().Be("john.updated@example.com");
        result.Phone.Should().Be("555-999-8888");
        result.Version.Should().BeGreaterThan(created.Version);
    }

    [Fact]
    public async Task UpdateClient_CanUpdateAddress()
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
                street1 = "123 Old St",
                city = "Old City",
                state = "OC",
                postalCode = "11111",
                country = "US"
            }
        };

        var createResponse = await client.PostAsJsonAsync("/api/v1/clients", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act - Update address
        var updateRequest = new
        {
            address = new
            {
                street1 = "456 New Ave",
                street2 = "Suite 100",
                city = "New City",
                state = "NC",
                postalCode = "22222",
                country = "US"
            },
            version = created!.Version
        };

        var response = await client.PatchAsJsonAsync($"/api/v1/clients/{created.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ClientDto>();
        result!.Address!.Street1.Should().Be("456 New Ave");
        result.Address.Street2.Should().Be("Suite 100");
        result.Address.City.Should().Be("New City");
    }

    [Fact]
    public async Task UpdateClient_CanUpdateNotes()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var createRequest = new
        {
            clientType = "Individual",
            firstName = "Test",
            lastName = "User",
            taxIdentifier = "111-22-3333",
            email = "test@example.com",
            address = new
            {
                street1 = "123 Test St",
                city = "Test City",
                state = "TS",
                postalCode = "12345",
                country = "US"
            }
        };

        var createResponse = await client.PostAsJsonAsync("/api/v1/clients", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act - Update notes
        var updateRequest = new
        {
            notes = "Updated notes for this client",
            version = created!.Version
        };

        var response = await client.PatchAsJsonAsync($"/api/v1/clients/{created.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateClient_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);
        var invalidId = Guid.NewGuid();

        var updateRequest = new
        {
            email = "updated@example.com",
            version = 1
        };

        // Act
        var response = await client.PatchAsJsonAsync($"/api/v1/clients/{invalidId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateClient_FromOtherTenant_ReturnsNotFound()
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

        // Act - Try to update from tenant B
        var updateRequest = new
        {
            email = "hacked@evil.com",
            version = created!.Version
        };

        var response = await clientB.PatchAsJsonAsync($"/api/v1/clients/{created.Id}", updateRequest);

        // Assert - Should return 404 to prevent enumeration attacks
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region T065: Contract test for optimistic concurrency (version conflict)

    [Fact]
    public async Task UpdateClient_WithStaleVersion_ReturnsConflict()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

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

        var createResponse = await client.PostAsJsonAsync("/api/v1/clients", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // First update succeeds
        var firstUpdate = new
        {
            email = "first.update@example.com",
            version = created!.Version
        };

        var firstResponse = await client.PatchAsJsonAsync($"/api/v1/clients/{created.Id}", firstUpdate);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act - Second update with stale version should fail
        var secondUpdate = new
        {
            email = "second.update@example.com",
            version = created.Version // Using old version
        };

        var secondResponse = await client.PatchAsJsonAsync($"/api/v1/clients/{created.Id}", secondUpdate);

        // Assert
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateClient_WithoutVersion_ReturnsBadRequest()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

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

        var createResponse = await client.PostAsJsonAsync("/api/v1/clients", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act - Update without version
        var updateRequest = new
        {
            email = "no.version@example.com"
            // Missing version field
        };

        var response = await client.PatchAsJsonAsync($"/api/v1/clients/{created!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region T066: Contract test for role restriction on edit

    [Theory]
    [InlineData("Admin", HttpStatusCode.OK)]
    [InlineData("TaxProfessional", HttpStatusCode.OK)]
    [InlineData("ReadOnly", HttpStatusCode.Forbidden)]
    public async Task UpdateClient_RoleRestriction(string role, HttpStatusCode expectedStatus)
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
            taxIdentifier = $"{Random.Shared.Next(100, 665)}-{Random.Shared.Next(10, 99)}-{Random.Shared.Next(1000, 9999)}",
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

        // Act - Try to update with specified role
        var updateRequest = new
        {
            email = "role.test@example.com",
            version = created!.Version
        };

        var response = await roleClient.PatchAsJsonAsync($"/api/v1/clients/{created.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(expectedStatus);
    }

    #endregion

    #region Business client updates

    [Fact]
    public async Task UpdateBusinessClient_CanUpdateBusinessFields()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var createRequest = new
        {
            clientType = "Business",
            businessName = "Original Corp",
            entityType = "LLC",
            taxIdentifier = "12-3456789",
            email = "info@original.com",
            address = new
            {
                street1 = "123 Business Blvd",
                city = "Chicago",
                state = "IL",
                postalCode = "60601",
                country = "US"
            }
        };

        var createResponse = await client.PostAsJsonAsync("/api/v1/clients", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act - Update business fields
        var updateRequest = new
        {
            businessName = "Updated Corporation",
            entityType = "CCorp",
            responsibleParty = "Jane Smith",
            email = "info@updated.com",
            version = created!.Version
        };

        var response = await client.PatchAsJsonAsync($"/api/v1/clients/{created.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ClientDto>();
        result!.BusinessName.Should().Be("Updated Corporation");
        result.EntityType.Should().Be("CCorp");
        result.Email.Should().Be("info@updated.com");
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
