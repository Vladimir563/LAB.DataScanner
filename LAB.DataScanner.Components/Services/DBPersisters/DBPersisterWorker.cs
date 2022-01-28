using Microsoft.Extensions.Configuration;
using System.Data;
using System;
using System.Linq;
using System.Data.SqlClient;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using RabbitMQ.Client.Events;
using System.Text;

namespace LAB.DataScanner.HtmlToJsonConverter
{
    public class DBPersisterWorker
    {
        private readonly IConfigurationRoot _configuration;

        private readonly IRmqConsumer _rmqConsumer;

        private readonly string _connectionString;

        private readonly string _dbo;

        private readonly string _schema;

        private readonly string _tableName;

        private readonly string _colums;

        public DBPersisterWorker(IConfigurationRoot configuration, IRmqConsumer consumer)
        {
             _configuration = configuration;

            _rmqConsumer = consumer;

            _connectionString = _configuration.GetSection("DBTableCreationSettings:SqlConnectionString").Value;

            _dbo = _configuration.GetSection("DBTableCreationSettings:dbo").Value;

            _schema = _configuration.GetSection("DBTableCreationSettings:schema").Value;

            _tableName = _configuration.GetSection("DBTableCreationSettings:tableName").Value;

            _colums = _configuration.GetSection("DBTableCreationSettings:colums").Value;
        }

        public void Start() 
        {
            _rmqConsumer.StartListening(OnReceive);
        }

        public void StartConfiguring()
        {
            var columsArr = _colums.Split(',');

            var columsNamesArr = columsArr.Select(p => p.Trim().Split(' ')).Select(p => p[0]).ToArray();

            var columsNamesString = $"{String.Join($", ", columsNamesArr)}";

            var columsForInsertArr = columsArr.Select(p => p + $" \'$.{p.Trim().Split(' ')[0]}\'");

            var columsForInsertString = $"{String.Join($", ", columsForInsertArr)}";

            var create_schema = $@"IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = '{_schema}')
                                BEGIN
                                    EXEC( 'CREATE SCHEMA {_schema} AUTHORIZATION db_owner' );
                                END"; //this call should be in his own batch

            var dropProcedureIfExists = $@"
                                IF EXISTS(
                                    SELECT type_desc, type
                                    FROM sys.procedures WITH(NOLOCK)
                                    WHERE NAME = 'parse_json_to_table'
                                        AND type = 'P'
                                        )
                                DROP PROCEDURE dbo.parse_json_to_table ";

            var procedure = $@"CREATE PROCEDURE dbo.parse_json_to_table (@json NVARCHAR(MAX))
                            AS
                            BEGIN
	                            IF OBJECT_ID(N'{_dbo}.{_schema}.{_tableName}', N'U') IS NULL 
	                            BEGIN
	                                CREATE TABLE {_dbo}.{_schema}.{_tableName} 
                                    ({_colums})
	                            END;
                                
                                INSERT INTO {_dbo}.{_schema}.{_tableName} ({columsNamesString})
                                SELECT * FROM  
                                OPENJSON (@json)  
                                WITH(   
                                        {columsForInsertString}
                                    );                         
                            END;";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using (SqlCommand cmd = new SqlCommand(create_schema, connection))
            {
                connection.Open();
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                connection.Close();
            }

            using (SqlCommand cmd = new SqlCommand(dropProcedureIfExists, connection))
            {
                connection.Open();
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                connection.Close();
            }

            using (SqlCommand cmd = new SqlCommand(procedure, connection))
            {
                connection.Open();
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void JsonToTableProcedureExecute(string json) 
        {
            var procedure = $@"
                                DECLARE @json NVARCHAR(MAX)
                                SET @json = {json}
                                EXEC dbo.parse_json_to_table @json;";

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand cmd = new SqlCommand(procedure, connection);

            connection.Open();

            cmd.CommandType = CommandType.Text;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //logging
                Console.WriteLine(e.Message);
            }

            connection.Close();
        }

        public void OnReceive(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();

            var htmlContent = Encoding.UTF8.GetString(body);

            _rmqConsumer.Ack(ea);

            JsonToTableProcedureExecute(htmlContent);
        }
    }
}
