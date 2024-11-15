﻿namespace Apito.Services;

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

}