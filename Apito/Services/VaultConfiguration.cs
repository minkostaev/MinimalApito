namespace Apito.Services;

using Apito.Models;
using System.Text;
using System.Text.Json;

public class VaultConfiguration
{
    private readonly string _vaultUri;
    private readonly string _vaultId;
    private readonly string _vaultCrypt;

    public VaultConfiguration(string hostLink)
    {
        _vaultUri = hostLink + "/vault";
        _vaultId = Environment.UserDomainName + " " + Environment.UserName;
        _vaultCrypt = _vaultId + " " + DateTime.UtcNow.Year;
    }

    public async Task<object> Get(string property)
    {
        var vaultDto = new VaultDto { Id = _vaultId, Property = property };
        string jsonDto = JsonSerializer.Serialize(vaultDto);

        using (var client = new HttpClient())
        {
            try
            {
                var content = new StringContent(jsonDto, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_vaultUri, content);
                if (!response.IsSuccessStatusCode)
                    return response.StatusCode;
                
                var resJson = await response.Content.ReadFromJsonAsync<VaultDto>();

                var crypto = new CryptographyAlgorithm();
                string result = crypto.DecryptCipherTextToPlainText(
                    resJson!.Property, _vaultCrypt, resJson.Id);

                return result;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }

}