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
    [Table("ParameterGroups", Schema = "dbo")]
    public class ParameterGroup
    {
        public long ParameterGroupID { get; set; }
        [Required(ErrorMessage = "Необходимо указать название!")]
        [Display(Name = "Название")]
        public string ParameterGroupShortName { get; set; }
        [Display(Name = "Описание")]
        public string ParameterGroupName { get; set; }
        //[Display(Name = "LoginID")]
        public long LoginID { get; set; }
    }

    public interface IRepositoryParameterGroup
    {
        List<ParameterGroup> GetParameterGroups();
        bool SetParameterGroup(ParameterGroup pg);
    }

    public class RepositoryParameterGroup : IRepositoryParameterGroup
    {
        string connectionString = null;
        public RepositoryParameterGroup(string conn)
        {
            connectionString = conn;
        }

        public List<ParameterGroup> GetParameterGroups()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<ParameterGroup>("dbo.ParameterGroupsGet", commandType: CommandType.StoredProcedure).ToList();            }
        }

        public bool SetParameterGroup(ParameterGroup pg)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //ParameterGroup gr =  db.Query<ParameterGroup>("dbo.ParameterGroupCreate", commandType: CommandType.StoredProcedure).FirstOrDefault();                //return gr;
                try
                { 
                db.Execute("dbo.ParameterGroupsSet",
                    new { ParameterGroupID = pg.ParameterGroupID, ParameterGroupShortName = pg.ParameterGroupShortName, ParameterGroupName = pg.ParameterGroupName, LoginID = pg.LoginID },
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
