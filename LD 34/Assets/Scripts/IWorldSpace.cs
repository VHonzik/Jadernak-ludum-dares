using System.Collections;
using UnityEngine;

public interface IWorldSpace  {
    GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation);
    Vector3 Position { get; }
	
}
