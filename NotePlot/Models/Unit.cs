using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NotePlot.Models
{
    [Table("Units", Schema = "dbo")]
    public class Unit
    {
        [Column("UnitID")]
        public int Id { get; set; }
        [Column("UnitName")]
        public string Name { get; set; }
        [Column("UnitShortName")]
        public string ShortName { get; set; }
        [Column("UnitCode")]
        public string Code { get; set; }

        public UnitCategory Category { get; set; }
    }
}
