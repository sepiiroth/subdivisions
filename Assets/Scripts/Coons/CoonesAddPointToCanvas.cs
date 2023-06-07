using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoonesAddPointToCanvas : MonoBehaviour
{
    public GameObject prefabsVertex;
    public GameObject prefabsEdge;

    public GameObject newPrefabsVertex;
    public GameObject newPrefabsEdge;

    private List<CoonesVertice> vertices = new List<CoonesVertice>();

    private List<CoonesVertice> newVertices = new List<CoonesVertice>();

    public CoonesPolygons2D currentPolygon2D;
    public CoonesPolygons3D currentPolygon3D;

    public CoonesPolygons2D newPolygon2D;

    private bool isPolygonCreated;
    private float offSet = -3;
    private bool isPolygon;

    [SerializeField] public float u;
    [SerializeField] public float v;

    // Start is called before the first frame update
    void Start()
    {
        isPolygonCreated = false;   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && !isPolygonCreated)
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
        //Create line / A
        if (Input.GetKeyDown(KeyCode.A) && !isPolygonCreated)
        {
            isPolygonCreated = true;
            CreatePolygonOrLine2D(false);
        }
        //Create polygons / Z
        if (Input.GetKeyDown(KeyCode.Z) && !isPolygonCreated)
        {
            isPolygonCreated = true;
            CreatePolygonOrLine2D(true);
        }
        //Clear everything / E
        if (Input.GetKeyDown(KeyCode.E))
        {
            isPolygonCreated = false;
            clearPolygon();
        }
        //Create 3D / R
        if (Input.GetKeyDown(KeyCode.R))
        {
            Create3DFrom2D(currentPolygon2D);
        }
        //Chaiking / R
        if (Input.GetKeyDown(KeyCode.T))
        {
            CutCorner(currentPolygon2D.edgesList);
        }
    }

    public void CreateChaiking()
    {
        //TODO Chaiking

    }

    private void CutCorner(List<CoonesEdge> edgeList)
    {
        for (int i = 0; i < edgeList.Count; i++)
        {
            var posNewVertU = Vector3.Lerp(edgeList[i].pointA.transform.position, edgeList[i].pointB.transform.position, u);
            var posNewVertV = Vector3.Lerp(edgeList[i].pointB.transform.position, edgeList[i].pointA.transform.position, v);
            var newVertU = Instantiate(newPrefabsVertex, posNewVertU, Quaternion.identity);
            newVertices.Add(newVertU.GetComponent<CoonesVertice>());
            var newVertV = Instantiate(newPrefabsVertex, posNewVertV, Quaternion.identity);
            newVertices.Add(newVertV.GetComponent<CoonesVertice>());
        }
        CoonesPolygons2D polygon = new CoonesPolygons2D(newVertices);
        for(int i = 0; i < polygon.verticesList.Count - 1; i++)
        {
            var edgeTemp = Instantiate(newPrefabsEdge, new Vector3(0, 0, -0.5f), Quaternion.identity);
            edgeTemp.GetComponent<LineRenderer>().SetPosition(0, polygon.verticesList[i].transform.position);
            edgeTemp.GetComponent<LineRenderer>().SetPosition(1, polygon.verticesList[i + 1].transform.position);
            edgeTemp.GetComponent<CoonesEdge>().pointA = polygon.verticesList[i];
            edgeTemp.GetComponent<CoonesEdge>().pointB = polygon.verticesList[i + 1];
            polygon.edgesList.Add(edgeTemp.GetComponent<CoonesEdge>());

            newPolygon2D = polygon;
            //clearPolygon();
            //currentPolygon2D = newPolygon2D;
            //vertices = newVertices;
        }
        if (this.isPolygon)
        {
            var ClosingEdge = Instantiate(newPrefabsEdge, new Vector3(0, 0, -0.5f), Quaternion.identity);
            ClosingEdge.GetComponent<LineRenderer>().SetPosition(0, polygon.verticesList[^1].transform.position);
            ClosingEdge.GetComponent<LineRenderer>().SetPosition(1, polygon.verticesList[0].transform.position);
            polygon.edgesList.Add(ClosingEdge.GetComponent<CoonesEdge>());
        }
    }

    private void clearPolygon()
    {
        if (currentPolygon2D != null)
        {
            foreach(var poly in currentPolygon2D.edgesList)
            {
                Destroy(poly.gameObject);
            }
            foreach (var poly in currentPolygon2D.verticesList)
            {
                Destroy(poly.gameObject);
            }
        }

        if(currentPolygon3D != null)
        {
            foreach (var poly in currentPolygon3D.edgesList)
            {
                Destroy(poly.gameObject);
            }
            foreach (var poly in currentPolygon3D.verticesList)
            {
                Destroy(poly.gameObject);
            }
        }
        vertices.Clear();
    }

    private void CreatePolygonOrLine2D(bool isPolygon)
    {
        CoonesPolygons2D polygon = new CoonesPolygons2D(vertices);

        for (int i = 0; i < polygon.verticesList.Count - 1; i++)
        {
            var edgeTemp = Instantiate(prefabsEdge, new Vector3(0, 0, -0.1f), Quaternion.identity);
            edgeTemp.GetComponent<LineRenderer>().SetPosition(0, polygon.verticesList[i].transform.position);
            edgeTemp.GetComponent<LineRenderer>().SetPosition(1, polygon.verticesList[i + 1].transform.position);
            edgeTemp.GetComponent<CoonesEdge>().pointA = polygon.verticesList[i];
            edgeTemp.GetComponent<CoonesEdge>().pointB = polygon.verticesList[i + 1];
            polygon.edgesList.Add(edgeTemp.GetComponent<CoonesEdge>());
        }
        if (isPolygon)
        {
            var ClosingEdge = Instantiate(prefabsEdge, new Vector3(0, 0, -0.1f), Quaternion.identity);
            ClosingEdge.GetComponent<LineRenderer>().SetPosition(0, polygon.verticesList[^1].transform.position);
            ClosingEdge.GetComponent<LineRenderer>().SetPosition(1, polygon.verticesList[0].transform.position);
            ClosingEdge.GetComponent<CoonesEdge>().pointA = polygon.verticesList[^1];
            ClosingEdge.GetComponent<CoonesEdge>().pointB = polygon.verticesList[0];
            polygon.edgesList.Add(ClosingEdge.GetComponent<CoonesEdge>());
        }
        this.isPolygon = isPolygon;

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
        currentPolygon3D = polygon;
    }
}
