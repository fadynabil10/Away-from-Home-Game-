using UnityEngine;

public class CameraFollow : MonoBehaviour
{

	#region Fields

	[SerializeField] private Controller2D target;
	[SerializeField] private Vector2 focusAreaSize;
	[SerializeField] private float verticalOffset = 3;
	[SerializeField] private float lookAheadDistanceX = 2;
	[SerializeField] private float lookSmoothTimeX = 0.9f;
	[SerializeField] private float verticalSmoothTime = 0.3f;
	private FocusArea focusArea;

	private float currentLookAheadX;
	private float targetLookAheadX;
	private float lookAheadDirectionX;
	private float smoothLookVelocityX;
	private float smoothVelocityY;

	private bool lookAheadStopped;

	#endregion

	private void Start()
	{
		if (target != null)
		{
			focusArea = new FocusArea(target.CharacterBoxCollider.bounds, focusAreaSize);
		}
	}

	private void LateUpdate()
	{
		if (target != null)
		{
			focusArea.Update(target.CharacterBoxCollider.bounds);
			Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;
			if (focusArea.velocity.x != 0)
			{
				lookAheadDirectionX = Mathf.Sign(focusArea.velocity.x);

				if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
				{
					lookAheadStopped = false;
					targetLookAheadX = lookAheadDirectionX * lookAheadDistanceX;
				}
				else
				{
					if (!lookAheadStopped)
					{
						lookAheadStopped = true;
						targetLookAheadX = currentLookAheadX + (lookAheadDirectionX * lookAheadDistanceX - currentLookAheadX) / 4f;
					}
				}
			}
			currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
			focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
			focusPosition += Vector2.right * currentLookAheadX;

			transform.position = (Vector3)focusPosition + Vector3.forward * -10;
		}
	}

	private struct FocusArea
	{
		public Vector2 center;
		public Vector2 velocity;
		private float left, right;
		private float top, bottom;

		public FocusArea(Bounds targetBounds, Vector2 size)
		{
			left = targetBounds.center.x - size.x / 2;
			right = targetBounds.center.x + size.x / 2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			center = new Vector2((left + right) / 2, (top + bottom) / 2);
			velocity = Vector2.zero;
		}

		public void Update(Bounds targetBounds)
		{
			// update x
			float shiftX = 0;
			if (targetBounds.min.x < left)
			{
				shiftX = targetBounds.min.x - left;
			}
			else if (targetBounds.max.x > right)
			{
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			// update y
			float shiftY = 0;
			if (targetBounds.min.y < bottom)
			{
				shiftY = targetBounds.min.y - bottom;
			}
			else if (targetBounds.max.y > top)
			{
				shiftY = targetBounds.max.y - top;
			}
			bottom += shiftY;
			top += shiftY;

			// update center
			center = new Vector2((left + right) / 2, (top + bottom) / 2);
			velocity = new Vector2(shiftX, shiftY);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5f);
		Gizmos.DrawCube(focusArea.center, focusAreaSize);
	}
}
