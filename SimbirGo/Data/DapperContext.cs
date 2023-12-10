// using System.Data;
// using Npgsql;
// using SimbirGo.Settings.Interfaces;
//
// namespace SimbirGo.Data;
//
// public class DapperContext
// {
//     private readonly string _connectionString;
//         
//     public DapperContext(IConfigurationSettings configuration)
//     {
//         this._connectionString = configuration.DbConnectionsOwn;
//     }
//         
//     public IDbConnection Connection => new NpgsqlConnection(_connectionString);
//
//         
//     public IDbConnection CreateConnection() 
//         => new NpgsqlConnection(_connectionString);
// }