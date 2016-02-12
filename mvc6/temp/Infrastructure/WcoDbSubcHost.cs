using System;
using System.Net.Http;
using Microsoft.AspNet.TestHost;
using Microsoft.Data.Entity;
using Wco.Migrations;
using WcoApi.Domain;

public class WcoDbSubcHost : IDisposable
{
    LocalDb _localDb;

    public string ConnectionString { get; private set; }
    public TestServer Server { get; set; }

    public WcoDbSubcHost()
    {
        _localDb = new LocalDb();
        ConnectionString = _localDb.ConnectionString;
        Wco.Migrations.Program.RunMigrations(new Options
        {
            ConnectionString = ConnectionString
        });
        Server = Create.TestServer(ConnectionString);
    }
    
    public HttpClient CreateClient()
    {
        return Server.CreateClient();
    }
    
    public PortalDbContext GetContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseSqlServer(ConnectionString);
        return new PortalDbContext(optionsBuilder.Options);
    }

    public void Dispose()
    {
        _localDb.Dispose();
    }
}