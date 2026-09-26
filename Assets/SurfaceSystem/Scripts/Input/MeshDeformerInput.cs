using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    public float force = 10f;

    void Update()
    {
        if (Input.GetMouseButton(0)) 
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            
        }
    }

}
