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
public class CreateClientTests
{
    private readonly ApiTestFixture _fixture;

    public CreateClientTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region T034: Contract test for POST /clients (individual)

    [Fact]
    public async Task CreateIndividualClient_WithValidData_ReturnsCreated()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
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
                street2 = "Apt 4B",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ClientDto>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.ClientType.Should().Be("Individual");
        result.DisplayName.Should().Be("Doe, John");
        result.Email.Should().Be("john.doe@example.com");
        result.TaxIdentifierLast4.Should().Be("6789");
        result.TaxIdentifierMasked.Should().Be("***-**-6789");

        // Verify Location header
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/v1/clients/{result.Id}");
    }

    [Fact]
    public async Task CreateIndividualClient_ReturnsClientInList()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Individual",
            firstName = "Jane",
            lastName = "Smith",
            taxIdentifier = "456-78-9012",  // Valid SSN format
            email = "jane.smith@example.com",
            address = new
            {
                street1 = "456 Oak Ave",
                city = "Los Angeles",
                state = "CA",
                postalCode = "90001",
                country = "US"
            }
        };

        // Act
        var createResponse = await client.PostAsJsonAsync("/api/v1/clients", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify in list
        var listResponse = await client.GetAsync("/api/v1/clients");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await listResponse.Content.ReadFromJsonAsync<PaginatedListResponse<ClientListItemDto>>(TestJsonOptions.Default);
        list!.Items.Should().ContainSingle(x => x.Email == "jane.smith@example.com");
    }

    #endregion

    #region T035: Contract test for SSN validation

    [Theory]
    [InlineData("123-45-6789", true)]  // Valid format
    [InlineData("123456789", true)]     // Valid without dashes
    [InlineData("12-345-6789", false)]  // Invalid format
    [InlineData("123-456-789", false)]  // Invalid format
    [InlineData("abc-de-fghi", false)]  // Non-numeric
    [InlineData("000-00-0000", false)]  // Invalid SSN (all zeros)
    [InlineData("123-45-678", false)]   // Too short
    [InlineData("123-45-67890", false)] // Too long
    public async Task CreateIndividualClient_SsnValidation(string ssn, bool shouldSucceed)
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Individual",
            firstName = "Test",
            lastName = "User",
            taxIdentifier = ssn,
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

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        if (shouldSucceed)
        {
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
        else
        {
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    #endregion

    #region T036: Contract test for required fields validation (individual)

    [Fact]
    public async Task CreateIndividualClient_MissingFirstName_ReturnsBadRequest()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Individual",
            lastName = "Doe",
            taxIdentifier = "123-45-6789",
            email = "test@example.com",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateIndividualClient_MissingLastName_ReturnsBadRequest()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Individual",
            firstName = "John",
            taxIdentifier = "123-45-6789",
            email = "test@example.com",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateIndividualClient_MissingEmail_ReturnsBadRequest()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Individual",
            firstName = "John",
            lastName = "Doe",
            taxIdentifier = "123-45-6789",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateIndividualClient_MissingAddress_ReturnsBadRequest()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Individual",
            firstName = "John",
            lastName = "Doe",
            taxIdentifier = "123-45-6789",
            email = "test@example.com"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateIndividualClient_InvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Individual",
            firstName = "John",
            lastName = "Doe",
            taxIdentifier = "123-45-6789",
            email = "not-an-email",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region T037: Contract test for duplicate SSN detection

    [Fact]
    public async Task CreateIndividualClient_DuplicateSsn_ReturnsConflict()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request1 = new
        {
            clientType = "Individual",
            firstName = "John",
            lastName = "Doe",
            taxIdentifier = "111-22-3333",
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

        var request2 = new
        {
            clientType = "Individual",
            firstName = "Jane",
            lastName = "Smith",
            taxIdentifier = "111-22-3333", // Same SSN
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

        // Act
        var response1 = await client.PostAsJsonAsync("/api/v1/clients", request1);
        response1.StatusCode.Should().Be(HttpStatusCode.Created);

        var response2 = await client.PostAsJsonAsync("/api/v1/clients", request2);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateIndividualClient_SameSsnDifferentTenant_Succeeds()
    {
        // Arrange
        await CleanupClientsAsync();
        using var clientA = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);
        using var clientB = _fixture.CreateClientWithTenant(_fixture.TenantBId, _fixture.UserBId);

        var request = new
        {
            clientType = "Individual",
            firstName = "John",
            lastName = "Doe",
            taxIdentifier = "444-55-6666",
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

        // Act - Same SSN in different tenants should both succeed
        var responseA = await clientA.PostAsJsonAsync("/api/v1/clients", request);
        var responseB = await clientB.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        responseA.StatusCode.Should().Be(HttpStatusCode.Created);
        responseB.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    #endregion

    #region T038: Contract test for role restriction

    [Theory]
    [InlineData("Admin", HttpStatusCode.Created)]
    [InlineData("TaxProfessional", HttpStatusCode.Created)]
    [InlineData("ReadOnly", HttpStatusCode.Forbidden)]
    public async Task CreateIndividualClient_RoleRestriction(string role, HttpStatusCode expectedStatus)
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, role);

        var request = new
        {
            clientType = "Individual",
            firstName = "Test",
            lastName = $"User-{Guid.NewGuid():N}",
            taxIdentifier = $"{Random.Shared.Next(100, 999)}-{Random.Shared.Next(10, 99)}-{Random.Shared.Next(1000, 9999)}",
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

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(expectedStatus);
    }

    #endregion

    #region T039: Contract test for POST /clients (business)

    [Fact]
    public async Task CreateBusinessClient_WithValidData_ReturnsCreated()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Business",
            businessName = "Acme Corporation",
            entityType = "CCorp",
            taxIdentifier = "12-3456789",
            email = "contact@acme.com",
            phone = "555-987-6543",
            responsibleParty = "John Smith",
            address = new
            {
                street1 = "100 Corporate Blvd",
                street2 = "Suite 500",
                city = "Chicago",
                state = "IL",
                postalCode = "60601",
                country = "US"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ClientDto>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.ClientType.Should().Be("Business");
        result.DisplayName.Should().Be("Acme Corporation");
        result.BusinessName.Should().Be("Acme Corporation");
        result.EntityType.Should().Be("CCorp");
        result.Email.Should().Be("contact@acme.com");
        result.TaxIdentifierLast4.Should().Be("6789");
        result.TaxIdentifierMasked.Should().Be("**-***6789");

        // Verify Location header
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/v1/clients/{result.Id}");
    }

    [Fact]
    public async Task CreateBusinessClient_ReturnsClientInList()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Business",
            businessName = "Test LLC",
            entityType = "LLC",
            taxIdentifier = "98-7654321",
            email = "info@testllc.com",
            address = new
            {
                street1 = "200 Business Way",
                city = "Houston",
                state = "TX",
                postalCode = "77001",
                country = "US"
            }
        };

        // Act
        var createResponse = await client.PostAsJsonAsync("/api/v1/clients", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify in list
        var listResponse = await client.GetAsync("/api/v1/clients");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await listResponse.Content.ReadFromJsonAsync<PaginatedListResponse<ClientListItemDto>>(TestJsonOptions.Default);
        list!.Items.Should().ContainSingle(x => x.Email == "info@testllc.com");
    }

    #endregion

    #region T040: Contract test for EIN validation

    [Theory]
    [InlineData("12-3456789", true)]   // Valid format with dash
    [InlineData("123456789", true)]     // Valid without dash
    [InlineData("12-345678", false)]    // Too short
    [InlineData("12-34567890", false)]  // Too long
    [InlineData("123-456789", false)]   // Wrong dash position
    [InlineData("ab-cdefghi", false)]   // Non-numeric
    public async Task CreateBusinessClient_EinValidation(string ein, bool shouldSucceed)
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Business",
            businessName = $"Test Business {Guid.NewGuid():N}",
            entityType = "LLC",
            taxIdentifier = ein,
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

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        if (shouldSucceed)
        {
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
        else
        {
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    #endregion

    #region T041: Contract test for required fields validation (business)

    [Fact]
    public async Task CreateBusinessClient_MissingBusinessName_ReturnsBadRequest()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Business",
            entityType = "LLC",
            taxIdentifier = "12-3456789",
            email = "test@example.com",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBusinessClient_MissingEntityType_ReturnsBadRequest()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Business",
            businessName = "Test Corp",
            taxIdentifier = "12-3456789",
            email = "test@example.com",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBusinessClient_MissingEmail_ReturnsBadRequest()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Business",
            businessName = "Test Corp",
            entityType = "CCorp",
            taxIdentifier = "12-3456789",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBusinessClient_MissingAddress_ReturnsBadRequest()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Business",
            businessName = "Test Corp",
            entityType = "CCorp",
            taxIdentifier = "12-3456789",
            email = "test@example.com"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBusinessClient_InvalidEntityType_ReturnsBadRequest()
    {
        // Arrange
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request = new
        {
            clientType = "Business",
            businessName = "Test Corp",
            entityType = "InvalidType",
            taxIdentifier = "12-3456789",
            email = "test@example.com",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region T042: Contract test for duplicate EIN detection

    [Fact]
    public async Task CreateBusinessClient_DuplicateEin_ReturnsConflict()
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);

        var request1 = new
        {
            clientType = "Business",
            businessName = "First Corp",
            entityType = "CCorp",
            taxIdentifier = "55-5555555",
            email = "first@example.com",
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
            clientType = "Business",
            businessName = "Second Corp",
            entityType = "LLC",
            taxIdentifier = "55-5555555", // Same EIN
            email = "second@example.com",
            address = new
            {
                street1 = "456 Oak Ave",
                city = "Boston",
                state = "MA",
                postalCode = "02101",
                country = "US"
            }
        };

        // Act
        var response1 = await client.PostAsJsonAsync("/api/v1/clients", request1);
        response1.StatusCode.Should().Be(HttpStatusCode.Created);

        var response2 = await client.PostAsJsonAsync("/api/v1/clients", request2);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateBusinessClient_SameEinDifferentTenant_Succeeds()
    {
        // Arrange
        await CleanupClientsAsync();
        using var clientA = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId);
        using var clientB = _fixture.CreateClientWithTenant(_fixture.TenantBId, _fixture.UserBId);

        var request = new
        {
            clientType = "Business",
            businessName = "Shared Name Corp",
            entityType = "CCorp",
            taxIdentifier = "77-7777777",
            email = "shared@example.com",
            address = new
            {
                street1 = "123 Main St",
                city = "New York",
                state = "NY",
                postalCode = "10001",
                country = "US"
            }
        };

        // Act - Same EIN in different tenants should both succeed
        var responseA = await clientA.PostAsJsonAsync("/api/v1/clients", request);
        var responseB = await clientB.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        responseA.StatusCode.Should().Be(HttpStatusCode.Created);
        responseB.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    #endregion

    #region T043: Contract test for role restriction (business)

    [Theory]
    [InlineData("Admin", HttpStatusCode.Created)]
    [InlineData("TaxProfessional", HttpStatusCode.Created)]
    [InlineData("ReadOnly", HttpStatusCode.Forbidden)]
    public async Task CreateBusinessClient_RoleRestriction(string role, HttpStatusCode expectedStatus)
    {
        // Arrange
        await CleanupClientsAsync();
        using var client = _fixture.CreateClientWithTenant(_fixture.TenantAId, _fixture.UserAId, role);

        var request = new
        {
            clientType = "Business",
            businessName = $"Test Business {Guid.NewGuid():N}",
            entityType = "LLC",
            taxIdentifier = $"{Random.Shared.Next(10, 99)}-{Random.Shared.Next(1000000, 9999999)}",
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

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/clients", request);

        // Assert
        response.StatusCode.Should().Be(expectedStatus);
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

// DTO for deserializing create response
public record ClientDto
{
    public Guid Id { get; init; }
    public string ClientType { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? BusinessName { get; init; }
    public string? EntityType { get; init; }
    public string TaxIdentifierLast4 { get; init; } = string.Empty;
    public string TaxIdentifierMasked { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public AddressDto? Address { get; init; }
    public bool IsArchived { get; init; }
    public int Version { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record AddressDto
{
    public string Street1 { get; init; } = string.Empty;
    public string? Street2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}
