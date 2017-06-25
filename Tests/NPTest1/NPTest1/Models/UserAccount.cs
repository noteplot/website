using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ДЛЯ АТРИБУТОВ!!!

namespace NPTest1.Models
{
    [Table("Logins", Schema = "dbo")]
    public class UserAccount
    {
        public long     loginId;
        public string   loginName;
        public string   loginEmail;
        public string   loginNickName;
        public string   loginPassword;
        public byte     loginRoleId;
        public string   loginView;
        public bool     isConfirmed;
        public bool     ShowNick;
    }
}
 