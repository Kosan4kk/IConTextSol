using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

public class Program
{
    public static string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "employees.json");

    static void Main(string[] args)
    {
        // Основной цикл программы
        while (true)
        {
            // Проверка аргументов командной строки
            if (args.Length == 0)
            {
                Console.WriteLine("Введите команду (или 'exit' для завершения):");
                string input = Console.ReadLine();

                // Прерывание цикла и завершение программы по команде 'exit'
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Команда не введена. Попробуйте еще раз.");
                    continue;
                }

                // Разделяем строку на части, чтобы передать их в виде аргументов
                args = input.Split(' ');
            }

            // Извлечение команды из аргументов
            string command = args[0].ToLower();

            // Выполнение соответствующей команды
            switch (command)
            {
                case "-add":
                    AddEmployee(args);
                    break;
                case "-update":
                    UpdateEmployee(args);
                    break;
                case "-get":
                    GetEmployee(args);
                    break;
                case "-delete":
                    DeleteEmployee(args);
                    break;
                case "-getall":
                    GetAllEmployees();
                    break;
                default:
                    Console.WriteLine("Неизвестная команда.");
                    break;
            }

            // Сброс args для следующей итерации
            args = new string[0];
        }
    }
    public static void AddEmployee(string[] args)
    {
        var newEmployee = new Employee();
        var employees = LoadEmployees();

        // Получаем максимальный Id
        int maxId = employees.Count > 0 ? employees.Max(e => e.Id) : 0;
        newEmployee.Id = maxId + 1;

        foreach (var arg in args.Skip(1))
        {
            var parts = arg.Split(':');
            if (parts.Length == 2)
            {
                var key = parts[0].ToLower();
                var value = parts[1];

                switch (key)
                {
                    case "firstname":
                        newEmployee.FirstName = value;
                        break;
                    case "lastname":
                        newEmployee.LastName = value;
                        break;
                    case "salary":
                        if (decimal.TryParse(value, out decimal salary))
                        {
                            newEmployee.SalaryPerHour = salary;
                        }
                        break;
                }
            }
        }

        employees.Add(newEmployee);
        SaveEmployees(employees);

        Console.WriteLine("Сотрудник добавлен.");
    }

    static void UpdateEmployee(string[] args)
    {
        var employees = LoadEmployees();
        int id = 0;
        if (!int.TryParse(args.FirstOrDefault(a => a.StartsWith("Id:"))?.Split(':')[1], out id))
        {
            Console.WriteLine("Указан неверный Id.");
            return;
        }

        var employee = employees.FirstOrDefault(e => e.Id == id);
        if (employee == null)
        {
            Console.WriteLine("Сотрудник с таким Id не найден.");
            return;
        }

        foreach (var arg in args.Skip(1))
        {
            var parts = arg.Split(':');
            if (parts.Length == 2)
            {
                var key = parts[0].ToLower();
                var value = parts[1];

                switch (key)
                {
                    case "firstname":
                        employee.FirstName = value;
                        break;
                    case "lastname":
                        employee.LastName = value;
                        break;
                    case "salary":
                        if (decimal.TryParse(value, out decimal salary))
                        {
                            employee.SalaryPerHour = salary;
                        }
                        break;
                }
            }
        }

        SaveEmployees(employees);
        Console.WriteLine("Сотрудник обновлен.");
    }

    static void GetEmployee(string[] args)
    {
        var employees = LoadEmployees();
        int id = 0;
        if (!int.TryParse(args.FirstOrDefault(a => a.StartsWith("Id:"))?.Split(':')[1], out id))
        {
            Console.WriteLine("Указан неверный Id.");
            return;
        }

        var employee = employees.FirstOrDefault(e => e.Id == id);
        if (employee == null)
        {
            Console.WriteLine("Сотрудник с таким Id не найден.");
            return;
        }

        Console.WriteLine($"Id = {employee.Id}, FirstName = {employee.FirstName}, LastName = {employee.LastName}, SalaryPerHour = {employee.SalaryPerHour}");
    }

    static void DeleteEmployee(string[] args)
    {
        var employees = LoadEmployees();
        int id = 0;
        if (!int.TryParse(args.FirstOrDefault(a => a.StartsWith("Id:"))?.Split(':')[1], out id))
        {
            Console.WriteLine("Указан неверный Id.");
            return;
        }

        var employee = employees.FirstOrDefault(e => e.Id == id);
        if (employee == null)
        {
            Console.WriteLine("Сотрудник с таким Id не найден.");
            return;
        }

        employees.Remove(employee);
        SaveEmployees(employees);
        Console.WriteLine("Сотрудник удален.");
    }

    static void GetAllEmployees()
    {
        var employees = LoadEmployees();
        foreach (var employee in employees)
        {
            Console.WriteLine($"Id = {employee.Id}, FirstName = {employee.FirstName}, LastName = {employee.LastName}, SalaryPerHour = {employee.SalaryPerHour}");
        }
    }

    static List<Employee> LoadEmployees()
    {
        if (!File.Exists(filePath))
        {
            return new List<Employee>();
        }

        var json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<List<Employee>>(json) ?? new List<Employee>();
    }

    static void SaveEmployees(List<Employee> employees)
    {
        var json = JsonConvert.SerializeObject(employees, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

}