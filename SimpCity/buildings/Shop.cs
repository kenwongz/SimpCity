using System;

namespace SimpCity.buildings {
    public class Shop : CityGridBuilding {
        public static string Name { get; } = "Shop";
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public static string Code { get; } = "SHP";
        public Shop(BuildingInfo info) : base(info) { }

        public override int CalcScore(ScoreCalculationArchive archive) {
            int score = 0;
            foreach (CityGridOffset offset in CityGrid.BesideOffsets()) {
                CityGridBuilding besideBuilding;
                try {
                    besideBuilding = Offset(offset);
                } catch (ArgumentOutOfRangeException) {
                    // ignore, nothing to do.
                    continue;
                }
                if (besideBuilding is Beach) {
                    score += 1;
                }
                if (besideBuilding is Factory) {
                    score += 1;
                }
                if (besideBuilding is Highway) {
                    score += 1;
                }
                if (besideBuilding is House) {
                    score += 1;
                }
                if (besideBuilding is Monument) {
                    score += 1;
                }
                if (besideBuilding is Park) {
                    score += 1;
                }
                if (besideBuilding is Shop) {
                    score += 0;
                }
            }

            return score;
        }
    }
}
