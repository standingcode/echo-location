using System.Collections;
using UnityEngine;

public struct HitLocation
{
	public bool? Active;
	public RaycastHit Hit;
}

public class EchoLocator : MonoBehaviour
{
	[SerializeField]
	public LayerMask ignoreRaycastLayer;

	[SerializeField]
	private int amountOfRays = 32;

	[SerializeField]
	private GameObject UIMarkerPrefab;

	[SerializeField]
	private float maxRayDistance = 20f;

	[SerializeField]
	private Transform[] rayLocations;

	[SerializeField]
	private Transform playerTransform;

	public bool castTheRays = true;
	public bool showRayDebugLines = true;

	private HitLocation[] hitLocations;
	private UIMarker[] uIMarkers;

	private void Start()
	{
		InitialiseHitLocations();
		GenerateUI();

		// Start the coroutine
		StartCoroutine(RayCastInAllDirections());
	}

	private void InitialiseHitLocations()
	{
		hitLocations = new HitLocation[amountOfRays];

		for (int i = 0; i < hitLocations.Length; i++)
		{
			HitLocation hitLocation = new HitLocation
			{
				Active = false,
				Hit = new RaycastHit(),
			};

			hitLocations[i] = hitLocation;
		}
	}

	private void GenerateUI()
	{
		uIMarkers = new UIMarker[amountOfRays];

		// Instantiate a ui object and add as a child to this transform
		for (int i = 0; i < hitLocations.Length; i++)
		{
			GenerateUIMarker(i);
		}
	}

	private void GenerateUIMarker(int i)
	{
		GameObject uIMarkerGameObject = Instantiate(UIMarkerPrefab, Vector3.zero, Quaternion.identity);
		uIMarkerGameObject.transform.SetParent(transform);

		float angle = i * (360f / amountOfRays);
		Vector3 direction = Quaternion.Euler(0, 0, -angle) * transform.up;

		uIMarkerGameObject.transform.localPosition = ((Screen.height / 2) - (uIMarkerGameObject.GetComponent<RectTransform>().rect.height / 2)) * direction;

		UIMarker uIMarker = uIMarkerGameObject.GetComponent<UIMarker>();
		uIMarker.SetMarkerAlpha(0f);

		uIMarkers[i] = uIMarker;
	}

	private IEnumerator RayCastInAllDirections()
	{
		while (true)
		{
			if (castTheRays)
			{
				for (int i = 0; i < amountOfRays; i++)
				{
					float angle = i * (360f / amountOfRays);
					Vector3 direction = Quaternion.Euler(0, angle, 0) * playerTransform.forward;

					float? shortestDistanceHitThisFrame = CastRaysForDirection(i, direction);

					if (shortestDistanceHitThisFrame == null)
					{
						hitLocations[i].Active = false;
						uIMarkers[i].SetMarkerAlpha(0f);
					}
					else
					{
						hitLocations[i].Active = true;
						uIMarkers[i].SetMarkerAlpha(1 - ((float)shortestDistanceHitThisFrame / maxRayDistance));
					}
				}
			}

			yield return null;
		}
	}

	private float? CastRaysForDirection(int rayIndex, Vector3 direction)
	{
		float? shortestDistanceHit = null;

		foreach (Transform t in rayLocations)
		{
			RaycastHit? hit = CastRay(t, direction);

			if (hit != null && (shortestDistanceHit == null || ((RaycastHit)hit).distance < shortestDistanceHit))
			{
				shortestDistanceHit = ((RaycastHit)hit).distance;
				hitLocations[rayIndex].Hit = ((RaycastHit)hit);
			}
		}

		return shortestDistanceHit;
	}

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
