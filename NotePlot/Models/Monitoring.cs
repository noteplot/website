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
        public string MonitoringDateDt { get; set; }
        public string MonitoringDateTm { get; set; }
        public string MonitoringComment { get; set; }
        public string MonitorShortName { get; set; }
        public DateTime? CreationDateUTC { get; }
        public DateTime? ModifiedDateUTC { get; }
        public string JSON { get; set; } // список параметров
    }

    public class MonitoringParameter
    {
        public long MonitoringID { get; set; }
        public long MonitoringParamID { get; set; }
        public byte ParameterTypeID { get; set; }
        public long ParameterID { get; set; }
        public decimal? ParameterValue { get; set; }
        public string ParameterShortName { get; set; }
        public string ParameterName { get; set; }
        public long ParameterUnitID { get; set; }
        public string ParameterUnitShortName { get; set; }
        public byte ParameterScale { get; set; }
        public byte ParameterPrecision { get; set; }
        public decimal? ParameterValueMax { get; set; }
        public decimal? ParameterValueMin { get; set; }
        public DateTime CreationDateUTC { get; }
        public DateTime ModifiedDateUTC { get; }
    }

    public class MonitoringFilter
    {
        public long MonitorID { get; set; }
        public string MonitorShortName { get; set; }
        public string MonitortName { get; set; }
        public byte Mode { get; set; } = 0; // режим фильтра  0 - последние записи 1-период
        public int  Tops { get; set; } = 10;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }

    public interface IRepositoryMonitoring
    {
        //List<Monitoring> GetMonitorings(long mId);
        MonitoringFilter GetMonitoringFilter(long mId, long lgId);
        List<Monitoring> GetMonitorings(long mId, int tops);
        List<Monitoring> GetMonitorings(MonitoringFilter mf);

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

        public string GetConnection()
        {
            return connectionString;
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

        public List<Monitoring> GetMonitorings(MonitoringFilter mf)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Monitoring>("dbo.MonitoringsGet", new { MonitorID = mf.MonitorID, Tops = mf.Tops, DateFrom = mf.DateFrom, DateTo = mf.DateTo, Mode = mf.Mode}, commandType: CommandType.StoredProcedure).ToList();
            }
        }
    }

}
