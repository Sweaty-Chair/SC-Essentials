using UnityEngine;
using System.Collections;

namespace SweatyChair
{
	
	public class ServerLeaderboardReward
	{
	
		public int key { get; private set; }

		public Reward reward { get; private set; }

		public ServerLeaderboardReward(int key, Reward reward)
		{
			this.key = key;
			this.reward = reward;
		}

		public ServerLeaderboardReward(int key, SerializableItem item)
		{
			Reward reward = new Reward(item);
			this.key = key;
			this.reward = reward;
		}

		public override string ToString()
		{
			return string.Format("[ServerLeaderboardReward: key={0}, reward={1}]", key, reward);
		}

	}

	public class ServerLeaderboardScore
	{

		public int playerId { get; private set; }

		public int rank { get; private set; }

		public string name { get; private set; }

		public int score { get; private set; }

		public ServerLeaderboardScore(int playerId, int rank, string name, int score)
		{
			this.playerId = playerId;
			this.rank = rank;
			this.name = name;
			this.score = score;
		}

		public void SetRank(int newRank)
		{
			rank = newRank;
		}

		public override string ToString()
		{
			return string.Format("[ServerLeaderboardScore: playerId={0}, rank={1}, name={2}, score={3}]", playerId, rank, name, score);
		}

	}

}