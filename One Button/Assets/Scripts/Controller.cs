using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	const float TOP_SPEED = 4.5f;
	const float SPEED_DAMPING = 0.25f;
	const float MAX_JUMP_TIME = 1f;
	Rigidbody2D body;
	float jumpStartTime = -1;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () {
		float xVelocity = Mathf.Lerp( body.velocity.x, TOP_SPEED * Input.GetAxisRaw( "Horizontal" ), SPEED_DAMPING );
		body.velocity = new Vector2( xVelocity, body.velocity.y );

		float yVelocity = body.velocity.y;
		if( Input.GetKeyDown( KeyCode.Jump) ) {
			jumpStartTime = jumpStartTime == -1 ? Time.time : jumpStartTime;
		}
		else {
			jumpStartTime = -1;
		}
	}
}
