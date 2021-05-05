using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StarPuzzle : MonoBehaviour
{

    private GameObject _gazingFire = null;
    private List<GameObject> _selectedStars = new List<GameObject>();
    private List<GameObject> _lines = new List<GameObject>();

    private bool[] _victory;
    private VictoryPrerequisite[] _victoryPrereq;
    private int _otherLinesCount;

    private bool _won = false;

    // Use this for initialization
    void Start()
    {
        _victory = new bool[0];
        _victoryPrereq = new VictoryPrerequisite[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(_gazingFire && _victoryPrereq.Length > 0 && !_won)
            CheckVictory();

    }

    private void CheckVictory()
    {
        // Lines ready?
        foreach (GameObject line in _lines)
        {
            if (!line.GetComponent<StarLine>()._appeared)
            {
                return;
            }
        }

        // All prerequisites met?
        foreach (bool preeq in _victory)
        {
            if (!preeq) return;
        }

        // No additional line
        if (_otherLinesCount > 0) return;

        _won = true;

        _gazingFire.GetComponent<StarGazer>().Victory();

    }

    public void PuzzleStarted(GameObject fire)
    {
        Task hideFog = new Task(WorldManager.Instance.ShowHideFog(false));
        _gazingFire = fire;
        _selectedStars.Clear();
        _lines.Clear();

        int n = _gazingFire.GetComponent<StarGazer>().VictoryConditions.Length;
        _victoryPrereq = new VictoryPrerequisite[n];
        Array.Copy(_gazingFire.GetComponent<StarGazer>().VictoryConditions, _victoryPrereq, n);

        _victory = new bool[n];

        _won = false;

        _otherLinesCount = 0;
    }

    public void PuzzleExited()
    {
        
        foreach (GameObject line in _lines)
        {
            Destroy(line);
        }

        _lines.Clear();
    }

    public void StarClicked(GameObject star)
    {
        if (_won) return; 

        // First select
        if(_selectedStars.Count == 0)
        {
            star.GetComponent<Star>().SetSelected(true);
            _selectedStars.Insert(0, star);
        }
        // Last selected clicked
        else if(_selectedStars[0] == star)
        {            
            if(_selectedStars.Count > 1)
            {
                GameObject line = _lines[0];
                CheckVictoryPrereq(line.GetComponent<StarLine>()._from, line.GetComponent<StarLine>()._to, false);
                _lines.RemoveAt(0);
                line.GetComponent<StarLine>().Disapper();
            }

            _selectedStars.RemoveAt(0);
            star.GetComponent<Star>().SetSelected(false);
        }
        // Previous to last clicked
        else if (_selectedStars.Count > 1 && _selectedStars[1] == star)
        {
            GameObject line = _lines[0];
            _lines.RemoveAt(0);
            line.GetComponent<StarLine>().Disapper();

            CheckVictoryPrereq(line.GetComponent<StarLine>()._from, line.GetComponent<StarLine>()._to, false);

            GameObject lastStar = _selectedStars[0];
            _selectedStars.RemoveAt(0);

            lastStar.GetComponent<Star>().SetSelected(false);
        }
        // Otherwise
        else 
        {
            // Check for already existing star
            GameObject from = _selectedStars[0];

            foreach(GameObject line in _lines)
            {
                bool checkFrom = line.GetComponent<StarLine>()._from == from || line.GetComponent<StarLine>()._from == star;
                bool checkTo = line.GetComponent<StarLine>()._to == from || line.GetComponent<StarLine>()._to == star;

                if(checkFrom && checkTo)
                {
                    line.GetComponent<StarLine>().Flash();
                    return;
                }
            }

            CheckVictoryPrereq(from, star, true);

            // Create a line
            star.GetComponent<Star>().SetSelected(true);
            _lines.Insert(0, StarLine.CreateStarLine(_selectedStars[0], star));
            _selectedStars.Insert(0, star);
        }
    }

    private void CheckVictoryPrereq(GameObject from, GameObject to, bool adding)
    {
        for (int i = 0; i < _victoryPrereq.Length; i++)
        {
            VictoryPrerequisite prereq = _victoryPrereq[i];
            if ((from.GetComponent<Star>()._index == prereq.starIndexA && to.GetComponent<Star>()._index == prereq.starIndexB) ||
                (from.GetComponent<Star>()._index == prereq.starIndexB && to.GetComponent<Star>()._index == prereq.starIndexA))
            {
                Debug.Log("Prerequisitie : " + prereq.starIndexA + "-" + prereq.starIndexB + (adding ? " met" : " unmet"));
                _victory[i] = adding;
                return;
            }
        }

        // If we got there it means the line was not part of prerequisites
        _otherLinesCount = _otherLinesCount + (adding ? +1 : -1);
    }
}
