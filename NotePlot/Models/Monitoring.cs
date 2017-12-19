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
    public class Monitoring
    {
        public long? MonitoringID { get; set; }
        public long MonitorID { get; set; }
        public DateTime MonitoringDate { get; set; }
        public string MonitoringComment { get; set; }
        public string MonitorName { get; }
        public DateTime CreationDate { get; }
        public DateTime ModifiedDate { get; }
        public string JSON { get; set; } // список параметров
    }

    public class MonitoringParam
    {
        public long MonitoringID { get; set; }
        public long MonitoringParamID { get; set; }
        public long ParamID { get; set; }
        public decimal? ParamValue { get; set; }
        public DateTime CreationDate { get; }
        public DateTime ModifiedDate { get; }
    }

    public class MonitoringFilter
    {
        public long MonitorID { get; set; }
        public string MonitorShortName { get; set; }
        public string MonitortName { get; set; }
        public byte Mode { get; set; } = 0; // режим фильтра  0 - последние записи 1-период
        public int? Tops { get; set; } = 10;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }

    public interface IRepositoryMonitoring
    {
        //List<Monitoring> GetMonitorings(long mId);
        MonitoringFilter GetMonitoringFilter(long mId, long lgId);
        List<Monitoring> GetMonitorings(long mId, int tops);
        List<Monitoring> GetMonitorings(long mId, MonitoringFilter mf);

        //List<MonitorParameter> GetMonitorParameters(long? mId);
        //Monitor GetMonitor(long mId, long lgId);
        //bool SetMonitor(Monitor mt, int md);
        //bool DeleteMonitor(long mId, long lgId);
    }

    public class RepositoryMonitoring : IRepositoryMonitoring
    {
        string connectionString = null;
        public RepositoryMonitoring(string conn)
        {
            connectionString = conn;
        }

        //фильтр монитора по-умолчанию
        public MonitoringFilter GetMonitoringFilter(long mId, long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<MonitoringFilter>("dbo.MonitoringFilterGet", new { MonitorID = mId, LoginID = lgId}, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        // выборка по-умолчанию
        public List<Monitoring> GetMonitorings(long mId, int tops)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Monitoring>("dbo.MonitoringsGet", new { MonitorID = mId, Tops = tops }, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public List<Monitoring> GetMonitorings(long mId, MonitoringFilter mf)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Monitoring>("dbo.MonitoringsGet", new { MonitorID = mId, Tops = mf.Tops, DateFrom = mf.DateFrom, DateTo = mf.DateTo}, commandType: CommandType.StoredProcedure).ToList();
            }
        }
    }

}
