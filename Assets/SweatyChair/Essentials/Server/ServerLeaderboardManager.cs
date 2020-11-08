using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using SweatyChair.UI;

namespace SweatyChair
{

	public class ServerLeaderboardManager : Singleton<ServerLeaderboardManager>
	{

		private const string PREF_CURRENT_LEADERBOARD_END_DATE_TIME = "ServerLeaderboardCurrentLeaderboardEndDateTime";

		public static event UnityAction<bool> toggledEvent;
		public static event UnityAction initializedEvent;
		public static event UnityAction scoresUpdatedEvent;
		public static event UnityAction highscoresUpdatedEvent;
		public static event UnityAction newTriggeredEvent;
		public static event UnityAction<int> myScoreUpdateEvent;

		public static List<ServerLeaderboardReward> rankRewards = new List<ServerLeaderboardReward>();
		public static List<ServerLeaderboardReward> scoreRewards = new List<ServerLeaderboardReward>();
		public static List<ServerLeaderboardReward> highscoreRankRewards = new List<ServerLeaderboardReward>();
		public static List<ServerLeaderboardScore> scores = new List<ServerLeaderboardScore>();
		public static List<ServerLeaderboardScore> highscores = new List<ServerLeaderboardScore>();

		public static ServerLeaderboardScore playerLeaderboardScore;
		public static ServerLeaderboardScore playerLeaderboardHighScore;

		public static bool isInitialized { get; private set; }

		public static bool hasNewLeaderboard { get; private set; }

		public static bool hasReward { get; private set; }

		public static int myScore { get; private set; }

		// Reset DateTime in UTC
		public static DateTime currentLeaderboardEndDateTime { get; private set; }

		private static int _myHighscore;

		// Last ranks, used to rewards
		private static int _lastRank = -1, _lastHighscoreRank = -1;
		// Current rewarded score
		private static int _rewardedScore = 0;

		private static UnityAction _checkInitializedSucceedAction;
		private static bool _showFailedOnInitialize;

		private void Start()
		{
			GetCurrentLeaderboard();
		}

		#region Requests

		public static bool CheckInitialized(UnityAction onSucceed)
		{
			if (!isInitialized) {
				Loading.Display();
				GetCurrentLeaderboard(true);
				_checkInitializedSucceedAction = onSucceed;
				return false;
			}
			return true;
		}

		public static void GetCurrentLeaderboard(bool showFailed = false)
		{
			// TODO: cache and download only changed
			_showFailedOnInitialize = showFailed;
			ServerManager.Get(
				"leaderboards/current?game_id={gameId}",
				OnGetCurrentLeaderboardComplete,
				OnGetCurrentLeaderboardFailed
			);
		}

		public static void GetScores()
		{
			ServerManager.Get(
				"leaderboards/scores?game_id={gameId}&player_id={playerId}",
				OnGetScoresComplete
			);
		}

		public static void GetHighscores()
		{
			ServerManager.Get(
				"leaderboards/highscores?game_id={gameId}&player_id={playerId}",
				OnGetHighscoresComplete
			);
		}

		public static void GetRewards()
		{
			ServerManager.Get(
				"leaderboards/rewards?game_id={gameId}&player_id={playerId}",
				OnGetRewardsComplete
			);
		}

		public static void PostScore(int score)
		{
			SetMyScore(myScore + score);
			var dict = new Dictionary<string, string> { { "score", score.ToString() }, { "player_id", "{playerId}" } };
			ServerManager.Post(
				"leaderboards/score?game_id={gameId}&hack=1",
				dict,
				OnPostScoresComplete
			);
		}

		public static void PostHighscore(int highscore)
		{
			if (highscore <= _myHighscore)
				return;
			_myHighscore = highscore;
			var dict = new Dictionary<string, string> { { "highscore", highscore.ToString() }, { "player_id", "{playerId}" } };
			ServerManager.Post(
				"leaderboards/score?game_id={gameId}&hack=1",
				dict
			);
		}

		public static void PostScores(int score, int highscore)
		{
			SetMyScore(myScore + score);
			var dict = new Dictionary<string, string> { { "score", score.ToString() }, { "player_id", "{playerId}" } };
			if (highscore > _myHighscore) {
				_myHighscore = highscore;
				dict["highscore"] = _myHighscore.ToString();
			}
			ServerManager.Post(
				"leaderboards/score?game_id={gameId}&hack=1",
				dict,
				OnPostScoresComplete
			);
		}

