using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace trpo7_sag_var18
{
    public class PackageApi
    {
        private readonly HttpClient _client;
        public PackageApi(HttpClient client)
        {
            _client = client;
        }
        public async Task<List<Package>> GetPackages()
        {
            return await _client.GetFromJsonAsync<List<Package>>("/Package");
        }
        public async Task<Package> GetPackage(int id)
        {
            return await _client.GetFromJsonAsync<Package>($"/Package/{id}");
        }
        public async Task<string> CreatePackage(Package package)
        {
            try
            {
                var response = await _client.PostAsJsonAsync<Package>("/Package", package);
                if (response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                return "Сервер не доступен";
            }
        }
        public async Task<string> DeletePackage(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"/Package/{id}");
                if (response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                return "Сервер не доступен";
            }
        }
        public async Task<string> PutPackage(int id, Package package)
        {
            try
            {
                var response = await _client.PutAsJsonAsync($"/Package/{id}", package);
                if (response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                return "Сервер недоступен";
            }
        }
        public async Task<string> PatchPackage(int id, Dictionary<string, object> changes)
        {
            try
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"/Package/{id}")
                {
                    Content = JsonContent.Create(changes)
                };

                var response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                return "Сервер недоступен";
            }
        }
    }
}
