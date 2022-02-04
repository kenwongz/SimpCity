namespace SimpCity.buildings {
    public class Monument : CityGridBuilding {
        public static string Name { get; } = "Monument";
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public static string Code { get; } = "MON";
        public Monument(BuildingInfo info) : base(info) { }
        public override int CalcScore(ScoreCalculationArchive archive) {
            int score = 0;
            int xPos = Position().X;
            int yPos = Position().Y;
            if(xPos == 0 && yPos == 0 || xPos == 0 && yPos )
            {
                score += 2;
            }
            else {
                score += 1;
            }
            return score;
        }
    }
}
