using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	float X_BOUNDS_CLOSE = 2f;
	float X_BOUNDS_FAR = 4f;
	float X_CAMERA_CHANGE_DURATION = 20f;
	float Y_CAMERA_CHANGE_DURATION = 1f;
	float DISTANCE_FROM_GROUND = 0.5f;
	float Y_BOUNDS = 3.5f;

	enum CameraState {
		LOCKED,
		FORWARD,
		DIRECTION_SWITCH,
	};
	CameraState cameraState = CameraState.FORWARD;
	GameObject player;
	Camera cameraComp;
	Vector2 CachedDirection;
	float cameraHeight;
	float cameraWidth;
	float currentGroundY;
	float xLerpCounter;
	float yLerpCounter;
	float initialY;
	bool platformTransitioning = false;

	// Use this for initialization
	void Start () {
		player = GameObject.Find( "Player" );
		cameraComp = GetComponent<Camera>();
		cameraHeight = 2f * cameraComp.orthographicSize;
		cameraWidth = cameraHeight * cameraComp.aspect;
		CachedDirection = player.GetComponent<Controller>().GetDirection();
		currentGroundY = transform.position.y;
	}

	float GetCameraX () {
		Vector2 playerPos = player.transform.position;
		float boundsMultiplier = CachedDirection == Vector2.right ? 1 : -1;
		bool isRight = CachedDirection == Vector2.right;
		float closeX, farX;
		if( isRight ) {
			closeX = transform.position.x - X_BOUNDS_CLOSE;
			farX = transform.position.x - X_BOUNDS_FAR;
		}
		else {
			closeX = transform.position.x + X_BOUNDS_CLOSE;
			farX = transform.position.x + X_BOUNDS_FAR;
		}
		CameraState newCameraState = cameraState;
		switch( cameraState ) {
			case ( CameraState.DIRECTION_SWITCH ):
				if( isRight ) {
					if( playerPos.x <= closeX && playerPos.x >= farX ) {
						newCameraState = CameraState.LOCKED;
					}
				}
				else {
					if( playerPos.x >= closeX && playerPos.x <= farX ) {
						newCameraState = CameraState.LOCKED;
					}
				}
				break;
			default:
				if( isRight ) {
					if( playerPos.x >= closeX ) {
						newCameraState = CameraState.FORWARD;
					}
					else if( playerPos.x <= farX ) {
						newCameraState = CameraState.DIRECTION_SWITCH;
						xLerpCounter = 0;
						CachedDirection = Vector2.left;
					}
					else {
						newCameraState = CameraState.LOCKED;
					}
				}
				else {
					if( playerPos.x <= closeX ) {
						newCameraState = CameraState.FORWARD;
					}
					else if( playerPos.x >= farX ) {
						newCameraState = CameraState.DIRECTION_SWITCH;
						xLerpCounter = 0;
						CachedDirection = Vector2.right;
					}
					else {
						newCameraState = CameraState.LOCKED;
					}
				}
				break;
		}

		float cameraX;
		switch( newCameraState ) {
			case ( CameraState.DIRECTION_SWITCH ):
				float cameraCenter = isRight ? playerPos.x + X_BOUNDS_CLOSE : playerPos.x - X_BOUNDS_CLOSE;
				xLerpCounter = Mathf.Min( xLerpCounter + ( Time.deltaTime / X_CAMERA_CHANGE_DURATION ), 1f );
				cameraX = Mathf.Lerp( transform.position.x, cameraCenter, xLerpCounter );
				break;

			case ( CameraState.FORWARD ):
				cameraX = isRight ? playerPos.x + X_BOUNDS_CLOSE : playerPos.x - X_BOUNDS_CLOSE;
				break;

			case( CameraState.LOCKED ):
			default:
				cameraX = transform.position.x;
				break;
		}

		cameraState = newCameraState;
		return cameraX;
	}

	float GetCameraY () {
		Vector2 playerPos = player.transform.position;
		if( player.GetComponent<Controller>().IsGrounded() && player.transform.position.y != currentGroundY ) {
			currentGroundY = player.transform.position.y;
			yLerpCounter = 0;
			initialY = transform.position.y;
			platformTransitioning = true;
		}
		float newY = transform.position.y;
		if( platformTransitioning && transform.position.y != currentGroundY + DISTANCE_FROM_GROUND ) {
			yLerpCounter = Mathf.Min( yLerpCounter + ( Time.deltaTime / Y_CAMERA_CHANGE_DURATION ), 1f );
			newY = Mathf.Lerp( initialY, currentGroundY + DISTANCE_FROM_GROUND, yLerpCounter );
		}

		if( !platformTransitioning && ( playerPos.y >= ( transform.position.y + Y_BOUNDS ) 
			|| playerPos.y <= ( transform.position.y - Y_BOUNDS ) ) ) {
			float newCameraPosition = playerPos.y + Y_BOUNDS;
			if( playerPos.y >= ( transform.position.y + Y_BOUNDS ) ) {
				newCameraPosition = playerPos.y - Y_BOUNDS;
			}
			newY = newCameraPosition;
		}

		return newY;
	}

	void DrawDebugLines() {
		float cameraX1 = transform.position.x - cameraWidth / 2;
		float cameraX2 = transform.position.x + cameraWidth / 2;
		float cameraY1 = transform.position.y - cameraHeight / 2;
		float cameraY2 = transform.position.y + cameraHeight / 2;
		Debug.DrawLine( new Vector3( transform.position.x - X_BOUNDS_CLOSE, cameraY1, transform.position.z ), new Vector3( transform.position.x - X_BOUNDS_CLOSE, cameraY2, transform.position.z ) );
		Debug.DrawLine( new Vector3( transform.position.x - X_BOUNDS_FAR, cameraY1, transform.position.z ), new Vector3( transform.position.x - X_BOUNDS_FAR, cameraY2, transform.position.z ) );

		Debug.DrawLine( new Vector3( transform.position.x + X_BOUNDS_CLOSE, cameraY1, transform.position.z ), new Vector3( transform.position.x + X_BOUNDS_CLOSE, cameraY2, transform.position.z ) );
		Debug.DrawLine( new Vector3( transform.position.x + X_BOUNDS_FAR, cameraY1, transform.position.z ), new Vector3( transform.position.x + X_BOUNDS_FAR, cameraY2, transform.position.z ) );

		Debug.DrawLine( new Vector3( cameraX1, transform.position.y + Y_BOUNDS, transform.position.z ), new Vector3( cameraX2, transform.position.y + Y_BOUNDS, transform.position.z ) );
		Debug.DrawLine( new Vector3( cameraX1, transform.position.y - Y_BOUNDS, transform.position.z ), new Vector3( cameraX2, transform.position.y - Y_BOUNDS, transform.position.z ) );
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if( GameUtils.DEBUG_MODE ) {
			DrawDebugLines();
		}
		transform.position = new Vector3( GetCameraX(), GetCameraY(), transform.position.z );
		if ( platformTransitioning && transform.position.y != ( currentGroundY + DISTANCE_FROM_GROUND ) ) {
			platformTransitioning = false;
		}
	}
}
