namespace Apito.Services;

using Apito.Models;
using System.Text;
using System.Text.Json;

public class VaultConfiguration
{
    private readonly string _vaultUri;
    private readonly string _vaultUri2;
    private readonly string _vaultId;
    private readonly string _vaultCrypt;

    public VaultConfiguration(string hostLink)
    {
        _vaultUri = hostLink + "/vault";
        _vaultUri2 = hostLink + "/vaults";

        var dashNames = Environment.UserDomainName.Split('-');
        string domName = dashNames[0];
        if (dashNames.Length > 1)
            domName = domName + "-" + dashNames[1];

        _vaultId = domName + " " + Environment.UserName;
        _vaultCrypt = _vaultId + " " + DateTime.UtcNow.Year;
    }

    public async Task<object?> Get(string property)
    {
        var vaultDto = new VaultDto { Id = _vaultId, Property = property };
        string jsonDto = JsonSerializer.Serialize(vaultDto);

        var clientHandler = new HttpClientHandler
        {
            #pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            #pragma warning restore S4830
        };
        using (var client = new HttpClient(clientHandler))
        {
            try
            {
                var content = new StringContent(jsonDto, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_vaultUri, content);
                if (!response.IsSuccessStatusCode)
                    return null;
                
                var resJson = await response.Content.ReadFromJsonAsync<VaultDto>();

                var crypto = new CryptographyAlgorithm();
                string result = crypto.DecryptCipherTextToPlainText(
                    resJson!.Property, _vaultCrypt, resJson.Id);

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public async Task<List<string>?> Get(string[] property)
    {
        var list = new List<string> { _vaultId };
        list.AddRange(property);
        string jsonDto = JsonSerializer.Serialize(list);

        var clientHandler = new HttpClientHandler
        {
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
#pragma warning restore S4830
        };
        string postInfo = string.Empty;
        using (var client = new HttpClient(clientHandler))
        {
            try
            {
                postInfo = _vaultUri2 == null ? "N/A" + " " + jsonDto : _vaultUri2 + " " + jsonDto;
                var content = new StringContent(jsonDto, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_vaultUri2, content);
                if (!response.IsSuccessStatusCode)
                {
                    AppValues.SecretError = "IsSuccessStatusCode = false" + " " + postInfo;
                    return null;
                }

                var resJson = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (resJson == null)
                {
                    AppValues.SecretError = "resJson = null" + " " + postInfo;
                    return null;
                }
                
                List<string> result = [];
                foreach (var d in resJson)
                {
                    var crypto = new CryptographyAlgorithm();
                    string key = crypto.DecryptCipherTextToPlainText(
                        d.Value, _vaultCrypt, d.Key);
                    result.Add(key);
                }
                return result;
            }
            catch (Exception ex)
            {
                string err = string.IsNullOrEmpty(ex.StackTrace) ? ex.Message : ex.Message + ex.StackTrace;
                AppValues.SecretError = err + " " + postInfo;
                return null;
            }
        }
    }

}