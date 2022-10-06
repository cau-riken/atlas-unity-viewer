using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class HighlightObject : MonoBehaviour
{
    public float animationTime = 1.5f;
    public float threshold = 1.5f;
    public string currentObjectName = "";

    private Material material;
    private Color normalColor;
    private Color selectedColor;
    public bool active = false;
  
    private string hemisphere = "";
    private string hemisphereInv = "";

    public Text displayText;
    private GameObject[] objList;

    private void Awake()
    {
        objList = FindObjectsOfType<GameObject>();
        displayText = GameObject.Find("Region_text").GetComponent<Text>();
        material = GetComponentInChildren<Renderer>().material;
        name = gameObject.name.ToString();
        normalColor = material.color;

        selectedColor = new Color(
            Mathf.Clamp01(255 * threshold),
            Mathf.Clamp01(255 * threshold),
            Mathf.Clamp01(0 * threshold)
            );
    }
    public void StartHighlight()
    {
        active = true;

        currentObjectName = gameObject.name;

        material.shader = Shader.Find("Outlined/UltimateOutline");
        material.SetColor("_SecondOutlineColor", Color.black);
        material.SetFloat("_FirstOutlineWidth", 0.3f);
        material.SetFloat("_SecondOutlineWidth", 0.05f);
        material.renderQueue = 3500;

        iTween.ColorTo(gameObject, iTween.Hash(
            "color", selectedColor,
            "time", animationTime,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.pingPong
        ));

        if(gameObject.name.Contains("LH"))
        {
            hemisphere = "LH";
            hemisphereInv = "RH";
        }
        else if (gameObject.name.Contains("RH"))
        {
            hemisphere = "RH";
            hemisphereInv = "LH";
        }

        for (var i = 0; i < objList.Length; i++)
        {
            if (objList[i].name.Contains(hemisphere) && objList[i].name.ToString() != gameObject.name.ToString())
            {
                Color current = objList[i].GetComponentInChildren<Renderer>().material.color;
                current.a = 0.1f;
                objList[i].GetComponentInChildren<Renderer>().material.color = current;
                SetupMaterialWithBlendMode(objList[i].GetComponentInChildren<Renderer>().material, BlendMode.Transparent);     
            }
            else if (objList[i].name.Contains(hemisphereInv))
            {
                Color current = objList[i].GetComponentInChildren<Renderer>().material.color;
                current.a = 1f;
                objList[i].GetComponentInChildren<Renderer>().material.color = current;
                SetupMaterialWithBlendMode(objList[i].GetComponentInChildren<Renderer>().material, BlendMode.Opaque);            
            }
        }
    }

    public void StopHighlight()
    {
        active = false;
        material.color = normalColor;
        material.shader = Shader.Find("Standard");
        material.renderQueue = 3000;
        iTween.Stop(gameObject, true);
    }

    public void ResetRenderingMode()
    {
        active = false;
      
        for (var i = 0; i < objList.Length; i++)
        {
            if (objList[i].name.Contains("LH") || objList[i].name.Contains("RH"))
            {
                Color current = objList[i].GetComponentInChildren<Renderer>().material.color;
                current.a = 1f;
                objList[i].GetComponentInChildren<Renderer>().material.color = current;
                SetupMaterialWithBlendMode(objList[i].GetComponentInChildren<Renderer>().material, BlendMode.Opaque);
            }
        }
    }

    public void SetRegionName()
    {
        string tempName = gameObject.name.ToString().Replace('_', ' ');
        RegexOptions options = RegexOptions.None;
        Regex regex = new("[ ]{2,}", options);
        tempName = regex.Replace(tempName, " ");

        string[] objectRegionName = tempName.Split(' ');
        int regionNameLength = objectRegionName.Length - 3;
        string regionNameFinal = "";

        for (int i = 0; i < regionNameLength; i++)
        {
            regionNameFinal += char.ToUpper(objectRegionName[3 + i][0]) + objectRegionName[3 + i].Substring(1);

            if (i < regionNameLength)
            {
                regionNameFinal += " ";
            }
        }

        regionNameFinal = regionNameFinal.Substring(0, regionNameFinal.LastIndexOf("(")) +
            regionNameFinal.Substring(regionNameFinal.LastIndexOf("("), regionNameFinal.LastIndexOf(")") - regionNameFinal.LastIndexOf("(") + 1).Replace(' ', '/');

        displayText.text = "Selected Region:  <color=yellow>" + regionNameFinal + "</color> ";
    }

    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,        // Old school alpha-blending mode, fresnel does not affect amount of transparency
        Transparent, // Physically plausible transparency mode, implemented as alpha pre-multiply
        Multiply
    }

    public void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                material.SetOverrideTag("RenderType", "");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                break;
            case BlendMode.Fade:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                //material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case BlendMode.Multiply:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
        }
    }

}
