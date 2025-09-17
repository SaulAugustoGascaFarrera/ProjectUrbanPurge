using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance { get; private set; }

    public const int UNIT_LAYER = 7;
    public const int BUILDING_LAYER = 8;


    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }
}
