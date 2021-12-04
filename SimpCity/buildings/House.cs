namespace SimpCity.buildings {
    class House : CityGridBuilding {
        public static string Name { get; } = "House";
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public static string Code { get; } = "HSE";
        public House(BuildingInfo info) : base(info) { }
    }
}
