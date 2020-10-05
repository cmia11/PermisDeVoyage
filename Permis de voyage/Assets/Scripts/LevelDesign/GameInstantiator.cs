using UnityEngine;

public class GameInstantiator : MonoBehaviour
{
    public Game gamePrefab;
    
    protected virtual void Awake()
    {
        if (Game.IsInstanceSet)
        {
            Instantiate(gamePrefab);
        }

        // We have no business here anymore
        Destroy(this);
    }
}
