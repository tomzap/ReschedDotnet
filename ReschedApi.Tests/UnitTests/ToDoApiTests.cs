using System.Net.Http.Json;

namespace ReschedApi.Tests;

public class ToDoApiTests
{
    private CustomWebApplicationFactory<Program>
        _factory;

    [SetUp]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory<Program>();
    }

    [TearDown]
    public void TearDown()
    {
        _factory.Dispose();
    }

    [Test]
    async public Task  GetTodosTest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/todoitems");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.That(response.Content.Headers.ContentType?.ToString(), Is.EqualTo("application/json; charset=utf-8"));        
        var todo = await response.Content.ReadFromJsonAsync<List<Todo>>();
        Assert.That(todo[0].Id, Is.EqualTo(1));
        Assert.That(todo[0].Name, Is.EqualTo("Test task"));
        Assert.That(todo[0].IsComplete, Is.EqualTo(false));
    }
}