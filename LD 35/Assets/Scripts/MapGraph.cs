using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;


public class MapGraph : MonoBehaviour {
    public  List<NodeGraph> nodes = new List<NodeGraph>();
    public  List<EdgeGraph> edges = new List<EdgeGraph>();

    public GameObject nodePrefab;


    private static MapGraph _instance;
    public static MapGraph Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MapGraph();
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        FillNodes();
    }
	// Use this for initialization
    void Start()
    {        
        CreateNodes();
    }

    public List<NodeGraph> GetNeighbours(NodeGraph node)
    {
        List<NodeGraph> result = new List<NodeGraph>();

        foreach(var edge in edges)
        {
            if (edge.PointA == node) result.Add(edge.PointB);
            if (edge.PointB == node) result.Add(edge.PointA);
        }

        return result;
    }

    public NodeGraph GetNode(int index)
    {
        if(index >=0 &&  index < nodes.Count)
        {
            return nodes[index];
        }
        else
        {
            return null;
        }
    }

    public NodeGraph GetStartNode()
    {
        if (nodes.Count > 0)
        {
            return nodes[10];
        }
        else
        {
            Debug.LogError("No nodes in a map available");
            return null;
        }
    }

    private void CreateNodes()
    {
        foreach(var node in nodes)
        {
            node.neighbours = GetNeighbours(node);

            Vector3 wantePos = UI.MapRenderer.Instance.PixelPositionToWorldPosition(node.PixelPosition);
            wantePos.z = -1;
            var nodeObject = GameObject.Instantiate(nodePrefab, wantePos, Quaternion.identity) as GameObject;
            nodeObject.transform.SetParent(gameObject.transform);

            nodeObject.GetComponent<UI.NodeRender>().SetNode(node);
            GameManager.Instance.CurrentNodeChangedEvent += nodeObject.GetComponent<UI.NodeRender>().OnCurrentNodeChanged;
            UI.MapRenderer.Instance.UIStateChangedEvent += nodeObject.GetComponent<UI.NodeRender>().OnUIStatechanged;
        }
    }

  

    private void FillNodes()
    {
        nodes.Add(new NodeGraph(new Vector2Int(960, 783), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 0
        nodes.Add(new NodeGraph(new Vector2Int(955, 730), Events.BiomeType.Lake, "THREE_QUEENS_TEXT")); //1
        edges.Add(new EdgeGraph(nodes[0], nodes[1]));

        nodes.Add(new NodeGraph(new Vector2Int(869, 795), Events.BiomeType.Lake, "THREE_QUEENS_TEXT")); // 2
        edges.Add(new EdgeGraph(nodes[0], nodes[2]));
        edges.Add(new EdgeGraph(nodes[1], nodes[2]));

        nodes.Add(new NodeGraph(new Vector2Int(750, 690), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 3
        edges.Add(new EdgeGraph(nodes[2], nodes[3]));

        nodes.Add(new NodeGraph(new Vector2Int(726, 767), Events.BiomeType.Grassland, "GENERIC_GRASSLAND_TEXT")); // 4
        edges.Add(new EdgeGraph(nodes[3], nodes[4]));

        nodes.Add(new NodeGraph(new Vector2Int(720, 840), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 5
        edges.Add(new EdgeGraph(nodes[2], nodes[5]));
        edges.Add(new EdgeGraph(nodes[4], nodes[5]));

        nodes.Add(new NodeGraph(new Vector2Int(820,692), Events.BiomeType.Mountain, "TOWERING_WELLS_TEXT")); // 6
        edges.Add(new EdgeGraph(nodes[2], nodes[6]));
        edges.Add(new EdgeGraph(nodes[3], nodes[6]));

        nodes.Add(new NodeGraph(new Vector2Int(588, 844), Events.BiomeType.Town, "MEANFORK_TEXT")); // 7
        edges.Add(new EdgeGraph(nodes[5], nodes[7]));

        nodes.Add(new NodeGraph(new Vector2Int(389, 999), Events.BiomeType.Town, "TWOBRIDGE_TEXT")); // 8
        edges.Add(new EdgeGraph(nodes[7], nodes[8]));

        nodes.Add(new NodeGraph(new Vector2Int(254, 760), Events.BiomeType.Town, "BOWTOWN_TEXT")); // 9
        edges.Add(new EdgeGraph(nodes[8], nodes[9]));

        nodes.Add(new NodeGraph(new Vector2Int(557, 520), Events.BiomeType.Town, "COINCITY_TEXT")); // 10

        nodes.Add(new NodeGraph(new Vector2Int(350, 306), Events.BiomeType.Town, "FARREACH_TEXT")); // 11

        nodes.Add(new NodeGraph(new Vector2Int(594, 363), Events.BiomeType.Town, "POETSHOUSE_TEXT")); // 12
        edges.Add(new EdgeGraph(nodes[10], nodes[12]));
        edges.Add(new EdgeGraph(nodes[11], nodes[12]));

        nodes.Add(new NodeGraph(new Vector2Int(902, 96), Events.BiomeType.Village, "CEASINGSPOT_TEXT")); // 13

        nodes.Add(new NodeGraph(new Vector2Int(1472, 587), Events.BiomeType.Town, "CITY_OF_SMOKES_TEXT")); // 14

        nodes.Add(new NodeGraph(new Vector2Int(1348, 1004), Events.BiomeType.Town, "BISHOPS_SEAT_TEXT")); // 15

        nodes.Add(new NodeGraph(new Vector2Int(995, 1116), Events.BiomeType.Village, "LAST_RESPITE_TEXT")); // 16

        nodes.Add(new NodeGraph(new Vector2Int(928, 975), Events.BiomeType.Town, "GRASPING_LEDGE_TEXT")); // 17
        edges.Add(new EdgeGraph(nodes[0], nodes[17]));
        edges.Add(new EdgeGraph(nodes[16], nodes[17]));

        nodes.Add(new NodeGraph(new Vector2Int(974, 914), Events.BiomeType.Forest, "GENERIC_FOREST_TEXT")); // 18
        edges.Add(new EdgeGraph(nodes[0], nodes[18]));
        edges.Add(new EdgeGraph(nodes[17], nodes[18]));

        nodes.Add(new NodeGraph(new Vector2Int(1067, 941), Events.BiomeType.Forest, "GENERIC_FOREST_TEXT")); // 19
        edges.Add(new EdgeGraph(nodes[18], nodes[19]));
        edges.Add(new EdgeGraph(nodes[17], nodes[19]));

        nodes.Add(new NodeGraph(new Vector2Int(854, 898), Events.BiomeType.Forest, "GENERIC_FOREST_TEXT")); // 20
        edges.Add(new EdgeGraph(nodes[17], nodes[20]));
        edges.Add(new EdgeGraph(nodes[19], nodes[20]));
        edges.Add(new EdgeGraph(nodes[2], nodes[20]));

        nodes.Add(new NodeGraph(new Vector2Int(1146, 993), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 21
        edges.Add(new EdgeGraph(nodes[16], nodes[21]));
        edges.Add(new EdgeGraph(nodes[19], nodes[21]));
        edges.Add(new EdgeGraph(nodes[15], nodes[21]));

        nodes.Add(new NodeGraph(new Vector2Int(1217, 1075), Events.BiomeType.Forest, "GENERIC_FOREST_TEXT")); // 22
        edges.Add(new EdgeGraph(nodes[21], nodes[22]));

        nodes.Add(new NodeGraph(new Vector2Int(812, 952), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 23
        edges.Add(new EdgeGraph(nodes[20], nodes[23]));
        edges.Add(new EdgeGraph(nodes[17], nodes[23]));
        edges.Add(new EdgeGraph(nodes[5], nodes[23]));

        nodes.Add(new NodeGraph(new Vector2Int(670, 1060), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 24
        edges.Add(new EdgeGraph(nodes[7], nodes[24]));
        edges.Add(new EdgeGraph(nodes[23], nodes[24]));
        edges.Add(new EdgeGraph(nodes[16], nodes[24]));

        nodes.Add(new NodeGraph(new Vector2Int(612, 968), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 25
        edges.Add(new EdgeGraph(nodes[7], nodes[25]));
        edges.Add(new EdgeGraph(nodes[8], nodes[25]));
        edges.Add(new EdgeGraph(nodes[24], nodes[25]));
        edges.Add(new EdgeGraph(nodes[23], nodes[25]));

        nodes.Add(new NodeGraph(new Vector2Int(466, 786), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 26
        edges.Add(new EdgeGraph(nodes[7], nodes[26]));
        edges.Add(new EdgeGraph(nodes[8], nodes[26]));
        edges.Add(new EdgeGraph(nodes[9], nodes[26]));

        nodes.Add(new NodeGraph(new Vector2Int(366, 704), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 27
        edges.Add(new EdgeGraph(nodes[26], nodes[27]));
        edges.Add(new EdgeGraph(nodes[9], nodes[27]));
        edges.Add(new EdgeGraph(nodes[10], nodes[27]));

        nodes.Add(new NodeGraph(new Vector2Int(400, 418), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 28
        edges.Add(new EdgeGraph(nodes[10], nodes[28]));
        edges.Add(new EdgeGraph(nodes[11], nodes[28]));
        edges.Add(new EdgeGraph(nodes[12], nodes[28]));

        nodes.Add(new NodeGraph(new Vector2Int(824, 428), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 29
        edges.Add(new EdgeGraph(nodes[10], nodes[29]));
        edges.Add(new EdgeGraph(nodes[12], nodes[29]));

        nodes.Add(new NodeGraph(new Vector2Int(800, 619), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 30
        edges.Add(new EdgeGraph(nodes[3], nodes[30]));
        edges.Add(new EdgeGraph(nodes[29], nodes[30]));
        edges.Add(new EdgeGraph(nodes[12], nodes[30]));

        nodes.Add(new NodeGraph(new Vector2Int(903, 601), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 31
        edges.Add(new EdgeGraph(nodes[1], nodes[31]));
        edges.Add(new EdgeGraph(nodes[6], nodes[31]));
        edges.Add(new EdgeGraph(nodes[30], nodes[31]));

        nodes.Add(new NodeGraph(new Vector2Int(1024, 462), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 32
        edges.Add(new EdgeGraph(nodes[29], nodes[32]));
        edges.Add(new EdgeGraph(nodes[31], nodes[32]));

        nodes.Add(new NodeGraph(new Vector2Int(1120, 429), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 33
        edges.Add(new EdgeGraph(nodes[32], nodes[33]));

        nodes.Add(new NodeGraph(new Vector2Int(1268, 519), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 34
        edges.Add(new EdgeGraph(nodes[14], nodes[34]));
        edges.Add(new EdgeGraph(nodes[33], nodes[34]));

        nodes.Add(new NodeGraph(new Vector2Int(1234, 668), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 35
        edges.Add(new EdgeGraph(nodes[34], nodes[35]));
        edges.Add(new EdgeGraph(nodes[14], nodes[35]));

        nodes.Add(new NodeGraph(new Vector2Int(1370, 703), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 36
        edges.Add(new EdgeGraph(nodes[35], nodes[36]));
        edges.Add(new EdgeGraph(nodes[14], nodes[36]));

        nodes.Add(new NodeGraph(new Vector2Int(1390, 416), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 37
        edges.Add(new EdgeGraph(nodes[34], nodes[37]));
        edges.Add(new EdgeGraph(nodes[14], nodes[37]));

        nodes.Add(new NodeGraph(new Vector2Int(1386, 267), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 38
        edges.Add(new EdgeGraph(nodes[37], nodes[38]));

        nodes.Add(new NodeGraph(new Vector2Int(1526, 939), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 39
        edges.Add(new EdgeGraph(nodes[15], nodes[39]));
        edges.Add(new EdgeGraph(nodes[36], nodes[39]));

        nodes.Add(new NodeGraph(new Vector2Int(1250, 899), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 40
        edges.Add(new EdgeGraph(nodes[15], nodes[40]));
        edges.Add(new EdgeGraph(nodes[21], nodes[40]));
        edges.Add(new EdgeGraph(nodes[36], nodes[40]));

        nodes.Add(new NodeGraph(new Vector2Int(1130, 838), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 41
        edges.Add(new EdgeGraph(nodes[0], nodes[41]));
        edges.Add(new EdgeGraph(nodes[21], nodes[41]));
        edges.Add(new EdgeGraph(nodes[40], nodes[41]));
        edges.Add(new EdgeGraph(nodes[35], nodes[41]));

        nodes.Add(new NodeGraph(new Vector2Int(1094, 713), Events.BiomeType.Village, "GENERIC_VILLAGE_TEXT")); // 42
        edges.Add(new EdgeGraph(nodes[0], nodes[42]));
        edges.Add(new EdgeGraph(nodes[41], nodes[42]));
        edges.Add(new EdgeGraph(nodes[40], nodes[42]));

        nodes.Add(new NodeGraph(new Vector2Int(295, 904), Events.BiomeType.Forest, "GENERIC_FOREST_TEXT")); // 43
        edges.Add(new EdgeGraph(nodes[8], nodes[43]));
        edges.Add(new EdgeGraph(nodes[9], nodes[43]));

        nodes.Add(new NodeGraph(new Vector2Int(285, 639), Events.BiomeType.Forest, "GENERIC_FOREST_TEXT")); // 44
        edges.Add(new EdgeGraph(nodes[27], nodes[44]));
        edges.Add(new EdgeGraph(nodes[9], nodes[44]));

        nodes.Add(new NodeGraph(new Vector2Int(196, 692), Events.BiomeType.Forest, "GENERIC_FOREST_TEXT")); // 45
        edges.Add(new EdgeGraph(nodes[44], nodes[45]));
        edges.Add(new EdgeGraph(nodes[9], nodes[45]));

        nodes.Add(new NodeGraph(new Vector2Int(346, 606), Events.BiomeType.Hills, "GENERIC_HILLS_TEXT")); // 46
        edges.Add(new EdgeGraph(nodes[10], nodes[46]));
        edges.Add(new EdgeGraph(nodes[44], nodes[46]));
        edges.Add(new EdgeGraph(nodes[27], nodes[46]));
        edges.Add(new EdgeGraph(nodes[28], nodes[46]));

        nodes.Add(new NodeGraph(new Vector2Int(298, 537), Events.BiomeType.Grassland, "GENERIC_GRASSLAND_TEXT")); // 47
        edges.Add(new EdgeGraph(nodes[11], nodes[47]));
        edges.Add(new EdgeGraph(nodes[28], nodes[47]));
        edges.Add(new EdgeGraph(nodes[44], nodes[47]));
        edges.Add(new EdgeGraph(nodes[46], nodes[47]));

        nodes.Add(new NodeGraph(new Vector2Int(295, 410), Events.BiomeType.Lake, "GENERIC_LAKE_TEXT")); // 48
        edges.Add(new EdgeGraph(nodes[11], nodes[48]));
        edges.Add(new EdgeGraph(nodes[47], nodes[48]));

        nodes.Add(new NodeGraph(new Vector2Int(481, 372), Events.BiomeType.Lake, "GENERIC_LAKE_TEXT")); // 49
        edges.Add(new EdgeGraph(nodes[11], nodes[49]));
        edges.Add(new EdgeGraph(nodes[12], nodes[49]));
        edges.Add(new EdgeGraph(nodes[28], nodes[49]));

        nodes.Add(new NodeGraph(new Vector2Int(533, 311), Events.BiomeType.Hills, "GENERIC_HILLS_TEXT")); // 50
        edges.Add(new EdgeGraph(nodes[11], nodes[50]));
        edges.Add(new EdgeGraph(nodes[12], nodes[50]));
        edges.Add(new EdgeGraph(nodes[49], nodes[50]));

        nodes.Add(new NodeGraph(new Vector2Int(237, 471), Events.BiomeType.Grassland, "GENERIC_GRASSLAND_TEXT")); // 51
        edges.Add(new EdgeGraph(nodes[11], nodes[51]));
        edges.Add(new EdgeGraph(nodes[47], nodes[51]));
        edges.Add(new EdgeGraph(nodes[48], nodes[51]));

        nodes.Add(new NodeGraph(new Vector2Int(1045, 355), Events.BiomeType.Desert, "GENERIC_DESERT_TEXT")); // 52
        edges.Add(new EdgeGraph(nodes[32], nodes[52]));
        edges.Add(new EdgeGraph(nodes[33], nodes[52]));

        nodes.Add(new NodeGraph(new Vector2Int(1027, 200), Events.BiomeType.Desert, "GENERIC_DESERT_TEXT")); // 53
        edges.Add(new EdgeGraph(nodes[13], nodes[53]));
        edges.Add(new EdgeGraph(nodes[52], nodes[53]));

        nodes.Add(new NodeGraph(new Vector2Int(1200, 167), Events.BiomeType.Desert, "GENERIC_DESERT_TEXT")); // 54
        edges.Add(new EdgeGraph(nodes[38], nodes[54]));
        edges.Add(new EdgeGraph(nodes[53], nodes[54]));

        nodes.Add(new NodeGraph(new Vector2Int(908, 400), Events.BiomeType.Hills, "GENERIC_HILLS_TEXT")); // 55
        edges.Add(new EdgeGraph(nodes[29], nodes[55]));
        edges.Add(new EdgeGraph(nodes[32], nodes[55]));
        edges.Add(new EdgeGraph(nodes[52], nodes[55]));

        nodes.Add(new NodeGraph(new Vector2Int(860, 250), Events.BiomeType.Hills, "GENERIC_HILLS_TEXT")); // 56
        edges.Add(new EdgeGraph(nodes[29], nodes[56]));
        edges.Add(new EdgeGraph(nodes[53], nodes[56]));
        edges.Add(new EdgeGraph(nodes[55], nodes[56]));

        nodes.Add(new NodeGraph(new Vector2Int(578, 663), Events.BiomeType.Hills, "GENERIC_HILLS_TEXT")); // 57
        edges.Add(new EdgeGraph(nodes[7], nodes[57]));
        edges.Add(new EdgeGraph(nodes[10], nodes[57]));
        edges.Add(new EdgeGraph(nodes[26], nodes[57]));
        edges.Add(new EdgeGraph(nodes[27], nodes[57]));

        nodes.Add(new NodeGraph(new Vector2Int(1231, 760), Events.BiomeType.Hills, "GENERIC_HILLS_TEXT")); // 58
        edges.Add(new EdgeGraph(nodes[35], nodes[58]));
        edges.Add(new EdgeGraph(nodes[36], nodes[58]));
        edges.Add(new EdgeGraph(nodes[40], nodes[58]));
        edges.Add(new EdgeGraph(nodes[41], nodes[58]));

        nodes.Add(new NodeGraph(new Vector2Int(1353, 597), Events.BiomeType.Swamp, "GENERIC_SWAMP_TEXT")); // 59
        edges.Add(new EdgeGraph(nodes[14], nodes[59]));
        edges.Add(new EdgeGraph(nodes[34], nodes[59]));
        edges.Add(new EdgeGraph(nodes[35], nodes[59]));
        edges.Add(new EdgeGraph(nodes[36], nodes[59]));

        nodes.Add(new NodeGraph(new Vector2Int(1567, 657), Events.BiomeType.Swamp, "GENERIC_SWAMP_TEXT")); // 60
        edges.Add(new EdgeGraph(nodes[14], nodes[60]));
        edges.Add(new EdgeGraph(nodes[36], nodes[60]));

        nodes.Add(new NodeGraph(new Vector2Int(1457, 471), Events.BiomeType.Swamp, "GENERIC_SWAMP_TEXT")); // 61
        edges.Add(new EdgeGraph(nodes[14], nodes[61]));
        edges.Add(new EdgeGraph(nodes[37], nodes[61]));

        nodes.Add(new NodeGraph(new Vector2Int(1524, 773), Events.BiomeType.Grassland, "GENERIC_GRASSLAND_TEXT")); // 62
        edges.Add(new EdgeGraph(nodes[36], nodes[62]));
        edges.Add(new EdgeGraph(nodes[39], nodes[62]));
        edges.Add(new EdgeGraph(nodes[60], nodes[62]));

        nodes.Add(new NodeGraph(new Vector2Int(1457, 847), Events.BiomeType.Hills, "GENERIC_HILLS_TEXT")); // 64
        edges.Add(new EdgeGraph(nodes[36], nodes[63]));
        edges.Add(new EdgeGraph(nodes[39], nodes[63]));
        edges.Add(new EdgeGraph(nodes[62], nodes[63]));


        nodes.Add(new NodeGraph(new Vector2Int(876, 512), Events.BiomeType.Forest, "GENERIC_FOREST_TEXT")); // 64
        edges.Add(new EdgeGraph(nodes[29], nodes[64]));
        edges.Add(new EdgeGraph(nodes[30], nodes[64]));
        edges.Add(new EdgeGraph(nodes[31], nodes[64]));
        edges.Add(new EdgeGraph(nodes[32], nodes[64]));

        nodes.Add(new NodeGraph(new Vector2Int(687, 592), Events.BiomeType.Grassland, "GENERIC_GRASSLAND_TEXT")); // 65
        edges.Add(new EdgeGraph(nodes[10], nodes[65]));
        edges.Add(new EdgeGraph(nodes[30], nodes[65]));
        edges.Add(new EdgeGraph(nodes[57], nodes[65]));

        nodes.Add(new NodeGraph(new Vector2Int(746, 316), Events.BiomeType.Forest, "GENERIC_FOREST_TEXT")); // 66
        edges.Add(new EdgeGraph(nodes[12], nodes[66]));
        edges.Add(new EdgeGraph(nodes[29], nodes[66]));
        edges.Add(new EdgeGraph(nodes[50], nodes[66]));
        edges.Add(new EdgeGraph(nodes[55], nodes[66]));
        edges.Add(new EdgeGraph(nodes[56], nodes[66]));

        nodes.Add(new NodeGraph(new Vector2Int(690, 903), Events.BiomeType.Lake, "GENERIC_LAKE_TEXT")); // 67
        edges.Add(new EdgeGraph(nodes[5], nodes[67]));
        edges.Add(new EdgeGraph(nodes[7], nodes[67]));
        edges.Add(new EdgeGraph(nodes[23], nodes[67]));
        edges.Add(new EdgeGraph(nodes[25], nodes[67]));

        nodes.Add(new NodeGraph(new Vector2Int(654, 242), Events.BiomeType.Lake, "GENERIC_LAKE_TEXT")); // 68
        edges.Add(new EdgeGraph(nodes[12], nodes[68]));
        edges.Add(new EdgeGraph(nodes[50], nodes[68]));
        edges.Add(new EdgeGraph(nodes[56], nodes[68]));
        edges.Add(new EdgeGraph(nodes[66], nodes[68]));

        nodes.Add(new NodeGraph(new Vector2Int(1406, 929), Events.BiomeType.Lake, "GENERIC_LAKE_TEXT")); // 69
        edges.Add(new EdgeGraph(nodes[15], nodes[69]));
        edges.Add(new EdgeGraph(nodes[39], nodes[69]));
        edges.Add(new EdgeGraph(nodes[40], nodes[69]));
        edges.Add(new EdgeGraph(nodes[63], nodes[69]));

        nodes.Add(new NodeGraph(new Vector2Int(1058, 1058), Events.BiomeType.Mountain, "GENERIC_MOUNTAINS_TEXT")); // 70
        edges.Add(new EdgeGraph(nodes[16], nodes[70]));
        edges.Add(new EdgeGraph(nodes[21], nodes[70]));

        nodes.Add(new NodeGraph(new Vector2Int(806, 1110), Events.BiomeType.Mountain, "GENERIC_MOUNTAINS_TEXT")); // 71
        edges.Add(new EdgeGraph(nodes[16], nodes[71]));
        edges.Add(new EdgeGraph(nodes[24], nodes[71]));

        nodes.Add(new NodeGraph(new Vector2Int(931, 1084), Events.BiomeType.Mountain, "GENERIC_MOUNTAINS_TEXT")); // 72
        edges.Add(new EdgeGraph(nodes[16], nodes[72]));
        edges.Add(new EdgeGraph(nodes[27], nodes[72]));

        nodes.Add(new NodeGraph(new Vector2Int(1075, 672), Events.BiomeType.Mountain, "GENERIC_MOUNTAINS_TEXT")); // 73
        edges.Add(new EdgeGraph(nodes[0], nodes[73]));
        edges.Add(new EdgeGraph(nodes[42], nodes[73]));

        nodes.Add(new NodeGraph(new Vector2Int(913, 133), Events.BiomeType.Mountain, "GENERIC_MOUNTAINS_TEXT")); // 74
        edges.Add(new EdgeGraph(nodes[13], nodes[74]));
        edges.Add(new EdgeGraph(nodes[53], nodes[73]));


        for(int i=0; i< nodes.Count; i++)
        {
            nodes[i].Index = i;
        }




    }


}
