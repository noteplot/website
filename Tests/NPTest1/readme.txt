1.Вместо @Html.Action -> ViewComponents
Класс ViewComponent размещение: /Components/Название класса (PanelLogin)
public class PanelLogin:ViewComponent

представление компонента размещается по-умолчанию:
Views/Shared/Components/<view_component_name>/<view_name>
в нашем случае:
Views/Shared/Components/PanelLogin/LoginInputPanelView.cshtml

вызывается из класса компонента: return View(LoginInputPanelView)