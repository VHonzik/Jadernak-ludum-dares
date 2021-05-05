using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class Rune : MonoBehaviour {

    private float _timeToAppear = 1.0f;

    public Color _pulseLow;
    public Color _pulseHigh;

	void Awake () {
        GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        NewPulse();
	}

    bool _staring;	

    // visibility
    float _visTimer;

    // pulsing;
    float _pulseDuration;
    float _pulsePeakDuration;
    float _pulseBackDuration;
    float _pulseLowDuration;
    float _pulseTimer;

	void Update () {

        _visTimer = Mathf.Clamp(_visTimer + (_staring ? +1 : -1) * Time.deltaTime, 0f, _timeToAppear);

        Color newColor = GetComponent<Renderer>().material.color;
        newColor.a = Mathf.Lerp(0,1,_visTimer / _timeToAppear);
        GetComponent<Renderer>().material.color = newColor;

        _pulseTimer += Time.deltaTime;

        Color newEmColor = GetComponent<Renderer>().material.GetColor("_EmissionColor");

        if(_pulseTimer < _pulseDuration)
        {
            float t = _pulseTimer / _pulseDuration;
            newEmColor = Color.Lerp(_pulseLow, _pulseHigh, t);
        }
        else if (_pulseTimer > _pulseDuration + _pulsePeakDuration && _pulseTimer < _pulseDuration + _pulsePeakDuration + _pulseBackDuration)
        {
            float t = (_pulseTimer - _pulseDuration - _pulsePeakDuration) / _pulseBackDuration;
            newEmColor = Color.Lerp(_pulseHigh, _pulseLow, t);
        }

        GetComponent<Renderer>().material.SetColor("_EmissionColor", newEmColor);

        if (Mathf.Abs(_pulseTimer) > _pulseDuration + _pulsePeakDuration + _pulseBackDuration + _pulseLowDuration)
        {
            NewPulse();
        }
	}

    public void UpdateStaring(bool staring)
    {
        _staring = staring;
    }

    private void NewPulse()
    {
        _pulseTimer = 0f;
        _pulseDuration = 0.5f + Random.value * 0.5f;
        _pulsePeakDuration = 0.2f + Random.value * 0.3f;
        _pulseBackDuration = 0.5f + Random.value * 1.0f;
        _pulseLowDuration = 0.2f + Random.value * 0.3f;

    }

}
