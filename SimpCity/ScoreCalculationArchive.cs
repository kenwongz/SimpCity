using System.Collections.Generic;

namespace SimpCity {
    /// <summary>
    /// Used by the Game to archive building score calculations into several data structures.
    /// Later, possibly referenced by CityGridBuilding subclasses to perform additional calculations.
    /// </summary>
    public class ScoreCalculationArchive {
        /// <summary>
        /// Lists all the calculated buildings.
        /// </summary>
        public IList<CityGridBuilding> Buildings { get; }

        /// <summary>
        /// Maps the calculated building types to all of the calculated buildings.
        /// </summary>
        public IDictionary<BuildingTypes, List<CityGridBuilding>> BuildingsPerType { get; }

        public ScoreCalculationArchive() {
            Buildings = new List<CityGridBuilding>();
            BuildingsPerType = new Dictionary<BuildingTypes, List<CityGridBuilding>>();
        }

        /// <summary>
        /// Called when a building's score has been calculated.
        /// </summary>
        public void calculated(CityGridBuilding building) {
            Buildings.Add(building);

            // Create a new list or add on to the existing one.
            if (BuildingsPerType.ContainsKey(building.Info.Type)) {
                BuildingsPerType[building.Info.Type].Add(building);
            } else {
                BuildingsPerType.Add(building.Info.Type, new List<CityGridBuilding> {
                    building
                });
            }
        }
    }
}
