using Xunit;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class EmployeeTests
{
    private string testFilePath = "test_employees.json";

    public EmployeeTests()
    {
        // Создаем тестовый файл с данными
        var employees = new List<Employee>
        {
            new Employee { Id = 1, FirstName = "John", LastName = "Doe", SalaryPerHour = 100.50m }
        };

        var json = JsonConvert.SerializeObject(employees, Formatting.Indented);
        File.WriteAllText(testFilePath, json);
    }

    ~EmployeeTests()
    {
        // Удаляем тестовый файл после тестов
        if (File.Exists(testFilePath))
        {
            File.Delete(testFilePath);
        }
    }

    [Fact]
    public void AddEmployee_ValidData_AddsEmployee()
    {
        // Перенаправляем вывод консоли для проверки
        using (var sw = new StringWriter())
        {
            Console.SetOut(sw);

            // Временно изменяем путь к файлу для теста
            var originalFilePath = Program.filePath;
            Program.filePath = testFilePath;

            try
            {
                // Вызываем метод AddEmployee с валидными данными
                Program.AddEmployee(new string[] { "-add", "FirstName:Jane", "LastName:Smith", "Salary:150.75" });

                // Загружаем сотрудников из файла
                var employees = LoadEmployees(testFilePath);

                // Проверяем, что новый сотрудник добавлен
                Assert.Equal(2, employees.Count);
                var newEmployee = employees[1];
                Assert.Equal(2, newEmployee.Id);
                Assert.Equal("Jane", newEmployee.FirstName);
                Assert.Equal("Smith", newEmployee.LastName);
                Assert.Equal(150.75m, newEmployee.SalaryPerHour);
            }
            finally
            {
                // Восстанавливаем оригинальный путь к файлу
                Program.filePath = originalFilePath;
            }
        }
    }

    private List<Employee> LoadEmployees(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<Employee>();
        }

        var json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<List<Employee>>(json) ?? new List<Employee>();
    }
}
