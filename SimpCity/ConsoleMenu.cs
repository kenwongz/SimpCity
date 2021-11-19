using System;
using System.Collections.Generic;

namespace SimpCity {
    public class ConsoleMenuCommand {
        public bool ExitNext { get; set; } = false;

        /// <summary>
        /// For interactive displays, this signals the console menu to return after the command
        /// callback is completed.
        /// </summary>
        public void Exit(bool exit = true) {
            this.ExitNext = exit;
        }
    }

    public class ConsoleMenuOption {
        public string Label { get; set; }
        public string Description { get; set; }
        public Action<ConsoleMenuCommand> Callback { get; set; }

        public ConsoleMenuOption(string label, string description, Action<ConsoleMenuCommand> callback = null) {
            this.Label = label;
            this.Description = description;
            this.Callback = callback;
        }
    }

    public class ConsoleMenuHeading {
        public string Description { get; set; }

        /// <param name="description">If this is `null`, it can act as a single line padding.</param>
        public ConsoleMenuHeading(string description = null) {
            this.Description = description;
        }
    }

    /// <summary>
    /// Creates an interactive console menu
    /// </summary>
    public class ConsoleMenu {
        private readonly List<ConsoleMenuOption> Options;
        private readonly IDictionary<string, ConsoleMenuOption> OptionsMap = new Dictionary<string, ConsoleMenuOption>();
        private readonly IDictionary<int, ConsoleMenuHeading> Headings = new Dictionary<int, ConsoleMenuHeading>();

        /// <summary>
        /// Executed before each interaction.
        /// </summary>
        private Action CustomAction = null;
        private int OptionsIndex = 0;

        public ConsoleMenu() {
            this.Options = new List<ConsoleMenuOption>();
        }

        public ConsoleMenu(List<ConsoleMenuOption> options) {
            this.Options = options;
        }

        protected string GetMenuText() {
            string buf = "";
            for (int i = 0; i < this.Options.Count; i++) {
                buf += this.Options[i].Label + ") " + this.Options[i].Description + "\n";
                
                // Display a header after this option
                if (this.Headings.ContainsKey(i)) {
                    buf += "\n";
                    string heading = this.Headings[i].Description;
                    if (heading != null) {
                        buf += heading + "\n";
                    }
                }
            }
            return buf;
        }

        public ConsoleMenu AddOption(string description, Action<ConsoleMenuCommand> callback = null, string customLabel = null) {
            if (customLabel == null) {
                this.OptionsIndex++;
                customLabel = this.OptionsIndex.ToString();
            }
            ConsoleMenuOption option = new ConsoleMenuOption(customLabel, description, callback);
            this.OptionsMap[customLabel] = option;
            this.Options.Add(option);
            return this;
        }

        public ConsoleMenu AddExitOption(string description, Action<ConsoleMenuCommand> callback = null) {
            // Overload callback with exit instruction and custom label
            return AddOption(description, (cmd) => {
                callback(cmd);
                cmd.Exit();
            }, "0");
        }

        public ConsoleMenu AddHeading(string description = null, int after = -1) {
            if (after < 0) {
                after = this.Options.Count - 1;
            }
            this.Headings[after] = new ConsoleMenuHeading(description);
            return this;
        }

        /// <summary>
        /// Executed before each interaction, and before the menu is displayed.
        /// </summary>
        public ConsoleMenu BeforeInteraction(Action callback) {
            this.CustomAction = callback;
            return this;
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
        /// <param name="testOption">Uses this as the input string instead of stdin, for tests purposes.</param>
        public bool AskInput(string testOption = null) {
            this.CustomAction();
            this.Display();

            string option = testOption;
            if (testOption == null) {
                Console.Write("Enter your option: ");
                option = Console.ReadLine().Trim();
            }

            if (!this.OptionsMap.ContainsKey(option)) {
                Console.WriteLine("Invalid option");
                return false;  // continue
            }

            ConsoleMenuOption consoleOpt = this.OptionsMap[option];
            // Print the option the user picked
            Console.WriteLine("Option " + option + ". " + consoleOpt.Description);
            Console.WriteLine(new string('-', 26));

            // Run the action
            ConsoleMenuCommand cmd = new ConsoleMenuCommand();
            consoleOpt.Callback?.Invoke(cmd);

            // Exit after this if signalled
            return cmd.ExitNext;
        }

        /// <summary>
        /// Blockingly displays the menu and asks for user input until a signal is sent to exit.
        /// </summary>
        public void DisplayInteraction() {
            while (true) {
                // Exit after this if signalled
                if (this.AskInput()) {
                    break;
                }
                Console.WriteLine();
            }
        }
    }
}
