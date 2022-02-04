using System;

namespace SimpCity.buildings {
    public class Park : CityGridBuilding {
        public static string Name { get; } = "Park";
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public static string Code { get; } = "PRK";
        public Park(BuildingInfo info) : base(info) { }

        /// <summary>
        /// Counts the number of linked Parks that are eligible for scoring.
        /// </summary>
        /// <param name="archive">The score archive to set as calculated.</param>
        /// <param name="count">The count to add on to.</param>
        public int CountLinked(ScoreCalculationArchive archive, int count = 0) {
            // Already calculated. Nothing to link, not even itself
            if (archive.IsCalculated(this)) {
                return count;
            }

            // Counting itself, + 1
            count++;
            // Set current park as calculated, so it won't be calculated again
            archive.Calculated(this);

            // Recursively grab neighbouring park count
            foreach (CityGridOffset offset in CityGrid.BesideOffsets()) {
                CityGridBuilding besideBuilding;
                try {
                    besideBuilding = Offset(offset);
                } catch (ArgumentOutOfRangeException) {
                    // ignore, nothing to do.
                    continue;
                }
                if (!(besideBuilding is Park)) {
                    // ignore, if not a park or already calculated.
                    continue;
                }

                Park besidePark = (Park)besideBuilding;
                // Recursive linking
                count = besidePark.CountLinked(archive, count);

                if (count == 8) {
                    // Stop counting!
                    break;
                }
            }

            return count;
        }

        public override int CalcScore(ScoreCalculationArchive archive) {
            int linkedCount = CountLinked(archive);
            int score = 0;

            switch (linkedCount) {
                case 1:
                    score = 1;
                    break;
                case 2:
                    score = 3;
                    break;
                case 3:
                    score = 8;
                    break;
                case 4:
                    score = 16;
                    break;
                case 5:
                    score = 22;
                    break;
                case 6:
                    score = 23;
                    break;
                case 7:
                    score = 24;
                    break;
                case 8:
                    score = 25;
                    break;
            }

            return score;
        }
    }
}
