using UnityEngine;
using System.Collections;
using System;
using Events;
using System.Collections.Generic;

public class NodeGraph  {

    public Vector2Int PixelPosition { get; set; }
    public Vector2Int WorldPosition { get; set; }

    public string TextKey { get; private set; }

    public int Index { get; set; }

    public bool Visited { get; set; }

    public BiomeType BiomeType { get; private set; }

    public List<NodeGraph> neighbours { get; set; }


    public NodeGraph(Vector2Int pixelPosition, Events.BiomeType biome, string text)
    {
        PixelPosition = pixelPosition;
        BiomeType = biome;
        TextKey = text;
        Visited = false;
    }

}
