1.������ @Html.Action -> ViewComponents ��������� ���� � ������ ����� Microsoft. 
����������  - ����� ��� ���������� ��� ������������� �������������.

����� ViewComponent ����������: /Components/�������� ������ (PanelLogin)
public class PanelLogin:ViewComponent

������������� ���������� ����������� ��-���������:
Views/Shared/Components/<view_component_name>/<view_name>
� ����� ������:
Views/Shared/Components/PanelLogin/LoginInputPanelView.cshtml

���������� �� ������ ����������: return View(LoginInputPanelView)

2.����� View :
"~Views/LogIn/LoginView.cshtml" - ��� �� ��������
 "Views/LogIn/LoginView.cshtml" -��� ��������, �.�. ��� "~"