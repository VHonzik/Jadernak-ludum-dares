using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarPuzzleInfo
{
    public Material runeMaterial;
    public Material pictureMaterial;
    public Vector2[] starPositions;
    public VictoryPrerequisite[] prereq;

    public int difficulty;
};


public class AssetManager : MonoBehaviour {

    public List<Material> Materials = new List<Material>();
    public List<GameObject> Prefabs = new List<GameObject>();
    public List<AudioClip> Sounds = new List<AudioClip>();

    // Winter - 0
    public List<StarPuzzleInfo> Puzzles = new List<StarPuzzleInfo>();


    public Dictionary<byte, Rect> TexturePosition { get; set; }

    private static AssetManager _instance;

    public static AssetManager Instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AssetManager>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        FillPuzzleInfo();

    }

    public Material GetMaterial(string name)
    {
        return Materials.Find(x => x.name == name);
    }

    public GameObject GetPrefab(string name)
    {
        return Prefabs.Find(x => x.name == name);
    }

    public AudioClip GetSound(string name)
    {
        return Sounds.Find(x => x.name == name);
    }


    void FillPuzzleInfo()
    {
        // Winter
        StarPuzzleInfo winter = new StarPuzzleInfo();
        winter.runeMaterial = GetMaterial("RuneWinter");
        winter.pictureMaterial = GetMaterial("PictureWinter");

        winter.starPositions = new Vector2[15];
        winter.starPositions[0] = new Vector2(129.66f, 64.22f);
        winter.starPositions[1] = new Vector2(256f, 110.11f);
        winter.starPositions[2] = new Vector2(297.14f, 196.77f);
        winter.starPositions[3] = new Vector2(382.34f, 183.05f);
        winter.starPositions[4] = new Vector2(324.57f, 258.66f);
        winter.starPositions[5] = new Vector2(374.77f, 324.57f);
        winter.starPositions[6] = new Vector2(293.33f, 324.57f);
        winter.starPositions[7] = new Vector2(256f, 398.96f);
        winter.starPositions[8] = new Vector2(216.66f, 324.57f);
        winter.starPositions[9] = new Vector2(140.33f, 324.57f);
        winter.starPositions[10] = new Vector2(176.81f, 254.94f);
        winter.starPositions[11] = new Vector2(129.05f,183.05f);
        winter.starPositions[12] = new Vector2(224.66f, 196.77f);
        winter.starPositions[13] = new Vector2(269.71f, 244.94f);
        winter.starPositions[14] = new Vector2(402.21f, 405.94f);

        winter.prereq = new VictoryPrerequisite[12];
        for (int i = 0; i < winter.prereq.Length; i++)
        {
            winter.prereq[i] = new VictoryPrerequisite();
        }
        winter.prereq[0].starIndexA = 1; winter.prereq[0].starIndexB = 2;
        winter.prereq[1].starIndexA = 2; winter.prereq[1].starIndexB = 3;
        winter.prereq[2].starIndexA = 3; winter.prereq[2].starIndexB = 4;
        winter.prereq[3].starIndexA = 4; winter.prereq[3].starIndexB = 5;
        winter.prereq[4].starIndexA = 5; winter.prereq[4].starIndexB = 6;
        winter.prereq[5].starIndexA = 6; winter.prereq[5].starIndexB = 7;
        winter.prereq[6].starIndexA = 7; winter.prereq[6].starIndexB = 8;
        winter.prereq[7].starIndexA = 8; winter.prereq[7].starIndexB = 9;
        winter.prereq[8].starIndexA = 9; winter.prereq[8].starIndexB = 10;
        winter.prereq[9].starIndexA = 10; winter.prereq[9].starIndexB = 11;
        winter.prereq[10].starIndexA = 11; winter.prereq[10].starIndexB = 12;
        winter.prereq[11].starIndexA = 12; winter.prereq[11].starIndexB = 1;

        winter.difficulty = 1;

        Puzzles.Add(winter);
    
    }

  
}
