using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	const float TOP_SPEED = 4.5f;
	const float SPEED_DAMPING = 0.25f;
	const float GROUND_RAY_LENGTH = 0.25f;
	bool grounded;
	Rigidbody2D body;
	CircleCollider2D controllerCollider;
	LayerMask groundLayer;
	Vector2 direction;
	Player player;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D>();
		controllerCollider = GetComponent<CircleCollider2D>();
		player = GetComponent<Player>();
		groundLayer = LayerMask.GetMask( "Ground" );
		direction = Vector2.right;
	}

	public Rigidbody2D GetBody () {
		return body;
	}

	public Vector2 GetDirection() {
		return direction;
	}
	
	public bool IsGrounded() {
		Vector2 lineEnd = new Vector2( controllerCollider.bounds.center.x, controllerCollider.bounds.min.y - GROUND_RAY_LENGTH );
		return Physics2D.Linecast( controllerCollider.bounds.center, lineEnd, groundLayer ) && body.velocity.y <= 0;
	}

	void UpdateGroundedState() {
		bool isGrounded = IsGrounded();
		if( isGrounded && !grounded ) {
			grounded = true;
			Actions.PlayerGrounded();
		}
		else if( !isGrounded && grounded ) {
			grounded = false;
		}
	}

	void UpdateDirection() {
		float horizontalInput = Input.GetAxisRaw( "Horizontal" );
		if( horizontalInput > 0 ) {
			direction = Vector2.right;
		}
		else if( horizontalInput < 0 ) {
			direction = Vector2.left;
		}
	}

	void FixedUpdate () {
		if( !Actions.IsDashing() && !Actions.IsShielding() ) {
			float xVelocity = Mathf.Lerp( body.velocity.x, TOP_SPEED * Input.GetAxisRaw( "Horizontal" ), SPEED_DAMPING );
			body.velocity = new Vector2( xVelocity, body.velocity.y );
			UpdateDirection();
		}
		
		UpdateGroundedState();

		if( Input.GetButtonDown( "Action" ) ) {
			bool success = Actions.ExecuteCurrentAction( player, this );
			if( success ) {
				Actions.holdingAction = true;
			}
		}
		if ( !Input.GetButton( "Action" ) && Actions.holdingAction ) {
			Actions.holdingAction = false;
		}
		Actions.Update( this );
	}
}
