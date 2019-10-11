using UnityEngine;
using System.Collections;

public class SpiderWeb : MonoBehaviour
{
    private Obstacle Obstacle;
    // Use this for initialization
    void Start()
    {
        Obstacle = gameObject.AddComponent<Obstacle>();

        Obstacle.RequiredItem = "Sword";
        Obstacle.NotRequiredItemMessage = new string[]
        {
            "These spider webs are way too thick to remove with hands. You will need something long and sharp."
        };

        Obstacle.RequiredItemMessage = new string[]
        {
            "These spider webs are way too thick to remove with hands.",
            "You slash through the webs with your sword, finding way forward."
        };

        Obstacle.UseItem = false;
        Obstacle.DestroyObstacle = true;
        Obstacle.ExplorePoints = 1;
    }
}
