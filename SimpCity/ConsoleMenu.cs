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
            throw new NotImplementedException();
        }



        /// <summary>
        /// Displays the menu
        /// </summary>
        public void Display() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asks for user input to execute the callback. Returns <i>true</i> if the user intends to exit.
        /// </summary>
        public bool AskInput(int? testOption = null) {
            throw new NotImplementedException();
        }
    }
}
