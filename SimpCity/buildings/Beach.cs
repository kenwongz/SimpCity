﻿namespace SimpCity.buildings {
    public class Beach : CityGridBuilding {
        public static string Name { get; } = "Beach";
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public static string Code { get; } = "BCH";
        public Beach(BuildingInfo info) : base(info) { }

        public override int CalcScore(ScoreCalculationArchive archive) {
            int xPos = Position().X;
            // scores 3 points if it is built in column A or last column, 1 otherwise
            return (xPos == 0 || xPos == (Info.Grid.Width - 1)) ? 3 : 1;
        }
    }
}
