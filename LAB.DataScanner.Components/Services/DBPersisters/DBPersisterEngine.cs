using System.Data;
using System;
using System.Linq;
using System.Data.SqlClient;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using RabbitMQ.Client.Events;
using System.Text;
using LAB.DataScanner.Components.Interfaces.Engines;
using LAB.DataScanner.Components.Settings;

namespace LAB.DataScanner.HtmlToJsonConverter
{
    public class DBPersisterEngine : IEngine
    {
        private readonly SimpleTableDBPersisterSettings _settings;

        private readonly IRmqConsumer _rmqConsumer;

        private StringBuilder _columsNamesString = new StringBuilder();

        private StringBuilder _columsForInsertString = new StringBuilder();

        public DBPersisterEngine(SimpleTableDBPersisterSettings settings, IRmqConsumer consumer)
        {
             _settings = settings;

            _rmqConsumer = consumer;

            _columsNamesString.AppendJoin($", ", _settings.Colums.Select(p => p.Trim().Split(' ')).Select(p => p[0]).ToArray());

            _columsForInsertString.AppendJoin($", ", _settings.Colums.Select(p => p + $" \'$.{p.Trim().Split(' ')[0]}\'"));
        }

        public void Start() 
        {
            StartConfiguring();

            _rmqConsumer.StartListening(OnReceive);
        }

        public void StartConfiguring()
        {
            StringBuilder columsWithTypes = new StringBuilder();

            columsWithTypes.AppendJoin($", ", _settings.Colums);

            var dropProcIfExistText = @"IF EXISTS 
                                        (
	                                        SELECT type_desc, type
                                            FROM sys.procedures WITH(NOLOCK)
                                            WHERE NAME = 'CreateSchemaIfNotExists' AND type = 'P'
                                        )
                                        DROP PROCEDURE dbo.CreateSchemaIfNotExists;

                                        IF EXISTS 
                                        (
	                                        SELECT type_desc, type
                                            FROM sys.procedures WITH(NOLOCK)
                                            WHERE NAME = 'CreateTableIfNotExists' AND type = 'P'
                                        )
                                        DROP PROCEDURE dbo.CreateTableIfNotExists;

                                        IF EXISTS 
                                        (
	                                        SELECT type_desc, type
                                            FROM sys.procedures WITH(NOLOCK)
                                            WHERE NAME = 'ParseJsonToTable' AND type = 'P'
                                        )
                                        DROP PROCEDURE dbo.ParseJsonToTable;";

            var createSchemaIfNotExistsText = @"CREATE PROC CreateSchemaIfNotExists(@Schema NVARCHAR(MAX), @OwnerName NVARCHAR(MAX))
                                                AS
                                                BEGIN
                                                IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = @Schema)
                                                    BEGIN
		                                                DECLARE @sqlCommand NVARCHAR(MAX) = CONCAT('CREATE SCHEMA ', @Schema, ' AUTHORIZATION ', @OwnerName);
		                                                EXEC (@sqlCommand);
                                                    END
                                                END;";

            var createTableIfNotExistsText = @"CREATE PROC CreateTableIfNotExists(@Schema NVARCHAR(MAX), @TableName NVARCHAR(MAX), @ColumsWithTypes NVARCHAR(MAX))
                                                AS
                                                BEGIN

                                                    IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = @TableName)
                                                    BEGIN
                                                        DECLARE @sqlCommand NVARCHAR(MAX) = CONCAT('CREATE TABLE ', @Schema, '.', @TableName, '( ', @ColumsWithTypes, ')');
                                                    EXEC(@sqlCommand);
                                                    END
                                                END; ";


