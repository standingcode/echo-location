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
	private int amountOfRays = 32;

	public GameObject debugMarkerPrefab;

	[SerializeField]
	private float maxRayDistance = 20f;

	[SerializeField]
	private Transform[] rayLocations;

	private RaycastHit hit;

	public bool castTheRays = true;
	public bool showRayDebugLines = true;
	public bool showHitSpheres = true;

	private HitLocation[] hitLocations;

	private void Start()
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

		// Start the coroutine
		StartCoroutine(RayCastInAllDirections());
	}

	// Update is called once per frame
	void Update()
	{

	}

	private Vector3 direction;
	private float currentHitDistanceThisFrame;
	private bool currentDirectionRayHitThisFrame;
	private IEnumerator RayCastInAllDirections()
	{
		while (true)
		{
			if (castTheRays)
			{
				for (int i = 0; i < amountOfRays; i++)
				{
					float angle = i * (360f / amountOfRays);

					direction = Quaternion.Euler(0, angle, 0) * transform.forward;

					currentDirectionRayHitThisFrame = false;
					currentHitDistanceThisFrame = Mathf.Infinity;

					foreach (Transform t in rayLocations)
					{
						if (showRayDebugLines)
							Debug.DrawLine(t.position, t.position + (direction * maxRayDistance), Color.green);

						if (Physics.Raycast(t.position, direction, out hit, maxRayDistance))
						{
							if (hitLocations[i].Hit.point == null || hit.distance < currentHitDistanceThisFrame)
							{
								currentDirectionRayHitThisFrame = true;
								currentHitDistanceThisFrame = hit.distance;

								hitLocations[i].Hit = hit;
								hitLocations[i].Active = true;

								hitLocations[i].debugMarker.gameObject.SetActive(true);
								hitLocations[i].debugMarker.position = hit.point;
							}
						}
					}

					if (!currentDirectionRayHitThisFrame)
					{
						hitLocations[i].Active = false;
						hitLocations[i].debugMarker.gameObject.SetActive(false);
					}
				}
			}

			yield return null;
		}
	}
}
