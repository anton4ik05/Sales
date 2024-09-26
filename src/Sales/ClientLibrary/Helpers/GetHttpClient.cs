using System.Net.Http.Headers;
 using BaseLibrary.Dtos;
 
 namespace ClientLibrary.Helpers;
 
 public class GetHttpClient(IHttpClientFactory httpClientFactory, LocalStorageService localStorageService)
 {
     private const string HeaderKey = "Authorization";
 
     public async Task<HttpClient> GetPrivateHttpClient()
     {
         var client = httpClientFactory.CreateClient("SystemApiClient");
         var stringToken = await localStorageService.GetToken();
 
         if (string.IsNullOrWhiteSpace(stringToken)) return client;
 
         var token = Serializations.DeserializeObj<UserSession>(stringToken);
         if (token == null) return client;
 
         client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
         return client;
     }
 
     public HttpClient GetPublicHttpClient()
     {
         var client = httpClientFactory.CreateClient("SystemApiClient");
         client.DefaultRequestHeaders.Remove(HeaderKey);
         return client;
     }
 }