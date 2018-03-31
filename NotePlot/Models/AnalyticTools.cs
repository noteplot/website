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
    public class ReportParam
    {
        public long ParamID { get; set; }
        public long MonitorID { get; set; }
    }

    public class ReportParamPick : ReportParam
    {
        public string ParamShortName { get; set; }
        public string MonitorShortName { get; set; }
        public string UnitShortName { get; set; }
        public string ParamValueTypeCode { get; set; }
        public byte ParamValueScale { get; set; }
        public long LoginID { get; set; }
        public bool Active { get; set; }
    }

    public class ReportJsonArray
    {
        public int ID { get; set; }
        public string JsonArray { get; set; }
    }

    public class AnalyticTools
    {
        string connectionString = null;
        public AnalyticTools(string conn)
        {
            connectionString = conn;
        }

        // отчет по измерениям монитора
        public DataSet GetReportMonitorData(long monitorId, long loginId, DateTime? db = null, DateTime? de = null, short md = 0)
        {
            using (var con = new SqlConnection(connectionString))
            using (var da = new SqlDataAdapter("dbo.ReportMonitorDataGet", con))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.Add("@MonitorID", SqlDbType.BigInt).Value = monitorId;
                da.SelectCommand.Parameters.Add("@LoginID", SqlDbType.BigInt).Value = loginId;
                da.SelectCommand.Parameters.Add("@DateBegin", SqlDbType.DateTime).Value = db;
                da.SelectCommand.Parameters.Add("@DateEnd", SqlDbType.DateTime).Value = de;
                da.SelectCommand.Parameters.Add("@Mode", SqlDbType.SmallInt).Value = md;
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }

        public Task<DataSet>  GetReportMonitorDataAsync(long monitorId, long loginId, DateTime? db = null, DateTime? de = null, short md = 0)
        {
            return Task.Run(() => GetReportMonitorData(monitorId, loginId, db , de , md));
        }

        // для аналитики (plot)
        // Выбор
        public List<ReportParamPick> GetAnalyticMonitorParams(long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<ReportParamPick>("dbo.ReportMonitorParamsGet", new { LoginID = lgId }, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public Task<List<ReportParamPick>> GetAnalyticMonitorParamsAsync(long lgId)
        {
            return Task.Run(() => GetAnalyticMonitorParams(lgId));
        }

        // отчет по измерениям монитора - графики
        public DataSet GetReportPlotData(long loginId, string MonitorParamsXML, DateTime? db = null, DateTime? de = null)
        {
            using (var con = new SqlConnection(connectionString))
            using (var da = new SqlDataAdapter("dbo.ReportMonitorParamsPlotGet", con))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.Add("@MonitorParams", SqlDbType.Xml).Value = MonitorParamsXML;
                da.SelectCommand.Parameters.Add("@DateBegin", SqlDbType.DateTime).Value = db;
                da.SelectCommand.Parameters.Add("@DateEnd", SqlDbType.DateTime).Value = de;
                da.SelectCommand.Parameters.Add("@LoginID", SqlDbType.BigInt).Value = loginId;
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }
        public Task<DataSet> GetReportPlotDataAsync(long loginId, string MonitorParamsXML, DateTime? db = null, DateTime? de = null)
        {
            return Task.Run(() => GetReportPlotData(loginId, MonitorParamsXML, db, de));
        }

        // отчет по измерениям монитора - графики
        public List<ReportJsonArray> GetReportJsonData(long loginId, string MonitorParamsXML, DateTime? db = null, DateTime? de = null)
        {
            using (IDbConnection dbс = new SqlConnection(connectionString))
            {
                return dbс.Query<ReportJsonArray>("dbo.ReportMonitorParamsPlotGet", new {
                    MonitorParams = MonitorParamsXML,
                    DateBegin = db,
                    @DateEnd = de,
                    LoginID = loginId,
                    ReportMode = 0 // по-умолчанию - массив json для Flot
                }, commandType: CommandType.StoredProcedure).ToList();
            }
            /*
            using (var con = new SqlConnection(connectionString))
            using (var da = new SqlDataAdapter("dbo.ReportMonitorParamsPlotGet", con))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.Add("@MonitorParams", SqlDbType.Xml).Value = MonitorParamsXML;
                da.SelectCommand.Parameters.Add("@DateBegin", SqlDbType.DateTime).Value = db;
                da.SelectCommand.Parameters.Add("@DateEnd", SqlDbType.DateTime).Value = de;
                da.SelectCommand.Parameters.Add("@LoginID", SqlDbType.BigInt).Value = loginId;
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            */
        }
        public Task<List<ReportJsonArray>> GetReportJsonDataAsync(long loginId, string MonitorParamsXML, DateTime? db = null, DateTime? de = null)
        {
            return Task.Run(() => GetReportJsonData(loginId, MonitorParamsXML, db, de));
        }

    }
}
