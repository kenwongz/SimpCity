using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpCity {
    public class ConsoleMenuOption {
        public string Description { get; }
        public Action Callback { get; }

        public ConsoleMenuOption(string description, Action callback) {
            this.Description = description;
            this.Callback = callback;
        }
    }

    /// <summary>
    /// Creates an interactive console menu
    /// </summary>
    public class ConsoleMenu {
        public List<ConsoleMenuOption> Options;
        public ConsoleMenu(List<ConsoleMenuOption> options) {
            this.Options = options;
        }

        protected string GetMenuText() {
            string buf = "";
            for (int i = 0; i < this.Options.Count; i++) {
                int opt = i + 1;
                buf += opt.ToString() + ") " + this.Options[i].Description + "\n";
            }
            buf += "\n0) Exit\n";
            return buf;
        }



        /// <summary>
        /// Displays the menu
        /// </summary>
        public void Display() {
            Console.WriteLine(GetMenuText());
        }

        /// <summary>
        /// Asks for user input to execute the callback. Returns <i>true</i> if the user intends to exit.
        /// </summary>
        public bool AskInput(int? testOption = null) {
            bool exit = false;
            int option = 0;

            if (testOption == null) {
                Console.Write("Enter your option: ");
                if (!int.TryParse(Console.ReadLine(), out option)) {
                    Console.WriteLine("Invalid option");
                    goto end;
                }
            } else {
                option = (int)testOption;
            }

            if (option == 0) {
                exit = true;
                goto end;
            }

            if (option > 0 && option <= this.Options.Count) {
                ConsoleMenuOption consoleOpt = this.Options[option - 1];
                // Print the option the user picked
                Console.WriteLine("Option " + option + ". " + consoleOpt.Description);
                Console.WriteLine(new string('-', 26));
                // Run the action
                consoleOpt.Callback();
            }

end:
            return exit;
        }
    }
}
