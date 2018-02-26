using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Actions {
	public enum ButtonActions {
		JUMP,
		DASH,
		FLAP,
		SHIELD
	};

	//Jump
	const float JUMP_FORCE = 100f;

	//Flap
	const float FLAP_FORCE = 250f;
	const float FLAP_DELAY = 0.5f;
	const int MAX_FLAPS_SINCE_GROUNDED = 3;
	static int flapsSinceGrounded = 0;
	static float timeSinceLastFlap = Time.time - FLAP_DELAY;

	//Dash
	const float DASH_DISTANCE = 1f;
	const float DASH_TIME = 0.1f;
	const float DASH_DELAY = 0.5f;
	static float timeSinceDash = Time.time - DASH_TIME;

	//Shield Action
	

	public static void ExecuteCurrentAction( Player player, Controller controller ) {
		ButtonActions currentAction = player.GetCurrentAction();
		switch( currentAction ) {
			case ButtonActions.JUMP:
				ExecuteJumpAction( controller );
				break;
			case ButtonActions.DASH:
				ExecuteDashAction( controller );
				break;
			case ButtonActions.FLAP:
				ExecuteFlapAction( controller );
				break;
			default:
				Debug.Assert( false, "Attempting to get force for non-existent action." );
				break;
		}
	}

	public static void PlayerGrounded() {
		flapsSinceGrounded = 0;
	}

	public static bool IsDashing() {
		return Time.time - timeSinceDash < DASH_TIME;
	}

	public static void Update( Controller controller ) {
		if( IsDashing() ) {
			Vector2 direction = controller.GetDirection();
			float dashDistance = Time.deltaTime / DASH_TIME;
			controller.transform.position += new Vector3( direction.x * DASH_DISTANCE * dashDistance, 0, 0 );
		}
	}

	public static void ExecuteDashAction( Controller controller ) {
		if( !IsDashing() && ( Time.time - timeSinceDash > DASH_DELAY ) ) {
			timeSinceDash = Time.time;
		}
	}

	public static void ExecuteFlapAction( Controller controller ) {
		bool flapDelayPassed = ( Time.time - timeSinceLastFlap ) >= FLAP_DELAY;
		bool canFlap = flapsSinceGrounded < MAX_FLAPS_SINCE_GROUNDED;
		if( canFlap && flapDelayPassed ) {
			controller.GetBody().AddForce( new Vector2( 0, FLAP_FORCE ) );
			timeSinceLastFlap = Time.time;
			flapsSinceGrounded += 1;
		}
	}

	public static void ExecuteJumpAction( Controller controller ) {
		if( controller.IsGrounded() ) {
			controller.GetBody().AddForce( new Vector2( 0, JUMP_FORCE ) );
		}
	}	
}
