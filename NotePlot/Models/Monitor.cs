using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; // ДЛЯ АТРИБУТОВ!!!
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace NotePlot.Models
{
    [Table("Monitor", Schema = "dbo")]
    public class Monitor
    {
        public long? MonitorID { get; set; }
        public string MonitorShortName { get; set; }
        public string MonitorName { get; set; }
        public long LoginID { get; set; }
        public bool Active { get; set; }
        public string JSON { get; set; } // список параметров
    }

    public interface IRepositoryMonitor
    {
        List<Monitor> GetMonitors(long lgId);
        //Parameter GetMonitor(long prId, long lgId);
        
    }

    public class RepositoryMonitor : IRepositoryMonitor
    {
        string connectionString = null;
        public RepositoryMonitor(string conn)
        {
            connectionString = conn;
        }

        public List<Monitor> GetMonitors(long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Monitor>("dbo.MonitorGet", new { LoginID = lgId }, commandType: CommandType.StoredProcedure).ToList();
            }
        }
    }
    }
