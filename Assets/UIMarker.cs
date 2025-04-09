using UnityEngine;
using UnityEngine.UI;

public class UIMarker : MonoBehaviour
{
	[SerializeField]
	private Image markerImage;

	public void SetMarkerAlpha(float alphaValue)
	{
		markerImage.color = new Color(markerImage.color.r, markerImage.color.g, markerImage.color.b, Mathf.Clamp01(alphaValue));
	}
}
