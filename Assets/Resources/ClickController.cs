using UnityEngine;
using UnityEngine.UI;

public class ClickController : MonoBehaviour
{
    public InputField inputFieldName;
    private HighlightController controller;

    private DropdownFilter dropFilter;
    private Dropdown dropdownList;

    private string input = "";
    void Awake()
    {
        controller = FindObjectOfType<HighlightController>();
        dropFilter = FindObjectOfType<DropdownFilter>();
        dropdownList = FindObjectOfType<Dropdown>();
    }

    public void Start()
    {
        inputFieldName.onValueChanged.AddListener(delegate{ dropFilter.FilterDropdown(inputFieldName.text.ToLower()); });
    }

    public void HighlightSelected()
    {
        if (dropdownList.options.Count == 1)
        {
            input = dropdownList.options[0].text;
        }
        else
        {
            input = dropdownList.options[dropdownList.value].text;
        }    

        if (input != "Region List:")
        {
            GameObject a = GameObject.Find(dropFilter.GetRegionFromDict(input));
        
            HighlightObject r = a.GetComponent<HighlightObject>();
            controller.SelectObject(r);
            r.SetRegionName();

            GameObject.Find("Show_region_button").GetComponent<Button>().interactable = false;

        }
        else
        {
            GameObject.Find("Show_region_button").GetComponent<Button>().interactable = false;
        }
    }
}
