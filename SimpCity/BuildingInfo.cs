namespace SimpCity {
    public delegate CityGridBuilding MakeNewFunc();

    /// <summary>
    /// Represents metadata of a building type in a grid.
    /// </summary>
    public class BuildingInfo {
        public BuildingTypes Type { get; set; }
        public CityGrid Grid { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public string Code { get; set; }
        public int CopiesLeft { get; set; }
        /// <summary>
        /// The callback to execute to return the building object
        /// </summary>
        public MakeNewFunc MakeNew { get; set; }
    }
}
