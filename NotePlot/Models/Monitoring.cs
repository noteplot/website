using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; // ДЛЯ АТРИБУТОВ!!!
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace NotePlot.Models
{
    public class Monitoring
    {
        public long? MonitoringID { get; set; }
        [Required(ErrorMessage = "Шаблон измерения должен быть указан!")]
        public long MonitorID { get; set; }
        public DateTime MonitoringDate { get; set; }
        [Required(ErrorMessage = "Дата должна быть указана!")]
        public string MonitoringDateDt { get; set; }
        [Required(ErrorMessage = "Время должно быть указано!")]
        public string MonitoringDateTm { get; set; }
        public string MonitoringComment { get; set; }
        public string MonitorShortName { get; set; }
        public DateTime? CreationDateUTC { get; }
        public DateTime? ModifiedDateUTC { get; }
        public string JSON { get; set; } // список параметров
    }

    public class MonitoringParameter
    {
        public long? MonitoringID { get; set; }
        public long? MonitoringParamID { get; set; }
        public long  MonitorParamID { get; set; }
        public int ParameterTypeID { get; set; }
        public long ParameterID { get; set; }
        public decimal? ParameterValue { get; set; }
        public string ParameterShortName { get; set; }
        public string ParameterName { get; set; }
        public long ParameterUnitID { get; set; }
        public string ParameterUnitShortName { get; set; }
        public byte ParameterScale { get; set; }
        public int ParameterPrecision { get; set; }
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
        Task<MonitoringFilter> GetMonitoringFilterAsync(long mId, long lgId);
        List<Monitoring> GetMonitorings(long mId, int tops);
        Task<List<Monitoring>> GetMonitoringsAsync(long mId, int tops);
        List<Monitoring> GetMonitorings(MonitoringFilter mf);
        Task<List<Monitoring>> GetMonitoringsAsync(MonitoringFilter mf);
        List<MonitoringParameter> GetMonitoringParams(long? monitoringId, long monitorId);
        Task<List<MonitoringParameter>> GetMonitoringParamsAsync(long? monitoringId, long monitorId);
        Monitoring GetMonitoring(long mrId);
        Task<Monitoring> GetMonitoringAsync(long mrId);
        bool SetMonitoring(Monitoring mr, int md);
        Task<bool> SetMonitoringAsync(Monitoring mr, int md);
        bool DeleteMonitoring(long mId);
        Task<bool> DeleteMonitoringAsync(long mId);
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

        public Task<MonitoringFilter> GetMonitoringFilterAsync(long mId, long lgId)
        {
            return Task.Run(() => GetMonitoringFilter(mId, lgId));
        }

        // выборка по-умолчанию
        public List<Monitoring> GetMonitorings(long mId, int tops)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Monitoring>("dbo.MonitoringsGet", new { MonitorID = mId, Tops = tops }, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public Task<List<Monitoring>> GetMonitoringsAsync(long mId, int tops)
        {
            return Task.Run(() => GetMonitorings(mId, tops));
        }

        public List<Monitoring> GetMonitorings(MonitoringFilter mf)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Monitoring>("dbo.MonitoringsGet", new { MonitorID = mf.MonitorID, Tops = mf.Tops, DateFrom = mf.DateFrom, DateTo = mf.DateTo, Mode = mf.Mode}, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public Task<List<Monitoring>> GetMonitoringsAsync(MonitoringFilter mf)
        {
            return Task.Run(() => GetMonitorings(mf));
        }

        public List<MonitoringParameter> GetMonitoringParams(long? monitoringId, long monitorId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<MonitoringParameter>("dbo.MonitoringParamsGet", new { MonitoringID = monitoringId, MonitorID = monitorId }, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public Task<List<MonitoringParameter>> GetMonitoringParamsAsync(long? monitoringId, long monitorId)
        {
            return Task.Run(() => GetMonitoringParams(monitoringId, monitorId));
        }

        public Monitoring GetMonitoring(long mrId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Monitoring>("dbo.MonitoringGet", new { MonitoringID = mrId}, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public Task<Monitoring> GetMonitoringAsync(long mrId)
        {
            return Task.Run(() => GetMonitoring(mrId));
        }

        public bool SetMonitoring(Monitoring mr, int md)
        {
            bool rt = false;

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    db.Execute("dbo.MonitoringSet",
                        new
                        {
                            MonitoringID    = mr.@MonitoringID,
                            MonitorID       = mr.MonitorID,
                            MonitoringDate  = mr.MonitoringDate,
                            MonitoringComment = mr.MonitoringComment,
                            JSON = mr.JSON,
                            Mode = md
                        },
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

        public Task<bool> SetMonitoringAsync(Monitoring mr, int md)
        {
            return Task.Run(() => SetMonitoring(mr, md));
        }

        public bool DeleteMonitoring(long mId)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //ParameterGroup gr =  db.Query<ParameterGroup>("dbo.ParameterGroupCreate", commandType: CommandType.StoredProcedure).FirstOrDefault();
                //return gr;
                try
                {
                    db.Execute("dbo.MonitoringDelete",
                        new
                        {
                            MonitoringID = mId
                        },
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

        public Task<bool> DeleteMonitoringAsync(long mId)
        {
            return Task.Run(() => DeleteMonitoring(mId));
        }

    }
}
