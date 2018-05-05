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
        List<ParameterGroup> GetParameterGroups(long lgId);
        Task<List<ParameterGroup>> GetParameterGroupsAsync(long lgId);
        ParameterGroup GetParameterGroup(long pgId, long lgId);
        Task<ParameterGroup> GetParameterGroupAsync(long pgId, long lgId);
        bool SetParameterGroup(ParameterGroup pg, int md);
        Task<bool> SetParameterGroupAsync(ParameterGroup pg, int md);
        //bool DelParameterGroup(long pgId);
    }

    public class RepositoryParameterGroup : IRepositoryParameterGroup
    {
        string connectionString = null;
        public RepositoryParameterGroup(string conn)
        {
            connectionString = conn;
        }

        public List<ParameterGroup> GetParameterGroups(long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<ParameterGroup>("dbo.ParameterGroupGet", new { LoginID = lgId }, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public Task<List<ParameterGroup>> GetParameterGroupsAsync(long lgId)
        {
            return Task.Run(()=> GetParameterGroups(lgId));
        }

        public ParameterGroup GetParameterGroup(long pgId, long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<ParameterGroup>("dbo.ParameterGroupGet", new { ParameterGroupID = pgId, LoginID = lgId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public Task<ParameterGroup> GetParameterGroupAsync(long pgId, long lgId)
        {
            return Task.Run(() => GetParameterGroup(pgId, lgId));
        }

        public bool SetParameterGroup(ParameterGroup pg, int md)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    var p = new DynamicParameters();
                    p.Add("@ParameterGroupID", pg.ParameterGroupID, dbType: DbType.Int64, direction: ParameterDirection.InputOutput);
                    p.Add("@ParameterGroupShortName", pg.ParameterGroupShortName);
                    p.Add("@ParameterGroupName", pg.ParameterGroupName);
                    p.Add("@LoginID", pg.LoginID);
                    p.Add("@Mode", md);
                    db.Execute("dbo.ParameterGroupSet",
                        p,
                        //new { UnitID = ut.UnitID, UnitShortName = ut.UnitShortName, UnitName = ut.UnitName, UnitGroupID = ut.UnitGroupID, LoginID = ut.LoginID, Mode = md },
                        commandType: CommandType.StoredProcedure);
                    if (md == 0)
                        pg.ParameterGroupID = p.Get<long>("@ParameterGroupID");
                    rt = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return rt;
        }

        public Task<bool> SetParameterGroupAsync(ParameterGroup pg, int md)
        {
            return Task.Run(() => SetParameterGroup(pg, md));
        }
    }
}
