using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;

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

    public class JsonLeaderboard {
        /// <summary>
        /// The leaderboard's grid width.
        /// </summary>
        public int GridWidth { get; set; }
        /// <summary>
        /// The leaderboard's grid height.
        /// </summary>
        public int GridHeight { get; set; }
        /// <summary>
        /// A sorted list of score records.
        /// </summary>
        public List<LeaderboardScore> Records { get; set; }
    }

    /// <summary>
    /// Represents a SimpCity leaderboard for a certain grid dimension.
    /// </summary>
    public class Leaderboard {
        /// <summary>
        /// The main internal data structure of the leaderboard.
        /// </summary>
        private readonly SortedDictionary<int, List<LeaderboardScore>> lb;

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
        /// The total count of score records in the leaderboard (cached).
        /// </summary>
        public int TotalScoresCount { get; set; }

        /// <summary>
        /// Construct a new leaderboard from specified grid dimensions.
        /// </summary>
        public Leaderboard(GlobalLeaderboard globalLb, int gridWidth, int gridHeight) {
            GlobalLeaderboard = globalLb;
            GridWidth = gridWidth;
            GridHeight = gridHeight;

            // Initialize an empty sorted dict
            lb = new SortedDictionary<int, List<LeaderboardScore>>();
            TotalScoresCount = 0;
        }

        /// <summary>
        /// Construct a leaderboard from JSON leaderboard.
        /// </summary>
        public Leaderboard(GlobalLeaderboard globalLb, JsonLeaderboard jsonLb) {
            GlobalLeaderboard = globalLb;
            GridWidth = jsonLb.GridWidth;
            GridHeight = jsonLb.GridHeight;

            // Create sorted dictionary for quicker search
            lb = new SortedDictionary<int, List<LeaderboardScore>>();
            TotalScoresCount = 0;

            // Unload into memory
            foreach (LeaderboardScore score in jsonLb.Records) {
                addLbScore(score);
            }
        }

        /// <summary>
        /// Adds a score into the sorted dict.
        /// </summary>
        private void addLbScore(LeaderboardScore score) {
            List<LeaderboardScore> scoreList;

            if (!lb.ContainsKey(score.Score)) {
                scoreList = new List<LeaderboardScore>();
                lb.Add(score.Score, scoreList);
            } else {
                scoreList = lb[score.Score];
            }

            // Lower priority; Add to the back
            scoreList.Add(score);
            TotalScoresCount++;
        }

        /// <summary>
        /// Adds a score into the leaderboard, if eligible, and saves the change.
        /// </summary>
        /// <param name="force">Whether the check to <i>ScorePointPosition</i> should be skipped.</param>
        public void AddScore(LeaderboardScore score, bool force = false) {
            if (!force && ScorePointPosition(score.Score) == 0) return;
            addLbScore(score);
            GlobalLeaderboard.Save();
        }

        /// <summary>
        /// Retrieves the position of the score on the leaderboard. The position returned starts counting from 1.
        /// </summary>
        /// <returns>The projected position on the leaderboard. <i>0</i> if not eligible.</returns>
        public uint ScorePointPosition(int scorePoint) {
            // Empty leaderboard.
            if (lb.Count == 0) return 1;

            // LINQ should hopefully allow us to retrieve min and max within O(log n) time..
            // See https://github.com/dotnet/runtime/issues/22912

            uint currPos = 1;
            // Traverse in reverse, starting from the highest score.
            foreach (var kv in lb.Reverse()) {
                if (scorePoint > kv.Key) {
                    // Highest position
                    return currPos;
                }

                // Skip pos to the end of the list
                currPos += (uint)kv.Value.Count;
                if (scorePoint == kv.Key) {
                    return currPos;
                }
            }

            // Not eligible
            return currPos;
        }

        /// <summary>
        /// Retrieves the flattened (sorted) list of scores on the leaderboard.
        /// </summary>
        public List<LeaderboardScore> FlattenScores() {
            // Convert sorted dict back into its raw, flattened form
            List<LeaderboardScore> flatLb = new List<LeaderboardScore>();
            foreach (var kv in lb.Reverse()) {
                foreach (LeaderboardScore score in kv.Value) {
                    flatLb.Add(score);
                }
            }
            return flatLb;
        }

        /// <summary>
        /// Converts leaderboard into JSON structure for saving.
        /// </summary>
        public JsonLeaderboard ToJsonLeaderboard() {
            return new JsonLeaderboard {
                GridWidth = GridWidth,
                GridHeight = GridHeight,
                Records = FlattenScores()
            };
        }

        /// <summary>
        /// Displays the leaderboard on the console.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void Display() {
            // Composite formatting with alignment
            string rowAlignment = "{0,3} {1,-19} {2,7}";

            // Header
            Console.WriteLine($"{new string('-', 9)} HIGH SCORES {new string('-', 9)}");
            Console.WriteLine(string.Format(rowAlignment, "Pos", "Player", "Score"));
            Console.WriteLine(string.Format(rowAlignment, "---", "------", "-----"));

            var scores = FlattenScores();
            for (int i = 0; i < scores.Count; i++) {
                LeaderboardScore score = scores[i];
                Console.WriteLine(string.Format(rowAlignment, $"{i}.", score.PlayerName, score.Score));
            }

            // Enclosure
            Console.WriteLine(new string('-', 31));
        }
    }

    public class JsonGlobalLeaderboard {
        public List<JsonLeaderboard> Leaderboards { get; set; }
    }

    /// <summary>
    /// Represents the SimpCity global leaderboard.
    /// </summary>
    public class GlobalLeaderboard {
        /// <summary>
        /// Internal dictionary of loaded leaderboards of defined grid dimensions (serialized string with <i>slugLeaderboardKey</i>).
        /// </summary>
        private readonly Dictionary<string, Leaderboard> leaderboards;

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
            leaderboards = new Dictionary<string, Leaderboard>();
            FilePath = filePath;
            Load();
        }

        private static string slugLeaderboardKey(int gridWidth, int gridHeight) {
            return $"lb_{gridWidth}_{gridHeight}";
        }

        /// <summary>
        /// Loads global leaderboard data from a JSON string.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">json is null.</exception>
        /// <exception cref="System.Text.Json.JsonException">
        /// The JSON is invalid. -or- TValue is not compatible with the JSON. -or- There
        /// is remaining data in the string beyond a single JSON value.
        /// </exception>
        protected internal void LoadJsonString(string jsonString) {
            List<JsonLeaderboard> jsonLbs = JsonSerializer.Deserialize<JsonGlobalLeaderboard>(jsonString).Leaderboards;

            // Load JSON leaderboard into memory
            foreach (JsonLeaderboard jsonLb in jsonLbs) {
                string key = slugLeaderboardKey(jsonLb.GridWidth, jsonLb.GridHeight);
                Leaderboard lb = new Leaderboard(this, jsonLb);
                // Override ref
                leaderboards[key] = lb;
            }
        }

        /// <summary>
        /// Loads data from the file.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">json is null.</exception>
        /// <exception cref="System.Text.Json.JsonException">
        /// The JSON is invalid. -or- TValue is not compatible with the JSON. -or- There
        /// is remaining data in the string beyond a single JSON value.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public void Load() {
            // No-op
            if (!IsFileSaving) return;
            // Nothing to load if file doesn't exist
            if (!File.Exists(FilePath)) return;

            string jsonString;
            // Stream is automatically closed at the end of the scope.
            using (StreamReader fileStream = new StreamReader(FilePath)) {
                jsonString = fileStream.ReadToEnd();
            }

            LoadJsonString(jsonString);
        }

        /// <summary>
        /// Converts global leaderboard into JSON string for saving.
        /// </summary>
        protected internal string ToJsonString() {
            // Convert to JSON leaderboard
            List<JsonLeaderboard> jsonLbs = new List<JsonLeaderboard>();
            foreach (Leaderboard lb in leaderboards.Values) {
                jsonLbs.Add(lb.ToJsonLeaderboard());
            }

            return JsonSerializer.Serialize(new JsonGlobalLeaderboard {
                Leaderboards = jsonLbs
            }, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Saves data into the file.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void Save() {
            // No-op
            if (!IsFileSaving) return;

            // Stream is automatically closed at the end of the scope.
            using (StreamWriter fileStream = new StreamWriter(FilePath, false)) {
                fileStream.Write(ToJsonString());
            }
        }

        /// <summary>
        /// Retrives the leaderboard for specified grid dimensions
        /// </summary>
        public Leaderboard GetLeaderboard(int gridWidth, int gridHeight) {
            string key = slugLeaderboardKey(gridWidth, gridHeight);

            Leaderboard lb;
            if (!leaderboards.ContainsKey(key)) {
                lb = new Leaderboard(this, gridWidth, gridHeight);
                // Add ref
                leaderboards.Add(key, lb);
            } else {
                lb = leaderboards[key];
            }

            return lb;
        }
    }
}
