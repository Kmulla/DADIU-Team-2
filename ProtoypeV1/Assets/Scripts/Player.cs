﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	int currentScore;
	public int health;
	public List<DifficultyProperties> Difficulties = new List<DifficultyProperties>();
	public bool FadeBetweenDifficulties = true;
	GameObject scoreText;

	[System.Serializable]
	public class DifficultyProperties
	{
		public float TimePosition = 0.0f;
		public float MoleSpeed = 1.0f;
		public float MoleStayTime = 4.0f;
	};

	// Use this for initialization
	void Start () {
		ParseDifficultyProperties ();
		currentScore = 0;
		UpdateHealthHUD ();
		UpdateScoreHUD ();
	}

	public void IncreaseScore(int score)
	{
		currentScore += score;
		UpdateScoreHUD ();
	}

	public void LoseHealth()
	{
		health--;
		UpdateHealthHUD ();
	}

	private void ParseDifficultyProperties()
	{
		bool HasStartKey = false;
		foreach (DifficultyProperties prop in Difficulties)
		{
			if (prop.TimePosition == 0.0f)
			{
				HasStartKey = true;
				break;
			}
		}
		if (!HasStartKey)
		{
			DifficultyProperties dp = new DifficultyProperties();
			dp.MoleSpeed = 1.0f;
			dp.MoleStayTime = 1.0f;
			dp.TimePosition = 0.0f;
			Difficulties.Add (dp);
		}
	}

	public float GetDifficultySpeed()
	{
		DifficultyProperties from, to;
		float alpha = 0.0f;
		if (!GetDifficultyPropertyAtPosition (Time.timeSinceLevelLoad, out from, out to, out alpha))
			return 0.0f;
		return Mathf.Lerp (from.MoleSpeed, to.MoleSpeed, alpha);
	}


	private bool GetDifficultyPropertyAtPosition(float p, out DifficultyProperties from, out DifficultyProperties to, out float alpha)
	{
		DifficultyProperties lowerBound = new DifficultyProperties();
		DifficultyProperties upperBound = new DifficultyProperties();
		bool hasLowerBound = false, hasUpperBound = false;

		foreach (DifficultyProperties prop in Difficulties)
		{
			float time = prop.TimePosition;

			// this is prior to this current time
			if (time <= p)
			{
				// we already have a better lower bound
				if (hasLowerBound && lowerBound.TimePosition > time) continue;
				lowerBound = prop;
				hasLowerBound = true;
			}
			// this is the target difficulty
			else
			{
				if (hasUpperBound && upperBound.TimePosition < time) continue;
				upperBound = prop;
				hasUpperBound = true;
			}
		}
		from = lowerBound;
		to = lowerBound;
		alpha = 0.0f;

		// we need a lower bound, if none, abort
		if (!hasLowerBound)
				return false;

		// if no upper bound, we are beyond the last point. Use lower bound.
		// Same behaviour if no fade
			if (!hasUpperBound || !FadeBetweenDifficulties)
			{
				return true;
			}

		alpha = (p - lowerBound.TimePosition) / (upperBound.TimePosition - lowerBound.TimePosition);
		to = upperBound;
		return true;
	}

	private void UpdateScoreHUD()
	{
		scoreText = GameObject.Find ("ScoreText");
		GUIText guiText = ((GUIText)scoreText.GetComponent (typeof(GUIText)));
		guiText.text = Localization.Instance.GetString(Localization.LocKey.Score)+": " + currentScore;
	}

	private void UpdateHealthHUD()
	{
		scoreText = GameObject.Find ("HealthText");
		GUIText guiText = ((GUIText)scoreText.GetComponent (typeof(GUIText)));
		guiText.text = Localization.Instance.GetString(Localization.LocKey.Health) + ": " + health;
	}
}