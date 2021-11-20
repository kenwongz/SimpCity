using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SimpCity;


namespace SimpCityTests {
    [TestClass]
    public class ConsoleMenuTests {
        [TestMethod]
        public void AskInput_ExecutesCallbacks() {
            bool testSwitch = false;

            ConsoleMenu menu = new ConsoleMenu(new List<ConsoleMenuOption> {
                new ConsoleMenuOption("This is an option", () => {
                    // change testSwitch to true
                    testSwitch = true;
                }),
            });

            menu.AskInput(1);
            // Check the switch is changed
            Assert.IsTrue(testSwitch);

        }

        [TestMethod]
        public void AskInput_Exits_WhenRequested() {
            ConsoleMenu menu = new ConsoleMenu(new List<ConsoleMenuOption> {
                new ConsoleMenuOption("This is an option", () => {}),
            });

            bool exit;

            // 1 means no exit
            exit = menu.AskInput(0);
            Assert.IsTrue(exit);

            // 0 means exit
            exit = menu.AskInput(0);
            Assert.IsTrue(exit);

        }
    }
}
