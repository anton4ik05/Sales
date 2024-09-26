using Blazored.LocalStorage;

namespace ClientLibrary.Helpers;

public class LocalStorageService(ILocalStorageService storageService)
{
    private const string StorageKey = "auth-token";
    
    public async Task<string?> GetToken() => await storageService.GetItemAsStringAsync(StorageKey);
    public async Task SetToken(string token) => await storageService.SetItemAsStringAsync(StorageKey, token);
    public async Task RemoveToken() => await storageService.RemoveItemAsync(StorageKey);
}