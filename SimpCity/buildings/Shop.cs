namespace SimpCity.buildings {
    public class Shop : CityGridBuilding {
        public static string Name { get; } = "Shop";
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public static string Code { get; } = "SHP";
        public Shop(BuildingInfo info) : base(info) { }

        public override int CalcScore(ScoreCalculationArchive archive) {
            // TODO: US-8
            throw new System.NotImplementedException();
        }
    }
}
