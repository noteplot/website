using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace NotePlot.Models
{
    [Table("Units", Schema = "dbo")]
    public class Unit
    {
        [Column("UnitID")]
        public int Id { get; set; }
        [Column("UnitShortName")]
        public string ShortName { get; set; }
        [Column("UnitName")]
        public string Name { get; set; }
        public string UnitGroupShortName { get; set; }
        public string UnitGroupName { get; set; }
        public long LoginID { get; set; }
        //public UnitCategory Category { get; set; }
    }

    public interface IRepositoryParameterUnit
    {
        List<Unit> GetParameterUnits(long lgId);
        //ParameterGroup GetParameterUnit(long pgId, long lgId);
        //bool SetParameterUnit(ParameterGroup pg, int md);
        //bool DelParameterGroup(long pgId);
    }

    public class RepositoryParameterUnit : IRepositoryParameterUnit
    {
        string connectionString = null;
        public RepositoryParameterUnit(string conn)
        {
            connectionString = conn;
        }

        public List<Unit> GetParameterUnits(long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Unit>("dbo.ParameterUnitGet", new { LoginID = lgId }, commandType: CommandType.StoredProcedure).ToList();            }
        }
    }

 }
