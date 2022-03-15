using LsiEndpointSupport.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LsiEndpointSupport
{
    public class LsiDbService
    {
        public readonly SqlConnection _sqlConnection;
        public bool IsError { get; set; }
        public string Message { get; set; }

        public LsiDbService(string connectionString)
        {
            IsError = false;
            _sqlConnection = new SqlConnection(connectionString);
        }

        public List<DocumentType> GetDocumentsTypes()
        {
            var documents = new List<DocumentType>();
            string sqlCommandText = "select DMID, SYMBOL from dok_mag_typ where ROZCHOD = 1 and ukryty = 0 and korekta = 0 and automatyczny = 0 and zewnetrzny = 0 and ZEJSCIE_MINUS = 1";
            var sqlCommand = new SqlCommand(sqlCommandText, _sqlConnection);
            try
            {
                _sqlConnection.Open();
                var sqlReader = sqlCommand.ExecuteReader();
                while (sqlReader.Read())
                {
                    documents.Add(new DocumentType
                    {
                        DocumentTypeId = (int)sqlReader["DMID"],
                        Symbol = (string)sqlReader["SYMBOL"]
                    });
                }
            }
            catch (Exception ex)
            {
                IsError = true;
                Message = ex.Message;
            }
            finally
            {
                _sqlConnection.Close();
            }

            return documents;
        }

        public bool AddDocumentInfo(int documentId, string documentDescription, DateTime documentDate)
        {
            var sqlCommand = new SqlCommand("update LSIdok_mag_oczek_nagl set UWAGI = @DocDesc, Data = @DocDate where DCID = @DocId", _sqlConnection);
            sqlCommand.Parameters.Add("@DocDesc", System.Data.SqlDbType.NVarChar).Value = documentDescription;
            sqlCommand.Parameters.Add("@DocId", System.Data.SqlDbType.Int).Value = documentId;
            sqlCommand.Parameters.Add("@DocDate", System.Data.SqlDbType.DateTime).Value = new DateTime(documentDate.Year, documentDate.Month, documentDate.Day, 23, 59, 59);
            try
            {
                _sqlConnection.Open();
                var sqlResponse = sqlCommand.ExecuteNonQuery();
                IsError = false;
            }
            catch (Exception ex)
            {
                IsError = true;
                Message = ex.Message;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return !IsError;
        }
    }
}
