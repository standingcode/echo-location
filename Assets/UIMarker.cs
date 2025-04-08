using UnityEngine;
using UnityEngine.UI;

public class UIMarker : MonoBehaviour
{
	[SerializeField]
	private Image markerImage;

	public void ShowMarker(bool show)
	{
		markerImage.gameObject.SetActive(show);
	}
}
