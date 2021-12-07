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
            Assert.IsTrue(exit);

            // 0 means exit
            exit = menu.AskInput("0");
            Assert.IsTrue(exit);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Trying to edit a non-existent label did not flag out..")]
        public void EditOption_Throws_WhenInvalidLabel() {
            ConsoleMenu menu = new ConsoleMenu()
                .AddOption("This is an option", (m) => { });

            menu.EditOption("ThisIsALabelThatDoesntExistDefinitely", "-");
        }
    }
}