		private static void PostRewards()
		{
			var dict = new Dictionary<string, string> { { "player_id", "{playerId}" } };
			if (_lastRank > 0)
				dict["rank"] = _lastRank.ToString();
			if (_lastHighscoreRank > 0)
				dict["highscore_rank"] = _lastHighscoreRank.ToString();
			ServerManager.Post(
				"leaderboards/player_rewards?game_id={gameId}",
				dict,
				OnPostRewardsComplete
			);
		}

		#endregion

		#region Callbacks

		private static void OnGetCurrentLeaderboardComplete(Hashtable ht)
		{
			currentLeaderboardEndDateTime = DataUtils.GetDateTime(ht["end_at"] as string).AddHours(24); // Server only return date and causing the DateTime to be YYmmdd 00:00:00, simply add 1 day here
			hasNewLeaderboard = DateTimeUtils.GetPlayerPrefs(PREF_CURRENT_LEADERBOARD_END_DATE_TIME) != currentLeaderboardEndDateTime;
			if (hasNewLeaderboard && newTriggeredEvent != null)
				newTriggeredEvent();

			if (ht.ContainsKey("utc")) // Set the server time
				UnbiasedTime.Instance.SetNetworkDateTime(DataUtils.GetDateTime(ht["utc"] as string));

			// Ranking rewards
			SetRewards((ht["rank_rewards"] as ArrayList).ToArray(), "rank", rankRewards);
			// Collect rewards
			SetRewards((ht["score_rewards"] as ArrayList).ToArray(), "score", scoreRewards);

			if (ht.ContainsKey("highscore_rank_rewards"))
				SetRewards((ht["highscore_rank_rewards"] as ArrayList).ToArray(), "highscore_rank", highscoreRankRewards);
			else // Use rank rewards if highscore rank rewards not presented
				highscoreRankRewards = rankRewards;

			isInitialized = true;
			if (initializedEvent != null)
				initializedEvent();
			if (_checkInitializedSucceedAction != null)
				_checkInitializedSucceedAction();

			// Get scores and highscores
			if (ServerPlayerManager.isRegistered) {
				GetScores();
				GetHighscores();
				GetRewards();
			} else {
				ServerPlayerManager.registerSucceed += GetScores;
				ServerPlayerManager.registerSucceed += GetHighscores;
				ServerPlayerManager.registerSucceed += GetRewards;
			}

			Loading.Hide();
			_showFailedOnInitialize = false;
		}

		private static void OnGetCurrentLeaderboardFailed(Hashtable errorHt)
		{
			if (_showFailedOnInitialize) {
				new Message {
					format = MessageFormat.Confirm,
					title = CommonTexts.MSG_CONNECTION_ERROR_TITLE,
					content = CommonTexts.MSG_CONNECTION_ERROR_CONTENT
				}.Show();
			}
			Loading.Hide();
		}

		private static void OnGetScoresComplete(Hashtable[] htArray)
		{
			scores.Clear();

			foreach (Hashtable ht in htArray) {

				int playerId = Convert.ToInt32(ht["player_id"]);
				string name = ht["name"] as string;
				int rank = Convert.ToInt32(ht["rank"]);
				int score = Convert.ToInt32(ht["score"]);

				ServerLeaderboardScore serverLeaderboardScore = new ServerLeaderboardScore(playerId, rank, name, score);
				scores.Add(serverLeaderboardScore);

				if (playerId == ServerPlayerManager.playerId) {
					playerLeaderboardScore = serverLeaderboardScore;
					SetMyScore(score);
					Debug.LogFormat("ServerLeaderboardManager:OnGetScoreComplete - playerLeaderboardScore={0}", playerLeaderboardScore);
				}

			}

			if (scoresUpdatedEvent != null)
				scoresUpdatedEvent();
		}

