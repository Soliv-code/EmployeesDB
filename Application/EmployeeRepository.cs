using Core;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Application
{
    public class EmployeeRepository
    {
        public static readonly string connectionString = "Server=localhost;Database=EmployeeDB;Trusted_Connection=False;User Id=sa;Password=ILyaoff12345@;TrustServerCertificate=True";
        public static async Task GetAllEmployees()
        {
            string sqlCommand = "spGetAllEmployees";
            var employees = new List<Employee>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("spGetAllEmployees", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var employee = new Employee
                                {
                                    EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    DataOfBirth = reader.IsDBNull(reader.GetOrdinal("DataOfBirth")) ? default
                                    : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DataOfBirth"))),
                                    Salary = reader.GetDecimal(reader.GetOrdinal("Salary"))
                                };
                                employees.Add(employee);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
            foreach (var employee in employees)
            {
                Console.WriteLine();
                Console.WriteLine($"\n " +
                    $" Уникальный идентификатор : {employee.EmployeeID},\n " +
                    $" Имя: {employee.FirstName},\n " +
                    $" Фамилия: {employee.LastName},\n " +
                    $" Эл. почта: {employee.Email},\n " +
                    $" День рождения: {employee.DataOfBirth},\n " +
                    $" Зарплата:{employee.Salary}");
                Console.WriteLine(new string('-', 75));
            }
        }
        //========================================================================================================================
        public static async Task<bool> GetEmployeeById(int employeeId)
        {
            bool employeeExists = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("spGetEmployeeById", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            return employeeExists = reader.GetBoolean(0);
                        else
                            return false;
                    }
                }
            }
        }
        //========================================================================================================================
        public static async Task AddNewEmployee()
        {
            var employee = new Employee()
            {
                FirstName = GetStringValue("Введите имя: ", 1, 50),
                LastName = GetStringValue("Введите фамилию: ", 1, 50),
                Email = GetStringValue("Введите почту: ", 1, 100),
                DataOfBirth = GetDateValue("Введите дату в формате (год-месяц-день): "),
                Salary = GetDecimalValue("Введите зарплату: ")
            };
            if (employee is not null)
                await InsertEmployeeIntoDb(employee);

            static string GetStringValue(string questionToTheUser, int minLength, int maxLength)
            {
                while (true)
                {
                    Console.WriteLine(questionToTheUser);
                    var readValue = Console.ReadLine();
                    if (readValue.Length >= minLength && readValue.Length <= maxLength)
                        return readValue;
                    else
                        Console.WriteLine($"Значение поля не может быть меньше {minLength} символа и больше {maxLength} символов");
                }
            }

            static DateOnly GetDateValue(string questionToTheUser)
            {
                while (true)
                {
                    Console.WriteLine(questionToTheUser);
                    var readValue = Console.ReadLine();
                    if (DateOnly.TryParse(readValue, out var date) && date <= DateOnly.FromDateTime(DateTime.Now))
                        return date;
                    else
                        Console.WriteLine($"Введите корректную дату");

                }
            }

            static decimal GetDecimalValue(string questionToTheUser)
            {
                while (true)
                {
                    Console.WriteLine(questionToTheUser);
                    var readValue = Console.ReadLine();
                    if (decimal.TryParse(readValue, out var decimalValue) && decimalValue >= 0)
                    {
                        return decimalValue;
                    }
                    else
                    {
                        Console.WriteLine("Зарплата не может быть отрицательной или равняться нулю");
                    }
                }
            }

            static async Task InsertEmployeeIntoDb(Employee employee)
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                using (var command = new SqlCommand("spAddEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@LastName", employee.LastName);
                    command.Parameters.AddWithValue("@Email", employee.Email);
                    command.Parameters.AddWithValue("@DateOfBirth", employee.DataOfBirth);
                    command.Parameters.AddWithValue("@Salary", employee.Salary);

                    try
                    {
                        await command.ExecuteNonQueryAsync();
                        Console.WriteLine();
                        Console.WriteLine("\n Добавлен новый сотрудник!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка добавления сотрудника " + ex.Message);
                    }

                }
            }

        }
        //========================================================================================================================
        public static async Task UpdateEmployeeField()
        {
            bool employeeExists = false;
            try
            {
                Console.WriteLine("Введите уникальный идентификатор сотрудника");
                if (!int.TryParse(Console.ReadLine(), out int employeeId))
                    Console.WriteLine("Не корректный формат идентификатора, требуется ввести числовое значение!");
                else
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        using (SqlCommand command = new SqlCommand("spGetEmployeeById", connection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@EmployeeID", employeeId);
                            using (SqlDataReader reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                    employeeExists = reader.GetBoolean(0);
                            }
                        }
                    }
                    if (!employeeExists)
                        Console.WriteLine($"Сотрудника с таким идентификатором: {employeeId} нет в базе данных");

                    Console.WriteLine("\n Выберите поле для обновления: ");
                    Console.WriteLine("1. Имя");
                    Console.WriteLine("2. Фамилия");
                    Console.WriteLine("3. Эл. почта");
                    Console.WriteLine("4. День рождения");
                    Console.WriteLine("5. Зарплата");
                    Console.WriteLine("6. Выход");
                    if (!int.TryParse(Console.ReadLine(), out int userChoice) || userChoice < 1 || userChoice > 6)
                        Console.WriteLine("Не корректный формат выбора, требуется ввести числовое значение от 1 до 6");

                    object fieldValue = null;

                    switch (userChoice)
                    {
                        case 1:
                        case 2:
                        case 3:
                            Console.WriteLine("Введите новое значение:");
                            fieldValue = Console.ReadLine();
                            break;
                        case 4:
                            Console.WriteLine("Введите день рождения в формате (год-месяц-день");
                            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateOfBirth))
                            {
                                Console.WriteLine("Неверный формат даты. Используйте yyyy-MM-dd");
                                return;
                            }
                            fieldValue = dateOfBirth;
                            break;
                        case 5:
                            Console.WriteLine("Введите новую зарплату");
                            if (!decimal.TryParse(Console.ReadLine(), out decimal salary))
                            {
                                Console.WriteLine("Неверный формат зарплаты. Необходимо использовать число, разделителем является '',''");
                                return;
                            }
                            fieldValue = salary;
                            break;
                        case 6:
                            return;
                    }
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            await connection.OpenAsync();
                            using (SqlCommand command = new SqlCommand("spUpdateEmployees", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@EmployeeID", employeeId);
                                command.Parameters.AddWithValue("@FieldType", userChoice);

                                switch (userChoice)
                                {
                                    case 1:
                                    case 2:
                                    case 3:
                                        command.Parameters.AddWithValue("@TextField", (string)fieldValue);
                                        command.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);
                                        command.Parameters.AddWithValue("@Salary", 0);
                                        break;
                                    case 4:
                                        command.Parameters.AddWithValue("@TextField", "");
                                        command.Parameters.AddWithValue("@DateOfBirth", (DateTime)fieldValue);
                                        command.Parameters.AddWithValue("@Salary", 0);
                                        break;
                                    case 5:
                                        command.Parameters.AddWithValue("@TextField", "");
                                        command.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);
                                        command.Parameters.AddWithValue("@Salary", (decimal)fieldValue);
                                        break;
                                }
                                await command.ExecuteNonQueryAsync();
                                Console.WriteLine("\nИнформация о сотруднике успешно обновлена!");
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine($"Ошибка обновления данных по сотруднику: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Возникла ошибка: {ex.Message}");
            }
        }
        //========================================================================================================================
        public static async Task DeleteEmployee()
        {
            bool employeeExists = false;
            try
            {
                Console.WriteLine("\n Введите уникальный идентификатор сотрудника");
                if (!int.TryParse(Console.ReadLine(), out int employeeId))
                    Console.WriteLine("\n Не корректный формат идентификатора, требуется ввести числовое значение!");
                else
                {
                    employeeExists = await GetEmployeeById(employeeId);
                    if (!employeeExists)
                    {
                        Console.WriteLine($"\n Сотрудника с таким идентификатором: {employeeId} нет в базе данных");
                        return;
                    }
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            await connection.OpenAsync();
                            using (SqlCommand command = new SqlCommand("spDeleteEmployee", connection))
                            {
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@EmployeeID", employeeId);

                                var result = await command.ExecuteScalarAsync();

                                if (result != null)
                                {
                                    int.TryParse(result.ToString(), out int rowsAffected);
                                    if (rowsAffected > 0)
                                        Console.WriteLine($"{rowsAffected} сотрудник успешно удален.");
                                    else
                                        Console.WriteLine("Сотрудник не найден или не удален!");
                                }
                                else
                                    Console.WriteLine("Возникла ошибка при выполнении хранимой процедуры!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Возникла ошибка: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Возникла ошибка: {ex.Message}");
            }
        }
    }
}
