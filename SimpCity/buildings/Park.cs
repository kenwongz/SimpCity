namespace SimpCity.buildings {
    public class Park : CityGridBuilding {
        public static string Name { get; } = "Park";
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public static string Code { get; } = "PRK";
        public Park(BuildingInfo info) : base(info) { }
        public override int CalcScore(ScoreCalculationArchive archive) {
            // TODO: US-8
            throw new System.NotImplementedException();
        }
    }
}
