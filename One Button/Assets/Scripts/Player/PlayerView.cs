using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour {
	SpriteRenderer spriteRenderer;
	Color currentColor = Color.white;
	bool usingTemporaryColor = false;

	// Use this for initialization
	void Start () {
	}

	public void SetTemporaryColor( Color color ) {
		spriteRenderer.color = color;
		usingTemporaryColor = true;
	}

	public void RestoreColor() {
		spriteRenderer.color = currentColor;
		usingTemporaryColor = false;
	}

	public SpriteRenderer GetRenderer() {
		if( spriteRenderer == null ) {
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
		return spriteRenderer;
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
		GetRenderer().color = currentColor;
	}
	
	// Update is called once per frame
	void Update () {
		if ( Actions.IsShielding() ) {
			SetTemporaryColor( Color.grey );
		} else if( usingTemporaryColor ) {
			RestoreColor();
		}
	}
}
