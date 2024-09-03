using System.Net.Mime;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SampleWebAPI.Tests;

public class BasicTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly WebApplicationFactory<Program> _factory = factory;

	[Fact]
	public async Task Should_return_success_and_content_type()
	{
		// Arrange
		var client = _factory.CreateClient();

		// Act
		var response = await client.GetAsync("/");

		// Assert
		response.EnsureSuccessStatusCode();
		Assert.Equal($"{MediaTypeNames.Application.Json}; charset=utf-8", response.Content.Headers.ContentType?.ToString());
	}

	[Fact]
	public async Task Should_return_current_status_and_timestamp()
	{
		// Arrange
		var client = _factory.CreateClient();

		// Act
		var response = await client.GetAsync("/");

		// Assert
		var responseContent = await response.Content.ReadAsStringAsync();
		Assert.Contains("energySavingMode", responseContent);
		Assert.Contains("timestamp", responseContent);
	}
}
