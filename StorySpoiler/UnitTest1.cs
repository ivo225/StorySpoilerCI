using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using StorySpoiler.Models;
using System.Net;
using System.Text.Json;


namespace StorySpoiler
{
    [TestFixture]
    public class StorySpoiler
    {
        private RestClient client;
        private static string createdStoryId;
        
        private const string baseUrl = "https://d3s5nxhwblsjbi.cloudfront.net";

        [OneTimeSetUp]
        public void Setup()
        {
            // your credentials
            string token = GetJwtToken("ivoiv", "ivoiv123");

            var options = new RestClientOptions(baseUrl)
            {
                Authenticator = new JwtAuthenticator(token)
            };

            client = new RestClient(options);
        }

        private string GetJwtToken(string username, string password)
        {
            var loginClient = new RestClient(baseUrl);
            var request = new RestRequest("/api/User/Authentication", Method.Post);

            request.AddJsonBody(new { username, password });
            var response = loginClient.Execute(request);
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            return json.GetProperty("accessToken").GetString() ?? string.Empty;
        }


        [Test, Order(1)]
        public void NewStorySpoiler_ShouldReturnCreated()
        {
            var story = new
            {
                title = "New Story Spoiler",
                description = "Test Description",
                url = ""
            };

            var request = new RestRequest("/api/Story/Create", Method.Post);
            request.AddJsonBody(story);

            var response = client.Execute(request);

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);

            createdStoryId = json.GetProperty("storyId").GetString() ?? string.Empty;

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(createdStoryId, Is.Not.Null.And.Not.Empty);
            Assert.That(response.Content, Does.Contain("Successfully created!"));
        }

        [Test, Order(2)]

        public void EditStorySpoiler_ShouldReturnOk()
        {
            if (string.IsNullOrEmpty(createdStoryId))
            {
                return;
            }

            var updatedStory = new StoryDTO()
            {
                Title = "Updated Story Spoiler",
                Description = "Updated Story Spoiler Description",
                Url = ""
            };

            var request = new RestRequest($"/api/Story/Edit/{createdStoryId}", Method.Put);
            request.AddJsonBody(updatedStory);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Does.Contain("Successfully edited"));
        }

        [Test, Order(3)]

        public void GetAllStorySpoilers_ShouldReturnList()
        {
            var request = new RestRequest("/api/Story/All", Method.Get);

            var response = client.Execute(request);

            var story = JsonSerializer.Deserialize<List<object>>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(story, Is.Not.Empty);

        }

        [Test, Order(4)]

        public void DeleteStorySpoiler_ShouldReturnOk()
        {
            var request = new RestRequest($"/api/Story/Delete/{createdStoryId}", Method.Delete);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Does.Contain("Deleted successfully!"));
        }

        [Test, Order(5)]

        public void StorySpoilerWithoutRequiredFields_ShouldReturnBadRequest()
        {
            var story = new
            {
                title = "",
                description = "",
                url = "http://fHNj8Vc!,4'vp5)w5Vk'|Hih5(0B.bmp"
            };

            var request = new RestRequest("/api/Story/Create", Method.Post);
            request.AddJsonBody(story);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        }

        [Test, Order(6)]
        public void EditNonExistingStorySpoiler_ShouldReturnNotFound()
        {
            var fakeID = "123456789";

            var request = new RestRequest($"/api/Story/Edit/{fakeID}", Method.Put);

            var updatedStory = new
            {
                title = "Updated Story Spoiler",
                description = "Updated Story Spoiler Description",
                url = ""
            };
            request.AddJsonBody(updatedStory);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(response.Content, Does.Contain("No spoilers..."));
        }

        [Test, Order(7)]

        public void DeleteNonExistingStorySpoiler_ShouldReturnBadRequest()
        {
            var fakeID = "123456789";

            var request = new RestRequest($"/api/Story/Delete/{fakeID}", Method.Delete);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Does.Contain("Unable to delete this story spoiler!"));

        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            client?.Dispose();
        }
    }
}