using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpCity;

namespace SimpCityTests {
    /// <summary>
    /// Tests the leaderboard.
    /// </summary>
    [TestClass]
    public class LeaderboardTests {
        /// <summary>
        /// Ensures add score functionality.
        /// </summary>
        [TestMethod]
        public void AddScore_RunsCorrectly_WhenCalled() {
            Leaderboard lb = new GlobalLeaderboard(null).GetLeaderboard(4, 4);

            lb.AddScore(new LeaderboardScore {
                PlayerName = "Player1",
                Score = 2,
                Time = new DateTime()
            });

            var scores = lb.FlattenScores();

            Assert.IsTrue(scores.Count == 1);

            // Check data integrity
            Assert.IsTrue(scores[0].PlayerName == "Player1" && scores[0].Score == 2);
        }

        /// <summary>
        /// Ensures the leaderboard row with a similar score to others, is added behind the others with lower priority.
        /// e.g.
        /// pos1: Player 1, score=20
        /// pos2: Player 2, score=20 (added after player 1)
        /// </summary>
        [TestMethod]
        public void FlattenScores_PutsNewestScoreBehind_WhenScSamePoints() {
            Leaderboard lb = new GlobalLeaderboard(null).GetLeaderboard(4, 4);

            lb.AddScore(new LeaderboardScore {
                PlayerName = "Player1",
                Score = 2,
                Time = new DateTime()
            });

            lb.AddScore(new LeaderboardScore {
                PlayerName = "Player2",
                Score = 2,
                Time = new DateTime()
            });

            var scores = lb.FlattenScores();

            Assert.IsTrue(scores.Count == 2);

            // Player 2 must be behind player 1
            Assert.IsTrue(scores[0].PlayerName == "Player1" && scores[1].PlayerName == "Player2");
        }


        /// <summary>
        /// Ensures JSON functionality of the Leaderboard.
        /// </summary>
        [TestMethod]
        public void JsonFeature_HasNoLossOfInformation_WhenDecodingAndEncoding() {
            var glb = new GlobalLeaderboard(null);
            var lb = glb.GetLeaderboard(4, 4);

            lb.AddScore(new LeaderboardScore {
                PlayerName = "Player3",
                Score = 3,
                Time = new DateTime()
            });

            lb.AddScore(new LeaderboardScore {
                PlayerName = "Player1",
                Score = 2,
                Time = new DateTime()
            });

            lb.AddScore(new LeaderboardScore {
                PlayerName = "Player2",
                Score = 2,
                Time = new DateTime()
            });

            string encoded = glb.ToJsonString();

            // Create a new glb2 and load it with the exported JSON from glb
            var glb2 = new GlobalLeaderboard(null);
            glb2.LoadJsonString(encoded);

            // A simple sanity check in the memory. There should be 3 scores in it.
            Assert.IsTrue(glb2.GetLeaderboard(4, 4).FlattenScores().Count == 3);

            // Check data integrity by comparing the exported JSON strings of the two objects
            Assert.IsTrue(glb2.ToJsonString() == encoded);
        }
    }
}
