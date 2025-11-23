using Microsoft.Data.SqlClient;
using System.Data;
using RecruitmentTasks.Common;

namespace RecruitmentTasks.Infrastructure.Services;

public static class StoredProcedureInstaller
{
    private const string ProcedureBody = @"CREATE PROCEDURE GetCategoryTree
AS
BEGIN
    SET NOCOUNT ON;
    
    WITH CategoryHierarchy AS (
        SELECT 
            Id, 
            Name, 
            ParentId, 
            Level,
            CAST(Name AS NVARCHAR(MAX)) AS SortPath
        FROM Categories 
        WHERE ParentId IS NULL
        
        UNION ALL
        
        SELECT 
            c.Id, 
            c.Name, 
            c.ParentId, 
            c.Level,
            CAST(ch.SortPath + '|' + c.Name AS NVARCHAR(MAX))
        FROM Categories c
        INNER JOIN CategoryHierarchy ch ON c.ParentId = ch.Id
    )
    SELECT 
        Id, 
        Name, 
        ParentId, 
        Level
    FROM CategoryHierarchy
    ORDER BY SortPath
    OPTION (MAXRECURSION 0);
END";

    public static async Task EnsureAsync(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // Drop existing procedure to ensure we have the latest version
        var dropCommand = new SqlCommand(
            "IF OBJECT_ID(@proc, 'P') IS NOT NULL DROP PROCEDURE " + Constants.Database.StoredProcedureName, connection);
        dropCommand.Parameters.Add(new SqlParameter("@proc", SqlDbType.NVarChar, Constants.Database.ProcedureNameMaxLength) { Value = Constants.Database.StoredProcedureName });
        await dropCommand.ExecuteNonQueryAsync();

        // Create new version
        using var create = new SqlCommand(ProcedureBody, connection);
        await create.ExecuteNonQueryAsync();
    }
}
