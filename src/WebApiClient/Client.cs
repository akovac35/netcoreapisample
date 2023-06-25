namespace WebApiClient
{
    public partial class Client
    {
        public static Client CreateTestClient(HttpClient httpClient)
        {
            return new Client(baseUrl: null, httpClient: httpClient);
        }
    }
}
