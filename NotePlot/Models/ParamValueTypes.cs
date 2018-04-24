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
        [Display(Name = "Краткое название")]
        public string ParamValueTypeShortName { get; set; }
        [Display(Name = "Название")]
        public string ParamValueTypeName { get; set; }
        [Display(Name = "Число?")]
        public bool IsNumeric { get; set; }
        [Display(Name = "После запятой")]
        public byte Scale { get; set; }
        [Display(Name = "Длина")]
        public int Precision { get; set; }
    }

    public interface IRepositoryParamValueType
    {
        List<ParamValueType> GetParamValueTypes();
        Task<List<ParamValueType>> GetParamValueTypesAsync();
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

        public Task<List<ParamValueType>> GetParamValueTypesAsync()
        {
            return Task.Run(()=> GetParamValueTypes());
        }

    }
}