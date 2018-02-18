using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Actions {
	public enum ButtonActions {
		JUMP,
		SLIDE,
		FLAP
	};
	const float JUMP_FORCE = 100f;

	public static void ExecuteCurrentAction( Controller controller ) {
		ButtonActions currentAction = controller.GetCurrentAction();
		switch( currentAction ) {
			case ButtonActions.JUMP:
				ExecuteJumpAction( controller );
				break;
			case ButtonActions.SLIDE:
				break;
			case ButtonActions.FLAP:
				break;
			default:
				Debug.Assert( false, "Attempting to get force for non-existent action." );
				break;
		}
	}

	public static void ExecuteJumpAction( Controller controller ) {
		if( controller.IsGrounded() ) {
			controller.GetBody().AddForce( new Vector2( 0, JUMP_FORCE ) );
		}
	}
}
