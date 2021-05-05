using UnityEngine;
using System.Collections;



    public static class GameSettings
    {
        //terrain
        public static int Seed { get { return 10; } }
        public static int TerrainChunkSize { get { return 64; } }
        public static int TerrainVertsPerChunk { get { return 64; } }
        public static int MaxWorldSizeInChunks { get { return 100; } }
        public static int ViewDistanceInChunks { get { return 4; } }
        //freeze mechanic
        public static float freezeLimit = 100f;
        public static float freezeGainPerSecond { get { return 0.25f; } }
        public static float warmGainPerSecond { get { return 10f; } }
        //events
        public static int SecondsToSpawnEvent { get { return 5; } }
        public static int SpawningDistance { get { return 50; } }
        //public static int MinDistanceBetweenEvents { get { return 100; } }
        //growing star
        public static float IntensityIncreasePerPuzzle { get { return 0.2f; } }
        //camera
        public static int ScrollWidth { get { return 0; } }
        public static float ScrollSpeed { get { return 25; } }
        public static float RotateAmount { get { return 10; } }
        public static float RotateSpeed { get { return 100; } }
        public static float MinCameraHeight { get { return 0; } }
        public static float MaxCameraHeight { get { return 5000; } }

        public static float GhostDuration { get { return 5; } }

        //snowing
        public static int EmissionRate { get { return 3000; } }
        public static Vector3 snowLocalPosition { get { return new Vector3(0,4,0); } }

        //fog
        public static int fogHeight { get { return 100; } }
    }

