using EfVersusDapper.Models;
using Npgsql;

namespace EfVersusDapper.Extensions;

public static class NpgExtensions
{
    internal static NpgsqlDataSource CreateDefaultNpgDataSource(string connectionString)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapEnum<Gender>("MyGender");

        var dataSource = dataSourceBuilder.Build();

        return dataSource;
    }
}