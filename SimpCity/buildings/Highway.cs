namespace SimpCity.buildings {
    public class Highway : CityGridBuilding {
        public static string Name { get; } = "Highway";
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public static string Code { get; } = "HWY";
        public Highway(BuildingInfo info) : base(info) { }

        public override int CalcScore(ScoreCalculationArchive archive) {
            // TODO: US-8
            throw new System.NotImplementedException();
        }
    }
}
