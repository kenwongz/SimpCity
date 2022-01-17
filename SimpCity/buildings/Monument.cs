namespace SimpCity.buildings {
    public class Monument : CityGridBuilding {
        public static string Name { get; } = "Monument";
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public static string Code { get; } = "MON";
        public Monument(BuildingInfo info) : base(info) { }
    }
}
