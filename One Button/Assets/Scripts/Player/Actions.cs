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

	const float DEFAULT_TIME = -100f;

	//Jump
	const float JUMP_FORCE = 100f;

	//Flap
	const float FLAP_FORCE = 250f;
	const float FLAP_DELAY = 0.5f;
	const int MAX_FLAPS_SINCE_GROUNDED = 3;
	static int flapsSinceGrounded = 0;
	static float timeSinceLastFlap = DEFAULT_TIME;

	//Dash
	const float DASH_DISTANCE = 1f;
	const float DASH_TIME = 0.1f;
	const float DASH_DELAY = 0.5f;
	static float dashStartTime = DEFAULT_TIME;

	//Shield Action
	const float SHIELD_TIME = 2f;
	const float SHIELD_DELAY = 0.2f;
	static float shieldStartTime = DEFAULT_TIME;

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
			case ButtonActions.SHIELD:
				ExecuteShieldAction( controller );
				break;
			default:
				Debug.Assert( false, "Attempting to get force for non-existent action." );
				break;
		}
	}

	public static bool IsActionClick( ButtonActions action ) {
		switch( action ) {
			case ButtonActions.JUMP:
				return false;
			case ButtonActions.DASH:
				return true;
			case ButtonActions.FLAP:
				return true;
			case ButtonActions.SHIELD:
				return true;
			default:
				Debug.Assert( false, "Attempt to check non-existent action." );
				return false;
		}
	}

	static bool IsDefault( float time ) {
		return time == DEFAULT_TIME;
	}

	public static void PlayerGrounded() {
		flapsSinceGrounded = 0;
	}

	public static bool IsDashing() {
		return !IsDefault( dashStartTime ) && ( Time.time - dashStartTime ) < DASH_TIME;
	}

	public static bool IsShielding() {
		return !IsDefault( shieldStartTime ) && ( Time.time - shieldStartTime ) < SHIELD_TIME;
	}

	public static void Update( Controller controller ) {
		if( IsDashing() ) {
			Vector2 direction = controller.GetDirection();
			float dashDistance = Time.deltaTime / DASH_TIME;
			controller.transform.position += new Vector3( direction.x * DASH_DISTANCE * dashDistance, 0, 0 );
		}
	}

	public static void ExecuteDashAction( Controller controller ) {
		if( !IsDashing() && ( Time.time - dashStartTime > DASH_DELAY ) ) {
			dashStartTime = Time.time;
		}
	}

	public static void ExecuteFlapAction( Controller controller ) {
		bool canFlap = IsDefault( timeSinceLastFlap );
		if( !canFlap ) {
			bool flapDelayPassed = ( Time.time - timeSinceLastFlap ) >= FLAP_DELAY;
			canFlap = flapsSinceGrounded < MAX_FLAPS_SINCE_GROUNDED && flapDelayPassed;
		}
		
		if( canFlap ) {
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

	public static void ExecuteShieldAction ( Controller controller ) {
		bool canShield = IsDefault( shieldStartTime );
		if( !canShield ) {
			canShield = !IsShielding() && ( Time.time - shieldStartTime ) > ( SHIELD_TIME + SHIELD_DELAY );
		}

		if( canShield ) {
			shieldStartTime = Time.time;
		}
	}
}
