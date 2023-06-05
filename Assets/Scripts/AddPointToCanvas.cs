using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPointToCanvas : MonoBehaviour
{
    public GameObject prefabsVertex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    //CurrentClickedGameObject(raycastHit.transform.gameObject);
                    Debug.Log(raycastHit.point);
                    Instantiate(prefabsVertex, raycastHit.point, Quaternion.Euler(0, 0, 0));
                }
            }
        }
    }
}
