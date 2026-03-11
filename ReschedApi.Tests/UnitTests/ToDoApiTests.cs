using System.Net;
using System.Net.Http.Json;
using System.Runtime;
using Microsoft.AspNetCore.Diagnostics;

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
   
    [Test]
    async public Task  GetTodoByIdTest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/todoitems/1");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.That(response.Content.Headers.ContentType?.ToString(), Is.EqualTo("application/json; charset=utf-8"));        
        var todo = await response.Content.ReadFromJsonAsync<Todo>();
        Assert.That(todo?.Id, Is.EqualTo(1));
        Assert.That(todo.Name, Is.EqualTo("Test task"));
        Assert.That(todo.IsComplete, Is.EqualTo(false));
    }

    [Test]
    async public Task  PutTodoTest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var request = new Todo{Name = "New task", IsComplete = false};
        var response = await client.PostAsJsonAsync("/todoitems", request);
        response.EnsureSuccessStatusCode();
        // Take note of Id 
        var newTodo = await response.Content.ReadFromJsonAsync<Todo>();
        var id = newTodo.Id;
        // Update 
        newTodo.Name = "Updated";
        newTodo.IsComplete = true;
        response = await client.PutAsJsonAsync($"/todoitems/{id}", newTodo);
        response.EnsureSuccessStatusCode();
        response = await client.GetAsync($"/todoitems/{id}"); 
        
        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.That(response.Content.Headers.ContentType?.ToString(), Is.EqualTo("application/json; charset=utf-8"));        
        var todo = await response.Content.ReadFromJsonAsync<Todo>();
        Assert.That(todo?.Id, Is.EqualTo(id));
        Assert.That(todo.Name, Is.EqualTo("Updated"));
        Assert.That(todo.IsComplete, Is.EqualTo(true));
    }
   [Test]
    async public Task  DeleteTodoTest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var request = new Todo{Name = "New task", IsComplete = false};
        var response = await client.PostAsJsonAsync("/todoitems", request);
        response.EnsureSuccessStatusCode();
        // Take note of Id 
        var newTodo = await response.Content.ReadFromJsonAsync<Todo>();
        var id = newTodo?.Id;
        // Delete
        response = await client.DeleteAsync($"/todoitems/{id}");
        response.EnsureSuccessStatusCode();
        response = await client.GetAsync($"/todoitems/{id}"); 
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    async public Task  GetCompletedTodosTest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var request = new Todo{Name = "Completed task", IsComplete = true};
        var response = await client.PostAsJsonAsync("/todoitems", request);
        response.EnsureSuccessStatusCode();
        response = await client.GetAsync("/todoitems/complete");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.That(response.Content.Headers.ContentType?.ToString(), Is.EqualTo("application/json; charset=utf-8"));        
        var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
        Assert.That(todos.Count, Is.GreaterThan(0));
        Assert.That(todos.Find(t => t.Name == "Completed task")?.IsComplete, Is.EqualTo(true));
        Assert.That(todos.TrueForAll(t => t.IsComplete), Is.EqualTo(true));
    }
}