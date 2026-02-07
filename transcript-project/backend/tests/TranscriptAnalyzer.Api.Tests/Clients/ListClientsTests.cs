using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TranscriptAnalyzer.Api.Tests.Common;
using TranscriptAnalyzer.Api.Tests.Fixtures;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Infrastructure.Persistence;

namespace TranscriptAnalyzer.Api.Tests.Clients;

[Collection("Api")]
public class ListClientsTests
{
    private readonly ApiTestFixture _fixture;

    public ListClientsTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// T019: Contract test for GET /clients
    /// </summary>
    [Fact]
    public async Task ListClients_ReturnsOk_WithPaginatedResponse()
    {
        // Arrange
        await SeedClientsAsync(3);
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        // Act
        var response = await client.GetAsync("/api/v1/clients");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedListResponse<ClientListItemDto>>(TestJsonOptions.Default);
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().BeGreaterThan(0);
    }

    /// <summary>
    /// T020: Contract test for pagination and sorting
    /// </summary>
    [Fact]
    public async Task ListClients_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        await SeedClientsAsync(25);
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        // Act
        var response = await client.GetAsync("/api/v1/clients?page=2&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedListResponse<ClientListItemDto>>(TestJsonOptions.Default);
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(10);
        result.Page.Should().Be(2);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task ListClients_WithSorting_ReturnsSortedResults()
    {
        // Arrange
        await SeedClientsAsync(5);
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        // Act
        var response = await client.GetAsync("/api/v1/clients?sortBy=name&sortOrder=desc");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedListResponse<ClientListItemDto>>(TestJsonOptions.Default);
        result.Should().NotBeNull();
        result!.Items.Should().BeInDescendingOrder(x => x.DisplayName);
    }

    /// <summary>
    /// T021: Contract test for search functionality
    /// </summary>
    [Fact]
    public async Task ListClients_WithSearch_FiltersResults()
    {
        // Arrange
        await SeedSpecificClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        // Act - Search by name
        var response = await client.GetAsync("/api/v1/clients?search=John");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedListResponse<ClientListItemDto>>(TestJsonOptions.Default);
        result.Should().NotBeNull();
        result!.Items.Should().OnlyContain(x => x.DisplayName.Contains("John", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ListClients_SearchByLast4_FiltersResults()
    {
        // Arrange
        await SeedSpecificClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        // Act - Search by last 4 of SSN
        var response = await client.GetAsync("/api/v1/clients?search=1234");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedListResponse<ClientListItemDto>>(TestJsonOptions.Default);
        result.Should().NotBeNull();
        result!.Items.Should().OnlyContain(x => x.TaxIdentifierLast4 == "1234");
    }

    /// <summary>
    /// T022: Contract test for role-based list access (all roles can view)
    /// </summary>
    [Theory]
    [InlineData("Admin")]
    [InlineData("TaxProfessional")]
    [InlineData("ReadOnly")]
    public async Task ListClients_AllRoles_CanView(string role)
    {
        // Arrange
        await SeedClientsAsync(1);
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, role);

        // Act
        var response = await client.GetAsync("/api/v1/clients");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(Skip = "Dev auth allows all requests - this test requires production Auth0 configuration")]
    public async Task ListClients_WithoutAuth_Returns401()
    {
        // Arrange - Use client without auth headers
        using var client = _fixture.CreateUnauthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/v1/clients");

        // Assert
        // Note: In production with Auth0, this returns 401
        // In dev mode, dev auth policy allows all requests
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// T023: Contract test for tenant isolation (RLS)
    /// </summary>
    [Fact]
    public async Task ListClients_OnlyReturnsClientsFromSameTenant()
    {
        // Arrange - Seed clients for both tenants
        await SeedClientsForBothTenantsAsync();
        using var clientA = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);
        using var clientB = _fixture.CreateClientWithTenant(_fixture.TenantBId, _fixture.UserBId);

        // Act
        var responseA = await clientA.GetAsync("/api/v1/clients");
        var responseB = await clientB.GetAsync("/api/v1/clients");

        // Assert
        responseA.StatusCode.Should().Be(HttpStatusCode.OK);
        responseB.StatusCode.Should().Be(HttpStatusCode.OK);

        var resultA = await responseA.Content.ReadFromJsonAsync<PaginatedListResponse<ClientListItemDto>>(TestJsonOptions.Default);
        var resultB = await responseB.Content.ReadFromJsonAsync<PaginatedListResponse<ClientListItemDto>>(TestJsonOptions.Default);

        // Each tenant should only see their own clients
        resultA!.Items.Should().OnlyContain(x => x.DisplayName.StartsWith("TenantA"));
        resultB!.Items.Should().OnlyContain(x => x.DisplayName.StartsWith("TenantB"));
    }

    [Fact]
    public async Task ListClients_MasksTaxIdentifiers()
    {
        // Arrange
        await SeedClientsAsync(1);
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        // Act
        var response = await client.GetAsync("/api/v1/clients");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedListResponse<ClientListItemDto>>(TestJsonOptions.Default);
        result.Should().NotBeNull();
        result!.Items.Should().OnlyContain(x =>
            x.TaxIdentifierLast4.Length == 4 &&
            x.TaxIdentifierMasked.Contains("***"));
    }

    private async Task SeedClientsAsync(int count)
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clear all existing clients to ensure test isolation
        await context.Database.ExecuteSqlRawAsync("DELETE FROM clients");

        for (int i = 0; i < count; i++)
        {
            var last4 = (1000 + i).ToString();
            await context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO clients (id, organization_id, client_type, first_name, last_name, tax_identifier_encrypted, tax_identifier_last4,
                    email, address_street1, address_city, address_state, address_postal_code, address_country,
                    created_by_user_id, version, created_at, updated_at)
                VALUES ({0}, {1}, 'Individual', {2}, {3}, 'encrypted', {4}, {5}, '123 Main', 'City', 'ST', '12345', 'US', {6}, 1, {7}, {7})",
                Guid.NewGuid(), _fixture.TenantAId, $"FirstName{i}", $"LastName{i}", last4,
                $"client{i}@test.com", _fixture.UserAId, DateTime.UtcNow);
        }
    }

    private async Task SeedSpecificClientsAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clear all existing clients to ensure test isolation
        await context.Database.ExecuteSqlRawAsync("DELETE FROM clients");

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO clients (id, organization_id, client_type, first_name, last_name, tax_identifier_encrypted, tax_identifier_last4,
                email, address_street1, address_city, address_state, address_postal_code, address_country,
                created_by_user_id, version, created_at, updated_at)
            VALUES
                ({0}, {1}, 'Individual', 'John', 'Doe', 'encrypted', '1234', 'john@test.com', '123 Main', 'City', 'ST', '12345', 'US', {2}, 1, {3}, {3}),
                ({4}, {1}, 'Individual', 'Jane', 'Smith', 'encrypted', '5678', 'jane@test.com', '456 Oak', 'Town', 'ST', '12346', 'US', {2}, 1, {3}, {3}),
                ({5}, {1}, 'Business', NULL, NULL, 'encrypted', '9012', 'acme@test.com', '789 Corp', 'Metro', 'ST', '12347', 'US', {2}, 1, {3}, {3})",
            Guid.NewGuid(), _fixture.TenantAId, _fixture.UserAId, DateTime.UtcNow,
            Guid.NewGuid(), Guid.NewGuid());

