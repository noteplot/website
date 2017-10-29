using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace NotePlot.Models
{
    public class ParameterType
    {
        public int ParameterTypeID { get; set; }
        public string ParameterTypeName { get; set; }
        public static IEnumerable<ParameterType> ParameterTypeList
        {
            get { return GetParameterTypeList(); }
        }
        private static IEnumerable<ParameterType> GetParameterTypeList()
        {
            return new List<ParameterType> // это почти константы, поэтому без обращения к БД
            {
                new ParameterType { ParameterTypeID = 0, ParameterTypeName = "Простой" },
                new ParameterType { ParameterTypeID = 1, ParameterTypeName = "Вычисляемый" },
                new ParameterType { ParameterTypeID = 2, ParameterTypeName = "Итоговый" }
            };
        }
    }

    public class Parameter
    {
        public long? ParameterID { get; set; }

        [Required(ErrorMessage = "Группа должна быть установлена")]
        public long? ParameterGroupID { get; set; }
        public string ParameterGroupShortName { get; set; }

        [Required(ErrorMessage = "Название параметра должно быть установлено")]
        public string ParameterShortName { get; set; }
        [Required(ErrorMessage = "Название параметра должно быть установлено")]
        public string ParameterName { get; set; }
        [Required(ErrorMessage = "Ед.изм. должна быть установлена")]
        public long? ParameterUnitID { get; set; }
        public string ParameterUnitShortName { get; set; }
        [Required(ErrorMessage = "Тип параметра должен быть установлен")]
        public byte? ParameterTypeID { get; set; }
        public string ParameterTypeName { get; set; }
        [Required(ErrorMessage = "Тип значение параметра должен быть установлен")]
        public int? ParameterValueTypeID { get; set; }
        public string ParameterValueTypeShortName { get; set; }

        public decimal? ParameterValueMax { get; set; }
        public decimal? ParameterValueMin { get; set; }
        public bool Active { get; set; }
        [Required(ErrorMessage = "Логин не определен")] // исключить из проверки?
        public long? LoginID { get; set; }

        public string JSON { get; set; }
    }

    public class ParameterRelation
    {
        public long ParameterID { get; set; }
        public string ParameterShortName { get; set; }
        public int  MathOperationID { get; set; }
        public string MathOperationShortName { get; set; }
        public string MathOperationName { get; set; }
        public string MathOperationFullName { get; set; }
    }

    public class MathOperation
    {
        public int MathOperationID { get; set; }
        public string MathOperationShortName { get; set; }
        public string MathOperationName { get; set; }
        public string MathOperationFullName { get; set; }
    }

    public class Packet
    {
        public long? PacketID { get; set; }
        [Required(ErrorMessage = "Группа должна быть установлена")]
        public long? ParameterGroupID { get; set; }
        public string ParameterGroupShortName { get; set; }
        [Required(ErrorMessage = "Краткое название пакета должно быть установлено")]
        public string PacketShortName { get; set; }
        [Required(ErrorMessage = "Название пакета должно быть установлено")]
        public string PacketName { get; set; }
        [Required(ErrorMessage = "Логин не определен")]
        public long? LoginID { get; set; }
        public bool Active { get; set; }
        public string JSON { get; set; } // список параметров
    }

    public interface IRepositoryParameter
    {
        List<Parameter> GetParameters(long lgId);
        Parameter GetParameter(long prId, long lgId);
        bool SetParameter(Parameter pr, int md);
        bool DeleteParameter(long prId, long lgId);
        List<ParameterRelation> GetRelationParameters(long pId);
        List<MathOperation> GetMathOperations();
        List<Packet> GetPackets(long lgId);
        bool SetPacket(Packet pt, int md);
        //bool DelParameterGroup(long pgId);
    }

    public class RepositoryParameter : IRepositoryParameter
    {
        string connectionString = null;
        public RepositoryParameter(string conn)
        {
            connectionString = conn;
        }

        public List<Parameter> GetParameters(long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Parameter>("dbo.ParameterGet", new { LoginID = lgId }, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public Parameter GetParameter(long prId, long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Parameter>("dbo.ParameterGet", new { ParameterID = prId, LoginID = lgId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public bool SetParameter(Parameter pr, int md)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //ParameterGroup gr =  db.Query<ParameterGroup>("dbo.ParameterGroupCreate", commandType: CommandType.StoredProcedure).FirstOrDefault();
                //return gr;
                try
                {
                    db.Execute("dbo.ParameterSet",
                        new { ParameterID = pr.ParameterID, ParamShortName = pr.ParameterShortName, ParamName = pr.ParameterName,
                            ParamUnitID = pr.ParameterUnitID,ParamValueTypeID = pr.ParameterValueTypeID,
                            ParamTypeID = pr.ParameterTypeID,ParameterGroupID = pr.ParameterGroupID,
                            ParamValueMAX = pr.ParameterValueMax,ParamValueMIN = pr.ParameterValueMin,
                            LoginID = pr.LoginID, Active = pr.Active, Mode = md, JSON = pr.JSON },
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

        public bool DeleteParameter(long prId, long lgId)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //ParameterGroup gr =  db.Query<ParameterGroup>("dbo.ParameterGroupCreate", commandType: CommandType.StoredProcedure).FirstOrDefault();
                //return gr;
                try
                {
                    db.Execute("dbo.ParameterDelete",
                        new
                        {
                            ParameterID = prId,
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

        public List<ParameterRelation> GetRelationParameters(long pId)
        {
            if (pId > 0)
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    return db.Query<ParameterRelation>("dbo.ParameterRelationsGet", new { PrimaryParamID = pId }, commandType: CommandType.StoredProcedure).ToList();
                }
            else
            {
                return new List<ParameterRelation>();
            }
                
        }

        public List<MathOperation> GetMathOperations()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<MathOperation>("dbo.MathOperationsGet", commandType: CommandType.StoredProcedure).ToList();
            }
        }

        // пакеты    
        public List<Packet> GetPackets(long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Packet>("dbo.PacketGet", new { LoginID = lgId }, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public bool SetPacket(Packet pt, int md)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //ParameterGroup gr =  db.Query<ParameterGroup>("dbo.ParameterGroupCreate", commandType: CommandType.StoredProcedure).FirstOrDefault();
                //return gr;
                try
                {
                    db.Execute("dbo.PacketSet",
                        new
                        {
                            PacketID        = pt.PacketID,
                            PacketShortName = pt.PacketShortName,
                            PacketName      = pt.PacketName,
                            ParameterGroupID = pt.ParameterGroupID,
                            LoginID         = pt.LoginID,
                            Active          = pt.Active,
                            Mode            = md,
                            JSON            = pt.JSON
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

        public bool DeletePacket(long prId, long lgId)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //ParameterGroup gr =  db.Query<ParameterGroup>("dbo.ParameterGroupCreate", commandType: CommandType.StoredProcedure).FirstOrDefault();
                //return gr;
                try
                {
                    db.Execute("dbo.PacketDelete",
                        new
                        {
                            PacketID = prId,
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

    }
}
