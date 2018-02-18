using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	const float TOP_SPEED = 4.5f;
	const float SPEED_DAMPING = 0.25f;
	const float GROUND_RAY_LENGTH = 0.25f;
	Rigidbody2D body;
	BoxCollider2D boxCollider;
	LayerMask groundLayer;
	Actions.ButtonActions currentAction;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<BoxCollider2D>();
		groundLayer = LayerMask.GetMask( "Ground" );
	}

	public Actions.ButtonActions GetCurrentAction() {
		return currentAction;
	}

	public Rigidbody2D GetBody () {
		return body;
	}
	
	public bool IsGrounded() {
		Vector2 lineEnd = new Vector2( boxCollider.bounds.center.x, boxCollider.bounds.min.y - GROUND_RAY_LENGTH );
		return Physics2D.Linecast( boxCollider.bounds.center, lineEnd, groundLayer );
	}

	void FixedUpdate () {
		float xVelocity = Mathf.Lerp( body.velocity.x, TOP_SPEED * Input.GetAxisRaw( "Horizontal" ), SPEED_DAMPING );
		body.velocity = new Vector2( xVelocity, body.velocity.y );

		if( Input.GetButton( "Action" ) ) {
			Actions.ExecuteCurrentAction( this );
		}
	}
}
