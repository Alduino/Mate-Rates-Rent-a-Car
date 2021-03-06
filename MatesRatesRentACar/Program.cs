﻿using System;
using System.IO;
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
            if (!Directory.Exists("../../../Data")) Directory.CreateDirectory("../../../Data");
            
            var crm = new CustomerResourceManager("../../../Data/customers.csv");
            var fleet = new Fleet("../../../Data/fleet.csv", "../../../Data/rentals.csv");
            
            var customerManager = new CustomerManager(crm, fleet);
            var fleetManager = new FleetManager(fleet, crm);
            
            var mainMenu = new Menu("Main Menu", new Component[]
            {
                MenuGenerator.GenerateManager("Customers", customerManager).Menu,
                MenuGenerator.GenerateManager("Fleet", fleetManager).Menu
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

                if (exitResult == "Ok")
                {
                    ProgramRunning = false;
                    CleanUp(crm, fleet);
                    Environment.Exit(0);
                }
            };

            Console.CursorVisible = false;

            while (true)
            {
                guac.HandleEventLoop();

                if (!ProgramRunning) break;
            }

            CleanUp(crm, fleet);

            // ReSharper disable once FunctionNeverReturns
        }

        private static void CleanUp(CustomerResourceManager crm, Fleet fleet)
        {
            Console.CursorVisible = true;
            Console.Clear();
            
            crm.Save();
            fleet.SaveRentals();
            fleet.SaveVehicles();
        }
    }
}