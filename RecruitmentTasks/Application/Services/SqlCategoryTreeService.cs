using Microsoft.Data.SqlClient;
using System.Data;
using RecruitmentTasks.Application.DTOs;
using RecruitmentTasks.Common;

namespace RecruitmentTasks.Application.Services;

public class SqlCategoryTreeService(string connectionString) : ICategoryTreeService
{
    public async Task<List<CategoryDto>> GetCategoryTreeAsync()
    {
        return await ExecuteStoredProcedureAsync(
            Constants.Database.StoredProcedureName,
            MapCategoryFromReader);
    }

    private async Task<List<T>> ExecuteStoredProcedureAsync<T>(
        string procedureName,
        Func<SqlDataReader, T> rowMapper)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = Constants.Database.CommandTimeoutSeconds
        };

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        
        var results = new List<T>();
        while (await reader.ReadAsync())
        {
            results.Add(rowMapper(reader));
        }

        return results;
    }

    private static CategoryDto MapCategoryFromReader(SqlDataReader reader)
    {
        return new CategoryDto
        {
            Id = reader.GetInt32(reader.GetOrdinal(nameof(CategoryDto.Id))),
            Name = reader.GetString(reader.GetOrdinal(nameof(CategoryDto.Name))),
            ParentId = reader.GetOrdinal(nameof(CategoryDto.ParentId)) is var parentOrdinal && reader.IsDBNull(parentOrdinal) 
                ? null 
                : reader.GetInt32(parentOrdinal),
            Level = reader.GetInt32(reader.GetOrdinal(nameof(CategoryDto.Level)))
        };
    }
}
