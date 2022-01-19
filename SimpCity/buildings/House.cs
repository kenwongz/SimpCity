using System;

namespace SimpCity.buildings {
    public class House : CityGridBuilding {
        public static string Name { get; } = "House";
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public static string Code { get; } = "HSE";
        public House(BuildingInfo info) : base(info) { }

        public override int CalcScore(ScoreCalculationArchive archive) {
            bool hasFactory = false;
            foreach (CityGridOffset offset in CityGrid.BesideOffsets()) {
                CityGridBuilding besideBuilding;
                try {
                    besideBuilding = Offset(offset);
                } catch (ArgumentOutOfRangeException) {
                    // ignore, nothing to do.
                    continue;
                }
                if (besideBuilding is Factory) {
                    hasFactory = true;
                    break;
                }
            }

            // 1. If next to a factory (FAC), then it scores 1 point only
            if (hasFactory) {
                return 1;
            }

            // This non-breaking loop increments the score based on adjacent buildings
            int score = 0;
            foreach (CityGridOffset offset in CityGrid.AdjacentOffsets()) {
                CityGridBuilding besideBuilding;
                try {
                    besideBuilding = Offset(offset);
                } catch (ArgumentOutOfRangeException) {
                    // ignore, nothing to do.
                    continue;
                }

                if (besideBuilding is House || besideBuilding is Shop) {
                    // 2. Scores 1 point for each adjacent house(HSE) or shop(SHP)
                    score += 1;
                } else if (besideBuilding is Beach) {
                    // 3. Scores 2 points for each adjacent beach(BCH)
                    score += 2;
                }
            }

            return score;
        }
    }
}
