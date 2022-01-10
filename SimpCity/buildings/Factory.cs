namespace SimpCity.buildings {
    public class Factory : CityGridBuilding {
        public static string Name { get; } = "Factory";
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public static string Code { get; } = "FAC";
        public Factory(BuildingInfo info) : base(info) { }
    }
}
