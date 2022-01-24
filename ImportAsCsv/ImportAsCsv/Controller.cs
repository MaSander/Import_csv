using System;
using System.Collections.Generic;
using System.Text;

namespace ImportAsCsv
{
    class Controller
    {
        private Repository repository;

        public Controller()
        {
            repository = new Repository();
        }

        public void ImportCollection()
        {

            this.loadCollection();
            Console.WriteLine("Chose a collection to import!");

            int choise = 0;

            List<string> list = repository.GetCollectionsNamesList();
            
            try
            {
                choise = int.Parse(Console.ReadLine());


                if (list.Count - 1 < choise)
                {
                    throw new Exception();
                };

                Console.WriteLine(list[choise]);
            
                repository.ImportCollection(list[choise]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("invalid choise");
                Console.WriteLine(ex);
            }

        }

        public void SemiAutomaticImport()
        {
            List<string> collections = repository.GetCollectionsNamesList();

            foreach (var name in collections)
            {
                Console.WriteLine($"Import collection: '{name}'");

                string choise = string.Empty;
                while (choise != "n" && choise != "y" && choise != "q")
                {
                    Console.WriteLine("\nyes(y) or no(n)? or quit(q)");
                    choise = Console.ReadLine();
                }
                if (choise == "q") return;
                if (choise == "y") repository.ImportCollection(name);
            }
        }

        public void loadCollection()
        {
            Int16 count = 0;

            List<string> list = repository.GetCollectionsNamesList();

            foreach (var item in list)
            {
                Console.WriteLine($" {count}: {item}");
                count++;

            }
        }

    }
}
