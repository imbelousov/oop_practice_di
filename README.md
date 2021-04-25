# Shop API

� ���� ������� ��� ����� ����� ������� ����������� Web API ��� �������� ���������, �������� ��������� ���� �������
��������-���������������� ����������������. Web API ������ ��������� ��������� �������� ������ ��������� ��������:
- �������� ������ ���������
- ��������� � ������ ����� ��������
- ��������� ���� � ��������������� ��������
- ������� ������������ ��������
- �������������� ������� � ���� �����
- �������� ���� (��� ��������� � ������ ������ ����������� ���� `docx`, MIME-��� `application/vnd.openxmlformats-officedocument.wordprocessingml.document`)

#### ���������� � API:
- � �������� ���������� ������ �������������� ASP&#46;NET Core
- ������������� [������������ �����������](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures) ����������: ������ � ������� ������ ����������� ������ ������������,
������ ������ ���� ��������� ��������������� ������ ���� ��������, ����������� ������ ���� ������������
� ��������� ������ ��� �������������
- Web API ������ ��������������� ������������� [REST](https://en.wikipedia.org/wiki/Representational_state_transfer), � ������ ������� ��� �������� [���������� �����](https://restfulapi.net/resource-naming/)
� ����������� ������ [HTTP �������](https://restfulapi.net/http-methods/)
- �������� � ��� API [Swagger](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-5.0&tabs=visual-studio)
- ����������� DI ���������

����� ������������ ��� ���������� ������� ���� ��� �������� � ������� `Shop.Core`. ������ ���� ��� ���� ������� ����������
������������.

#### �������������� �������

##### 1. �����������
����� �������� ���������, ���������� � ����� API, ������ �������. ������������ ��������, ��� ����� ������ �������
�������� ������ � ����. ��� �������� ����� ��������� � ���� ����� ���������� ����. � C# ��� ���������� [�����](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.caching.memorycache?view=dotnet-plat-ext-5.0), �����������
��� � ��������� ������ ����������. ���������, � ����� ���� ����� ����� ���� �� ���������� ������ � �����? �������� ���
� ���� ����������. � �� �������� ��� �����������.

##### 2. ����-�����
��������� ����-������ ������������� �������� �������� ��������� ������ � ������� ��� �� ����� ������������.
�������� ��������� ����-������ ��� ������-������ �������. �� ����������� � ��������� ������ - �������������� Mock'��� ������������.
����������� ���� ������� Mock-���������� � ���������� ��� ����-������������, ��������, [Moq](https://github.com/Moq/moq4/wiki/Quickstart) � [NUnit](https://github.com/nunit/nunit-csharp-samples).