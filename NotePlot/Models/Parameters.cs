using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Dapper;
using System.Data;
using System.Data.SqlClient;

using Newtonsoft.Json;
using NotePlot.Tools;

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

    public class PacketParameter
    {
        public long? PacketID { get; set; }
        public long ParameterID { get; set; }
        public string ParameterShortName { get; set; }
        public long? LoginID { get; set; }
        public bool PacketParameterActive { get; set; }
    }

    //===========================================================
    public interface IRepositoryParameter
    {
        List<Parameter> GetParameters(long lgId, short md = 0);
        Task<List<Parameter>> GetParametersAsync(long lgId, short md = 0);
        Parameter GetParameter(long prId, long lgId);
        Task<Parameter> GetParameterAsync(long prId, long lgId);
        bool SetParameter(Parameter pr, int md);
        Task<bool> SetParameterAsync(Parameter pr, int md);
        bool DeleteParameter(long prId, long lgId);
        Task<bool> DeleteParameterAsync(long prId, long lgId);
        List<ParameterRelation> GetRelationParameters(long pId);
        Task<List<ParameterRelation>> GetRelationParametersAsync(long pId);
        List<MathOperation> GetMathOperations();
        Task<List<MathOperation>> GetMathOperationsAsync();
        List<Packet> GetPackets(long lgId);
        Task<List<Packet>> GetPacketsAsync(long lgId);
        Packet GetPacket(long ptId, long lgId);
        Task<Packet> GetPacketAsync(long ptId, long lgId);
        bool SetPacket(Packet pt, int md);
        Task<bool> SetPacketAsync(Packet pt, int md);
        List<PacketParameter> GetPacketParameters(long? pId);
        Task<List<PacketParameter>> GetPacketParametersAsync(long? pId);
        bool DeletePacket(long prId, long lgId);
        Task<bool> DeletePacketAsync(long prId, long lgId);
    }

    public class RepositoryParameter : IRepositoryParameter
    {
        string connectionString = null;
        public RepositoryParameter(string conn)
        {
            connectionString = conn;
        }

        public List<Parameter> GetParameters(long lgId, short md = 0)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Parameter>("dbo.ParameterGet", new { LoginID = lgId, Mode = md }, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public Task<List<Parameter>> GetParametersAsync(long lgId, short md = 0)
        {
            return Task.Run(()=> GetParameters(lgId, md));
        }

        public Parameter GetParameter(long prId, long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Parameter>("dbo.ParameterGet", new { ParameterID = prId, LoginID = lgId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public Task<Parameter> GetParameterAsync(long prId, long lgId)
        {
            return Task.Run(()=> GetParameter(prId, lgId));
        }

        public bool SetParameter(Parameter pr, int md)
        {
            bool rt = false;
            string ParameterRelationXML = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {                
                try
                {
                    if (!string.IsNullOrEmpty(pr.JSON))
                    {
                        var strJson = "[" + pr.JSON + "]";// TO DO: добавлять скобки в js
                        // в список объектов
                        List<ParameterRelation> lpr = JsonConvert.DeserializeObject< List<ParameterRelation>>(strJson);
                        // в XML
                        ParameterRelationXML = ToolKit.SerializeToStringXML(lpr, "ParameterRelations");
                    }
                    
                    db.Execute("dbo.ParameterSet",
                        new { ParameterID = pr.ParameterID, ParamShortName = pr.ParameterShortName, ParamName = pr.ParameterName,
                            ParamUnitID = pr.ParameterUnitID,ParamValueTypeID = pr.ParameterValueTypeID,
                            ParamTypeID = pr.ParameterTypeID,ParameterGroupID = pr.ParameterGroupID,
                            ParamValueMAX = pr.ParameterValueMax,ParamValueMIN = pr.ParameterValueMin,
                            LoginID = pr.LoginID, Active = pr.Active, Mode = md,ParameterRelations = ParameterRelationXML
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

        public Task<bool> SetParameterAsync(Parameter pr, int md)
        {
            return Task.Run(() => SetParameter(pr,md));
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

        public Task<bool> DeleteParameterAsync(long prId, long lgId)
        {
            return Task.Run(()=> DeleteParameter(prId, lgId));
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

        public Task<List<ParameterRelation>> GetRelationParametersAsync(long pId)
        {
            return Task.Run(()=> GetRelationParameters(pId));
        }

        public List<MathOperation> GetMathOperations()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<MathOperation>("dbo.MathOperationsGet", commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public Task<List<MathOperation>> GetMathOperationsAsync()
        {
            return Task.Run(()=> GetMathOperations());
        }

        // пакеты    
        public List<Packet> GetPackets(long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Packet>("dbo.PacketGet", new { LoginID = lgId }, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public Task<List<Packet>> GetPacketsAsync(long lgId)
        {
            return Task.Run(()=> GetPackets(lgId));
        }

        public Packet GetPacket(long ptId, long lgId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Packet>("dbo.PacketGet", new { PacketID = ptId, LoginID = lgId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public Task<Packet> GetPacketAsync(long ptId, long lgId)
        {
            return Task.Run(()=> GetPacket(ptId, lgId));
        }

        public bool SetPacket(Packet pt, int md)
        {
            bool rt = false;
            string PacketParameterXML = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                //ParameterGroup gr =  db.Query<ParameterGroup>("dbo.ParameterGroupCreate", commandType: CommandType.StoredProcedure).FirstOrDefault();
                //return gr;                
                try
                {
                    if (!string.IsNullOrEmpty(pt.JSON))
                    {
                        var strJson = "[" + pt.JSON + "]";// TO DO: добавлять скобки в js
                        // в список объектов
                        List<PacketParameter> lpr = JsonConvert.DeserializeObject<List<PacketParameter>>(strJson);
                        // в XML
                        PacketParameterXML = ToolKit.SerializeToStringXML(lpr, "PacketParameters");
                    }

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
                            PacketParameters = PacketParameterXML
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

        public Task<bool> SetPacketAsync(Packet pt, int md)
        {
            return Task.Run(()=> SetPacket(pt, md));
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

        public Task<bool> DeletePacketAsync(long prId, long lgId)
        {
            return Task.Run(()=> DeletePacket(prId, lgId));
        }

        public List<PacketParameter> GetPacketParameters(long? pId)
        {
            if (pId > 0)
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    return db.Query<PacketParameter>("dbo.PacketParamsGet", new { PacketID = pId }, commandType: CommandType.StoredProcedure).ToList();
                }
            else
            {
                return new List<PacketParameter>();
            }
        }

        public Task<List<PacketParameter>> GetPacketParametersAsync(long? pId)
        {
            return Task.Run(()=> GetPacketParameters(pId));        
        }

    }
}
