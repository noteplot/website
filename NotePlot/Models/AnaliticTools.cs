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
    public class AnaliticTools
    {
        string connectionString = null;
        public AnaliticTools(string conn)
        {
            connectionString = conn;
        }

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
    }
}
