﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Settings : MonoBehaviour {

	public List<DifficultyProperties> Difficulties = new List<DifficultyProperties>();
	public bool FadeBetweenDifficulties = true;
	[HideInInspector]
	private bool isPaused = false;
	private static Settings _instance;

	[System.Serializable]
	public class DifficultyProperties
	{
		public float TimePosition = 0.0f;
		public float MoleSpeed = 1.0f;
		public float MoleStayTime = 1.0f;
		public float SpecialMoleOccuranceMultiplier = 1.0f;
	};

	// Singleton istancing
	public static Settings instance
	{
		get
		{
			if (!_instance)
			{
				Settings[] scripts = FindObjectsOfType(typeof(Settings)) as Settings[];
				foreach (Settings script in scripts) {
					_instance = script;
					break;
				}
			}
			return _instance;
		}
	}

	// Use this for initialization
	void Start() {
		ParseDifficultyProperties ();
	}

	public float GetDeltaTime()
	{
		if (isPaused)
			return 0.0f;
		return Time.deltaTime;
	}

	public void SetPause(bool b)
	{
		if (isPaused == b)
			return;
		isPaused = b;
	}
	public void TogglePause() {SetPause(!isPaused);}

	// multiplier for speed
	public float GetDifficultySpeed()
	{
		DifficultyProperties from, to;
		float alpha = 0.0f;
		if (!GetDifficultyPropertyAtPosition (Time.timeSinceLevelLoad, out from, out to, out alpha))
			return 1.0f;
		return Mathf.Lerp (from.MoleSpeed, to.MoleSpeed, alpha);
	}

	// multiplier for stay time
	public float GetDifficultyStayTime()
	{
		DifficultyProperties from, to;
		float alpha = 0.0f;
		if (!GetDifficultyPropertyAtPosition (Time.timeSinceLevelLoad, out from, out to, out alpha))
			return 1.0f;
		return Mathf.Lerp (from.MoleStayTime, to.MoleStayTime, alpha);
	}

	// multiplier for special moles
	public float GetDifficultySpecialMoleMultiplier()
	{
		DifficultyProperties from, to;
		float alpha = 0.0f;
		if (!GetDifficultyPropertyAtPosition (Time.timeSinceLevelLoad, out from, out to, out alpha))
			return 1.0f;
		return Mathf.Lerp (from.SpecialMoleOccuranceMultiplier, to.SpecialMoleOccuranceMultiplier, alpha);
	}

	// adds a 0.0f key as one is needed
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

	// retrieves a from->to lerp between difficulty curves
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
}
