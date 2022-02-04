namespace SimpCity.buildings {
    public class Monument : CityGridBuilding {
        public static string Name { get; } = "Monument";
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public static string Code { get; } = "MON";
        public Monument(BuildingInfo info) : base(info) { }
        public override int CalcScore(ScoreCalculationArchive archive) {
            //initialise score to 0
            int score = 0;
            int xPos = Position().X;
            int yPos = Position().Y;
            //corners
            if(xPos == 0 && yPos == 0 || xPos == 0 && yPos == (Info.Grid.Height - 1) || xPos == (Info.Grid.Width - 1) && yPos == (Info.Grid.Height - 1) ||
                xPos == (Info.Grid.Width - 1) && yPos == 0)
            {
                score += 2;
            }
            //anywhere on grid except corners
            else {
                score += 1;
            }
            return score;
        }
    }
}