		private static void OnGetHighscoresComplete(Hashtable[] htArray)
		{
			highscores.Clear();

			foreach (object obj in htArray) {

				Hashtable ht = (Hashtable)obj;

				int playerId = Convert.ToInt32(ht["player_id"]);
				string name = ht["name"] as string;
				int rank = Convert.ToInt32(ht["rank"]);
				int highscore = Convert.ToInt32(ht["highscore"]);

				ServerLeaderboardScore leaderboardHighScore = new ServerLeaderboardScore(playerId, rank, name, highscore);
				highscores.Add(leaderboardHighScore);

				if (playerId == ServerPlayerManager.playerId) {
					_myHighscore = highscore;
					playerLeaderboardHighScore = leaderboardHighScore;
					Debug.LogFormat("ServerLeaderboardManager:OnGetHighscoresComplete - playerLeaderboardHighScore={0}", playerLeaderboardHighScore);
				}

			}
			if (highscoresUpdatedEvent != null)
				highscoresUpdatedEvent();
		}

		private static void OnGetRewardsComplete(Hashtable ht)
		{
			if (ht.ContainsKey("rewarded_score"))
				_rewardedScore = Convert.ToInt32(ht["rewarded_score"]);

			// Check rewards
			bool rewardCheck = false;
			if (ht.ContainsKey("last_rank")) {
				_lastRank = Convert.ToInt32(ht["last_rank"]);
				rewardCheck = true;
			}

			if (ht.ContainsKey("last_highscore_rank")) {
				_lastHighscoreRank = Convert.ToInt32(ht["last_highscore_rank"]);
				rewardCheck = true;
			}

			if (!rewardCheck) { // Check score rewards
				if (ht.ContainsKey("rewarded_score")) { // Has uploaded server rewarded score
					foreach (var scoreReward in scoreRewards) {
						if (scoreReward.key > _rewardedScore && scoreReward.key <= myScore) {
							rewardCheck = true;
							break;
						}
					}
				} else { // No submited server rewarded score, check local
					foreach (var scoreReward in scoreRewards) {
						if (scoreReward.key <= myScore) {
							rewardCheck = true;
							break;
						}
					}
				}
			}

			if (rewardCheck) {
				hasReward = true;
				if (newTriggeredEvent != null)
					newTriggeredEvent();
			}
		}

		private static void OnPostScoresComplete(Hashtable ht)
		{
			SetMyScore(Convert.ToInt32(ht["score"]));
			// Check if any score reward can obtain
			foreach (var scoreReward in scoreRewards) {
				if (scoreReward.key > _rewardedScore) {
					if (scoreReward.key <= myScore) {
						hasReward = true;
						break;
					}
				} else {
					break;
				}
			}
			Debug.LogFormat("ServerLeaderboardManager:OnPostScoresComplete - hasReward={0}, myScore={1}", hasReward, myScore);
			if (hasReward && newTriggeredEvent != null)
				newTriggeredEvent();
			// Update ranking everything after posting new scores
			GetScores();
			GetHighscores();
		}

		public static void SetMyScore()
		{
			SetMyScore(myScore);
		}

		private static void SetMyScore(int score)
		{
			myScore = score;
			if (myScoreUpdateEvent != null)
				myScoreUpdateEvent(myScore);
		}

		private static void OnPostRewardsComplete(Hashtable dump) // No response needed
		{
			if (myScore > _rewardedScore) { // Score rewards show last, so add it first if any
				for (int i = scoreRewards.Count - 1; i > 0; i--) {
					var scoreReward = scoreRewards[i];
					if (scoreReward.key <= myScore) {
						if (scoreReward.key > _rewardedScore) {
							scoreReward.reward.Claim(LocalizeUtils.GetFormat(TermCategory.Leaderboard, "Collect Rewards: Tier {0}", i + 1));
							_rewardedScore = scoreReward.key;
						}
					} else {
						break;
					}
				}
			}
			if (_lastRank > 0) { // Collect ranking rewards show second
				foreach (var rankReward in rankRewards) {
					if (_lastRank <= rankReward.key) {
						rankReward.reward.Claim(LocalizeUtils.GetFormat(TermCategory.Leaderboard, "Collect Ranking Rewards: #{0}", _lastRank));
						break;
					}
				}
				_lastRank = -1;
			}
			if (_lastHighscoreRank > 0) { // Highscore ranking rewards show first
				foreach (var highscoreRank in highscoreRankRewards) {
					if (_lastHighscoreRank <= highscoreRank.key) {
						highscoreRank.reward.Claim(LocalizeUtils.GetFormat(TermCategory.Leaderboard, "Wave Ranking Rewards: #{0}", _lastHighscoreRank));
						break;
					}
				}
				_lastHighscoreRank = -1;
			}
			hasReward = false;
		}

