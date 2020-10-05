using UnityEngine;

public class LayerStartLevel : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player" && Level.Instance.HasReachedFarSide)
        {
            Game.Instance.WinLevel();
        }
    }
}
