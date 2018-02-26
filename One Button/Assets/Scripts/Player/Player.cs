using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	Actions.ButtonActions currentAction;
	PlayerView playerView;

	// Use this for initialization
	void Start () {
		playerView = GetComponent<PlayerView>();
		SetCurrentAction( Actions.ButtonActions.JUMP );
	}

	public void SetCurrentAction( Actions.ButtonActions action ) {
		currentAction = action;
		playerView.SetColorByAction( currentAction );
	}

	public Actions.ButtonActions GetCurrentAction() {
		return currentAction;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
