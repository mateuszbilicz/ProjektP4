using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Linq;

namespace ProjektP4;

class CurrencyData
{
    public decimal EUR { get; set; }
    public decimal USD { get; set; }
}

class CurrencyResponse
{
    public CurrencyData data { get; set; }
}

public class CurrencyValuesContext : DbContext
{
    String connectionString = ConfigurationManager.ConnectionStrings["CompanyDelegations"].ConnectionString;
    public DbSet<CurrencyValues> CurrencyValues { get; set; }

    public async void SyncCurrencyValues()
    {
        /*HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get,
            "https://api.freecurrencyapi.com/v1/latest?apikey=" +
            Environment.GetEnvironmentVariable("FREECURRENCYAPI_API_KEY") + "&currencies=EUR%2CUSD&base_currency=PLN");
        HttpClient client = new HttpClient();
        var res = await client.SendAsync(req);
        var json = await res.Content.ReadAsStringAsync();
        var currencyObj = JsonSerializer.Deserialize<CurrencyResponse>(json)!;
        CurrencyValues.RemoveRange(CurrencyValues);
        CurrencyValues.Add(new CurrencyValues { name = "EUR", value = 1 / currencyObj.data.EUR });
        CurrencyValues.Add(new CurrencyValues { name = "USD", value = 1 / currencyObj.data.USD });
        CurrencyValues.Add(new CurrencyValues { name = "PLN", value = 1 });
        SaveChanges();*/
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CurrencyValues>(cv => { cv.HasKey(["currencyValueId"]); });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(connectionString);
}

public class CurrencyValues
{
    public int currencyValueId { get; set; }
    public string name { get; set; }
    public decimal value { get; set; }
}