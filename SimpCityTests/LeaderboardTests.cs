using System;
using System.Linq;
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

        /// <summary>
        /// Ensures position finding functionality. Leaderboard has space for new records.
        /// </summary>
        [TestMethod]
        public void ScorePointPosition_FindsCorrectPosition_WhenCalledWithSpace() {
            var glb = new GlobalLeaderboard(null);
            var lb = glb.GetLeaderboard(4, 4);

            lb.AddScore(new LeaderboardScore {
                PlayerName = "Player1",
                Score = 3,
                Time = new DateTime()
            });

            lb.AddScore(new LeaderboardScore {
                PlayerName = "Player2",
                Score = 2,
                Time = new DateTime()
            });

            lb.AddScore(new LeaderboardScore {
                PlayerName = "Player3",
                Score = 2,
                Time = new DateTime()
            });

            // Score point 4 should be first pos
            Assert.IsTrue(lb.ScorePointPosition(4) == 1);

            // Score point 2 should be 4th pos
            Assert.IsTrue(lb.ScorePointPosition(2) == 4);
        }

        /// <summary>
        /// Ensures position finding functionality (non-eligible score). Leaderboard has no space
        /// for new records.
        /// </summary>
        [TestMethod]
        public void ScorePointPosition_ReturnsZero_WhenCalledWithNoSpace() {
            var glb = new GlobalLeaderboard(null);
            var lb = glb.GetLeaderboard(4, 4);

            int lowestScore = 30;

            // Create lowest score
            lb.AddScore(new LeaderboardScore {
                PlayerName = "Player1",
                Score = lowestScore,
                Time = new DateTime()
            });

            // Create 9 more of such records to fill up space
            for (int sc = lowestScore; sc < lowestScore + 9; sc++) {
                lb.AddScore(new LeaderboardScore {
                    PlayerName = "Player" + sc,
                    Score = sc,
                    Time = new DateTime()
                });
            }

            // Score point 30 should be non-eligble (out of space)
            Assert.IsTrue(lb.ScorePointPosition(4) == 0);
        }

        /// <summary>
        /// Ensures AddScore removes the last score record, when a higher score is added to a full
        /// leaderboard, freeing up space.
        /// </summary>
        [TestMethod]
        public void AddScore_RemovesLastScore_WhenCalledWithHigherScoreAndNoSpace() {
            var glb = new GlobalLeaderboard(null);
            var lb = glb.GetLeaderboard(4, 4);

            // Create 9 records to fill up space
            for (int i = 0; i < 9; i++) {
                lb.AddScore(new LeaderboardScore {
                    PlayerName = "Player" + i,
                    Score = 1,
                    Time = new DateTime()
                });
            }

            // Add the 10th player who should be removed later
            lb.AddScore(new LeaderboardScore {
                PlayerName = "PlayerShouldBeRemoved",
                Score = 1,
                Time = new DateTime()
            });

            // Sanity check that our PlayerShouldBeRemoved is the last
            Assert.IsTrue(lb.FlattenScores().Last().PlayerName == "PlayerShouldBeRemoved");

            // Sanity check that our new score should be first pos
            Assert.IsTrue(lb.ScorePointPosition(2) == 1);

            // Add a higher score
            lb.AddScore(new LeaderboardScore {
                PlayerName = "PlayerAtas",
                Score = 2,
                Time = new DateTime()
            });

            // "PlayerShouldBeRemoved" should no longer exist
            Assert.IsTrue(lb.FlattenScores().Last().PlayerName != "PlayerShouldBeRemoved");
        }
    }
}
