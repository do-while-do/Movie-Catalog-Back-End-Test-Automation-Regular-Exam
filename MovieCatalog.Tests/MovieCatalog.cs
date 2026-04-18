using System.Net;
using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using MovieCatalog.Tests.DTOs;

namespace MovieCatalog.Tests;

public class MovieCatalog
{
    private RestClient client;
    private static string? MovieId;

    // OhOCRopPQL_First_name
    // NEByVeaupC_Last_name
    // CMPrMwpmGO_username
    // xNVDNAmqHW@xNVDNAmqHW.com
    // jnJkPySZuJ

    [OneTimeSetUp]
    public void Setup()
    {
        string jwtToken = GetJwtToken("xNVDNAmqHW@xNVDNAmqHW.com", "jnJkPySZuJ");
        RestClientOptions options = new RestClientOptions("http://144.91.123.158:5000")
        {
            Authenticator = new JwtAuthenticator(jwtToken)
        };
        client = new RestClient(options);
    }

    private string GetJwtToken(string email, string password)
    {
        RestClient client = new RestClient("http://144.91.123.158:5000");
        RestRequest request = new RestRequest("/api/User/Authentication", Method.Post);
        request.AddJsonBody(new { email, password });
        RestResponse response = client.Execute(request);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (string.IsNullOrWhiteSpace(response.Content))
            {
                throw new InvalidOperationException("Response content is null or empty.");
            }
            var content = JsonSerializer.Deserialize<JsonElement>(response.Content);
            var token = content.GetProperty("accessToken").GetString();

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("Token not found in the response.");
            }
            return token;
        }
        else
        {
            throw new InvalidOperationException($"Failed to authenticate. Status code: {response.StatusCode}, Response: {response.Content}");
        }
    }

    [Order(1)]
    [Test]
    public void Create_New_Movie_WithRequiredFields_Should_Return_Success()
    {
        // {
        //   "title": "string1",
        //   "description": "string1",
        //   "posterUrl": "",
        //   "trailerLink": "",
        //   "isWatched": true
        // }
        MovieDto movie = new MovieDto
        {
            Title = "SoupMovie4",
            Description = "SoupMovie4SoupMovie4SoupMovie4SoupMovie4 SoupMovie4SoupMovie4 SoupMovie4",
            IsWatched = true
        };

        RestRequest request = new RestRequest("api/Movie/Create", Method.Post);
        request.AddJsonBody(movie);
        RestResponse response = client.Execute(request);

        //Assert that the response status code is OK (200).
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        if (string.IsNullOrWhiteSpace(response.Content))
        {
            throw new InvalidOperationException("Response content is null or empty.");
        }
        ApiResponseDto? readyResponse = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);

        // Assert that a Movie object is returned in the response
        Assert.That(readyResponse, Is.Not.Null, "Response should not be null");
        Assert.That(readyResponse.Movie, Is.Not.Null, "Movie object should be returned in the response");

        // Assert that the Id of the returned movie is not null or empty
        Assert.That(readyResponse.Movie.Id, Is.Not.Null.And.Not.Empty, "Movie Id should not be null or empty");

        // Assert that the response message indicates the movie was created successfully
        Assert.That(readyResponse.Msg, Is.EqualTo("Movie created successfully!"), "Response message should indicate successful movie creation");

        // Store the returned movie Id as a static member for use in subsequent tests
        MovieId = readyResponse.Movie.Id;
    }
    [Order(2)]
    [Test]
    public void Edit_Movie_Title_Description_IsWatched_Should_Update_Movie()
    {
        // Create an updated movie with new values
        MovieDto updatedMovie = new MovieDto
        {
            Title = "SoupMovie3Updated",
            Description = "SoupMovie3 UpdatedSoup Movie4UpdatedSoup Movie4Updated",
            IsWatched = true
        };

        // Send a PUT request to edit the movie using MovieId as a query parameter
        RestRequest request = new RestRequest($"api/Movie/Edit?movieId={MovieId}", Method.Put);
        request.AddJsonBody(updatedMovie);
        RestResponse response = client.Execute(request);

        // Assert that the response status code is OK (200)
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Response status code should be OK (200)");

        if (string.IsNullOrWhiteSpace(response.Content))
        {
            throw new InvalidOperationException("Response content is null or empty.");
        }
        ApiResponseDto? readyResponse = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);

        // Assert that a response object is returned
        Assert.That(readyResponse, Is.Not.Null, "Response should not be null");

        // Assert that the response message indicates the movie was edited successfully
        Assert.That(readyResponse.Msg, Is.EqualTo("Movie edited successfully!"), "Response message should indicate successful movie edit");
    }

    // [Test]
    // public void Test1()
    // {
    //     Assert.Pass();
    // }

    [OneTimeTearDown]
    public void TearDown()
    {
        this.client?.Dispose();
    }
}
