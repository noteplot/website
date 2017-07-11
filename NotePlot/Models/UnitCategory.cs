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
        public int UnitGroupId { get; set; }
        public string UnitGroupName { get; set; }
        public string UnitGroupShortName { get; set; }
        public string UnitGroupCode { get; set; }
    }

    public interface IUnitCategoryRepository
    {
        List<UnitCategory> GetCategories();
        UnitCategory GetCategory(int id);
    }

    class UnitCategoryRepository : IUnitCategoryRepository
    {
        string connectionString = null;

        public UnitCategoryRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<UnitCategory> GetCategories()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<UnitCategory>("SELECT * FROM UnitGroups ORDER BY UnitGroupName").ToList();
            }
        }

        public UnitCategory GetCategory(int id)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<UnitCategory>("SELECT * FROM UnitGroups WHERE UnitGroupId = @id", new { id = id }).FirstOrDefault();
            }
        }
    }
}
