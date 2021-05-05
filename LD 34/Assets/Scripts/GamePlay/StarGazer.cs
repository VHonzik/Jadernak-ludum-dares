using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

[System.Serializable]
public class VictoryPrerequisite // Prerequisite
{
    public int starIndexA;
    public int starIndexB;
}

public class StarGazer : MonoBehaviour
{

    private GameObject _fireCameraGO;
    private Camera _fireCamera;
    private GameObject _rune;

    private List<GameObject> _stars;

    private Vector2[] _starPositions;
    private VictoryPrerequisite[] _victoryConditions;

    public VictoryPrerequisite[] VictoryConditions
    {
        get { return _victoryConditions; }
        private set { _victoryConditions = value; }
    }

    private float _starDistance = 10f;
    private float _starScale = 0.2f;

    private float _runeDistance = 3f;

    private Quaternion _prepareRotStart;
    private Quaternion _prepareRotEnd;
    private Vector3 _prepareStart;
    private Vector3 _prepareEnd;

    private float _cameraForwardSpeed = 1.0f;
    private float _cameraDownSpeed = 1.0f;
    private float _cameraRotateSpeed = 40.0f;

    private bool _prepared = false;
    private bool _preparing = false;
    private bool _returning = false;
    private bool _cleared = false;

    private int _puzzle;

    private Texture _victoryTexture;
    private float _victoryTimer = -1f;
    private float _victoryAppear = 3f;
    private float _victoryStay = 5f;
    private float _victoryDisappear = 2f;
    private float _maxVictoryAlpha = 0.4f;

    void Awake()
    {
        _stars = new List<GameObject>();
    }


    void Update()
    {
        if (_preparing && _fireCameraGO)
        {
            Vector3 newPos = _fireCameraGO.transform.position;
            Quaternion newRot = _fireCameraGO.transform.rotation;

            Vector3 XZdiff = (newPos - _prepareEnd);
            XZdiff.y = 0;

            if (XZdiff.magnitude > 1e-1)
            {
                newPos = Vector3.MoveTowards(newPos, _prepareEnd, _cameraForwardSpeed * Time.deltaTime);
                newPos.y = _fireCameraGO.transform.position.y;
            }
            else if (Mathf.Abs(_prepareEnd.y - newPos.y) > 1e-2)
            {
                newPos = Vector3.MoveTowards(newPos, _prepareEnd, _cameraDownSpeed * Time.deltaTime);
                newPos.x = _fireCameraGO.transform.position.x;
                newPos.z = _fireCameraGO.transform.position.z;
            }
            else if (Quaternion.Angle(newRot, _prepareRotEnd) > 1e-1)
            {
                newRot = Quaternion.RotateTowards(newRot, _prepareRotEnd, _cameraRotateSpeed * Time.deltaTime);
            }
            else
            {
                _preparing = false;
                GazeReady();
            }

            _fireCameraGO.transform.position = newPos;
            _fireCameraGO.transform.rotation = newRot;
        }


        if (_returning && _fireCameraGO)
        {

            Vector3 newPos = _fireCameraGO.transform.position;
            Quaternion newRot = _fireCameraGO.transform.rotation;

            Vector3 XZdiff = (newPos - _prepareEnd);
            XZdiff.y = 0;


            if (Quaternion.Angle(newRot, _prepareRotEnd) > 1e-1)
            {
                newRot = Quaternion.RotateTowards(newRot, _prepareRotEnd, _cameraRotateSpeed * Time.deltaTime);
            }
            else if (Mathf.Abs(_prepareEnd.y - newPos.y) > 1e-2)
            {
                newPos = Vector3.MoveTowards(newPos, _prepareEnd, _cameraDownSpeed * Time.deltaTime);
                newPos.x = _fireCameraGO.transform.position.x;
                newPos.z = _fireCameraGO.transform.position.z;
            }
            else if (XZdiff.magnitude > 1e-1)
            {
                newPos = Vector3.MoveTowards(newPos, _prepareEnd, _cameraForwardSpeed * Time.deltaTime);
                newPos.y = _fireCameraGO.transform.position.y;
            }
            else
            {
                _returning = false;
                SightReturned();
            }

            if (_fireCameraGO)
            {
                _fireCameraGO.transform.position = newPos;
                _fireCameraGO.transform.rotation = newRot;
            }


        }

        if (_prepared && Input.GetMouseButtonDown(1) && !_returning && !_cleared)
        {
            ReturnSight();
        }

    }

    void OnGUI()
    {
        if (_victoryTimer >= 0f)
        {
            _victoryTimer += Time.deltaTime;

            Vector2 start;
            float size;
            GetStarScreenSize(out start, out size);

            Color prev = GUI.color;

            if (_victoryTimer < _victoryAppear + _victoryStay)
            {
                Color color = Color.white;
                color.a = Mathf.Lerp(0, _maxVictoryAlpha, (_victoryTimer / _victoryAppear));
                GUI.color = color;
            }

            if (_victoryTimer > _victoryAppear + _victoryStay)
            {
                Color color = Color.white;
                color.a = (1f - ((_victoryTimer - (_victoryAppear + _victoryStay)) / _victoryDisappear)) * _maxVictoryAlpha;
                GUI.color = color;
            }

            GUI.DrawTexture(new Rect(start, new Vector2(size, size)), _victoryTexture);


            GUI.color = prev;


            if (_victoryTimer > _victoryAppear + _victoryStay + _victoryDisappear && !_returning)
            {
                ReturnSight();
            }
        }
    }

