using System.Collections.Generic;

namespace SimpCity {
    /// <summary>
    /// Used by the Game and CityGridBuilding subclasses to archive building score calculations into
    /// several data structures.
    /// Later, possibly referenced by CityGridBuilding subclasses to perform additional calculations.
    /// </summary>
    public class ScoreCalculationArchive {
        /// <summary>
        /// Lists all the calculated buildings in a dictionary.
        /// </summary>
        public IDictionary<CityGridBuilding, bool> Buildings { get; }

        /// <summary>
        /// Maps the calculated building types to all of the calculated buildings.
        /// </summary>
        public IDictionary<BuildingTypes, List<CityGridBuilding>> BuildingsPerType { get; }

        public ScoreCalculationArchive() {
            // O(1) search!
            Buildings = new Dictionary<CityGridBuilding, bool>();
            BuildingsPerType = new Dictionary<BuildingTypes, List<CityGridBuilding>>();
        }

        /// <summary>
        /// Called when a building's score has been calculated.
        /// </summary>
        public void Calculated(CityGridBuilding building) {
            Buildings.Add(building, true);

            // Create a new list or add on to the existing one.
            if (BuildingsPerType.ContainsKey(building.Info.Type)) {
                BuildingsPerType[building.Info.Type].Add(building);
            } else {
                BuildingsPerType.Add(building.Info.Type, new List<CityGridBuilding> {
                    building
                });
            }
        }

        /// <summary>
        /// Checks whether a building's score has been calculated.
        /// </summary>
        /// <returns></returns>
        public bool IsCalculated(CityGridBuilding building) {
            return Buildings.ContainsKey(building);
        }
    }
}
