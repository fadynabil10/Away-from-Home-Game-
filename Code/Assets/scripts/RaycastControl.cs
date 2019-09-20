using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastControl : MonoBehaviour
{

	#region Fields

	// boxCollider2D Support
	public BoxCollider2D CharacterBoxCollider { get; private set; }
	protected RaycastOrigins raycastOrigins;
	protected const float SkinWidth = 0.01f;

	// raycast Support
	[Header("Raycasts")]
	[SerializeField] private float distanceBetweenRays = 0.25f;
	protected int horizontalRayCount;
	protected int verticalRayCount;
	protected float horizontalRaySpacing;
	protected float verticalRaySpacing;

	#endregion

	protected virtual void Awake()
	{
		CharacterBoxCollider = GetComponent<BoxCollider2D>();
		InstantiateBoxBounds();
		CalculateRaySpacing();
	}

	#region Raycast Origins

	/// <summary>
	/// Sets the origins of the rays according to the new bounds
	/// </summary>
	protected void UpdateRaycastOrigins()
	{
		raycastOrigins.bottomLeft.localPosition = CharacterBoxCollider.offset + new Vector2(-CharacterBoxCollider.size.x / 2 + SkinWidth, -CharacterBoxCollider.size.y / 2 + SkinWidth);
		raycastOrigins.bottomRight.localPosition = CharacterBoxCollider.offset + new Vector2(CharacterBoxCollider.size.x / 2 - SkinWidth, -CharacterBoxCollider.size.y / 2 + SkinWidth);
		raycastOrigins.topLeft.localPosition = CharacterBoxCollider.offset + new Vector2(-CharacterBoxCollider.size.x / 2 + SkinWidth, CharacterBoxCollider.size.y / 2 - SkinWidth);
		raycastOrigins.topRight.localPosition = CharacterBoxCollider.offset + new Vector2(CharacterBoxCollider.size.x / 2 - SkinWidth, CharacterBoxCollider.size.y / 2 - SkinWidth);
	}

	/// <summary>
	/// Instantiate the four bounds of the collider box and set them as local childern to the charachter
	/// </summary>
	private void InstantiateBoxBounds()
	{
		GameObject bound = new GameObject();
		raycastOrigins.bottomLeft = Instantiate(bound.transform, transform);
		raycastOrigins.bottomRight = Instantiate(bound.transform, transform);
		raycastOrigins.topLeft = Instantiate(bound.transform, transform);
		raycastOrigins.topRight = Instantiate(bound.transform, transform);
		UpdateRaycastOrigins();
	}

	/// <summary>
	/// Calculates the count and spacing of rays
	/// </summary>
	private void CalculateRaySpacing()
	{
		//Calculates the bounds of the collider and adds skinWidth support onto the collider bounds
		Bounds bounds = CharacterBoxCollider.bounds;
		bounds.Expand(SkinWidth * -2);
		float width = Vector2.Distance(raycastOrigins.bottomLeft.position, raycastOrigins.bottomRight.position);
		float height = Vector2.Distance(raycastOrigins.bottomLeft.position, raycastOrigins.topLeft.position);

		// Clamp the count of rays between 3 and max number
		horizontalRayCount = Mathf.Clamp(Mathf.RoundToInt(height / distanceBetweenRays), 3, int.MaxValue);
		verticalRayCount = Mathf.Clamp(Mathf.RoundToInt(width / distanceBetweenRays), 3, int.MaxValue);

		// calculate spaces between rays
		horizontalRaySpacing = height / (horizontalRayCount - 1);
		verticalRaySpacing = width / (verticalRayCount - 1);
	}

	/// <summary>
	/// Stores the four bounds of the character boxCollider
	/// </summary>
	protected struct RaycastOrigins
	{
		public Transform topLeft, topRight;
		public Transform bottomLeft, bottomRight;
	}

	#endregion
}
