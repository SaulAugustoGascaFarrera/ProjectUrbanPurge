using UnityEngine;

public class MouseManager : MonoBehaviour
{

    public static MouseManager Instance { get; private set; }


    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }

    public Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 

        if(Physics.Raycast(ray,out RaycastHit hitInfo,float.MaxValue,1 << 6))
        {
            return hitInfo.point;
        }


        return Vector3.zero;
    }
}
