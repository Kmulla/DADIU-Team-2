﻿using UnityEngine;
using System.Collections;

public class Jelly : Mole {

	
	void Start () 
	{
		int size = Random.Range(1, 4);
		Health = size;
		transform.scale = transform.scale*size;
	}

	public override void OnTap()
	{
		//mist liv
		Health--;

	}

	public override void OnDeath()
	{
		//forsvind/eksplodér/bliv skåret over etc
		base.OnDeath();
	}
}
