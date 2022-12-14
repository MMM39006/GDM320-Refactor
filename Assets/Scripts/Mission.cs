using UnityEngine;
using System.Collections;

[System.Serializable]
public class Mission
{
	public enum MissionType
	{
		Distance,
		DistanceWithNoCoins,
		DistanceWithNoPowerUps,
		FailBetween,
		Coin,
		PowerUps,
		ExtraSpeed,
		Shield,
		Abracadabra,
		Obstacles,
		Revive,
		BirdBrown,
		BirdBody,
		BirdWhite,
		StorkTall
	};

	public enum GoalType
	{
		InOneRun,
		InMultipleRun,
		InShop,
		Other
	};

	public string name = "";

	public MissionType missionType = MissionType.Distance;
	public GoalType goalType = GoalType.InOneRun;

	public string description = "";

	public int valueA = 0, valueB = 0;

	int storedValue = 0, startingValue = 0;

	bool IsComplete = false;

	public void SetStoredValue(int v)
	{
		storedValue = v;
		startingValue = v;
	}

	public int StoredValue()
	{
		return storedValue;
	}

	public void ModifyStoredValue(bool addValue, int ammount)
	{
		if (addValue)
			storedValue += ammount;
		else
			storedValue = startingValue + ammount;
	}

	public void ResetThis()
	{
		storedValue = 0;
	}

	public bool IsCompleted()
	{
		return IsComplete;
	}

	public void Complete()
	{
		IsComplete = true;
	}
}
