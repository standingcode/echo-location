using System.Collections;
using UnityEngine;

public struct HitLocation
{
	public bool? Active;
	public RaycastHit Hit;
	public Transform debugMarker;
}

public class EchoLocator : MonoBehaviour
{
	[SerializeField]
	public LayerMask ignoreRaycastLayer;

	[SerializeField]
	private int amountOfRays = 32;

	public GameObject debugMarkerPrefab;

	[SerializeField]
	private GameObject UIMarkerPrefab;

	[SerializeField]
	private float maxRayDistance = 20f;

	[SerializeField]
	private Transform[] rayLocations;

	[SerializeField]
	private Transform playerTransform;

	private RaycastHit hit;

	public bool castTheRays = true;
	public bool showRayDebugLines = true;
	public bool showHitSpheres = true;

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
				debugMarker = Instantiate(debugMarkerPrefab, Vector3.zero, Quaternion.identity).transform
			};

			hitLocations[i] = hitLocation;
			hitLocations[i].debugMarker.gameObject.SetActive(false);
		}

	}

	private void GenerateUI()
	{
		uIMarkers = new UIMarker[amountOfRays];

		// Instantiate a ui object and add as a child to this transform
		for (int i = 0; i < hitLocations.Length; i++)
		{
			GameObject uIMarkerGameObject = Instantiate(UIMarkerPrefab, Vector3.zero, Quaternion.identity);
			uIMarkerGameObject.transform.SetParent(transform);

			angle = i * (360f / amountOfRays);
			direction = Quaternion.Euler(0, 0, -angle) * transform.up;

			uIMarkerGameObject.transform.localPosition = ((Screen.height / 2) - (uIMarkerGameObject.GetComponent<RectTransform>().rect.height / 2)) * direction;

			UIMarker uIMarker = uIMarkerGameObject.GetComponent<UIMarker>();
			uIMarker.SetMarkerAlpha(0f);

			uIMarkers[i] = uIMarker;
		}
	}

	private Vector3 direction;
	private float currentHitDistanceThisFrame;
	private bool currentDirectionRayHitThisFrame;
	float angle;
	private IEnumerator RayCastInAllDirections()
	{
		while (true)
		{
			if (castTheRays)
			{
				for (int i = 0; i < amountOfRays; i++)
				{
					angle = i * (360f / amountOfRays);
					direction = Quaternion.Euler(0, angle, 0) * playerTransform.forward;

					currentDirectionRayHitThisFrame = false;
					currentHitDistanceThisFrame = Mathf.Infinity;

					foreach (Transform t in rayLocations)
					{
						if (showRayDebugLines)
							Debug.DrawLine(t.position, t.position + (direction * maxRayDistance), Color.green);

						if (Physics.Raycast(t.position, direction, out hit, maxRayDistance, ~ignoreRaycastLayer))
						{
							if (hitLocations[i].Hit.point == null || hit.distance < currentHitDistanceThisFrame)
							{
								currentDirectionRayHitThisFrame = true;
								currentHitDistanceThisFrame = hit.distance;
								hitLocations[i].Hit = hit;
								hitLocations[i].debugMarker.position = hit.point;
							}
						}
					}

					if (!currentDirectionRayHitThisFrame)
					{
						hitLocations[i].Active = false;
						uIMarkers[i].SetMarkerAlpha(0f);

						if (showHitSpheres)
							hitLocations[i].debugMarker.gameObject.SetActive(false);

					}
					else
					{
						hitLocations[i].Active = true;
						uIMarkers[i].SetMarkerAlpha(1 - (hitLocations[i].Hit.distance / maxRayDistance));

						if (showHitSpheres)
							hitLocations[i].debugMarker.gameObject.SetActive(true);
					}
				}
			}

			yield return null;
		}
	}
}
