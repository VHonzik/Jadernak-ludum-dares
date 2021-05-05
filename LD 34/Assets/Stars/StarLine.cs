using UnityEngine;
using System.Collections;

public class StarLine : MonoBehaviour
{

    public GameObject _from;
    public GameObject _to;

    public Color _color;
    public Color _flashColor;

    public bool _appeared { get; private set; }

    private Vector3 _endPos;

    private float _lineWidth = 0.1f;

    private float _appearingSpeed = 10f;
    private bool _appearing = false;

    private float _disappearingSpeed = 20f;
    private bool _disappearing = false;

    private bool _flashing = false;
    private float _flashDuration = 1.0f;
    private float _flashTimer = 0.0f;


    // Use this for initialization
    void Awake()
    {
        GetComponent<LineRenderer>().SetPosition(0, Vector3.zero);
        GetComponent<LineRenderer>().SetPosition(1, Vector3.zero);
        GetComponent<LineRenderer>().SetWidth(_lineWidth, _lineWidth);
        _endPos = Vector3.zero;
        GetComponent<LineRenderer>().material.SetColor("_TintColor", _color);
    }

    // Update is called once per frame
    void Update()
    {
        if (_appearing)
        {
            Vector3 dir = (_to.transform.position - _endPos);
            Vector3 update = dir.normalized * Time.deltaTime * _appearingSpeed;
            if(update.magnitude < dir.magnitude)
            {
                _endPos = _endPos + update;
                GetComponent<LineRenderer>().SetPosition(1, _endPos);
            }
            else 
            {
                GetComponent<LineRenderer>().SetPosition(1, _to.transform.position);
                _appearing = false;
                _appeared = true;
            }            
        }
        else if (_disappearing)
        {
            Vector3 dir = (_from.transform.position - _endPos);
            Vector3 update = dir.normalized * Time.deltaTime * _disappearingSpeed;
            if(update.magnitude < dir.magnitude)
            {
                _endPos = _endPos + update;
                GetComponent<LineRenderer>().SetPosition(1, _endPos);
            }
            else 
            {
                Destroy(this.gameObject);
            }  
        }
        
        if(_flashing)
        {
            _flashTimer += Time.deltaTime;
            float t;
            if(_flashTimer < _flashDuration * 0.5f)
            {
                t = _flashTimer / (_flashDuration * 0.5f);
            }
            else
            {
                t = 1f - ((_flashTimer - (_flashDuration * 0.5f)) / (_flashDuration * 0.5f));
            }
            GetComponent<LineRenderer>().material.SetColor("_TintColor", Color.Lerp(_color, _flashColor, t));

            if(_flashTimer > _flashDuration)
            {
                _flashTimer = 0f;
                _flashing = false;
            }
        }

    }

    public static GameObject CreateStarLine(GameObject from, GameObject to)
    {
        GameObject line = (GameObject)Instantiate(AssetManager.Instance.GetPrefab("StarLine"));
        line.GetComponent<StarLine>()._from = from;
        line.GetComponent<StarLine>()._to = to;

        line.GetComponent<StarLine>().Appear();

        return line;
    }

    public void Appear()
    {
        GetComponent<LineRenderer>().SetPosition(0, _from.transform.position);
        GetComponent<LineRenderer>().SetPosition(1, _from.transform.position);
        _endPos = _from.transform.position;
        _appearing = true;
    }

    public void Disapper()
    {
        _disappearing = true;
    }

    public void Flash()
    {
        _flashing = true;
    }


}