    private void GazeReady()
    {
        GameObject.FindGameObjectWithTag("Player").transform.Find("Particles").transform.localPosition = new Vector3(0, -1, 0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _rune.transform.position = _fireCamera.ViewportToWorldPoint(new Vector3(0.9f, 0.15f, _runeDistance));

        _rune.GetComponent<Rune>().UpdateStaring(true);
        _prepared = true;

        SpawnStars();

        GameManager.Instance.GetComponent<StarPuzzle>().PuzzleStarted(this.gameObject);
    }

    private void SpawnStars()
    {
        Vector2 start;
        float size;
        GetStarScreenSize(out start, out size);

        int i = 0;
        foreach (Vector2 pos in _starPositions)
        {
            Vector2 relativePos = pos / 512;
            relativePos.y = 1f - relativePos.y;

            Vector3 screenPos = new Vector3(start.x + relativePos.x * size, start.y + relativePos.y * size, _starDistance);
            Vector3 worldPos = _fireCamera.ScreenToWorldPoint(screenPos);

            GameObject star = (GameObject)GameObject.Instantiate(AssetManager.Instance.GetPrefab("Star"), worldPos, Quaternion.identity);
            _stars.Add(star);
            star.GetComponent<Star>()._index = i;
            star.GetComponent<CameraFacingBillboard>().m_Camera = _fireCamera;
            star.GetComponent<CameraFacingBillboard>().amActive = true;
            star.transform.localScale = Vector3.one * _starScale;

            i++;
        }
    }

    private static void GetStarScreenSize(out Vector2 start, out float size)
    {
        if (Screen.width > Screen.height)
        {
            size = Screen.height;
            start = new Vector2((Screen.width - Screen.height) / 2f, 0f);
        }
        else
        {
            size = Screen.width;
            start = new Vector2(0f, (Screen.height - Screen.width) / 2f);
        }
    }

    private void DestroyStars()
    {
        foreach (GameObject star in _stars)
        {
            Destroy(star);
        }
    }

    private void ReturnSight()
    {
        GameObject.FindGameObjectWithTag("Player").transform.Find("Particles").transform.localPosition = GameSettings.snowLocalPosition;
        Task showFog = new Task(WorldManager.Instance.ShowHideFog(true));
        _rune.GetComponent<Rune>().UpdateStaring(false);

        Vector3 tmp = _prepareStart;
        _prepareStart = _prepareEnd;
        _prepareEnd = tmp;

        Quaternion tmp2 = _prepareRotStart;
        _prepareRotStart = _prepareRotEnd;
        _prepareRotEnd = tmp2;

        _returning = true;
    }

    private void SightReturned()
    {
        _prepared = false;

        Destroy(_rune);

        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        //player.GetComponent<MouseLook>().enabled = true;
        player.GetComponent<FirstPersonController>().enabled = true;

        DestroyStars();
        GameManager.Instance.GetComponent<StarPuzzle>().PuzzleExited();

        _fireCameraGO = null;
        _fireCamera = null;

        _victoryTimer = -1f;

        Cursor.lockState = CursorLockMode.Locked;

        if (_cleared)
            GetComponent<CampFire>()._cleared = true;
    }

    public void GazeUponStars(int puzzle)
    {
        if (_cleared) return;

        _puzzle = puzzle;


        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<FirstPersonController>().enabled = false;
        //player.GetComponent<FPSInputController>().enabled = false;
        //mainCamera.gameObject.GetComponent<MouseLook>().enabled = false;

        _fireCameraGO = mainCamera.gameObject;
        _fireCamera = mainCamera;

        _starPositions = AssetManager.Instance.Puzzles[_puzzle].starPositions;
        _victoryConditions = AssetManager.Instance.Puzzles[_puzzle].prereq;

        _prepareStart = _fireCameraGO.transform.position;
        Vector3 prepareRel = _prepareStart - transform.position;
        prepareRel.y = 0f;
        // Come close if necessary
        if (prepareRel.magnitude > 2.78f)
        {
            prepareRel.Normalize();
            prepareRel *= 2.78f;
        }
        prepareRel.y = 1.0f;
        _prepareEnd = transform.position + prepareRel;

        _prepareRotStart = _fireCameraGO.transform.rotation;
        _prepareRotEnd = Quaternion.Euler(270f, _prepareRotStart.eulerAngles.y, _prepareRotStart.eulerAngles.z);

        _rune = (GameObject)GameObject.Instantiate(AssetManager.Instance.GetPrefab("RunePrefab"), Vector3.zero, Quaternion.identity);
        _rune.GetComponent<CameraFacingBillboard>().m_Camera = _fireCamera;
        _rune.GetComponent<CameraFacingBillboard>().amActive = true;
        _rune.GetComponent<Renderer>().material = AssetManager.Instance.Puzzles[puzzle].runeMaterial;

        _victoryTexture = AssetManager.Instance.Puzzles[_puzzle].pictureMaterial.mainTexture;

        _preparing = true;

    }

    public void Victory()
    {
        _victoryTimer = 0f;
        _cleared = true;
        WorldManager.Instance.solvedFires.Add(gameObject);
        Task FirstCampfire = new Task(WorldManager.Instance.SpawnCampfireInTime());
    }
}
