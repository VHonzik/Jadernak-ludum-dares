using UnityEngine;
using System.Collections;
using System;
using LibNoise;
using System.Collections.Generic;


namespace ProceduralGeneration
{
    public enum TerrainType { ClassicPerlin, FlatSurface };

    public class ProcTerrainChunk : IWorldSpace
    {
        public Vector3 Position { get; private set; }
        public int XSize { get; private set; }
        public int ZSize { get; private set; }
        public float VertPerUnit { get; private set; }
        public GameObject UnityObject;
        public TerrainType _TerrainType;


        public ProcTerrainChunk(Vector3 position, int xSize, int zSize, float vertPerUnit, TerrainType terrainType)
        {
            Position = position;
            XSize = xSize;
            ZSize = zSize;
            VertPerUnit = vertPerUnit;
            this._TerrainType = terrainType;
            Initialize();
            Randomize();

        }
        /*
        private void LowPoly()
        {
            MMData_SurfaceInfo surf = new MMData_SurfaceInfo();
            surf.forceOneSubMaterial = false;
            surf.surfaceType = MM_SURFACE_TYPE.Flat;
            surf.combineIntoOneMesh = true;
            MMData[] data = new MMData[1] { surf };
            var renderer = UnityObject.GetComponent<MeshRenderer>();
            var filter = UnityObject.GetComponent<MeshFilter>();
            var mesh = MMGenerator.MaterializeMesh(renderer, data);
            filter.mesh = mesh;
            UnityObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
        */

        private void Randomize()
        {
            float seaLevel = 0.5f;
            Perlin perlin = new Perlin();
            perlin.Frequency = 0.4f;
            perlin.NoiseQuality = NoiseQuality.Standard;
            perlin.Seed = GameSettings.Seed;
            perlin.OctaveCount = 6;
            perlin.Lacunarity = 2.0;
            perlin.Persistence = 0.5;

            float sparsity = 30f;
            float mountainHeight = 2f;
            float mountainPower = 2.8f;
            Mesh originalMesh = UnityObject.GetComponent<MeshFilter>().mesh;
            List<Vector3> newVertices = new List<Vector3>();
            foreach (Vector3 vertice in originalMesh.vertices)
            {
                if (_TerrainType == TerrainType.ClassicPerlin)
                {
                    newVertices.Add(new Vector3(vertice.x, Mathf.Pow(mountainHeight * ((float)perlin.GetValue((Position.x + vertice.x) / sparsity, 0, (Position.z + vertice.z) / sparsity) + 1) / 2f, mountainPower), vertice.z));
                }
                if (_TerrainType == TerrainType.FlatSurface)
                {
                    newVertices.Add(new Vector3(vertice.x, vertice.y + seaLevel, vertice.z));
                }
            }
            originalMesh.vertices = newVertices.ToArray();
            originalMesh.RecalculateBounds();
            originalMesh.RecalculateNormals();
            originalMesh.Optimize();
            UnityObject.GetComponent<MeshCollider>().sharedMesh = UnityObject.GetComponent<MeshFilter>().mesh;
        }

        private void Initialize()
        {
            UnityObject = new GameObject("ProcTerrainChunk_" + Position.x + "_" + Position.y + "_" + Position.z + "_" + _TerrainType.ToString());
            Mesh mesh = new Mesh();
            int BACHAtexturarozliseni = 4;
            //float distance = 1f / VertPerUnit;
            float distance = (float)GameSettings.TerrainChunkSize / (float)GameSettings.TerrainVertsPerChunk;
            //int XSizeNumber = Convert.ToInt32(XSize / distance);
            int XSizeNumber = GameSettings.TerrainVertsPerChunk;
            //int ZSizeNumber = Convert.ToInt32(ZSize / distance);
            int ZSizeNumber = GameSettings.TerrainVertsPerChunk;

            Vector3[] vertices = new Vector3[(XSizeNumber + 1) * (ZSizeNumber + 1)];
            Vector2[] uv = new Vector2[vertices.Length];
            Vector4[] tangents = new Vector4[vertices.Length];
            //might have to add minus sign BEWARE
            Vector4 tangent = new Vector4(1f, 0f, 0f, 1f);
            for (int i = 0, z = 0; z <= ZSizeNumber; z++)
            {
                for (int x = 0; x <= XSizeNumber; x++, i++)
                {
                    vertices[i] = new Vector3(x * distance, 0, z * distance);
                    uv[i] = new Vector2(BACHAtexturarozliseni * (float)x / XSizeNumber, BACHAtexturarozliseni * (float)z / ZSizeNumber);
                    tangents[i] = tangent;
                }
            }
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.tangents = tangents;

            int[] triangles = new int[XSizeNumber * ZSizeNumber * 6];
            for (int ti = 0, vi = 0, z = 0; z < ZSizeNumber; z++, vi++)
            {
                for (int x = 0; x < XSizeNumber; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + XSizeNumber + 1;
                    triangles[ti + 5] = vi + XSizeNumber + 2;
                }
            }
            mesh.triangles = triangles;

            UnityObject.AddComponent<MeshFilter>().mesh = mesh;
            MeshRenderer renderer = UnityObject.AddComponent<MeshRenderer>();
            MeshCollider collider = UnityObject.AddComponent<MeshCollider>();
            
            
            //also should add UVs and Tangents

            collider.sharedMesh = mesh;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.Optimize();

            UnityObject.transform.position = Position;


            if (_TerrainType == TerrainType.ClassicPerlin)
            {
                renderer.material = AssetManager.Instance.GetMaterial("Snow Full");
            }
            if (_TerrainType == TerrainType.FlatSurface)
            {
                renderer.material = AssetManager.Instance.GetMaterial("Snow Full Reflective");
            }

        }

        public GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation)
        {
            return null;
        }

    }
}
