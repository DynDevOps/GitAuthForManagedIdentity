using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace GitAuthForManagedIdentity;

public record TokenResponse(string access_token, string client_id, string expires_on, string resource, string token_type)
{
    internal static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        TypeInfoResolver = SourceGenerationContext.Default,
    };

    public const string TokenUrl = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=499b84ac-1321-427f-aa17-267ca6975798";
    public static async Task<TokenResponse?> GetTokenAsync()
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("Metadata", "true");
        var token = await client.GetFromJsonAsync<TokenResponse>(TokenUrl, JsonSerializerOptions);
        return token;
    }
}

[JsonSerializable(typeof(TokenResponse))]
internal partial class SourceGenerationContext : JsonSerializerContext
{

}
