using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimpCity {
    /// <summary>
    /// Represents an individual score on the leaderboard.
    /// </summary>
    public class LeaderboardScore {
        /// <summary>
        /// The score obtained.
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string PlayerName { get; set; }
        /// <summary>
        /// The time that the score was generated.
        /// </summary>
        public DateTime Time { get; set; }
    }


    /// <summary>
    /// Represents a SimpCity leaderboard for a certain grid dimension.
    /// </summary>
    public class Leaderboard {
        /// <summary>
        /// The global leaderboard hosting this leaderboard.
        /// </summary>
        public GlobalLeaderboard GlobalLeaderboard { get; set; }
        /// <summary>
        /// The leaderboard's grid width.
        /// </summary>
        public int GridWidth { get; set; }
        /// <summary>
        /// The leaderboard's grid height.
        /// </summary>
        public int GridHeight { get; set; }

        /// <summary>
        /// Construct a new leaderboard from specified grid dimensions.
        /// </summary>
        public Leaderboard(GlobalLeaderboard globalLb, int gridWidth, int gridHeight) {
            GlobalLeaderboard = globalLb;
            GridWidth = gridWidth;
            GridHeight = gridHeight;

        }


        /// <summary>
        /// Adds a score into the leaderboard, and saves the change.
        /// </summary>
        public void AddScore(LeaderboardScore score) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the flattened (sorted) list of scores on the leaderboard.
        /// </summary>
        /// <returns></returns>
        public List<LeaderboardScore> FlattenScores() {
            throw new NotImplementedException();
        }

    }


    /// <summary>
    /// Represents the SimpCity global leaderboard.
    /// </summary>
    public class GlobalLeaderboard {
        /// <summary>
        /// File path to use for the global leaderboard persistent data.
        /// </summary>
        public string FilePath { get; protected internal set; }

        /// <summary>
        /// Whether the global leaderboard should save persistent data.
        /// </summary>
        public bool IsFileSaving {
            get { return FilePath != null; }
        }

        /// <param name="filePath">
        /// File path to use for the global leaderboard persistent data.
        /// If <i>null</i>, it can be used to disable file saving.
        /// </param>
        public GlobalLeaderboard(string filePath) {

        }


        /// <summary>
        /// Loads global leaderboard data from a JSON string.
        /// </summary>
        protected internal void LoadJsonString(string jsonString) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads data from the file.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void Load() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts global leaderboard into JSON string for saving.
        /// </summary>
        /// <returns></returns>
        protected internal string ToJsonString() {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void Save() {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Retrives the leaderboard for specified grid dimensions
        /// </summary>
        public Leaderboard GetLeaderboard(int gridWidth, int gridHeight) {
            throw new NotImplementedException();
        }
    }
}
