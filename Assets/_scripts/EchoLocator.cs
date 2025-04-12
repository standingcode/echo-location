using System.Collections;
using UnityEngine;

public struct RaycastDirection
{
	public float Angle;
	public RaycastHit Hit;
	public UIMarker UIMarker;
}

public class EchoLocator : MonoBehaviour
{
	[SerializeField]
	public LayerMask ignoreRaycastLayer;

	[SerializeField]
	private int amountOfRays = 32;

	[SerializeField]
	private GameObject uIMarkerPrefab;

	[SerializeField]
	private float maxRayDistance = 20f;

	[SerializeField]
	private Transform[] rayLocations;

	[SerializeField]
	private Transform playerTransform;

	public bool castTheRays = true;
	public bool showRayDebugLines = true;

	private RaycastDirection[] raycastDirections;

	private void Start()
	{
		Initialize();

		// Start the coroutine
		StartCoroutine(RayCastInAllDirections());
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	/// <summary>
	/// Initializes the hit locations and their corresponding UI markers.
	/// </summary>
	private void Initialize()
	{
		raycastDirections = new RaycastDirection[amountOfRays];

		for (int i = 0; i < raycastDirections.Length; i++)
		{
			float angle = i * (360f / amountOfRays);

			raycastDirections[i] = new RaycastDirection
			{
				Angle = angle,
				Hit = new RaycastHit(),
				UIMarker = GenerateUIMarker(angle)
			};
		}
	}

	/// <summary>  
	/// Generates a single UI marker at the relevant angle from the middle of the screen out towards the edge.  
	/// </summary>  
	/// <param name="angle">The angle in degrees at which the UI marker will be generated.</param>  
	/// <returns>The generated UI marker.</returns>  
	private UIMarker GenerateUIMarker(float angle)
	{
		GameObject uIMarkerGameObject = Instantiate(uIMarkerPrefab, Vector3.zero, Quaternion.identity);
		uIMarkerGameObject.transform.SetParent(transform);

		Vector3 direction = Quaternion.Euler(0, 0, -angle) * transform.up;

		// Determine the actual position of the marker and place it
		uIMarkerGameObject.transform.localPosition
			= ((Screen.height / 2) - (uIMarkerGameObject.GetComponent<RectTransform>().rect.height / 2)) * direction;

		// Get a reference to the UIMarker script from the newly created GameObject
		UIMarker uIMarker = uIMarkerGameObject.GetComponent<UIMarker>();

		// Initialize the marker's alpha value to hide as default
		uIMarker.SetMarkerAlpha(0f);

		return uIMarker;
	}

	/// <summary>
	/// The coroutine method which loops through all of the ray locations 
	/// and for each direction calls another method to cast rays for different heights
	/// </summary>
	/// <returns></returns>
	private IEnumerator RayCastInAllDirections()
	{
		while (true)
		{
			if (castTheRays)
			{
				for (int i = 0; i < amountOfRays; i++)
				{
					// Calculate the correct direction based on where player is currently facing
					Vector3 direction = Quaternion.Euler(0, raycastDirections[i].Angle, 0) * playerTransform.forward;

					// Get the shortest from x amount of rays from same direction but different height
					float shortestDistanceHitThisFrame = CastRaysForDirection(i, direction);

					// If -1 then none of the rays hit anything set to alpha 0, otherwise we set the marker alpha based on the distance / max possible distance
					SetMarkerAlpha(i, shortestDistanceHitThisFrame == -1 ? 0f : 1 - (shortestDistanceHitThisFrame / maxRayDistance));
				}
			}

			yield return null;
		}
	}

	/// <summary>
	/// Sets the alpha value of the UI marker at the specified index.
	/// </summary>
	/// <param name="index"></param>
	/// <param name="alphaValue"></param>
	private void SetMarkerAlpha(int index, float alphaValue)
	{
		raycastDirections[index].UIMarker.SetMarkerAlpha(alphaValue);
	}

	/// <summary>
	/// Casts rays in one direction, but for different heights. The heights to raycast are stored as array of transforms
	/// </summary>
	/// <param name="index"></param>
	/// <param name="direction"></param>
	/// <returns></returns>
	private float CastRaysForDirection(int index, Vector3 direction)
	{
		float shortestDistanceHit = -1;

		// For each of the heights
		foreach (Transform t in rayLocations)
		{
			RaycastHit? returnedHit = CastRay(t, direction);

			// If the ray actually hit something
			if (returnedHit != null)
			{
				RaycastHit hit = (RaycastHit)returnedHit;

				// If none of the other rays so far have hit anything, we can just set this as shortest,
				// otherwise we check if this hit is shorter than the previous one
				if (shortestDistanceHit == -1 || hit.distance < shortestDistanceHit)
				{
					shortestDistanceHit = hit.distance;
					raycastDirections[index].Hit = hit;
				}
			}
		}

		return shortestDistanceHit;
	}

	/// <summary>
	/// Cast a single ray, return null if the ray did not hit anything
	/// The debug lines are drawn within this method.
	/// </summary>
	/// <param name="t"></param>
	/// <param name="direction"></param>
	/// <returns></returns>
	private RaycastHit? CastRay(Transform t, Vector3 direction)
	{
		RaycastHit hit;

		if (showRayDebugLines)
			Debug.DrawLine(t.position, t.position + (direction * maxRayDistance), Color.green);

		if (Physics.Raycast(t.position, direction, out hit, maxRayDistance, ~ignoreRaycastLayer))
		{
			return hit;
		}

		return null;
	}
}
