using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastControl
{
	// refrence to the player input
	public Vector2 playerInput { get; private set; }

	// collision Support
	[SerializeField] private LayerMask collisionMask;
	[SerializeField] private LayerMask movingPlatformMask;
	public CollisionInfo collisions;

	protected override void Awake()
	{
		base.Awake();
		collisions.faceDirection = 1;
	}

	#region Horizontal

	/// <summary>
	/// Checks for collisions left and right
	/// </summary>
	/// <param name="velocity"></param>
	private void HorizontalCollisions(ref Vector2 velocity)
	{
		// ray calculations
		float directionX = collisions.faceDirection;
		float rayLength = Mathf.Abs(velocity.x) + SkinWidth;

		for (int i = 0; i < horizontalRayCount; i++)
		{
			// Sets the ray origin and position it properly along the yAxis
			Vector2 rayOrigin = (directionX == -1) ?
				transform.TransformPoint((Vector2)raycastOrigins.bottomLeft.localPosition + Vector2.up * (horizontalRaySpacing * i)) :
				transform.TransformPoint((Vector2)raycastOrigins.bottomRight.localPosition + Vector2.up * (horizontalRaySpacing * i));

			// Cast the actual ray
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,
				transform.right * directionX,
				rayLength,
				collisionMask);

			// On collision detection
			if (hit)
			{
				// skip one way platform interference
				if (hit.distance == 0)
				{
					continue;
				}
				// update our length to a new distance so we only hit the closest obstacle
				velocity.x = (hit.distance - SkinWidth) * directionX;
				rayLength = hit.distance;

				// updtate collision Info
				collisions.right = directionX == 1;
				collisions.left = directionX == -1;

				// Debug Support
				Debug.DrawRay(rayOrigin, transform.right * directionX, Color.magenta);
			}
		}
	}

	#endregion

	#region Vertical

	/// <summary>
	/// Checks for collisions up and down
	/// </summary>
	/// <param name="velocity"></param>
	private void VerticalCollisions(ref Vector2 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{
			// sets the ray origin and position it properly along the xAxis
			Vector2 rayOrigin = (directionY == -1) ?
				transform.TransformPoint((Vector2)raycastOrigins.bottomLeft.localPosition + Vector2.right * (verticalRaySpacing * i + velocity.x)) :
				transform.TransformPoint((Vector2)raycastOrigins.topLeft.localPosition + Vector2.right * (verticalRaySpacing * i + velocity.x));

			// Cast the actual ray
			Physics2D.SyncTransforms();
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,
				transform.up * directionY,
				rayLength,
				collisionMask);

			// on collision detection
			if (hit)
			{
				// one way platform support
				if (hit.collider.CompareTag("OneWayPlatform"))
				{
					if (directionY == 1 || hit.distance == 0)
					{
						continue;
					}
					if (playerInput.y == -1)
					{
						continue;
					}
				}

				// attach to moving plarform support
				if (hit.collider.CompareTag("MovingPlatform"))
				{
					// become a child to the moving platform
					transform.SetParent(hit.transform);
				}
				else
				{
					// detach from the platform on leaving
					transform.parent = null;
				}

				// update our length to a knew distance so we only hit the closest obstacle
				velocity.y = (hit.distance - SkinWidth) * directionY;
				rayLength = hit.distance;

				// updtate collision info
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;

				// debug Support
				Debug.DrawRay(rayOrigin, transform.up * directionY, Color.magenta);
			}
		}
	}

	#endregion

	#region Riding Platform

	/// <summary>
	/// Checks for collisions on top of moving platform
	/// </summary>
	/// <param name="velocity"></param>
	private void RidingPlatformCollisions(ref Vector2 velocity)
	{
		float rayLength = Mathf.Abs(velocity.y) + SkinWidth;
		for (int i = 0; i < verticalRayCount; i++)
		{
			// sets the ray origin and position it properly along the xAxis
			Vector2 rayOrigin = transform.TransformPoint((Vector2)raycastOrigins.bottomLeft.localPosition + Vector2.right * (verticalRaySpacing * i + velocity.x));

			// Cast the actual ray
			Physics2D.SyncTransforms();
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,
				-transform.up,
				rayLength,
				movingPlatformMask);

			// on collision detection
			if (hit)
			{
				// become a child to the moving platform
				transform.SetParent(hit.transform);

				// update our length to a knew distance so we only hit the closest obstacle
				rayLength = hit.distance;

				// debug Support
				Debug.DrawRay(rayOrigin, -transform.up, Color.blue);
			}
			else
			{
				// if we left the platform detach from it
				if (i == 0)
				{
					transform.SetParent(null);
				}
			}
		}
	}

	#endregion

	/// <summary>
	/// Stores collision properties
	/// </summary>
	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public float slopeAngle;
		public int faceDirection;

		public void Reset()
		{
			above = below = false;
			left = right = false;

			slopeAngle = 0;
		}
	}

	/// <summary>
	/// Moves the charachter from charachter script
	/// </summary>
	/// <param name="velocity"></param>
	public void Move(Vector2 velocity, Vector2 input, bool standingOnPlatform = false)
	{
		// raycast calculations
		UpdateRaycastOrigins();
		collisions.Reset();

		// input used for one way platforms
		playerInput = input;

		// direction calculation
		if (velocity.x != 0)
		{
			collisions.faceDirection = (int)Mathf.Sign(velocity.x);
		}

		// collisions detection
		HorizontalCollisions(ref velocity);
		VerticalCollisions(ref velocity);

		// applying movment
		transform.Translate(velocity);
	}
}