        // Update business client with business name
        await context.Database.ExecuteSqlRawAsync(@"
            UPDATE clients SET business_name = 'Acme Corp', entity_type = 'LLC'
            WHERE tax_identifier_last4 = '9012' AND organization_id = {0}", _fixture.TenantAId);
    }

    private async Task SeedClientsForBothTenantsAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean all clients
        await context.Database.ExecuteSqlRawAsync("DELETE FROM clients");

        // Tenant A clients - DisplayName will be "TenantA_Last, TenantA_First"
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO clients (id, organization_id, client_type, first_name, last_name, tax_identifier_encrypted, tax_identifier_last4,
                email, address_street1, address_city, address_state, address_postal_code, address_country,
                created_by_user_id, version, created_at, updated_at)
            VALUES ({0}, {1}, 'Individual', 'TenantA_First', 'TenantA_Last', 'encrypted', '1111', 'a1@test.com', '123 A', 'CityA', 'ST', '11111', 'US', {2}, 1, {3}, {3})",
            Guid.NewGuid(), _fixture.TenantAId, _fixture.UserAId, DateTime.UtcNow);

        // Tenant B clients - DisplayName will be "TenantB_Last, TenantB_First"
        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO clients (id, organization_id, client_type, first_name, last_name, tax_identifier_encrypted, tax_identifier_last4,
                email, address_street1, address_city, address_state, address_postal_code, address_country,
                created_by_user_id, version, created_at, updated_at)
            VALUES ({0}, {1}, 'Individual', 'TenantB_First', 'TenantB_Last', 'encrypted', '2222', 'b1@test.com', '123 B', 'CityB', 'ST', '22222', 'US', {2}, 1, {3}, {3})",
            Guid.NewGuid(), _fixture.TenantBId, _fixture.UserBId, DateTime.UtcNow);
    }
}
