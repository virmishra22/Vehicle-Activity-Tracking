using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Vehicle_Activity_Tracking
{
    public class classTransConnection
    {
        //Live String
        private static string conApi = "AxpVinsum";

        private string strConnection = "";
        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        private SqlTransaction sqlTransaction;

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "0ram@1234xxxxxxxxxxtttttuuuuuiiiiio";  //we can change the code converstion key as per our requirement, but the decryption key should be same as encryption key    
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public string abc(string DatabaseName)
        {
            string returnValue = "";
            if (DatabaseName == "")
                DatabaseName = conApi;
            XElement xElement = XElement.Load(@"D:\Vinsum Connection List\dbData.xml");
            IEnumerable<XElement> dbList = xElement.Elements();
            foreach (var db in dbList)
            {
                if (db.Attribute("DBNAME").ToString().Contains(DatabaseName))
                {
                    string serverIP = db.Element("SERVER").Value.ToString();
                    string dbName = db.Element("DATABASENAME").Value.ToString();
                    string userID = db.Element("USERID").Value.ToString();
                    string pass = Decrypt(db.Element("PASS").Value.ToString());
                    returnValue = @"Server=" + serverIP + @";Database=" + dbName + @";user id=" + userID + ";pwd=" + pass + @";TrustServerCertificate=True; Connection Lifetime=0;Enlist=true;Max Pool Size=5000;Min Pool Size=0;Pooling=true;";
                    break;
                }
            }
            return returnValue;
        }
        public classTransConnection()
        {
            strConnection = abc("");
            sqlConnection = new SqlConnection(strConnection);
        }
        public classTransConnection(string ConStr)
        {
            sqlConnection = new SqlConnection(abc(ConStr));
        }


        public void TransactionBegin()
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlTransaction = sqlConnection.BeginTransaction();
            sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.CommandTimeout = 300;
        }
        public void ConnectionOpen()
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
        }
        public void ConnectionClose()
        {
            sqlConnection.Close();
            sqlConnection.Dispose();
        }
        public void Transaction_Commit()
        {
            sqlTransaction.Commit();
            sqlConnection.Close();
        }
        public void Transaction_Rollback()
        {
            sqlTransaction.Rollback();
            sqlConnection.Close();
        }
        public DataSet getDataSet(string strSQL)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter da;
            da = new SqlDataAdapter(strSQL, sqlConnection);
            da.Fill(ds);
            return ds;
        }
        public SqlDataAdapter getDataAdapter(string strSQL)
        {
            SqlDataAdapter da;
            da = new SqlDataAdapter(strSQL, sqlConnection);
            return da;
        }
        public DataSet getTableDataSet(string strSQL, string strTableName)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter da;
            da = new SqlDataAdapter(strSQL, sqlConnection);
            da.Fill(ds, strTableName);
            return ds;
        }
        public SqlDataReader getDataReader(string strSQL)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand = new SqlCommand(strSQL, sqlConnection);
            sqlCommand.CommandTimeout = 300;
            return sqlCommand.ExecuteReader();
        }
        public SqlDataReader getDataReaderTimeTaken(string strSQL)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand = new SqlCommand(strSQL, sqlConnection);
            sqlCommand.CommandTimeout = 900;
            return sqlCommand.ExecuteReader();
        }
        public SqlDataReader getDataReaderParam(string strSQL, SqlParameter[] Param)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand = new SqlCommand(strSQL, sqlConnection);
            sqlCommand.CommandTimeout = 300;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Parameters.Clear();
            for (int i = 0; i < Param.Length; i++)
            {
                sqlCommand.Parameters.Add(Param[i]);
            }
            return sqlCommand.ExecuteReader();
        }
        public SqlDataReader getDataReaderTrans(string strSQL)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand.CommandText = strSQL;
            return sqlCommand.ExecuteReader();
        }
        public SqlDataReader getDataReaderParamTrans(string strSQL, SqlParameter[] Param)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand.CommandText = strSQL;
            sqlCommand.Parameters.Clear();
            for (int i = 0; i < Param.Length; i++)
            {
                sqlCommand.Parameters.Add(Param[i]);
            }
            return sqlCommand.ExecuteReader();
        }
        public double executeQuery(string strSQL)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand = new SqlCommand(strSQL, sqlConnection);
            sqlCommand.CommandTimeout = 300;
            double n = sqlCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
            return n;
        }
        public double executeQueryTrans(string strSQL)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand.CommandText = strSQL;
            double n = sqlCommand.ExecuteNonQuery();
            return n;
        }
        public double executeParamQuery(string strSQL, SqlParameter[] Param)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand = new SqlCommand(strSQL, sqlConnection);
            sqlCommand.CommandTimeout = 300;
            sqlCommand.Parameters.Clear();
            for (int i = 0; i < Param.Length; i++)
            {
                sqlCommand.Parameters.Add(Param[i]);
            }
            double n = sqlCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
            return n;
        }
        public double executeParamQueryTrans(string strSQL, SqlParameter[] Param)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand.CommandText = strSQL;
            sqlCommand.Parameters.Clear();
            for (int i = 0; i < Param.Length; i++)
            {
                sqlCommand.Parameters.Add(Param[i]);
            }
            double n = sqlCommand.ExecuteNonQuery();
            return n;
        }
        public SqlDataReader executeGetDataParam(string strSQL, SqlParameter[] Param)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand = new SqlCommand(strSQL, sqlConnection);
            sqlCommand.CommandTimeout = 300;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Parameters.Clear();
            for (int i = 0; i < Param.Length; i++)
            {
                sqlCommand.Parameters.Add(Param[i]);
            }
            return sqlCommand.ExecuteReader();
        }
        public void executeProcedure(string strSQL, SqlParameter[] Param)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand = new SqlCommand(strSQL, sqlConnection);
            sqlCommand.CommandTimeout = 300;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.Clear();
            for (int i = 0; i < Param.Length; i++)
            {
                sqlCommand.Parameters.Add(Param[i]);
            }
            sqlCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
        }
        public void executeProcedureTran(string strSQL, SqlParameter[] Param)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand.CommandText = strSQL;

            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.Clear();
            for (int i = 0; i < Param.Length; i++)
            {
                sqlCommand.Parameters.Add(Param[i]);
            }
            sqlCommand.ExecuteNonQuery();
        }
        public SqlDataReader executeProcedureGetData(string strSQL, SqlParameter[] Param)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand = new SqlCommand(strSQL, sqlConnection);
            sqlCommand.CommandTimeout = 300;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.Clear();
            for (int i = 0; i < Param.Length; i++)
            {
                sqlCommand.Parameters.Add(Param[i]);
            }
            return sqlCommand.ExecuteReader();
        }
        public SqlDataReader executeProcedureGetDataTran(string strSQL, SqlParameter[] Param)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            //sqlCommand = new SqlCommand(strSQL, sqlConnection);
            //sqlCommand.CommandTimeout = 300;
            sqlCommand.CommandText = strSQL;

            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.Clear();
            for (int i = 0; i < Param.Length; i++)
            {
                sqlCommand.Parameters.Add(Param[i]);
            }
            return sqlCommand.ExecuteReader();
        }
        public void executeProcedureNoParam(string strSQL)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            sqlCommand = new SqlCommand(strSQL, sqlConnection);
            sqlCommand.CommandTimeout = 300;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
        }


    }
}
