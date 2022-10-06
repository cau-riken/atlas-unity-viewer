using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class HighlightController : MonoBehaviour
{
    private HighlightObject highlightObject;
    private bool allReset = false;
    public string currentObject = "";
    private float timer;
    public Vector3 mousePosBefore;

    public void SelectObject(HighlightObject selectedhiglightObject)
    {   
        if (highlightObject != null)
        {
            highlightObject.StopHighlight();
        }

        highlightObject = selectedhiglightObject;
        highlightObject.StartHighlight();

        currentObject = highlightObject.currentObjectName;
        allReset = false;
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    void Update()
    {
        if (!IsPointerOverUIObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                mousePosBefore = Input.mousePosition;
                timer = Time.time;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector3 mousePositionAfter = Input.mousePosition;
                float distance = Vector3.Distance(mousePositionAfter, mousePosBefore);        

                if ((Time.time - timer) < 0.2 && distance < 5)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out RaycastHit hit) || EventSystem.current.IsPointerOverGameObject())
                    {
                        if (hit.collider != null)
                        {
                            if (currentObject == hit.collider.gameObject.name)
                            {
                                Text displayText = GameObject.Find("Region_text").GetComponent<Text>();
                                displayText.text = "Selected Region:";

                                highlightObject.StopHighlight();
                                highlightObject.ResetRenderingMode();
                                highlightObject = null;
                                allReset = true;

                                currentObject = "";

                                if (GameObject.Find("Show_region_button").GetComponent<Button>().interactable == false)
                                {
                                    GameObject.Find("Show_region_button").GetComponent<Button>().interactable = true;
                                }

                                DropdownFilter filter = FindObjectOfType<DropdownFilter>();
                                filter.currentSelected = "";                            
                            }
                            else
                            {
                                HighlightObject r = hit.collider.gameObject.GetComponent<HighlightObject>();
                                SelectObject(r);
                                r.SetRegionName();
                            }
                        }
                    }
                    else
                    {
                        Text displayText = GameObject.Find("Region_text").GetComponent<Text>();
                        displayText.text = "Selected Region:";

                        if (highlightObject != null && allReset == false)
                        {                         
                            highlightObject.StopHighlight();
                            highlightObject.ResetRenderingMode();
                            highlightObject = null;
                            allReset = true;

                            currentObject = "";

                            if (GameObject.Find("Show_region_button").GetComponent<Button>().interactable == false)
                            {
                                GameObject.Find("Show_region_button").GetComponent<Button>().interactable = true;
                            }

                            DropdownFilter filter = FindObjectOfType<DropdownFilter>();
                            filter.currentSelected = "";                       
                        }
                    }
                }
            }
        }
    }
}
