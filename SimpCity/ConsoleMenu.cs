﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimpCity {
    [ExcludeFromCodeCoverage]
    public class ConsoleMenuOption {
        public string Label { get; set; }
        /// <summary>
        /// The description of the option.
        /// If this is <i>null</i>, it can hide the option from displaying.
        /// </summary>
        public string Description { get; set; }
        public Action<ConsoleMenu> Callback { get; set; }

        /// <param name="description">If this is <i>null</i>, it can hide the option from displaying.</param>
        public ConsoleMenuOption(string label, string description = null, Action<ConsoleMenu> callback = null) {
            this.Label = label;
            this.Description = description;
            this.Callback = callback;
        }
    }

    [ExcludeFromCodeCoverage]
    public class ConsoleMenuHeading {
        /// <summary>
        /// The description of the heading.
        /// If this is <i>null</i>, it can act as a single line padding.
        /// </summary>
        public string Description { get; set; }

        /// <param name="description">If this is <i>null</i>, it can act as a single line padding.</param>
        public ConsoleMenuHeading(string description = null) {
            this.Description = description;
        }
    }

    /// <summary>
    /// Creates an interactive console menu
    /// </summary>
    public class ConsoleMenu {
        private readonly List<ConsoleMenuOption> options;
        private readonly IDictionary<string, ConsoleMenuOption> optionsMap = new Dictionary<string, ConsoleMenuOption>();
        private readonly IDictionary<int, ConsoleMenuHeading> headings = new Dictionary<int, ConsoleMenuHeading>();

        /// <summary>
        /// Executed before each interaction.
        /// </summary>
        private Action<ConsoleMenu> customAction = null;
        private int optionsIndex = 0;
        private bool exitNext = false;

        public ConsoleMenu() {
            this.options = new List<ConsoleMenuOption>();
        }

        public ConsoleMenu(List<ConsoleMenuOption> options) {
            this.options = options;
        }

        protected string GetMenuText() {
            string buf = "";
            for (int i = 0; i < this.options.Count; i++) {
                string description = this.options[i].Description;
                if (description != null) {
                    buf += this.options[i].Label + ") " + description + "\n";
                }
                
                // Display a header after this option
                if (this.headings.ContainsKey(i)) {
                    buf += "\n";
                    string heading = this.headings[i].Description;
                    if (heading != null) {
                        buf += heading + "\n";
                    }
                }
            }
            return buf;
        }

        /// <param name="description">If this is <i>null</i>, it can hide the option from displaying.</param>
        public ConsoleMenu AddOption(string description, Action<ConsoleMenu> callback = null, string customLabel = null) {
            if (customLabel == null) {
                this.optionsIndex++;
                customLabel = this.optionsIndex.ToString();
            }
            ConsoleMenuOption option = new ConsoleMenuOption(customLabel, description, callback);
            this.optionsMap[customLabel] = option;
            this.options.Add(option);
            return this;
        }

        public ConsoleMenu AddExitOption(string description, Action<ConsoleMenu> callback = null) {
            // Overload callback with exit instruction and custom label
            return AddOption(description, (menu) => {
                callback?.Invoke(menu);
                menu.Exit();
            }, "0");
        }

        public ConsoleMenu AddHeading(string description = null, int after = -1) {
            if (after < 0) {
                after = this.options.Count - 1;
            }
            this.headings[after] = new ConsoleMenuHeading(description);
            return this;
        }

        /// <summary>
        /// Edit an option added by AddOption
        /// </summary>
        /// <param name="description">If this is <i>null</i>, it can hide the option from displaying.</param>
        /// <exception cref="System.InvalidOperationException">When the label does not exist in the options</exception>
        public ConsoleMenu EditOption(string label, string description, Action<ConsoleMenu> callback = null) {
            if (!this.optionsMap.ContainsKey(label)) {
                throw new InvalidOperationException("Label does not exist in the options: " + label);
            }
            this.optionsMap[label].Description = description;
            this.optionsMap[label].Callback = callback;
            return this;
        }

        /// <summary>
        /// Executed before each interaction, and before the menu is displayed.
        /// </summary>
        public ConsoleMenu BeforeInteraction(Action<ConsoleMenu> callback) {
            this.customAction = callback;
            return this;
        }

        /// <summary>
        /// For interactive display, this signals the console menu to return after the command
        /// callback is completed.
        /// </summary>
        public void Exit(bool exit = true) {
            this.exitNext = exit;
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
            this.customAction?.Invoke(this);
            if (this.exitNext) {
                // Exit after this if signalled by custom action
                return this.exitNext;
            }
            this.Display();

            string option = testOption;
            if (testOption == null) {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("Enter your option: ");
                Console.ResetColor();
                option = Console.ReadLine().Trim();
            }

            if (!this.optionsMap.ContainsKey(option)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Invalid option!");
                Console.ResetColor();
                // Do not allow buffers after content to be colored
                Console.WriteLine();
                return false;  // continue
            }

            ConsoleMenuOption consoleOpt = this.optionsMap[option];
            // Print the option the user picked
            Console.WriteLine("Option " + option + ". " + consoleOpt.Description ?? "");
            Console.WriteLine(new string('-', 26));

            // Run the action
            consoleOpt.Callback?.Invoke(this);

            // Exit after this if signalled
            return this.exitNext;
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
