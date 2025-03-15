namespace Integration.Tests;

using System.Net.Http.Json;
using System.Net;
using Integration.Tests.Extensions;
using FluentAssertions;
using MsClean.Application;

public class PermissionEndpointsTests : IClassFixture<ApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _pathUrl = "/api/permission";

    public PermissionEndpointsTests(ApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task RequestPermission_Post_ShouldReturnCreated()
    {
        // Arrange
        var request = new RequestPermissionCommand("Gerson", "Navarro", 1);

        // Act
        var response = await _client.PostAsJsonAsync(_pathUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdId = await response.Content.ReadFromJsonAsync<int>();
        createdId.Should().BeGreaterThan(0);
    }


    [Fact]
    public async Task RequestPermission_Post_ShouldReturnBadRequest_WhenDataIsInvalid()
    {
        // Arrange
        var request = new RequestPermissionCommand("", "", 0);

        // Act
        var response = await _client.PostAsJsonAsync(_pathUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPermission_ById_ShouldReturnOk_WhenPermissionExists()
    {
        // Arrange
        var id = 1;

        // Act
        var response = await _client.GetAsync($"{_pathUrl}/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPermission_ById_ShouldReturnNotFound_WhenPermissionDoesNotExist()
    {
        // Arrange
        var id = 999;

        // Act
        var response = await _client.GetAsync($"{_pathUrl}/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnOk_WithPermissionsList()
    {
        // Arrange , Act
        var response = await _client.GetAsync(_pathUrl);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var permissions = await response.Content.ReadFromJsonAsync<IEnumerable<PermissionViewModel>>();
        permissions.Should().NotBeNull();
        permissions.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task UpdatePermission_Put_ShouldReturnOk()
    {
        // Arrange
        var request = new ModifyPermissionCommand(1, "Eduardo", "Navarro", 1);

        // Act
        var response = await _client.PutAsJsonAsync(_pathUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdatePermission_Put_ShouldReturnBadRequest_WhenDataIsInvalid()
    {
        // Arrange
        var request = new ModifyPermissionCommand(1, "", "", 0);

        // Act
        var response = await _client.PutAsJsonAsync(_pathUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

}
