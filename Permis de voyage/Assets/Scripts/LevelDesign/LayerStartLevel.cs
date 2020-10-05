using UnityEngine;

public class LayerStartLevel : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Level currentLevel = Level.Instance;
        if (other.name == "Player" && currentLevel.HasReachedFarSide)
        {
            Game.Instance.WinLevel(currentLevel);
        }
    }
}