            var createParseJsonToTableProcText = @"CREATE PROC ParseJsonToTable (@Dbo NVARCHAR(MAX), @Schema NVARCHAR(MAX), @TableName NVARCHAR(MAX), 
                                                    @ColumsNamesString NVARCHAR(MAX), @JsonContent NVARCHAR(MAX), @ColumsForInsertString NVARCHAR(MAX))
                                                    AS
                                                    BEGIN
	                                                    DECLARE @sql NVARCHAR(MAX) = N'INSERT INTO ' + @Dbo + '.' + @Schema + '.' + @TableName +' (' + @ColumsNamesString + ') ' + 
									                        'SELECT * FROM  
									                        OPENJSON (''' + @JsonContent + ''' )  
									                        WITH(   
											                        ' + @ColumsForInsertString + '
										                        )';
	                                                    EXECUTE sp_executesql @sql;  
                                                    END;";

            using (SqlConnection connection = new SqlConnection(_settings.SqlConnectionString))
            {
                connection.Open();

                using (SqlCommand dropProceduresIfExists = new SqlCommand(dropProcIfExistText, connection))
                {
                    dropProceduresIfExists.ExecuteNonQuery();
                }

                using (SqlCommand createSchemaIfNotExists = new SqlCommand(createSchemaIfNotExistsText, connection))
                {
                    createSchemaIfNotExists.ExecuteNonQuery();
                }

                using (SqlCommand createTableIfNotExists = new SqlCommand(createTableIfNotExistsText, connection))
                {
                    createTableIfNotExists.ExecuteNonQuery();
                }

                using (SqlCommand createParseJsonToTableProc = new SqlCommand(createParseJsonToTableProcText, connection))
                {
                    createParseJsonToTableProc.ExecuteNonQuery();
                }

                using (SqlCommand createSchemaIfNotExistsProc = new SqlCommand("CreateSchemaIfNotExists", connection))
                {
                    createSchemaIfNotExistsProc.CommandType = CommandType.StoredProcedure;

                    createSchemaIfNotExistsProc.Parameters.AddWithValue("@Schema", SqlDbType.NVarChar).Value = _settings.Schema;

                    createSchemaIfNotExistsProc.Parameters.AddWithValue("@OwnerName", SqlDbType.NVarChar).Value = _settings.OwnerName;

                    createSchemaIfNotExistsProc.ExecuteNonQuery();
                }

                using (SqlCommand createTableIfNotExistsProc = new SqlCommand("CreateTableIfNotExists", connection))
                {
                    createTableIfNotExistsProc.CommandType = CommandType.StoredProcedure;

                    createTableIfNotExistsProc.Parameters.AddWithValue("@Schema", SqlDbType.NVarChar).Value = _settings.Schema;

                    createTableIfNotExistsProc.Parameters.AddWithValue("@TableName", SqlDbType.NVarChar).Value = _settings.TableName;

                    createTableIfNotExistsProc.Parameters.AddWithValue("@ColumsWithTypes", SqlDbType.NVarChar).Value = columsWithTypes.ToString();

                    createTableIfNotExistsProc.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public void JsonToTableProcedureExecute(string jsonContent) 
        {
            if (jsonContent.Equals("")) return;

            SqlConnection connection = new SqlConnection(_settings.SqlConnectionString);

            connection.Open();

            using (SqlCommand createParseJsonToTableProc = new SqlCommand("ParseJsonToTable", connection))
            {
                createParseJsonToTableProc.CommandType = CommandType.StoredProcedure;

                createParseJsonToTableProc.Parameters.AddWithValue("@Dbo", SqlDbType.NVarChar).Value = _settings.Dbo;

                createParseJsonToTableProc.Parameters.AddWithValue("@Schema", SqlDbType.NVarChar).Value = _settings.Schema;

                createParseJsonToTableProc.Parameters.AddWithValue("@TableName", SqlDbType.NVarChar).Value = _settings.TableName;

                createParseJsonToTableProc.Parameters.AddWithValue("@ColumsNamesString", SqlDbType.NVarChar).Value = _columsNamesString.ToString();

                createParseJsonToTableProc.Parameters.AddWithValue("@JsonContent", SqlDbType.NVarChar).Value = jsonContent;

                createParseJsonToTableProc.Parameters.AddWithValue("@ColumsForInsertString", SqlDbType.NVarChar).Value = _columsForInsertString.ToString();

                createParseJsonToTableProc.ExecuteReader();
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
