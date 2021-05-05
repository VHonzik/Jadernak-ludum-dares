using UnityEngine;
using System.Collections;

public class CampFire : MonoBehaviour
{

    public int puzzle = 0;

    private float distanceTreshold = 3.5f;
    private float stareMinimum = 0.1f;
    private float stareRequired = 0.5f;

    private Vector3 runeOffset = new Vector3(0f, 0.9f, 0f);

    private float kindleDuration = 0.5f;
    private float origLightSize;
    private float kindleLightSize = 20f;
    private float origFireScale;
    private float kindleFireScale = 3f;

    public bool _cleared = false;

    private Material _runeMaterial;

    private GameObject rune;

    private float staringTimer;

    private AudioSource _audioSourceLow;
    private AudioSource _audioSourceKindled;

    // Use this for initialization
    void Awake()
    {
        origLightSize = GetComponentInChildren<Light>().range;
        origFireScale = transform.FindChild("Flame Particles").transform.localScale.x;

        staringTimer = 0f;
    }

    void Start()
    {
        _runeMaterial = AssetManager.Instance.Puzzles[puzzle].runeMaterial;
        rune = (GameObject)GameObject.Instantiate(AssetManager.Instance.GetPrefab("RunePrefab"), transform.position + runeOffset, transform.rotation);
        rune.GetComponent<Renderer>().material = _runeMaterial;

        GameObject audioSourceLow = new GameObject("fire-sound-source_low");
        audioSourceLow.transform.position = transform.position;
        audioSourceLow.transform.parent = transform;
        _audioSourceLow = audioSourceLow.AddComponent<AudioSource>();        
        _audioSourceLow.clip = AssetManager.Instance.GetSound("fire-small");
        _audioSourceLow.loop = true;
        _audioSourceLow.spatialBlend = 1f;
        _audioSourceLow.Play();

        GameObject audioSourceHigh = new GameObject("fire-sound-source_low");
        audioSourceHigh.transform.position = transform.position;
        audioSourceHigh.transform.parent = transform;
        _audioSourceKindled = audioSourceHigh.AddComponent<AudioSource>();
        _audioSourceKindled.clip = AssetManager.Instance.GetSound("fire-medium");
        _audioSourceKindled.loop = true;
        _audioSourceKindled.spatialBlend = 1f;
        _audioSourceKindled.Play();
        _audioSourceKindled.Pause();

    }

    // Update is called once per frame
    void Update()
    {
        if (_cleared)
        {
            Kindle(true);
            rune.GetComponent<Rune>().UpdateStaring(false);
        }
        else
        {
            Kindle(false);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            
            rune.GetComponent<Rune>().UpdateStaring(false);

            // Close enough
            if (distanceToPlayer <= distanceTreshold)
            {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

                // Hitting the fire stare collider?
                RaycastHit hit;
                bool hitted = Physics.Raycast(ray, out hit, distanceToPlayer * 1.5f, 1 << 8);

                if (hitted)
                {
                    staringTimer += Time.deltaTime;
                    if (staringTimer > stareMinimum)
                    {
                        rune.GetComponent<Rune>().UpdateStaring(true);

                        if (staringTimer > stareRequired && Input.GetMouseButtonDown(1))
                        {
                            GetComponent<StarGazer>().GazeUponStars(puzzle);
                        }
                    }

                }
                else
                {
                    staringTimer = 0f;
                }
            }
            else
            {
                staringTimer = 0f;
            }

        }
    }

    public void Kindle(bool shouldKindle)
    {
        
        GameObject flameGO = transform.FindChild("Flame Particles").gameObject;

        if (shouldKindle && GetComponentInChildren<Light>().range < kindleLightSize)
        {
            float newRange = GetComponentInChildren<Light>().range + Time.deltaTime * (kindleLightSize - origLightSize) / kindleDuration;
            newRange = Mathf.Min(newRange, kindleLightSize);
            GetComponentInChildren<Light>().range = newRange;
        }

        if (!shouldKindle && GetComponentInChildren<Light>().range > origLightSize)
        {
            float newRange = GetComponentInChildren<Light>().range - Time.deltaTime * (kindleLightSize - origLightSize) / kindleDuration;
            newRange = Mathf.Max(newRange, origLightSize);
            GetComponentInChildren<Light>().range = newRange;
        }

        if (shouldKindle && flameGO.transform.localScale.x < kindleFireScale)
        {
            float newScale = flameGO.transform.localScale.x + Time.deltaTime * (kindleFireScale - origFireScale) / kindleDuration;
            newScale = Mathf.Min(newScale, kindleFireScale);
            flameGO.transform.localScale = Vector3.one * newScale; 
        }

        if (!shouldKindle && flameGO.transform.localScale.x > origFireScale)
        {
            float newScale = flameGO.transform.localScale.x - Time.deltaTime * (kindleFireScale - origFireScale) / kindleDuration;
            newScale = Mathf.Max(newScale, origFireScale);
            flameGO.transform.localScale = Vector3.one * newScale;
        }


        if (_audioSourceLow.isPlaying && shouldKindle)
            _audioSourceLow.Pause();
        else if (!_audioSourceLow.isPlaying && !shouldKindle)
            _audioSourceLow.UnPause();

        if (!_audioSourceKindled.isPlaying && shouldKindle)
            _audioSourceKindled.UnPause();
        else if(_audioSourceKindled.isPlaying && !shouldKindle)
            _audioSourceLow.Pause();
    }
}
