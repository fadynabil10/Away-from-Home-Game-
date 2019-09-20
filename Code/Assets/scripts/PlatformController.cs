using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlatformController : RaycastControl
{

	#region Fields

	[SerializeField] private LayerMask passengerMask;
	[SerializeField] private Vector2[] localWaypoints;
	[SerializeField] private float movementSpeed = 1;
	[SerializeField] private float waitTime = 0;
	[SerializeField] [Range(1, 3)] private float easeAmount = 1;
	[SerializeField] private bool cyclic;

	private Vector2[] globalWaypoints;
	private int fromWaypointIndex;
	private float percentBetweenPoints;
	private float nextMoveTime;

	#endregion

	protected override void Awake()
	{
		base.Awake();

		globalWaypoints = new Vector2[localWaypoints.Length];
		for (int i = 0; i < localWaypoints.Length; i++)
		{
			globalWaypoints[i] = localWaypoints[i] + (Vector2)transform.position;
		}
	}

	void FixedUpdate()
	{
		UpdateRaycastOrigins();
		transform.Translate(CalculatePlatformMovment());
		//MovePassenger(move);
	}

	/// <summary>
	/// Moves the platform between waypoints at a set speed
	/// </summary>
	/// <returns></returns>
	private Vector2 CalculatePlatformMovment()
	{
		// timeing the movment of the platform
		if (Time.time < nextMoveTime)
		{
			return Vector2.zero;
		}

		// make waypoints reset and not excede their number
		fromWaypointIndex %= globalWaypoints.Length;
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;

		float distanceBetweenWaypoints = Vector2.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
		percentBetweenPoints += Time.deltaTime * movementSpeed / distanceBetweenWaypoints;
		percentBetweenPoints = Mathf.Clamp01(percentBetweenPoints);

		Vector2 newPosition = Vector2.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], Ease(percentBetweenPoints));
		if (percentBetweenPoints >= 1)
		{
			percentBetweenPoints = 0;
			fromWaypointIndex++;

			if (!cyclic)
			{
				if (fromWaypointIndex >= globalWaypoints.Length - 1)
				{
					fromWaypointIndex = 0;
					System.Array.Reverse(globalWaypoints);
				}
			}

			nextMoveTime = Time.time + waitTime;
		}
		return newPosition - (Vector2)transform.position;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="x"></param>
	/// <returns></returns>
	private float Ease(float x)
	{
		float a = easeAmount;
		return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
	}

	/// <summary>
	/// Show the way points positions in the editor
	/// </summary>
	private void OnDrawGizmos()
	{
		if (localWaypoints != null)
		{
			Gizmos.color = Color.red;
			float length = 0.2f;
			for (int i = 0; i < localWaypoints.Length; i++)
			{
				// if we are in game don't move gizmos only in the editor.
				Vector2 wayPointPos = Application.isPlaying ? globalWaypoints[i] : localWaypoints[i] + (Vector2)transform.position;

				// draw gizmos cross
				Gizmos.DrawLine(wayPointPos - Vector2.up * length, wayPointPos + Vector2.up * length);
				Gizmos.DrawLine(wayPointPos - Vector2.right * length, wayPointPos + Vector2.right * length);
			}
		}
	}

	private void MovePassenger(Vector2 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

		// list to store all the riders on the platform
		Transform passenger = null;

		for (int i = 0; i < verticalRayCount; i++)
		{
			// sets the ray origin and position it properly along the xAxis
			Vector2 rayOrigin = (Vector2)transform.TransformPoint(raycastOrigins.topLeft.localPosition) + Vector2.right * (verticalRaySpacing * i);

			// Cast the actual ray
			Physics2D.SyncTransforms();
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,
				transform.up,
				rayLength,
				passengerMask);

			// debug Support
			Debug.DrawRay(rayOrigin, transform.up, Color.white);

			if (hit)
			{
				hit.transform.SetParent(transform);
				passenger = hit.transform;
			}
			else
			{
				if (passenger != null)
				{
					passenger.SetParent(null);
					passenger = null;
				}
			}
		}
	}
}
