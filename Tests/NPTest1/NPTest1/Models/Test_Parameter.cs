using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ДЛЯ АТРИБУТОВ!!!


namespace NPTest1.Models
{
    [Table("Test_Parameters", Schema = "dbo")]
    public class Test_Parameter
    {
        [Key]
        public int? ParameterID { get; set; }
        public string ParameterName { get; set; }
		public decimal ParameterValue { get; set; }
        
        // для отладки
		public int ParameterPrecision { get; set; }
		public int ParameterScale { get; set; }
        public bool IsNegative { get; set; }
    }

    [Table("Test_Parameters", Schema = "dbo")]
    public class Parameter
    {
        [Key]
        public int? ParameterID { get; set; }
        public string ParameterName { get; set; }
        public decimal ParameterValue { get; set; }
    }

}
