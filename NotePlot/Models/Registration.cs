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

        [Display(Name = "e-mail")]
        public string Email { get; set; }

        //[Display(Name = "Забыли пароль?")]
        //public bool ForgetPassword { get; set; }

        //[Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Введите число с картинки")]
        public string Captcha { get; set; }
    }



    // Учетная запись
    [Table("Logins", Schema = "dbo")]
    public class UserAccount
    {
        public long LoginID;
        public byte LoginRoleID;
        public string LoginRoleName;
        public string LoginName;    // имя входа == email при регистрации
        public string ScreenName;   // псевдоним   
        public string LoginView;
        public string Email;
        public string Password;
        public bool IsConfirmed;
        public bool ShowScreenName;
    }

    // класс и таблица dbo.Login(s)
    public interface IRepositoryLogin
    {
        UserAccount LogIn(string logName, string passw);
        Task<UserAccount> LogInAsync(string logName, string passw);
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

        public Task<UserAccount> LogInAsync(string logName, string passw)
        {
            return Task.Run(()=> LogIn(logName, passw));
        }
    }

}
