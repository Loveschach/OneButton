﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	PlayerView playerView;
	int health = 1;
	public Actions.ButtonActions currentAction;

	// Use this for initialization
	void Start () {
		playerView = GetComponent<PlayerView>();
		SetCurrentAction( currentAction );
	}

	public void SetCurrentAction( Actions.ButtonActions action ) {
		currentAction = action;
		playerView.SetColorByAction( currentAction );
	}

	public Actions.ButtonActions GetCurrentAction() {
		return currentAction;
	}

	void OnCollisionEnter2D ( Collision2D coll ) {
		if( coll.gameObject.tag == "Enemy" ) {
			--health;
		}
	}

	void Die() {
		Destroy( gameObject );
	}

	// Update is called once per frame
	void Update () {
		if( health <= 0 && !Actions.IsShielding() ) {
			Die();
		}
	}
}
