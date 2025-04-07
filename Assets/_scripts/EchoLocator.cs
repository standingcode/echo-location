using System.Collections;
using UnityEngine;

public class EchoLocator : MonoBehaviour
{
	[SerializeField]
	private int amountOfRays = 16;

	Vector3 closestHit = Vector3.positiveInfinity;
	RaycastHit hit;

	private void Start()
	{
		// Start the coroutine
		StartCoroutine(RayCastInAllDirections());

	}

	// Update is called once per frame
	void Update()
	{

	}

	private IEnumerator RayCastInAllDirections()
	{
		while (true)
		{
			Debug.Log("Raycasting in all directions...");

			for (int i = 0; i < amountOfRays; i++)
			{
				float angle = i * (360f / amountOfRays);

				Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;


				if (Physics.Raycast(transform.position, direction, out hit))
				{
					if (hit.collider == null || hit.distance < Vector3.Distance(transform.position, closestHit))
					{
						closestHit = hit.point;
					}
				}
			}

			if (closestHit != null)
			{
				Debug.DrawLine(transform.position, closestHit, Color.red);
			}

			yield return new WaitForSeconds(0.1f);
		}
	}
}
