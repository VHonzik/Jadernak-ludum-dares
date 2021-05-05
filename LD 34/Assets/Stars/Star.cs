using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour
{
    public Color[] _colors;

    public int _index = -1;

    private float _appearDuration = 1.0f;
    private float _hiddenScale = 0.5f;
    private float _scaleSpeed = 0.4f;
    private float _maxScale = 1.2f;
    private float _origScale = -1f;
    
    private float _appearTimer;
    private Color _color;

    private GameObject _bg;
    private float _bgAppearDuration = 0.5f;
    private float _bgAppearTimer = 0f;


    private bool _mouseOver;
    private bool _selected;


    // Use this for initialization
    void Awake()
    {
        GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        _color = _colors[Random.Range(0, _colors.Length)];
        GetComponent<Renderer>().material.SetColor("_EmissionColor", _color);
        _appearTimer = 0f;
        _bg = transform.FindChild("StarBG").gameObject;

        _bg.GetComponent<Renderer>().material.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        if(_origScale < 0f )
        {
            _origScale = transform.localScale.x;
        }
        if (_appearTimer < _appearDuration)
        {
            GetComponent<Renderer>().material.color = Color.Lerp(Color.clear, Color.white, _appearTimer / _appearDuration);
            _appearTimer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one * _origScale * _hiddenScale, Vector3.one * _origScale, _appearTimer / _appearDuration);
        }

        if (transform.localScale.x < _maxScale * _origScale && _mouseOver)
        {
            transform.localScale += Vector3.one * _scaleSpeed * Time.deltaTime;
            transform.localScale = Vector3.Min(transform.localScale, Vector3.one * _maxScale * _origScale);
        }
        else if (transform.localScale.x > 1.0f * _origScale && !_mouseOver )
        {
            transform.localScale -= Vector3.one * _scaleSpeed * Time.deltaTime;
            transform.localScale = Vector3.Max(transform.localScale, Vector3.one * _origScale);
        }

        if (_selected)
        {
            _bgAppearTimer = Mathf.Min(_bgAppearDuration, _bgAppearTimer + Time.deltaTime);
            _bg.GetComponent<Renderer>().material.color = Color.Lerp(Color.clear, Color.white, _bgAppearTimer);
        }
        else
        {
            _bgAppearTimer = Mathf.Max(0f, _bgAppearTimer - Time.deltaTime);
            _bg.GetComponent<Renderer>().material.color = Color.Lerp(Color.clear, Color.white, _bgAppearTimer);
        }

        if (_mouseOver && Input.GetMouseButtonDown(0))
        {
            GameManager.Instance.GetComponent<StarPuzzle>().StarClicked(this.gameObject);
            _bgAppearTimer = 0f;
        }

    }

    void OnMouseEnter()
    {
        _mouseOver = true;
    }

    void OnMouseExit()
    {
        _mouseOver = false;
    }

    public void SetSelected(bool selected)
    {
        _selected = selected;
    }
}