		#endregion

		#region Set data

		private static void SetRewards(object[] objArray, string keyName, List<ServerLeaderboardReward> rewards)
		{
			rewards.Clear();

			ServerLeaderboardReward lastReward = null;

			foreach (var obj in objArray) {

				Hashtable ht = (Hashtable)obj;

				ItemType itemType = (ItemType)Convert.ToInt32(ht["type"]);
				int amount = Convert.ToInt32(ht["amount"]);
				SerializableItem item = new SerializableItem(itemType, amount);

				int itemId = Convert.ToInt32(ht["item_id"]);
				// TODO: Hack mannually change christmas rewards for cny here
				if (itemId == 99) // Fire Cricket I replace Ice Rockfall III
					itemId = 109;
				else if (itemId == 103) // Fire Cricket II replace Ice Bomb III
					itemId = 110;
				else if (itemId == 107) // Fire Cricket IIII replace Ice Maker III
					itemId = 111;
				// End TODO
				if (itemId > 0)
					item.SetId(itemId);

				int key = Convert.ToInt32(ht[keyName]);

				if (lastReward != null && key == lastReward.key) {
					lastReward.reward.AddItem(item);
				} else {
					lastReward = new ServerLeaderboardReward(key, item);
					rewards.Add(lastReward);
				}

			}
		}

		#endregion

		public static void WaitAndShow(float seconds)
		{
			TimeManager.Invoke(Show, seconds);
		}

		private static void Show()
		{
			Toggle(true);
		}

		public static void Toggle(bool doShow = true)
		{
			if (toggledEvent != null)
				toggledEvent(doShow);
			if (doShow) {
				UnsetHasNewLeaderboard();
				DateTime timeNow = DateTimeUtils.UtcNow();
				//				Debug.LogFormat("ServerLeaderboardManager:Toggle - currentLeaderboardEndDateTime={0}, timeNow={1}", currentLeaderboardEndDateTime, timeNow);
				if (currentLeaderboardEndDateTime < timeNow) { // Passed reset date while playing
					Reset();
					return;
				}
				if (hasReward) // Post rewards if there's any
					PostRewards();
			}
		}

		public static void Reset()
		{
			hasReward = false;
			myScore = 0;
			_myHighscore = 0;
			_lastRank = -1;
			_lastHighscoreRank = -1;
			_rewardedScore = 0;
			GetCurrentLeaderboard();
		}

		public static void UnsetHasNewLeaderboard()
		{
			hasNewLeaderboard = false;
			DateTimeUtils.SetPlayerPrefs(PREF_CURRENT_LEADERBOARD_END_DATE_TIME, currentLeaderboardEndDateTime);
		}

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Debug/Server/Leaderboard/Print Parameters")]
		private static void PrintParameters()
		{
			DebugUtils.CheckPlaying(() => {
				DebugUtils.Log(rankRewards, "rankRewards");
				DebugUtils.Log(scoreRewards, "scoreRewards");
				DebugUtils.Log(highscoreRankRewards, "highscoreRankRewards");
				DebugUtils.Log(scores, "scores");
				DebugUtils.Log(highscores, "highscores");
				DebugUtils.Log(isInitialized, "isInitialized");
				DebugUtils.Log(hasNewLeaderboard, "hasNewLeaderboard");
				DebugUtils.Log(hasReward, "hasReward");
				DebugUtils.Log(myScore, "myScore");
				DebugUtils.Log(currentLeaderboardEndDateTime, "currentLeaderboardEndDateTime");
				DebugUtils.Log(_myHighscore, "_myHighscore");
				DebugUtils.Log(_lastRank, "_lastRank");
				DebugUtils.Log(_lastHighscoreRank, "_lastHighscoreRank");
				DebugUtils.Log(_rewardedScore, "_rewardedScore");
			});
		}

		[UnityEditor.MenuItem("Debug/Server/Leaderboard/Set New")]
		private static void SetNew()
		{
			if (Application.isPlaying) {
				hasNewLeaderboard = true;
				newTriggeredEvent?.Invoke();
			} else {
				PlayerPrefs.DeleteKey(PREF_CURRENT_LEADERBOARD_END_DATE_TIME);
			}
		}

#endif

	}

}