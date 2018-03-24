using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Controllers;
using Microsoft.AspNetCore.Http;

namespace NotePlot.Components
{
    public class PanelLogin : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
             if (HttpContext.User.Identity.IsAuthenticated)
            {
                // если идентфицирован выводим панель с названием учетной записи
                var loginView = LoginController.GetLoginView(HttpContext.User);
                return View("LoginNamePanelView", loginView?? User.Identity.Name);
            }
            else
                // если нет, панель для регистрации
                return View("LoginInputPanelView");
        }
    }
}
/*
при вызове метода View в компоненте(имя представления явным образом не указано):
Views/Название_Контроллера/Components/Название_Компонента/Название_Представления.cshtml
Views/Shared/Components/Название_Компонента/Название_Представления.cshtml
*/
