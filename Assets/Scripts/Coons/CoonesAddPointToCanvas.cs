using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoonesAddPointToCanvas : MonoBehaviour
{
    public GameObject prefabsVertex;
    public GameObject prefabsEdge;
    private List<CoonesVertice> vertices = new List<CoonesVertice>();
    public CoonesPolygons2D currentPolygon2D;
    public CoonesPolygons3D currentPolygon3D;
    private bool isPolygonCreated;
    private float offSet = -3;

    // Start is called before the first frame update
    void Start()
    {
        isPolygonCreated = false;   
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
                    var verticeTemp = Instantiate(prefabsVertex, raycastHit.point, Quaternion.Euler(0, 0, 0));
                    vertices.Add(verticeTemp.GetComponent<CoonesVertice>());
                }
            }
        }
        //Create polygons
        if (Input.GetKeyDown(KeyCode.A) && !isPolygonCreated)
        {
            isPolygonCreated = true;
            CreatePolygon2D();
        }
        //Clear everything
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isPolygonCreated = false;
            clearPolygon(currentPolygon2D);
            vertices.Clear();
        }
        //Create 3D
        if (Input.GetKeyDown(KeyCode.E))
        {
            Create3DFrom2D(currentPolygon2D);
        }
    }

    private void clearPolygon(CoonesPolygons2D polygonToDelete)
    {
        foreach(var poly in polygonToDelete.edgesList)
        {
            Destroy(poly.gameObject);
        }

        foreach (var poly in polygonToDelete.verticesList)
        {
            Destroy(poly.gameObject);
        }
    }

    private void CreatePolygon2D()
    {
        CoonesPolygons2D polygon = new CoonesPolygons2D(vertices);

        for (int i = 0; i < polygon.verticesList.Count - 1; i++)
        {
            var edgeTemp = Instantiate(prefabsEdge, new Vector3(0, 0, -0.1f), Quaternion.identity);
            edgeTemp.GetComponent<LineRenderer>().SetPosition(0, polygon.verticesList[i].transform.position);
            edgeTemp.GetComponent<LineRenderer>().SetPosition(1, polygon.verticesList[i + 1].transform.position);
            polygon.edgesList.Add(edgeTemp.GetComponent<CoonesEdge>());
        }
        currentPolygon2D = polygon;
    }

    private void Create3DFrom2D(CoonesPolygons2D polygonToTransform)
    {
        CoonesPolygons3D polygon = new CoonesPolygons3D(polygonToTransform);

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < polygonToTransform.verticesList.Count - 1; j++)
            {
                var verticeTemp = Instantiate(prefabsVertex, polygonToTransform.verticesList[j].transform.position + new Vector3(0, 0, i * offSet), Quaternion.identity);
                polygon.verticesList.Add(verticeTemp.GetComponent<CoonesVertice>());

                var edgeTemp = Instantiate(prefabsEdge, new Vector3(0, 0, i * offSet -0.1f), Quaternion.identity);
                edgeTemp.GetComponent<LineRenderer>().SetPosition(0, polygonToTransform.verticesList[j].transform.position);
                edgeTemp.GetComponent<LineRenderer>().SetPosition(1, polygonToTransform.verticesList[j + 1].transform.position);
                polygon.edgesList.Add(edgeTemp.GetComponent<CoonesEdge>());

                //Add last vertice
                if(j == polygonToTransform.verticesList.Count - 2)
                {
                    var lastVert = Instantiate(prefabsVertex, polygonToTransform.verticesList[j + 1].transform.position + new Vector3(0, 0, i * offSet), Quaternion.identity);
                    polygon.verticesList.Add(lastVert.GetComponent<CoonesVertice>());
                }
            }
        }
        for(int j = 0; j < 4; j++)
        {
            for(int i = 0; i < polygonToTransform.verticesList.Count; i++)
            {
                var edgeTemp = Instantiate(prefabsEdge, new Vector3(0, 0, j * offSet - 0.1f), Quaternion.identity);
                edgeTemp.GetComponent<LineRenderer>().SetPosition(0, polygon.verticesList[i].transform.position);
                edgeTemp.GetComponent<LineRenderer>().SetPosition(1, polygon.verticesList[i + polygonToTransform.verticesList.Count].transform.position);
                polygon.edgesList.Add(edgeTemp.GetComponent<CoonesEdge>());
            }
        }
    }

}
