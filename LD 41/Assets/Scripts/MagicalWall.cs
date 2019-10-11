using UnityEngine;
using System.Collections;

public class MagicalWall : MonoBehaviour
{
    private Obstacle Obstacle;
    // Use this for initialization
    void Start()
    {
        Obstacle = gameObject.AddComponent<Obstacle>();
        Obstacle.DestroyObstacle = true;
        Obstacle.ExplorePoints = 1;
        Obstacle.RequiredItem = "Urn";
        Obstacle.UseItem = false;
        Obstacle.RequiredItemMessage = new string[]
        {
            "This wall clearly does not belong here yet there it stands.",
            "As you rummage through you backpack you notice the magical urn is glowing slightly.",
            "You promptly take it out and after being almost blinded by its light you realize the wall is gone.",
            "You can't see anything beyond it. It's so dark even your light does not help."
        };
        Obstacle.NotRequiredItemMessage = new string[]
        {
            "This wall clearly does not belong here yet there it stands.",
            "On the other hand nothing in your current inventory would be of much help.",
        };
    }
}
