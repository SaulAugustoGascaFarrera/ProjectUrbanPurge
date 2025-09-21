using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance { get; private set; }

    public const int PLAYER_LAYER = 7;
    public const int BUILDING_LAYER = 8;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
        }


        Instance = this;
    }
}
