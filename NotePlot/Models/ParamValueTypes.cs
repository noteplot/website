using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ДЛЯ АТРИБУТОВ!!!
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace NotePlot.Models
{
    [Table("ParamValueTypes", Schema = "dbo")]
    public class ParamValueType
    {
        [Display(Name = "ID")]
        public int ParamValueTypeID { get; set; }
        [Column("ParamValueTypeShortCode")]
        [Display(Name = "Код")]
        public string ParamValueTypeCode { get; set; }
        [Display(Name = "Название типа")]
        public string ParamValueTypeShortName { get; set; }
        [Display(Name = "Название типа")]
        public string ParamValueTypeName { get; set; }
        [Display(Name = "Признак числового значения")]
        public bool IsNumeric { get; set; }
        [Display(Name = "Знаков после запятой")]
        public byte Scale { get; set; }
        [Display(Name = "Общее количество цифр/символов")]
        public int Precision { get; set; }
    }

    public interface IRepositoryParamValueType
    {
        List<ParamValueType> GetParamValueTypes();
    }

    public class RepositoryParamValueType : IRepositoryParamValueType
    {
        string connectionString = null;
        public RepositoryParamValueType(string conn)
        {
            connectionString = conn;
        }

        public List<ParamValueType> GetParamValueTypes()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<ParamValueType>("dbo.ParamValueTypesGet", commandType: CommandType.StoredProcedure).ToList();
                //return db.Query<ParamValueType>("SELECT * FROM dbo.ParamValueTypes").ToList();
            }
        }

    }
}