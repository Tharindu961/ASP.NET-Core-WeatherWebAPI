using System.Data.SqlClient;
using Weather_API.Models;
using Weather_API.Services.Interfaces;

namespace Weather_API.Services;

public class WeatherRepository : IWeatherRepository
{
    private readonly string _connectionString;

    public WeatherRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<WeatherRecord?> GetByCityAsync(string city)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            using (var cmd = new SqlCommand("SELECT * FROM WeatherRecord WHERE City = @City", conn))
            {
                cmd.Parameters.AddWithValue("@City", city);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapRecord(reader);
                    }
                }
            }
        }

        return null;
    }

    public async Task<int> InsertAsync(WeatherRecord record)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            using (var cmd = new SqlCommand(@"
                INSERT INTO WeatherRecord
                (City, Country, Temperature, Description, Humidity, Pressure, WindSpeed, Cloudiness, LastUpdated)
                VALUES
                (@City, @Country, @Temperature, @Description, @Humidity, @Pressure, @WindSpeed, @Cloudiness, GETDATE());
                SELECT SCOPE_IDENTITY();
            ", conn))
            {
                cmd.Parameters.AddWithValue("@City", record.City);
                cmd.Parameters.AddWithValue("@Country", record.Country);
                cmd.Parameters.AddWithValue("@Temperature", record.Temperature);
                cmd.Parameters.AddWithValue("@Description", record.Description);
                cmd.Parameters.AddWithValue("@Humidity", record.Humidity);
                cmd.Parameters.AddWithValue("@Pressure", record.Pressure);
                cmd.Parameters.AddWithValue("@WindSpeed", record.WindSpeed);
                cmd.Parameters.AddWithValue("@Cloudiness", record.Cloudiness);

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
    }

    public async Task<int> UpdateAsync(WeatherRecord record)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            using (var cmd = new SqlCommand(@"
                UPDATE WeatherRecord
                SET Country = @Country,
                    Temperature = @Temperature,
                    Description = @Description,
                    Humidity = @Humidity,
                    Pressure = @Pressure,
                    WindSpeed = @WindSpeed,
                    Cloudiness = @Cloudiness,
                    LastUpdated = GETDATE()
                WHERE City = @City
            ", conn))
            {
                cmd.Parameters.AddWithValue("@City", record.City);
                cmd.Parameters.AddWithValue("@Country", record.Country);
                cmd.Parameters.AddWithValue("@Temperature", record.Temperature);
                cmd.Parameters.AddWithValue("@Description", record.Description);
                cmd.Parameters.AddWithValue("@Humidity", record.Humidity);
                cmd.Parameters.AddWithValue("@Pressure", record.Pressure);
                cmd.Parameters.AddWithValue("@WindSpeed", record.WindSpeed);
                cmd.Parameters.AddWithValue("@Cloudiness", record.Cloudiness);

                return await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task<List<WeatherRecord>> GetAllAsync()
    {
        var records = new List<WeatherRecord>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM WeatherRecord", conn))
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    records.Add(MapRecord(reader));
                }
            }
        }
        return records;
    }

    private WeatherRecord MapRecord(SqlDataReader reader)
    {
        return new WeatherRecord
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            City = reader["City"].ToString() ?? "",
            Country = reader["Country"].ToString() ?? "",
            Temperature = Convert.ToDouble(reader["Temperature"]),
            Description = reader["Description"].ToString() ?? "",
            Humidity = Convert.ToInt32(reader["Humidity"]),
            Pressure = Convert.ToInt32(reader["Pressure"]),
            WindSpeed = Convert.ToDouble(reader["WindSpeed"]),
            Cloudiness = Convert.ToInt32(reader["Cloudiness"]),
            LastUpdated = Convert.ToDateTime(reader["LastUpdated"])
        };
    }
}
