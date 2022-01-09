using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpCity {
    public enum BuildingTypes {
        Beach,
        Factory,
        Shop



    }

    public delegate CityGridBuilding MakeNewFunc();

    public class BuildingInfo {
        public BuildingTypes Type { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public string Code { get; set; }
        public int CopiesLeft { get; set; }
        public CityGrid Grid { get; set; }
        /// <summary>
        /// The callback to execute to return the building object
        /// </summary>
        public MakeNewFunc MakeNew { get; set; }
    }
}
