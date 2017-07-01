using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NPTest1.Components
{
    public class PanelLogin:ViewComponent
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
