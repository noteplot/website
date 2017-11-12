using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NotePlot.Components
{
    public class PanelLogin : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
             if (HttpContext.User.Identity.IsAuthenticated)
            {
                // если идентфицирован выводим панель с названием учетной записи
                return View("LoginNamePanelView", User.Identity.Name);
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
