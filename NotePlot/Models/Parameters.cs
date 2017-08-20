using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace NotePlot.Models
{
    public class ParameterType
    {
        public int ParameterTypeID { get; set; }
        public string ParameterTypeName { get; set; }
        public static IEnumerable<ParameterType> ParameterTypeList
        {
            get { return GetParameterTypeList(); }
        }
        private static IEnumerable<ParameterType> GetParameterTypeList()
        {
            return new List<ParameterType> // это почти константы, поэтому без обращения к БД
            {
                new ParameterType { ParameterTypeID = 0, ParameterTypeName = "Простой" },
                new ParameterType { ParameterTypeID = 1, ParameterTypeName = "Вычисляемый" },
                new ParameterType { ParameterTypeID = 2, ParameterTypeName = "Итоговый" }
            };
        }
    }


    public class Parameter
    {
        public long ParameterID { get; set; }

        [Required(ErrorMessage = "Группа должна быть установлена")]
        public long ParameterGroupID { get; set; }
        public string ParameterGroupShortName { get; set; }

        [Required(ErrorMessage = "Название параметра должно быть установлено")]
        public string ParameterShortName { get; set; }
        [Required(ErrorMessage = "Название параметра должно быть установлено")]
        public string ParameterName { get; set; }
        [Required(ErrorMessage = "Ед.изм. должна быть установлена")]
        public long ParameterUnitID { get; set; }
        public string ParameterUnitShortName { get; set; }

        public byte ParameterTypeID { get; set; }
        public string ParameterTypeName { get; set; }

        public int ParameterValueTypeID { get; set; }
        public string ParameterValueTypeShortName { get; set; }

        public decimal? ParameterValueMax { get; set; }
        public decimal? ParameterValueMin { get; set; }

        public long LoginID { get; set; }
    }

    public interface IRepositoryParameter
    {
        List<Parameter> GetParameters(long lgId);
        Parameter GetParameter(long prId, long lgId);
        bool SetParameter(Parameter pr, int md);
        //bool DelParameterGroup(long pgId);
    }

    public class RepositoryParameter : IRepositoryParameter
    {
        string connectionString = null;
        public RepositoryParameter(string conn)
        {
            connectionString = conn;
        }

        public List<Parameter> GetParameters(long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Parameter>("dbo.ParameterGet", new { LoginID = lgId }, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public Parameter GetParameter(long prId, long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Parameter>("dbo.ParameterGet", new { ParameterID = prId, LoginID = lgId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public bool SetParameter(Parameter pr, int md)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //ParameterGroup gr =  db.Query<ParameterGroup>("dbo.ParameterGroupCreate", commandType: CommandType.StoredProcedure).FirstOrDefault();
                //return gr;
                try
                {
                    db.Execute("dbo.ParameterSet",
                        new { ParameterID = pr.ParameterID, ParameterShortName = pr.ParameterShortName, ParameterName = pr.ParameterName, LoginID = pr.LoginID, Mode = md },
                        commandType: CommandType.StoredProcedure);
                    rt = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return rt;
        }

    }
}
