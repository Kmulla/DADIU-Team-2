﻿using UnityEngine;
using System.Collections;

public class Jelly : Mole {

	
	void Start () 
	{
		Health = Random.Range(1, 4);
	}

	protected override void OnTap()
	{
		//mist liv
		Health--;

	}

	private void UpdateScale()
	{
		transform.localScale = new Vector3(2,2,2) * (0.5f + Health * 0.5f);
	}



	protected override void OnHealthChange()
	{
		UpdateScale ();
	}
}
