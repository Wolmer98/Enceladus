using UnityEngine;
using System.Collections.Generic;

public class GlowObject : MonoBehaviour
{
    public Color GlowColor;
    public float LerpFactor = 10;
    [Tooltip("If this is assigned this is the only renderers which will recieve an outline.")]
    [SerializeField] Renderer[] overrideRenderers;

    public Renderer[] Renderers
    {
        get;
        private set;
    }

    public Color CurrentColor
    {
        get { return _currentColor; }
    }

    private List<Material> _materials = new List<Material>();
    private Color _currentColor;
    private Color _targetColor;
    private PlayerController Player;

    void Start()
    {
        //Move up the component to the pickup-parent.
        //if (transform.parent != null && !GetComponent<Pickup>() && transform.parent.name != "World" && !transform.parent.name.Contains("Prop"))
        //{
        //    GlowObject parentComp = transform.parent.gameObject.AddComponent<GlowObject>();
        //    parentComp.GlowColor = GlowColor;
        //    parentComp.LerpFactor = LerpFactor;

        //    Destroy(gameObject.GetComponent<GlowObject>());
        //}

        FindObjectOfType<WorldGenerator>().OnWorldStart.AddListener(delegate { Init(); });
    }

    private void Init()
    {
        Player = FindObjectOfType<PlayerController>();

        FetchMaterials();
    }

    //private void OnMouseEnter()
    //{
    //    Debug.Log("NAME: " + gameObject.name);
    //    if (Vector3.Distance(transform.position, Player.transform.position) <= Player.Reach)
    //    {
    //        _targetColor = GlowColor;
    //        enabled = true;
    //    }
    //}

    //private void OnMouseExit()
    //{
    //    _targetColor = Color.black;
    //    enabled = true;
    //}

    public void SetTargetColor(Color col)
    {
        _targetColor = col;
        enabled = true;
    }

    private void FetchMaterials()
    {
        _materials.Clear();

        if (overrideRenderers.Length == 0)
        {
            Renderers = GetComponentsInChildren<Renderer>();
        }
        else
        {
            Renderers = overrideRenderers;
        }

        foreach (var renderer in Renderers)
        {
            _materials.AddRange(renderer.materials);
        }
    }

    /// <summary>
    /// Loop over all cached materials and update their color, disable self if we reach our target color.
    /// </summary>
    private void Update()
    {
        if (Player == null)
        {
            Player = FindObjectOfType<PlayerController>();
        }

        if (_materials.Count <= 0)
        {
            FetchMaterials();
        }

        _currentColor = Color.Lerp(_currentColor, _targetColor, Time.deltaTime * LerpFactor);

        for (int i = 0; i < _materials.Count; i++)
        {
            _materials[i].SetColor("_GlowColor", _currentColor);
        }

        if (_currentColor.Equals(_targetColor))
        {
            enabled = false;
        }
    }
}
