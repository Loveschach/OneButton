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
	public static bool holdingAction = false;

	//Jump
	const float JUMP_FORCE = 200f;
	const float JUMP_HOLD_FORCE = 15f;
	const float JUMP_HOLD_TIME = 0.2f;
	static float jumpStartTime = DEFAULT_TIME;

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

	public static bool ExecuteCurrentAction( Player player, Controller controller ) {
		ButtonActions currentAction = player.GetCurrentAction();
		bool success = false;
		switch( currentAction ) {
			case ButtonActions.JUMP:
				success = ExecuteJumpAction( controller );
				break;
			case ButtonActions.DASH:
				success = ExecuteDashAction( controller );
				break;
			case ButtonActions.FLAP:
				success = ExecuteFlapAction( controller );
				break;
			case ButtonActions.SHIELD:
				success = ExecuteShieldAction( controller );
				break;
			default:
				Debug.Assert( false, "Attempting to get force for non-existent action." );
				break;
		}
		return success;
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

		if( ( Time.time - jumpStartTime ) <= JUMP_HOLD_TIME && holdingAction ) {
			AddJumpForce( controller, JUMP_HOLD_FORCE );
		}
	}

	public static bool ExecuteDashAction( Controller controller ) {
		if( !IsDashing() && ( Time.time - dashStartTime > DASH_DELAY ) ) {
			dashStartTime = Time.time;
			return true;
		}
		else {
			return false;
		}
	}

	public static bool ExecuteFlapAction( Controller controller ) {
		bool canFlap = IsDefault( timeSinceLastFlap );
		if( !canFlap ) {
			bool flapDelayPassed = ( Time.time - timeSinceLastFlap ) >= FLAP_DELAY;
			canFlap = flapsSinceGrounded < MAX_FLAPS_SINCE_GROUNDED && flapDelayPassed;
		}
		
		if( canFlap ) {
			controller.GetBody().AddForce( new Vector2( 0, FLAP_FORCE ) );
			timeSinceLastFlap = Time.time;
			flapsSinceGrounded += 1;
			return true;
		}

		return false;
	}

	private static void AddJumpForce( Controller controller, float force ) {
		controller.GetBody().AddForce( new Vector2( 0, force ) );
	}

	public static bool ExecuteJumpAction( Controller controller ) {
		if( controller.IsGrounded() ) {
			jumpStartTime = Time.time;
			AddJumpForce( controller, JUMP_FORCE );
			return true;
		}

		return false;
	}

	public static bool ExecuteShieldAction ( Controller controller ) {
		bool canShield = IsDefault( shieldStartTime );
		if( !canShield ) {
			canShield = !IsShielding() && ( Time.time - shieldStartTime ) > ( SHIELD_TIME + SHIELD_DELAY );
		}

		if( canShield ) {
			shieldStartTime = Time.time;
			return true;
		}
		return false;
	}
}
