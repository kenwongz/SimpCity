using System;

namespace SimpCity {
    /// <summary>
    /// Represents a building in the grid.
    /// </summary>
    public abstract class CityGridBuilding {
        public CityGrid Grid { get; protected set; }
        public BuildingInfo Info { get; protected set; }

        public CityGridBuilding(BuildingInfo info) {
            Info = info;
            Grid = info.Grid;
        }

        /// <summary>
        /// Retrives  current position of the building in the grid.
        /// </summary>
        public CityGridPosition Position() {
            return Grid.PositionOf(this);
        }

        /// <summary>
        /// Retrives the building at the offset of the given current position.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">When the offset position is out of bounds</exception>
        public CityGridBuilding Offset(CityGridOffset offset) {
            return Grid.Get(Position().Offset(offset));
        }

        /// <summary>
        /// Calculate the score to award in the current state.
        /// </summary>
        public abstract int CalcScore(ScoreCalculationArchive archive);
    }
}
