1.Вместо @Html.Action -> ViewComponents очередной финт и тонкое место Microsoft. 
Фактически  - класс это контроллер для встраиваемого представления.

Класс ViewComponent размещение: /Components/Название класса (PanelLogin)
public class PanelLogin:ViewComponent

представление компонента размещается по-умолчанию:
Views/Shared/Components/<view_component_name>/<view_name>
в нашем случае:
Views/Shared/Components/PanelLogin/LoginInputPanelView.cshtml

вызывается из класса компонента: return View(LoginInputPanelView)

2.Вызов View :
"~Views/LogIn/LoginView.cshtml" - так НЕ работает
 "Views/LogIn/LoginView.cshtml" -так работает, т.е. без "~"

3.
AntiForgeryToken 
нужно указать и в атрибуте контроллера [ValidateAntiForgeryToken] и в форме  View,
 @Html.AntiForgeryToken() , обязательно в двух местах сразу, без этого не работает

4.Для CheckBox или использовать хелпер @Html.CheckBox("RememberMe", false)
или перед POST устанваливать checked принудительно: 
$("input:checkbox").each(function () { // this - это не объект jQuery , а html-element
                this.value = (this.checked == true);
});