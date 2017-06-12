using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace NPTest1.Models
{
    // класс и таблица dbo.Test_Parameter(s)
    public interface IRepositoryParameter
    {
        void Create(Test_Parameter par);
        void Delete(int id);
        List<Test_Parameter> GetParameters();
        void Update(Test_Parameter par);
        void UpdateValue(Test_Parameter par);
        Test_Parameter Get(int id);
    }

    public class RepositoryParameter : IRepositoryParameter
    {
        string connectionString = null;
        public RepositoryParameter(string conn)
        {
            connectionString = conn;
        }
        public List<Test_Parameter> GetParameters()
        {
            List<Test_Parameter> parameters = new List<Test_Parameter>();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                parameters = db.Query<Test_Parameter>("SELECT * FROM dbo.Test_Parameters").ToList();
            }
            return parameters;
        }

        public Test_Parameter Get(int id)
        {
            Test_Parameter par = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                par = db.Query<Test_Parameter>("SELECT * FROM dbo.Test_Parameters WHERE ParameterID = @id", new { id }).FirstOrDefault();
            }
            return par;
        }

        public void Create(Test_Parameter par)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //var sqlQuery = "INSERT INTO dbo.Test_Parameters ( ParameterName,  ParameterValue) VALUES(@ParameterName, @ParameterValue)";
                //db.Execute(sqlQuery, par);

                // если мы хотим получить id 
                var sqlQuery = "INSERT INTO dbo.Test_Parameters ( ParameterName,  ParameterValue) VALUES(@ParameterName, @ParameterValue); SELECT CAST(SCOPE_IDENTITY() as int)";
                int? parId = db.Query<int>(sqlQuery, par).FirstOrDefault();
                par.ParameterID = parId.Value;
            }
        }

        public void Update(Test_Parameter par)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "UPDATE dbo.Test_Parameters SET ParameterName = @ParameterName, ParameterValue = @ParameterValue, " +
                    "ParameterPrecision = @ParameterPrecision, ParameterScale = @ParameterScale,IsNegative=@IsNegative WHERE ParameterID = @ParameterID";
                db.Execute(sqlQuery, par);
            }
        }

        public void UpdateValue(Test_Parameter user)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "UPDATE dbo.Test_Parameters SET ParameterValue = @ParameterValue WHERE ParameterID = @ParameterID";
                db.Execute(sqlQuery, user);
            }
        }

        /*
        public void UpdateValue(decimal value)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "UPDATE dbo.Test_Parameters SET ParameterValue = @value WHERE ParameterID = @ParameterID";
                db.Execute(sqlQuery, user);
            }
        }
        */
        public void Delete(int id)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "DELETE FROM dbo.Test_Parameters WHERE ParameterID = @id";
                db.Execute(sqlQuery, new { id });
            }
        }
    }
}
