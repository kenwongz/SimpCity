using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SimpCity;


namespace SimpCityTests {
    [TestClass]
    public class ConsoleMenuTests {
        /// <summary>
        /// QA-SN-1.1:
        /// This ensures that the game will blockingly run.
        /// </summary>
        [TestMethod]
        public void AskInput_ExecutesCallbacks() {
            bool testSwitch = false;

            ConsoleMenu menu = new ConsoleMenu()
                .AddOption("This is an option", (m) => {
                    // change testSwitch to true
                    testSwitch = true;
                });

            menu.AskInput("1");
            // Check the switch is changed
            Assert.IsTrue(testSwitch);
        }

        /// <summary>
        /// QA-SN-1.2:
        /// This ensures that the game will display the grid before the menu.
        /// </summary>
        [TestMethod]
        public void AskInput_Executes_BeforeInteraction() {
            bool testSwitch = false;

            ConsoleMenu menu = new ConsoleMenu()
                .BeforeInteraction((m) => {
                    // change testSwitch to true
                    testSwitch = true;
                })
                .AddOption("This is an option", (m) => {});

            menu.AskInput("1");
            // Check the switch is changed
            Assert.IsTrue(testSwitch, "BeforeInteraction did not execute");
        }

        /// <summary>
        /// QA-SN-2.1, QA-SN-9.1:
        /// This ensures that the game will exit when requested.
        /// </summary>
        [TestMethod]
        public void AskInput_Exits_WhenRequested() {
            ConsoleMenu menu = new ConsoleMenu()
                .AddOption("This is an option", (m) => { })
                .AddExitOption("Exit");

            bool exit;

            // 1 means no exit
            exit = menu.AskInput("1");
            Assert.IsFalse(exit);

            // 0 means exit
            exit = menu.AskInput("0");
            Assert.IsTrue(exit);
        }

        /// <summary>
        /// QA-SN-3.1, QA-SN-10.1:
        /// This ensures that the function will not break and resumes regular operation when an
        /// invalid option is supplied.
        /// </summary>
        [TestMethod]
        public void AskInput_RunsNormally_OnInvalidInput() {
            bool testSwitch = false;

            // Create two options
            ConsoleMenu menu = new ConsoleMenu()
                .AddOption("This is an option", (m) => { testSwitch = true; })
                .AddOption("This is an option 2", (m) => { testSwitch = true; });

            // Input 3, a non-existent option
            bool exit = menu.AskInput("3");

            // 1. Check the switch is NOT changed
            Assert.IsFalse(testSwitch, "Test Switch was changed");

            // 2. Will not exit
            Assert.IsFalse(exit, "Must not exit");
        }

        /// <summary>
        /// N/A:
        /// This ensures that the function throws when trying to edit a non-existent label.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Trying to edit a non-existent label did not flag out..")]
        public void EditOption_Throws_WhenInvalidLabel() {
            ConsoleMenu menu = new ConsoleMenu()
                .AddOption("This is an option", (m) => { });

            menu.EditOption("ThisIsALabelThatDoesntExistDefinitely", "-");
        }
    }
}
