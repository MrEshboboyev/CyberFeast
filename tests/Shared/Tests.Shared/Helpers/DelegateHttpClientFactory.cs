namespace Tests.Shared.Helpers;

public class DelegateHttpClientFactory(Func<string, Lazy<HttpClient>> httpClientProvider) : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        return name is "k8s-cluster-service" 
            or "health-checks-webhooks" 
            or "health-checks" 
            ? new HttpClient() 
            : httpClientProvider(name).Value;
    }
}
