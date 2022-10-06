using UnityEngine;
using UnityEngine.UI;
public class ScrollPosition : MonoBehaviour
{
	private ScrollRect sr;

	public void Awake()
	{
		sr = gameObject.GetComponent<ScrollRect>();
	}
	public void Start()
	{
		var dropdown = GetComponentInParent<Dropdown>();
		if (dropdown != null)
		{
			var viewport = transform.Find("Viewport").GetComponent<RectTransform>();
			var contentArea = transform.Find("Viewport/Content").GetComponent<RectTransform>();
			var contentItem = transform.Find("Viewport/Content/Item").GetComponent<RectTransform>();

			var areaHeight = contentArea.rect.height - viewport.rect.height;
			var cellHeight = contentItem.rect.height;
			var scrollRatio = (cellHeight * dropdown.value) / areaHeight;
			sr.verticalNormalizedPosition = 1.0f - Mathf.Clamp(scrollRatio, 0.0f, 1.0f);
		}
	}
}
