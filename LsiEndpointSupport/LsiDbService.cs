using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using LsiEndpointSupport.Models;
using System.IO;
using Newtonsoft.Json;

namespace LsiEndpointSupport
{
    public class LsiDbService
    {
        public SqlConnection sqlConnection { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }

        public LsiDbService(string connectionString)
        {
            IsError = false;
            this.sqlConnection = new SqlConnection(connectionString);
        }

        public List<DocumentType> GetDocumentsTypes()
        {
            var documents = new List<DocumentType>();
            string sqlCommandText = "select DMID, SYMBOL from dok_mag_typ where ROZCHOD = 1 and ukryty = 0 and korekta = 0 and automatyczny = 0 and zewnetrzny = 0 and ZEJSCIE_MINUS = 1";
            var sqlCommand = new SqlCommand(sqlCommandText, sqlConnection);
            try
            {
                sqlConnection.Open();
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

            return documents;
        }

        public bool AddDocumentInfo(int documentId, string documentDescription)
        {

            return true;
        }
    }
}
