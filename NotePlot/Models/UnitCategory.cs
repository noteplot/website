using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace NotePlot.Models
{
    [Table("UnitGroups", Schema = "dbo")]
    public class UnitCategory
    {
        public long? UnitGroupID { get; set; }
        public string UnitGroupShortName { get; set; }
        public string UnitGroupName { get; set; }
        public long LoginID { get; set; }
    }

    public interface IUnitCategoryRepository
    {
        List<UnitCategory> GetCategories(long lgId);
        Task<List<UnitCategory>> GetCategoriesAsync(long lgId);
        UnitCategory GetCategory(long ucId, long lgId);
        Task<UnitCategory> GetCategoryAsync(long ucId, long lgId);
        bool SetCategory(UnitCategory uc, int md);
        Task<bool> SetCategoryAsync(UnitCategory uc, int md);
    }

    class UnitCategoryRepository : IUnitCategoryRepository
    {
        string connectionString = null;

        public UnitCategoryRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<UnitCategory> GetCategories(long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<UnitCategory>("dbo.UnitGroupGet", new { LoginID = lgId }, commandType: CommandType.StoredProcedure).ToList();
                //return db.Query<UnitCategory>("SELECT * FROM UnitGroups ORDER BY UnitGroupName").ToList();
            }
        }

        public Task<List<UnitCategory>> GetCategoriesAsync(long lgId)
        {
            return Task.Run(()=> GetCategories(lgId));
        }

        public UnitCategory GetCategory(long ucId, long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<UnitCategory>("dbo.UnitGroupGet", new { UnitGroupID = ucId, LoginID = lgId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                //return db.Query<UnitCategory>("SELECT * FROM UnitGroups WHERE UnitGroupId = @id", new { id = id }).FirstOrDefault();
            }
        }

        public Task<UnitCategory> GetCategoryAsync(long ucId, long lgId)
        {
            return Task.Run(()=> GetCategory(ucId, lgId));
        }
        
        public bool SetCategory(UnitCategory uc, int md)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    db.Execute("dbo.UnitGroupSet",
                        new { UnitGroupID = uc.UnitGroupID, UnitGroupShortName = uc.UnitGroupShortName, UnitGroupName = uc.UnitGroupName, LoginID = uc.LoginID, Mode = md },
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

        public Task<bool> SetCategoryAsync(UnitCategory uc, int md)
        {
            return Task.Run(() => SetCategory(uc, md));
        }

    }
}
