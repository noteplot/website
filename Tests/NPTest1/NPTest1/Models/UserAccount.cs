using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ДЛЯ АТРИБУТОВ!!!
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace NPTest1.Models
{
    // вспомогательные классы
    public class LoginViewModel
    {
        [Required]
        //[Display(Name = "Имя")]
        public string LoginName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[Display(Name = "Пароль")]
        public string Password { get; set; }

        //[Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
    }

    [Table("Logins", Schema = "dbo")]
    public class UserAccount
    {
        public long     loginId;
        public byte     loginRoleId;
        public string   loginRoleName;
        public string   loginNickName;
        public string   loginName;
        public string   loginView;
        public string   loginEmail;
        public string   loginPassword;
        public bool     isConfirmed;
        public bool     ShowName;
    }

    // класс и таблица dbo.Login(s)
    public interface IRepositoryLogin
    {
        UserAccount LogIn(string logName, string passw);
        //void GetLogin(string email, string password);
        //void CreateLogin(string email, string password);
        //void Delete(int id);
        //List<UserAccount> GetParameters();
        //void Update(UserAccount par);
        //UserAccount Get(int id);
    }

    public class RepositoryLogin : IRepositoryLogin
    {
        string connectionString = null;
        public RepositoryLogin(string conn)
        {
            connectionString = conn;
        }

        public UserAccount LogIn(string logName, string passw)
        {
            UserAccount us = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                us = db.Query<UserAccount>("SELECT * FROM dbo.Logins WHERE LoginName = @logName and password = @passw", new { logName, passw }).FirstOrDefault();
            }
            return us;
        }

    }
}
 