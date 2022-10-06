using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DropdownFilter : MonoBehaviour
{
    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private Dropdown dropdown;

    private Dictionary<string, string> regionDictionary;

    private List<Dropdown.OptionData> dropdownOptions;

    private List<string> regionList;
    private List<string> regionKeys;
    private Dropdown dropdownList;

    public string currentSelected = "";

    void Awake()
    {
        dropdownList = FindObjectOfType<Dropdown>();
        regionDictionary = new Dictionary<string, string>();

        GameObject[] objList = FindObjectsOfType<GameObject>();

        regionList = new List<string>();
        regionKeys = new List<string>();

        for (var i = 0; i < objList.Length; i++)
        {
            if (objList[i].name.Contains("LH"))
            {
                regionList.Add(objList[i].name.ToString());
            }
        }

        for(int i = 0; i < regionList.Count; i++)
        {
            string tempName = regionList[i].Replace('_', ' ');

            RegexOptions options = RegexOptions.None;
            Regex regex = new("[ ]{2,}", options);

            tempName = regex.Replace(tempName, " ");

            string[] objectRegionName = tempName.Split(' ');
            int regionNameLength = objectRegionName.Length - 3;
            string regionNameFinal = "";

            for (int j = 0; j < regionNameLength; j++)
            {
                regionNameFinal += char.ToUpper(objectRegionName[3 + j][0]) + objectRegionName[3 + j][1..];

                if (j < regionNameLength)
                {
                    regionNameFinal += " ";
                }
            }

            regionNameFinal = regionNameFinal[..regionNameFinal.LastIndexOf("(")] +
                regionNameFinal.Substring(regionNameFinal.LastIndexOf("("), regionNameFinal.LastIndexOf(")") - regionNameFinal.LastIndexOf("(") + 1).Replace(' ', '/');

            regionKeys.Add(regionNameFinal);
        }

        for(int i = 0; i < regionKeys.Count; i++)
        {
            regionDictionary.Add(regionKeys[i], regionList[i]);
        }

        regionKeys.Sort((x, y) => string.Compare(x.ToString(), y.ToString()));
        regionKeys.Insert(0, "Region List:");

        dropdown.AddOptions(regionKeys);
        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged();
        });

        GameObject.Find("Show_region_button").GetComponent<Button>().interactable = false;
    }

    void Start()
    {
        dropdownOptions = dropdown.options;
    }

    public void DropdownValueChanged()
    {
        string input =  dropdownList.options[dropdownList.value].text;  

        if (currentSelected != input && input != "Region List:")
        {    
            currentSelected = input;
            GameObject.Find("Show_region_button").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("Show_region_button").GetComponent<Button>().interactable = false;
        }
    }

    public string GetRegionFromDict(string key)
    {
        return regionDictionary[key];
    }

    public void FilterDropdown(string input)
    {
        dropdown.options = dropdownOptions.FindAll(option => option.text.ToLower().IndexOf(input) >= 0);

        if (dropdownList.options.Count != 0)
        {
            GameObject.Find("Show_region_button").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("Show_region_button").GetComponent<Button>().interactable = false;
        }
    }
}
