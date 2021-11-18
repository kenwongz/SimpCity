using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpCity {
    class Program {
        static void Main(string[] args) {
            bool exit = false;
            while (exit == false) {
                Console.WriteLine("Welcome, mayor of Simp City!\n" + new string('=', 26));
                ConsoleMenu menu = new ConsoleMenu(new List<ConsoleMenuOption> {
                    new ConsoleMenuOption("Start new game", () => Console.WriteLine("TODO new game")),
                    new ConsoleMenuOption("Load saved game", () => Console.WriteLine("TODO load game"))
                });

                menu.Display();
                exit = menu.AskInput();
            }
        }
    }
}
