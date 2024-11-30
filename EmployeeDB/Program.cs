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
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Выберите операцию:", Console.ForegroundColor);
                Console.WriteLine("1. Показать всех сотрудников", Console.ForegroundColor);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("2. Добавить сотрудника", Console.ForegroundColor);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("3. Обновления данных сотрудника", Console.ForegroundColor);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("4. Удаление сотрудника", Console.ForegroundColor);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("5. Выход", Console.ForegroundColor);
                Console.WriteLine();
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
                            Console.ForegroundColor = ConsoleColor.White;
                            exitApplication = true;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("Не корректный ввод данных!", Console.ForegroundColor);
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
