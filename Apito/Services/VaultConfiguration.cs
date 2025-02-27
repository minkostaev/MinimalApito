﻿namespace Apito.Services;

using Apito.Models;
using Mintzat.Email.Services;
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
        using var client = new HttpClient(clientHandler);
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
        using var client = new HttpClient(clientHandler);
        try
        {
            var content = new StringContent(jsonDto, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_vaultUri2, content);
            if (!response.IsSuccessStatusCode)
            {
                CustomLogger.Add(this, CustomLogger.GetLine(), $"IsSuccessStatusCode = false {jsonDto}");
                return null;
            }

            var resJson = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            if (resJson == null)
            {
                CustomLogger.Add(this, CustomLogger.GetLine(), "resJson = null " + jsonDto);
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
            CustomLogger.Add(this, CustomLogger.GetLine(), string.IsNullOrEmpty(ex.StackTrace) ? ex.Message : ex.Message + ex.StackTrace);
            return null;
        }
    }

}