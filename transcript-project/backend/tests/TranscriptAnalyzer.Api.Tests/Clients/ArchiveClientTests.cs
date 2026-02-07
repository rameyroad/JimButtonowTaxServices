using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TranscriptAnalyzer.Api.Tests.Common;
using TranscriptAnalyzer.Api.Tests.Fixtures;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Infrastructure.Persistence;

namespace TranscriptAnalyzer.Api.Tests.Clients;

[Collection("Api")]
public class ArchiveClientTests
{
    private readonly ApiTestFixture _fixture;

    public ArchiveClientTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region T095: Contract test for DELETE /clients/{id} (archive)

    [Fact]
    public async Task ArchiveClient_AsAdmin_ReturnsNoContent()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, "Admin");

        // Create a client first
        var createRequest = new
        {
            clientType = "Individual",
            firstName = "John",
            lastName = "Doe",
            taxIdentifier = "123-45-6789",
            email = "john.doe@example.com",
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

        // Act
        var response = await client.DeleteAsync($"/api/v1/clients/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ArchiveClient_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, "Admin");
        var invalidId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/v1/clients/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ArchiveClient_FromOtherTenant_ReturnsNotFound()
    {
        // Arrange
        await CleanupClientsAsync();
        using var clientA = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, "Admin");
        using var clientB = _fixture.CreateClientWithTenant(_fixture.TenantBId, _fixture.UserBId, "Admin");

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

        // Act - Try to archive from tenant B
        var response = await clientB.DeleteAsync($"/api/v1/clients/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region T096: Contract test for POST /clients/{id}/restore

    [Fact]
    public async Task RestoreClient_AsAdmin_ReturnsOk()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, "Admin");

        // Create and archive a client
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

        // Archive the client
        var archiveResponse = await client.DeleteAsync($"/api/v1/clients/{created!.Id}");
        archiveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act - Restore the client
        var response = await client.PostAsync($"/api/v1/clients/{created.Id}/restore", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ClientDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.IsArchived.Should().BeFalse();
    }

    [Fact]
    public async Task RestoreClient_NotArchived_ReturnsBadRequest()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, "Admin");

        // Create a client (not archived)
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

        // Act - Try to restore a non-archived client
        var response = await client.PostAsync($"/api/v1/clients/{created!.Id}/restore", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RestoreClient_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, "Admin");
        var invalidId = Guid.NewGuid();

        // Act
        var response = await client.PostAsync($"/api/v1/clients/{invalidId}/restore", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region T097: Contract test for archive role restriction (Admin only)

    [Theory]
    [InlineData("Admin", HttpStatusCode.NoContent)]
    [InlineData("TaxProfessional", HttpStatusCode.Forbidden)]
    [InlineData("ReadOnly", HttpStatusCode.Forbidden)]
    public async Task ArchiveClient_RoleRestriction(string role, HttpStatusCode expectedStatus)
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
            lastName = $"User-{Guid.NewGuid():N}",
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

        // Act - Try to archive with specified role
        var response = await roleClient.DeleteAsync($"/api/v1/clients/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(expectedStatus);
    }

    [Theory]
    [InlineData("Admin", HttpStatusCode.OK)]
    [InlineData("TaxProfessional", HttpStatusCode.Forbidden)]
    [InlineData("ReadOnly", HttpStatusCode.Forbidden)]
    public async Task RestoreClient_RoleRestriction(string role, HttpStatusCode expectedStatus)
    {
        // Arrange
        await CleanupClientsAsync();
        using var adminClient = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, "Admin");
        using var roleClient = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, role);

        // Create and archive a client as admin
        var createRequest = new
        {
            clientType = "Individual",
            firstName = "Test",
            lastName = $"User-{Guid.NewGuid():N}",
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

        // Archive the client as admin
        var archiveResponse = await adminClient.DeleteAsync($"/api/v1/clients/{created!.Id}");
        archiveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act - Try to restore with specified role
        var response = await roleClient.PostAsync($"/api/v1/clients/{created.Id}/restore", null);

        // Assert
        response.StatusCode.Should().Be(expectedStatus);
    }

    #endregion

    #region T098 & T099: Contract test for includeArchived and archived not in default list

    [Fact]
    public async Task ListClients_ArchivedNotInDefaultList()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, "Admin");

        // Create two clients
        var request1 = new
        {
            clientType = "Individual",
            firstName = "Active",
            lastName = "Client",
            taxIdentifier = "111-22-3333",
            email = "active@example.com",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        var request2 = new
        {
            clientType = "Individual",
            firstName = "Archived",
            lastName = "Client",
            taxIdentifier = "444-55-6666",
            email = "archived@example.com",
            address = new
            {
                street1 = "456 Oak Ave",
                city = "Boston",
                state = "MA",
                postalCode = "02101",
                country = "US"
            }
        };

        var create1 = await client.PostAsJsonAsync("/api/v1/clients", request1);
        create1.StatusCode.Should().Be(HttpStatusCode.Created);

        var create2 = await client.PostAsJsonAsync("/api/v1/clients", request2);
        create2.StatusCode.Should().Be(HttpStatusCode.Created);
        var archived = await create2.Content.ReadFromJsonAsync<ClientDto>();

        // Archive the second client
        var archiveResponse = await client.DeleteAsync($"/api/v1/clients/{archived!.Id}");
        archiveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act - Get list without includeArchived
        var listResponse = await client.GetAsync("/api/v1/clients");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await listResponse.Content.ReadFromJsonAsync<PaginatedListResponse<ClientListItemDto>>(TestJsonOptions.Default);

        // Assert - Only active client should be in list
        list!.Items.Should().HaveCount(1);
        list.Items.Should().ContainSingle(c => c.Email == "active@example.com");
        list.Items.Should().NotContain(c => c.Email == "archived@example.com");
    }

    [Fact]
    public async Task ListClients_WithIncludeArchived_ShowsArchivedClients()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, "Admin");

        // Create two clients
        var request1 = new
        {
            clientType = "Individual",
            firstName = "Active",
            lastName = "Client",
            taxIdentifier = "111-22-3333",
            email = "active@example.com",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        var request2 = new
        {
            clientType = "Individual",
            firstName = "Archived",
            lastName = "Client",
            taxIdentifier = "444-55-6666",
            email = "archived@example.com",
            address = new
            {
                street1 = "456 Oak Ave",
                city = "Boston",
                state = "MA",
                postalCode = "02101",
                country = "US"
            }
        };

        var create1 = await client.PostAsJsonAsync("/api/v1/clients", request1);
        create1.StatusCode.Should().Be(HttpStatusCode.Created);

        var create2 = await client.PostAsJsonAsync("/api/v1/clients", request2);
        create2.StatusCode.Should().Be(HttpStatusCode.Created);
        var archived = await create2.Content.ReadFromJsonAsync<ClientDto>();

        // Archive the second client
        var archiveResponse = await client.DeleteAsync($"/api/v1/clients/{archived!.Id}");
        archiveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act - Get list with includeArchived=true
        var listResponse = await client.GetAsync("/api/v1/clients?includeArchived=true");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await listResponse.Content.ReadFromJsonAsync<PaginatedListResponse<ClientListItemDto>>(TestJsonOptions.Default);

        // Assert - Both clients should be in list
        list!.Items.Should().HaveCount(2);
        list.Items.Should().Contain(c => c.Email == "active@example.com");
        list.Items.Should().Contain(c => c.Email == "archived@example.com");

        // Archived client should have IsArchived = true
        var archivedClient = list.Items.Single(c => c.Email == "archived@example.com");
        archivedClient.IsArchived.Should().BeTrue();
    }

    [Fact]
    public async Task GetClient_ArchivedClient_ReturnsNotFoundByDefault()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, "Admin");

        // Create and archive a client
        var createRequest = new
        {
            clientType = "Individual",
            firstName = "Archived",
            lastName = "Client",
            taxIdentifier = "123-45-6789",
            email = "archived@example.com",
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

        // Archive the client
        var archiveResponse = await client.DeleteAsync($"/api/v1/clients/{created!.Id}");
        archiveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act - Try to get the archived client
        var response = await client.GetAsync($"/api/v1/clients/{created.Id}");

        // Assert - Archived client should return 404 by default
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
