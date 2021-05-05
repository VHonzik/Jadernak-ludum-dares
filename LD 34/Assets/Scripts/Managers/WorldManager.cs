using UnityEngine;
using System.Collections;
using System;
using Entities;
using System.Collections.Generic;
using ProceduralGeneration;



public class WorldManager : MonoBehaviour
{
    private static WorldManager _instance;
    private ProcTerrainChunk[,] world;
    private ProcTerrainChunk[,] ice;
    private GameObject Player;
    public List<GameObject> solvedFires = new List<GameObject>();
    public Transform GrowingStar;

    public static WorldManager Instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<WorldManager>();
            }
            return _instance;
        }
    }


    void Awake()
    {
        _instance = this;
        Player = GameObject.FindGameObjectWithTag("Player");
        int xNumberOf = GameSettings.MaxWorldSizeInChunks;
        int zNumberOf = GameSettings.MaxWorldSizeInChunks;

        world = new ProcTerrainChunk[xNumberOf, zNumberOf];
        ice = new ProcTerrainChunk[xNumberOf, zNumberOf];

        for (int i = xNumberOf / 2 - GameSettings.ViewDistanceInChunks; i < xNumberOf / 2 + GameSettings.ViewDistanceInChunks; i++)
        {

            for (int k = zNumberOf / 2 - GameSettings.ViewDistanceInChunks; k < zNumberOf / 2 + GameSettings.ViewDistanceInChunks; k++)
            {
                world[i, k] = new ProcTerrainChunk(
                    new Vector3(0, 0, 0) + new Vector3(i * GameSettings.TerrainChunkSize, 0, k * GameSettings.TerrainChunkSize),
                    GameSettings.TerrainChunkSize, GameSettings.TerrainChunkSize, GameSettings.TerrainVertsPerChunk, TerrainType.ClassicPerlin);

                ice[i, k] = new ProcTerrainChunk(
                    new Vector3(0, 0, 0) + new Vector3(i * GameSettings.TerrainChunkSize, 0, k * GameSettings.TerrainChunkSize),
                    GameSettings.TerrainChunkSize, GameSettings.TerrainChunkSize, GameSettings.TerrainVertsPerChunk, TerrainType.FlatSurface);
            }

        }

        //Place Player
        RaycastHit hit;
        Vector3 fromRay = new Vector3((xNumberOf * GameSettings.TerrainChunkSize) / 2, 100f, (zNumberOf * GameSettings.TerrainChunkSize) / 2);
        Vector3 toRay = new Vector3((xNumberOf * GameSettings.TerrainChunkSize) / 2, -100f, (zNumberOf * GameSettings.TerrainChunkSize) / 2);
        if (Physics.Raycast(fromRay, toRay - fromRay, out hit))
        {
            Player.transform.position = new Vector3((xNumberOf * GameSettings.TerrainChunkSize) / 2, hit.point.y + 1f, (zNumberOf * GameSettings.TerrainChunkSize) / 2);
        }
        else
        {
            Player.transform.position = new Vector3((xNumberOf * GameSettings.TerrainChunkSize) / 2, 15f, (zNumberOf * GameSettings.TerrainChunkSize) / 2);
        }
        Task Generating = new Task(ContinuousGeneration(0.1f));
        Task FirstCampfire = new Task(SpawnCampfireInTime());


    }

    void Update()
    {
        GrowingStar.position = Player.transform.position + new Vector3(-75, 50, -25);
    }

    IEnumerator ContinuousGeneration(float period)
    {
        while (true)
        {
            Vector2Int playerPos = GetChunkPositionOfPlayer();
            //Debug.Log(playerPos);
            for (int i = playerPos.x - 2 * GameSettings.ViewDistanceInChunks; i < playerPos.x + 2 * GameSettings.ViewDistanceInChunks; i++)
            {
                for (int k = playerPos.y - 2 * GameSettings.ViewDistanceInChunks; k < playerPos.y + 2 * GameSettings.ViewDistanceInChunks; k++)
                {
                    if (world[i, k] == null)
                    {
                        if (i >= playerPos.x - GameSettings.ViewDistanceInChunks && i < playerPos.x + GameSettings.ViewDistanceInChunks &&
                            k >= playerPos.y - GameSettings.ViewDistanceInChunks && k < playerPos.y + GameSettings.ViewDistanceInChunks)
                        {

                            world[i, k] = new ProcTerrainChunk(
                            new Vector3(0, 0, 0) + new Vector3(i * GameSettings.TerrainChunkSize, 0, k * GameSettings.TerrainChunkSize),
                            GameSettings.TerrainChunkSize, GameSettings.TerrainChunkSize, GameSettings.TerrainVertsPerChunk, TerrainType.ClassicPerlin);

                            ice[i, k] = new ProcTerrainChunk(
                            new Vector3(0, 0, 0) + new Vector3(i * GameSettings.TerrainChunkSize, 0, k * GameSettings.TerrainChunkSize),
                            GameSettings.TerrainChunkSize, GameSettings.TerrainChunkSize, GameSettings.TerrainVertsPerChunk, TerrainType.FlatSurface);

                            yield return new WaitForSeconds(period);
                        }
                    }
                    else
                    {
                        if (i < playerPos.x - GameSettings.ViewDistanceInChunks || i >= playerPos.x + GameSettings.ViewDistanceInChunks ||
                            k < playerPos.y - GameSettings.ViewDistanceInChunks || k >= playerPos.y + GameSettings.ViewDistanceInChunks)
                        {
                            GameObject.Destroy(world[i, k].UnityObject);
                            GameObject.Destroy(ice[i, k].UnityObject);
                            world[i, k] = null;
                            ice[i, k] = null;
                            yield return new WaitForSeconds(period);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(period);

        }
    }

    public IEnumerator SpawnCampfireInTime()
    {
        Debug.Log("TriggerToStarSpawning");
        yield return new WaitForSeconds(GameSettings.SecondsToSpawnEvent);
        System.Random random = new System.Random(GameSettings.Seed + solvedFires.Count);
        Vector3 direction = GrowingStar.transform.position - Player.transform.position;
        int angle = 60;
        int finalAngle = random.Next(-angle / 2, angle / 2);
        Vector3 finalDirection = Quaternion.AngleAxis(finalAngle, Vector3.up) * new Vector3(direction.x, 0, direction.z);

        RaycastHit hit;
        Vector3 fromRay = finalDirection.normalized * GameSettings.SpawningDistance + Player.transform.position + new Vector3(0, 200, 0);
        Vector3 toRay = finalDirection.normalized * GameSettings.SpawningDistance + Player.transform.position + new Vector3(0, -100, 0);
        if (Physics.Raycast(fromRay, toRay - fromRay, out hit))
        {
            GameObject.Instantiate(AssetManager.Instance.GetPrefab("Campfire"), hit.point, Quaternion.AngleAxis(-90f, new Vector3(1, 0, 0)));
            Debug.Log("CampfireSpawned at: " + hit.point.ToString());
        }
    }

    public IEnumerator ShowHideFog(bool show)
    {
        var fog = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.GlobalFog>();
        while (true)
        {
            if (show)
            {
                if (fog.height >= GameSettings.fogHeight)
                {
                    break;
                }
                else
                {
                    fog.height += 4;
                }
            }
            else
            {
                if (fog.height > 0)
                {
                    fog.height -= 4;
                }
                else
                {
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public Vector2Int GetChunkPositionOfPlayer()
    {
        return new Vector2Int(Convert.ToInt32(Player.transform.position.x / GameSettings.TerrainChunkSize), Convert.ToInt32(Player.transform.position.z / GameSettings.TerrainChunkSize));
    }
}
