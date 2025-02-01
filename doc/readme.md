# WebApi RestClient V1.0.0

## What is WebApi RestClient
WebApi RestClient is a library designed to simplify and make fluid
API calls to any type of web service.

The library is designed to integrate with Microsoft's **HttpClientFactory**,
taking advantage of its features for managing HTTP clients,
such as connection pooling, lifecycle management and centralized configuration of
the `HttpClients`.

The main feature of this library is the **Fluent** architecture,
which allows you to build and send API requests in an intuitive and readable way,
with a compact and easily configurable syntax.

## How to use it?
In the `Program.cs` file, you can register the various **HttpClients** using the
`AddHttpClient` method. This allows you to set common properties
of API calls, keeping the logic of creating the `HttpClient`
compliant with the **ASP.NET Core** standards.

Here is an example of configuring a client:
```csharp
services.AddHttpClient("api", client =>
{
  client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
  client.DefaultRequestHeaders.Add("Accept", "application/json");
});
```
If you are in blazor WebAssembly you can register your HttpClient like this :
```csharp
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5154/") });
```
You must insert this after your HttpClient :
```csharp
builder.Services.AddRestClient();

//If you have WebAssembly project
builder.Services.AddRestClient(builder); //builder is WebAssemblyHostBuilder
```

At this point you can use the **HttpClientBuilderFactory** class
that allows you to create a client with the specified name that must
be the same as the one inserted inside the AddHttpClient method.

```csharp
var client = _httpClientBuilderFactory.CreateClient("api");
```
The **CreateClient** method will return an instance of **HttpClientBuilder**
which allows us to build our API call in a
Fluent way.

## Usage Example

With the HttpClientBuilder instance, you can easily configure the API call,
specifying the HTTP method, endpoint, query parameters, body, and so on,
using a Fluent syntax.

Here is an example of an API call with a POST method, query parameters, and a JSON body:

```csharp
var response = await client
    .WithMethod(HttpMethod.Post)
    .WithEndpoint("login")
    .WithQueryParameters(new Dictionary<string, string>
    {
    { "useCookies", "false" },
    { "useSessionCookies", "false" }
    })
    .WithBody(new { email = "test@mail.com", password = "Passw0rd" })
    .BuildRequest()
    .SendAsync<LoginInfoResponse>();
```

In this example, the POST call is sent to the `login` endpoint with the query
parameters and the body containing the login credentials.
The response is then deserialized into the `LoginInfoResponse` object.

## Response Handling

When you send a request, the library returns an object of type
`RestResponseWithData<T>`, which extends the base class `RestResponse`.
This object includes several properties useful for managing the status of the response,
such as the HTTP status code, a description of the status, any errors, and the
data returned by the call.

```csharp
public class RestResponse
{
/// <summary>
/// Indicates whether the status of the response is successful or not.
/// </summary>
public bool IsSuccessful { get; internal set; }

/// <summary>
/// Status code of the response.
/// </summary>
public HttpStatusCode StatusCode { get; internal set; }

/// <summary>
/// Description of the status of the response.
/// </summary>
public string? StatusDescription { get; internal set; }

/// <summary>
/// Any error message.
/// </summary>
public string? ErrorBody { get; internal set; }
}
```
```csharp
public class RestResponseWithData<T> : RestResponse
{
/// <summary>
/// The data returned by the API response.
/// </summary>
public T? Data { get; internal set; }
}
```

This framework allows you to easily handle errors and data returned
from API calls, centralizing the response handling logic and
improving code readability.

## Key Features
- **Fluent API**: Build and send API calls in an intuitive and readable way.
- **Support for major HTTP methods**: GET, POST, PUT, DELETE, etc.
- **Centralized HttpClient Management**: Leverage HttpClientFactory capabilities.
- **Automatic Response Deserialization**: Automatically deserialize the data returned by the API call based on the specified type.
- **Error Handling**: Easily handle API call errors and exceptions.
