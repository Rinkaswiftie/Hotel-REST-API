using HotelAPI.Data;
using HotelAPI.Data.Repositories;
using HotelAPI.Data.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Tests.Helpers;
public class InMemoryDBHelper
{
    private readonly HotelierDBConText dbContext;

    public InMemoryDBHelper()
    {

        var builder = new DbContextOptionsBuilder<HotelierDBConText>();
        builder.UseInMemoryDatabase(databaseName: "Hotelier");
        var dbContextOptions = builder.Options;

        dbContext = new HotelierDBConText(dbContextOptions);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        DatabaseSeed.Seed(dbContext);
    }

    public IUnitOfWork GetUnitOfWork()
    {
        return new UnitOfWork(dbContext);
    }
}