﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour {
	SpriteRenderer renderer;
	Color currentColor = Color.white;
	// Use this for initialization
	void Start () {
		renderer = GetComponent<SpriteRenderer>();
	}

	public void SetTemporaryColor( Color color ) {
		renderer.color = color;
	}

	public void RestoreColor() {
		renderer.color = currentColor;
	}

	public void SetColorByAction( Actions.ButtonActions action ) {
		switch( action ) {
			case Actions.ButtonActions.JUMP:
				currentColor = Color.white;
				break;
			case Actions.ButtonActions.DASH:
				currentColor = Color.red;
				break;
			case Actions.ButtonActions.FLAP:
				currentColor = Color.yellow;
				break;
			case Actions.ButtonActions.SHIELD:
				currentColor = Color.blue;
				break;
			default:
				Debug.Assert( false, "Attempting to set a non-existant action." );
				currentColor = Color.white;
				break;
		}
		renderer.color = currentColor;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}