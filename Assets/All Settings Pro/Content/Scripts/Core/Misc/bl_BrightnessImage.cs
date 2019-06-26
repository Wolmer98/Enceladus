using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class bl_BrightnessImage : MonoBehaviour {

    private float Value = 1;
    private Material material;

    void Start()
    {
        transform.SetAsLastSibling();
        material = GetComponent<Image>().material;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="val">brightness value 0 to 1</param>
    public void SetValue(float val)
    {
        val /= 2;
        val = Mathf.Max(0.001f, val);
        Value = val;
        Alpha.alpha = (Value);

        if (material != null)
        {
            material.SetColor("_BrightnessColor", Color.white * Value);
        }
    }

    private CanvasGroup _Alpha;
    private CanvasGroup Alpha
    {
        get
        {
            if(_Alpha == null) { _Alpha = GetComponent<CanvasGroup>(); }
            return _Alpha;
        }
    }
}