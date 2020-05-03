using System;
using MCCR.Data;
using MRRC.Guacamole;
using MRRC.Guacamole.Components;
using MRRC.Guacamole.MenuGeneration;

namespace MateRatesRentACar
{
    internal class Program
    {
        public static bool ProgramRunning = true;
        
        public static void Main(string[] args)
        {
            var crm = new CustomerResourceManager("db/customers.csv");
            
            var customerManager = new CustomerManager(crm);
            
            var mainMenu = new Menu("Main Menu", new Component[]
            {
                MenuGenerator.GenerateManager("Customers", customerManager).Menu
            });

            var manager = new MenuManager(mainMenu);

            var guac = new Guac(manager);
            guac.Focus(mainMenu);

            guac.KeyPressed += async (_, ev) =>
            {
                if (ev.Key.Key != ConsoleKey.Escape) return;
                ev.Cancel = true;
                
                var exitResult = await guac.ShowDialogue(
                    "Exit", "Are you sure you want to exit?", new[] 
                    {
                        "Ok", 
                        "Cancel"
                    });

                if (exitResult == "Ok") ProgramRunning = false;
            };

            Console.CursorVisible = false;

            while (true)
            {
                guac.HandleEventLoop();

                if (!ProgramRunning) break;
            }

            Console.CursorVisible = true;
            Console.Clear();
            
            crm.Save();

            // ReSharper disable once FunctionNeverReturns
        }
    }
}