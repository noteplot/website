using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; // ДЛЯ АТРИБУТОВ!!!
using Dapper;
using System.Data;
using System.Data.SqlClient;

using System.Xml.Serialization; 
using Newtonsoft.Json;
using NotePlot.Tools;

namespace NotePlot.Models
{
    [Table("Monitor", Schema = "dbo")]
    public class Monitor
    {
        public long? MonitorID { get; set; }
        public string MonitorShortName { get; set; }
        public string MonitorName { get; set; }
        public long? LoginID { get; set; }
        public bool Active { get; set; }
        public string JSON { get; set; } // список параметров
    }

    
    public class MonitorParameter
    {
        //public long? MonitorID { get; set; }
        public long ParameterID { get; set; }
        [System.Xml.Serialization.XmlIgnore()] // исключаем из XML-сериализации ParameterTypeID
        public byte? ParameterTypeID { get; set; } 
        public string ParameterTypeName { get; set; }
        public string ParameterShortName { get; set; }
        public string ParameterName { get; set; }
        //public short MonitorParamPosition { get; set; }
        //[XmlElement(IsNullable = false)] - не работает
        public decimal? MonitorParameterValue { get; set; }
        //public long? LoginID { get; set; }
        public bool MonitorParameterActive { get; set; }
        // ф-ция исключения пустых значений для MonitorParameterActive
        public bool ShouldSerializeMonitorParameterValue() 
        {
            return MonitorParameterValue.HasValue;
        }

    }

    public class MonitorTotalParameter : MonitorParameter
    {
        public long? ParameterUnitID { get; set; }
        public string ParameterUnitShortName { get; set; }
        public byte Scale { get; set; }
        public int Precision { get; set; }
    }

    public interface IRepositoryMonitor
    {
        List<Monitor> GetMonitors(long lgId);
        List<MonitorParameter> GetMonitorParameters(long? mId);
        Task<List<MonitorParameter>> GetMonitorParametersAsync(long? mId);
        Task<List<MonitorTotalParameter>> GetMonitorTotalParametersAsync(long? mId);
        Monitor GetMonitor(long mId, long lgId);
        bool SetMonitor(Monitor mt, int md);
        bool DeleteMonitor(long mId, long lgId);
        Task<List<Monitor>> GetMonitorsAsync(long lgId);
        Task<Monitor> GetMonitorAsync (long mId, long lgId);
        Task<bool> SetMonitorAsync(Monitor mt, int md);
        Task<bool> DeleteMonitorAsync(long mId, long lgId);
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

        public Task<List<Monitor>> GetMonitorsAsync(long lgId)
        {
            return Task.Run(() => GetMonitors(lgId));
        }

        public Monitor GetMonitor(long mId, long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Monitor>("dbo.MonitorGet", new { MonitorID = mId,LoginID = lgId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public Task<Monitor> GetMonitorAsync(long mId, long lgId)
        {
            return Task.Run(() => GetMonitor(mId,lgId));
        }

        public List<MonitorParameter> GetMonitorParameters(long? mId)
        {
            if (mId > 0)
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    return db.Query<MonitorParameter>("dbo.MonitorParamsGet", new { MonitorID = mId }, commandType: CommandType.StoredProcedure).ToList();
                }
            else
            {
                return new List<MonitorParameter>();
            }
        }

        public Task<List<MonitorParameter>> GetMonitorParametersAsync(long? mId)
        {
            return Task.Run(() => GetMonitorParameters(mId));
        }

        public List<MonitorTotalParameter> GetMonitorTotalParameters(long? mId)
        {
            if (mId > 0)
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    return db.Query<MonitorTotalParameter>("dbo.MonitorTotalParamsGet", new { MonitorID = mId }, commandType: CommandType.StoredProcedure).ToList();
                }
            else
            {
                return new List<MonitorTotalParameter>();
            }
        }

        public Task<List<MonitorTotalParameter>> GetMonitorTotalParametersAsync(long? mId)
        {
            return Task.Run(() => GetMonitorTotalParameters(mId));
        }

        public bool SetMonitor(Monitor mt, int md)
        {
            bool rt = false;
            string MonitorParameterXML = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //ParameterGroup gr =  db.Query<ParameterGroup>("dbo.ParameterGroupCreate", commandType: CommandType.StoredProcedure).FirstOrDefault();
                //return gr;
                try
                {
                    if (!string.IsNullOrEmpty(mt.JSON))
                    {
                        var strJson = "[" + mt.JSON + "]";// TO DO: добавлять скобки в js
                        // в список объектов
                        List<MonitorParameter> lpr = JsonConvert.DeserializeObject<List<MonitorParameter>>(strJson);
                        // в XML
                        MonitorParameterXML = ToolKit.SerializeToStringXML(lpr, "MonitorParameters");
                    }
                    db.Execute("dbo.MonitorSet",
                        new
                        {
                            MonitorID = mt.MonitorID,
                            MonitorShortName = mt.MonitorShortName,
                            MonitorName = mt.MonitorName,
                            LoginID = mt.LoginID,
                            Active = mt.Active,
                            Mode = md,
                            MonitorParameters = MonitorParameterXML
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

        public Task<bool> SetMonitorAsync(Monitor mt, int md)
        {
            return Task.Run(()=>SetMonitor(mt, md));
        }

        public bool DeleteMonitor(long mId, long lgId)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //ParameterGroup gr =  db.Query<ParameterGroup>("dbo.ParameterGroupCreate", commandType: CommandType.StoredProcedure).FirstOrDefault();
                //return gr;
                try
                {
                    db.Execute("dbo.MonitorDelete",
                        new
                        {
                            MonitorID = mId,
                            LoginID = lgId
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

        public Task<bool> DeleteMonitorAsync(long mId, long lgId)
        {
            return Task.Run(() => DeleteMonitor(mId, lgId));
        }
    }
}
