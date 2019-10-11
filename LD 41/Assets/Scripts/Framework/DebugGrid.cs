using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugGrid : MonoBehaviour
{
    public GameObject GridElement;
    public int GridSize;
    public int TeleportDistance;

    private List<GameObject> Grid;

    // Use this for initialization
    void Start()
    {
        Grid = new List<GameObject>(GridSize * GridSize);
        for (var i=0; i < GridSize; i++)
        {
            for (var j = 0; j < GridSize; j++)
            {
                Grid.Add(GameObject.Instantiate(GridElement));
                Grid[Grid.Count - 1].transform.position = new Vector3(-GridSize * 0.5f + i + 0.5f, -GridSize * 0.5f + j + 0.5f, 0);
                Grid[Grid.Count - 1].transform.parent = transform;
                Grid[Grid.Count - 1].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            if (GUILayout.Button("OffGrid"))
            {
                for (var i = 0; i < GridSize; i++)
                {
                    for (var j = 0; j < GridSize; j++)
                    {
                        Grid[i * GridSize + j].GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }

            if (GUILayout.Button("WalkableGrid"))
            {
                for (var i = 0; i < GridSize; i++)
                {
                    for (var j = 0; j < GridSize; j++)
                    {
                        Grid[i * GridSize + j].GetComponent<SpriteRenderer>().enabled = true;
                        Vector2 position = Grid[i * GridSize + j].transform.position;

                        if (Physics2D.OverlapCircleNonAlloc(position, 0.0f, new Collider2D[1], 1 << 8) > 0)
                        {
                            Grid[i * GridSize + j].GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);
                        }
                        else
                        {
                            Grid[i * GridSize + j].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                        }
                    }
                }
            }

            if (GUILayout.Button("TeleportGrid"))
            {
                List<int[]> walkables = new List<int[]>();
                for (var i = 0; i < GridSize; i++)
                {
                    for (var j = 0; j < GridSize; j++)
                    {
                        Grid[i * GridSize + j].GetComponent<SpriteRenderer>().enabled = true;
                        Vector2 position = Grid[i * GridSize + j].transform.position;

                        if (Physics2D.OverlapCircleNonAlloc(position, 0.0f, new Collider2D[1], 1 << 8) > 0)
                        {
                            Grid[i * GridSize + j].GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, 0.5f);
                            walkables.Add(new int[2] { i, j });
                        }
                        else
                        {
                            Grid[i * GridSize + j].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                        }
                    }
                }

                foreach (var walkable in walkables)
                {
                    int indexLeft = (walkable[0] - TeleportDistance) * GridSize + walkable[1];
                    if (walkables.Exists(x => x[0] == (walkable[0] - TeleportDistance) && x[1] == walkable[1]))
                    {
                        Grid[indexLeft].GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);
                    }

                    int indexRight = (walkable[0] + TeleportDistance) * GridSize + walkable[1];
                    if (walkables.Exists(x => x[0] == (walkable[0] + TeleportDistance) && x[1] == walkable[1]))
                    {
                        Grid[indexRight].GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);
                    }

                    int indexUp = walkable[0] * GridSize + (walkable[1] + TeleportDistance);
                    if (walkables.Exists(x => x[0] == walkable[0] && x[1] == (walkable[1] + TeleportDistance)))
                    {
                        Grid[indexUp].GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);
                    }

                    int indexDown = walkable[0] * GridSize + (walkable[1] - TeleportDistance);
                    if (walkables.Exists(x => x[0] == walkable[0] && x[1] == (walkable[1] - TeleportDistance)))
                    {
                        Grid[indexDown].GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);
                    }

                }

            }
        }

    }
}
