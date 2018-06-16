using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ДЛЯ АТРИБУТОВ!!!
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace NotePlot.Models
{
    // класс для ввода логина
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

    // класс для регистрации
    public class LoginRegistration
    {
        //[Display(Name = "Имя")]
        //public string LoginName { get; set; }

        //[Display(Name = "e-mail")]
        public string Email { get; set; }

        //[Display(Name = "Забыли пароль?")]
        //public bool ForgetPassword { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "Password")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Введите число с картинки")]
        public string Captcha { get; set; }
    }



    // Учетная запись
    [Table("Logins", Schema = "dbo")]
    public class UserAccount
    {
        public long LoginID { get; set; }
        public byte LoginRoleID { get; set; }
        public string LoginRoleName { get; set; }
        public string LoginName { get; set; }    // имя входа == email при регистрации
        public string ScreenName { get; set; }   // псевдоним   
        public string LoginView { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsConfirmed { get; set; }
        public bool ShowScreenName { get; set; }
    }

    // класс и таблица dbo.Login(s)
    public interface IRepositoryLogin
    {
        UserAccount LogIn(string logName, string passw);
        Task<UserAccount> LogInAsync(string logName, string passw);
        //UserAccount CreateLogin(string email, string passw);
        //Task<UserAccount> CreateLoginAsync(string email, string passw);
        long CreateLogin(string email, string passw);
        Task<long> CreateLoginAsync(string email, string passw);
        bool ConfirmLogin(long lgId);
        Task<bool> ConfirmLoginAsync(long lgId);
        UserAccount GetLogin(string logName);
        Task<UserAccount> GetLoginAsync(string logName);
        UserAccount GetUserAccount(long loginId);
        Task<UserAccount> GetUserAccountAsync(long loginId);
        bool SetUserAccount(UserAccount ua);
        Task<bool> SetUserAccountAsync(UserAccount ua);
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
            using (IDbConnection db = new SqlConnection(connectionString)) // TO DO: заменить процедурой
            {
                us = db.Query<UserAccount>("SELECT * FROM dbo.Logins WHERE LoginName = @logName and password = @passw", new { logName, passw }).FirstOrDefault();
                //us = db.Query<UserAccount>("dbo.LoginGet", new { LoginName = logName, Password = passw }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return us;
        }

        public Task<UserAccount> LogInAsync(string logName, string passw)
        {
            return Task.Run(() => LogIn(logName, passw));
        }

        public long CreateLogin(string email, string passw)
        {
            long rt = -1;
            var p = new DynamicParameters();
            p.Add("@LoginName", email);
            p.Add("@Password", passw);
            p.Add("@LoginID", dbType: DbType.Int64, direction: ParameterDirection.Output);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    db.Execute("dbo.LoginCreate", p, commandType: CommandType.StoredProcedure);
                    rt = p.Get<long>("@LoginID");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return rt;
        }

        public Task<long> CreateLoginAsync(string email, string passw)
        {
            return Task.Run(() => CreateLogin(email, passw));
        }

        public bool ConfirmLogin(long lgId)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    var affectedRows = db.Execute("dbo.LoginConfirm", new { LoginID = lgId }, commandType: CommandType.StoredProcedure);
                    //if (affectedRows > 0)
                    rt = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return rt;
        }

        public Task<bool> ConfirmLoginAsync(long lgId)
        {
            return Task.Run(() => ConfirmLogin(lgId));
        }

        public UserAccount GetLogin(string logName)
        {
            UserAccount us = null;
            using (IDbConnection db = new SqlConnection(connectionString)) // TO DO: заменить процедурой
            {
                us = db.Query<UserAccount>("SELECT * FROM dbo.Logins WHERE LoginName = @logName", new { logName }).FirstOrDefault();
                //us = db.Query<UserAccount>("dbo.LoginGet", new { LoginName = logName, Password = passw }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return us;
        }

        public Task<UserAccount> GetLoginAsync(string logName)
        {
            return Task.Run(() => GetLogin(logName));
        }

        public UserAccount GetUserAccount(long loginId)
        {
            UserAccount us = null;
            using (IDbConnection db = new SqlConnection(connectionString)) // TO DO: заменить процедурой
            {
                us = db.Query<UserAccount>("dbo.UserAccountGet", new { LoginID = loginId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return us;
        }

        public Task<UserAccount> GetUserAccountAsync(long loginId)
        {
            return Task.Run(() => GetUserAccount(loginId));
        }

        public bool SetUserAccount(UserAccount ua)
        {
            bool rt = false;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    var p = new DynamicParameters();
                    p.Add("@LoginID",ua.LoginID);
                    p.Add("@Password", ua.Password);
                    p.Add("@ScreenName", ua.ScreenName);
                    p.Add("@ShowScreenName", ua.ShowScreenName);
                    p.Add("@LoginView", ua.LoginView, dbType: DbType.String, size:128, direction: ParameterDirection.InputOutput);
                    db.Execute("dbo.LoginSet",
                        p,
                        //new { UnitID = ut.UnitID, UnitShortName = ut.UnitShortName, UnitName = ut.UnitName, UnitGroupID = ut.UnitGroupID, LoginID = ut.LoginID, Mode = md },
                        commandType: CommandType.StoredProcedure);
                        ua.LoginView = p.Get<string>("@LoginView");
                    rt = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                /*
                try
                {
                    var affectedRows = db.Execute("dbo.LoginSet", 
                        new { LoginID = ua.LoginID,Password = ua.Password,ScreenName = ua.ScreenName,ShowScreenName = ua.ShowScreenName}, 
                        commandType: CommandType.StoredProcedure);
                    //if (affectedRows > 0)
                    rt = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                */
            }
            return rt;
        }

        public Task<bool> SetUserAccountAsync(UserAccount ua)
        {
            return Task.Run(() => SetUserAccount(ua));
        }

    }

}
