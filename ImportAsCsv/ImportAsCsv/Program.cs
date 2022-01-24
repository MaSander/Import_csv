using System;

namespace ImportAsCsv
{
    class Program
    {
        static void Main(string[] args)
        {
            Controller controller = new Controller();

            var ruuning = true;
            do
            {
                Console.WriteLine(
                    "\n1. List Collections" +
                    "\n2. Import Collections" +
                    "\n3. Semi Automatic Import Collections" +
                    "\n0. Exit"
                    );

                var choise = Console.ReadLine();

                switch (choise)
                {

                    case "1":
                        controller.loadCollection();
                        break;

                    case "2":
                        controller.ImportCollection();
                        break;

                    case "3":
                        controller.SemiAutomaticImport();
                        break;

                    case "0":
                        ruuning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid Choise");
                        break;
                }

            }while(ruuning);

            Console.WriteLine("By by, gracias por usar este sistema!!!");
        }
    }
}
