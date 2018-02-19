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
	BoxCollider2D boxCollider;
	LayerMask groundLayer;
	Vector2 direction;
	Actions.ButtonActions currentAction = Actions.ButtonActions.DASH;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<BoxCollider2D>();
		groundLayer = LayerMask.GetMask( "Ground" );
		direction = Vector2.right;
	}

	public Actions.ButtonActions GetCurrentAction() {
		return currentAction;
	}

	public Rigidbody2D GetBody () {
		return body;
	}

	public Vector2 GetDirection() {
		return direction;
	}
	
	public bool IsGrounded() {
		Vector2 lineEnd = new Vector2( boxCollider.bounds.center.x, boxCollider.bounds.min.y - GROUND_RAY_LENGTH );
		return Physics2D.Linecast( boxCollider.bounds.center, lineEnd, groundLayer );
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
		if( !Actions.IsDashing() ) {
			float xVelocity = Mathf.Lerp( body.velocity.x, TOP_SPEED * Input.GetAxisRaw( "Horizontal" ), SPEED_DAMPING );
			body.velocity = new Vector2( xVelocity, body.velocity.y );
			UpdateDirection();
		}
		
		UpdateGroundedState();

		if( Input.GetButtonDown( "Action" ) ) {
			Actions.ExecuteCurrentAction( this );
		}
		Actions.Update( this );
	}
}
