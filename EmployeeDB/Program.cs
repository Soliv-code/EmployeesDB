using Application;

namespace EmployeeDB
{
    public class Program
    {
        
        static async Task Main(string[] args)
        {
            bool exitApplication = false;
            while (!exitApplication)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Показать всех сотрудников");
                Console.WriteLine("2. Добавить сотрудника");
                Console.WriteLine("3. Обновления данных сотрудника");
                Console.WriteLine("4. Удаление сотрудника");
                Console.WriteLine("5. Выход");
                string input = Console.ReadLine();

                if(int.TryParse(input, out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            await EmployeeRepository.GetAllEmployees();
                            break;
                        case 2:
                            await EmployeeRepository.AddNewEmployee();
                            break;
                        case 3:
                            await EmployeeRepository.UpdateEmployeeField();
                            break;
                        case 4:
                            await EmployeeRepository.DeleteEmployee();
                            break;
                        case 5:
                            exitApplication = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice!");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Не корректный ввод данных, вы можете выбрать варианты между 1 и 5");
                }
            }
        }
    }
}
