using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IGraphMember {

    void Show(bool show);
    void Highlight(bool highlight);
    List<NodeGraph> GetNeighbourNodes();
    List<EdgeGraph> GetNeighbourEdges();
    
}
