using UnityEngine;
using System.Collections;
using System;
[Serializable]
public class EdgeGraph  {

    public NodeGraph PointA { private set; get; }
    public NodeGraph PointB { private set; get; }

    public EdgeGraph(NodeGraph pointA, NodeGraph pointB)
    {
        PointA = pointA;
        PointB = pointB;
    }
}
