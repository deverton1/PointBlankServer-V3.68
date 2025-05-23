using Npgsql;
using System;

namespace PointBlank.Core.Sql
{
    public class SqlConnection
    {
        private static SqlConnection sql = new SqlConnection();
        private NpgsqlConnectionStringBuilder connBuilder;
        private NpgsqlConnection _sharedConnection;
        private readonly object _lock = new object();

        static SqlConnection()
        {
        }

        public SqlConnection()
        {
            connBuilder = new NpgsqlConnectionStringBuilder
            {
                Database = Config.dbName,
                Host = Config.dbHost,
                Username = Config.dbUser,
                Password = Config.dbPass,
                Port = Config.dbPort,
                Pooling = true,  // Habilita pooling de conexões
                MinPoolSize = 1,
                MaxPoolSize = 20
            };
        }

        public static SqlConnection getInstance()
        {
            return sql;
        }

        public NpgsqlConnection conn()
        {
            // Mantém o mesmo comportamento original (cria nova conexão cada vez)
            // Mas agora com pooling habilitado
            return new NpgsqlConnection(connBuilder.ConnectionString);
        }

        // Método adicional para quem quiser usar conexão compartilhada
        public NpgsqlConnection GetSharedConnection()
        {
            lock (_lock)
            {
                if (_sharedConnection == null || _sharedConnection.State != System.Data.ConnectionState.Open)
                {
                    _sharedConnection = new NpgsqlConnection(connBuilder.ConnectionString);
                    _sharedConnection.Open();
                }
                return _sharedConnection;
            }
        }

        // Método para fechar a conexão compartilhada (se estiver em uso)
        public void CloseSharedConnection()
        {
            lock (_lock)
            {
                if (_sharedConnection != null)
                {
                    _sharedConnection.Close();
                    _sharedConnection = null;
                }
            }
        }
    }
}