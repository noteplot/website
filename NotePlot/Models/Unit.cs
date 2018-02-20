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
        public long? UnitID { get; set; }
        [Column("UnitShortName")]
        public string UnitShortName { get; set; }
        [Column("UnitName")]
        public string UnitName { get; set; }
        public long?  UnitGroupID { get; set; }
        public string UnitGroupShortName { get; set; }
        public string UnitGroupName { get; set; }
        public long LoginID { get; set; }
        //public UnitCategory Category { get; set; }
    }

    public interface IRepositoryParameterUnit
    {
        string GetConnection();
        List<Unit> GetParameterUnits(long lgId);
        Task<List<Unit>> GetParameterUnitsAsync(long lgId);
        Unit GetParameterUnit(long unitID, long lgId);
        Task<Unit> GetParameterUnitAsync(long unitID, long lgId);
        bool SetUnit(Unit ut, int md);
        Task<bool> SetUnitAsync(Unit ut, int md);
    }

    public class RepositoryParameterUnit : IRepositoryParameterUnit
    {
        string connectionString = null;
        public RepositoryParameterUnit(string conn)
        {
            connectionString = conn;
        }

        public string GetConnection()
        {
            return connectionString;
        }

        public List<Unit> GetParameterUnits(long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Unit>("dbo.ParameterUnitGet", new { LoginID = lgId }, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public Task<List<Unit>> GetParameterUnitsAsync(long lgId)
        {
            return Task.Run(() => GetParameterUnits(lgId));
        }

        public Unit GetParameterUnit(long unitID, long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Unit>("dbo.ParameterUnitGet", new { UnitID = unitID,LoginID = lgId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public Task<Unit> GetParameterUnitAsync(long unitID, long lgId)
        {
            return Task.Run(() => GetParameterUnit(unitID, lgId));
        }

        public bool SetUnit(Unit ut, int md)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    var p = new DynamicParameters();
                    p.Add("@UnitID", ut.UnitGroupID, dbType: DbType.Int64, direction: ParameterDirection.InputOutput);
                    p.Add("@UnitShortName", ut.UnitShortName);
                    p.Add("@UnitName", ut.UnitName);
                    p.Add("@UnitGroupID", ut.UnitGroupID);
                    p.Add("@LoginID", ut.LoginID);
                    p.Add("@Mode", md);
                    db.Execute("dbo.ParameterUnitSet",
                        p,
                        //new { UnitID = ut.UnitID, UnitShortName = ut.UnitShortName, UnitName = ut.UnitName, UnitGroupID = ut.UnitGroupID, LoginID = ut.LoginID, Mode = md },
                        commandType: CommandType.StoredProcedure);
                    if (md == 0)
                        ut.UnitID = p.Get<long>("@UnitGroupID");
                    rt = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return rt;
        }

        public Task<bool> SetUnitAsync(Unit ut, int md)
        {
            return Task.Run(() => SetUnit(ut, md));
        }

    }
}
