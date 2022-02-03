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
            int score = 1;
            bool hasFactory = false;
            bool hasBeach = false;
            bool hasHighway = false;
            bool hasHouse = false;
            bool hasMonument = false;
            bool hasPark = false;
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
                }
                if (besideBuilding is Beach) {
                    hasBeach = true;
                }
                if (besideBuilding is Highway) {
                    hasBeach = true;
                }
                if (besideBuilding is House) {
                    hasHouse = true;
                }
                if (besideBuilding is Monument) {
                    hasHouse = true;
                }
                if (besideBuilding is Park) {
                    hasHouse = true;
                }
            }
            if (hasFactory) {
                score+=1;
            }
            if (hasBeach) {
                score += 1;
            }
            if (hasHighway) {
                score += 1;
            }
            if (hasHouse) {
                score += 1;
            }
            if (hasMonument) {
                score += 1;
            }
            if (hasPark) {
                score += 1;
            }

            return score;
        }
    }
}
