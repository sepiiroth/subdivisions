using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Structure;
using UnityEngine;

public class AddPointToCanvas : MonoBehaviour
{
    public GameObject prefabsVertex;
    public GameObject prefabsEdge;
    public MathManager mm;

    // Start is called before the first frame update
    void Start()
    {
        mm = MathManager.Instance();
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
                    switch (raycastHit.transform.gameObject.tag)
                    {
                        case "Background":
                            Instantiate(prefabsVertex, raycastHit.point, Quaternion.Euler(0, 0, 0));
                            if (mm.Polygons.Count == 0)
                            {
                                mm.Polygons.Add(new Polygon());
                            }
                            mm.Polygons.Last().Vertices.Add(new Vertex(raycastHit.point));
                            if (mm.Polygons.Last().Vertices.Count > 1)
                            {
                                mm.Polygons.Last().Edges.Add(new Edge(mm.Polygons.Last().Vertices.Last(),mm.Polygons.Last().Vertices[^2],mm.Polygons.Last()  ));
                            }
                            break;
                        case "Vertex":
                            if (mm.Polygons.Last().Vertices.Count == 1)
                            {
                                mm.Polygons.Last().Vertices.Add(new Vertex(raycastHit.transform.position));
                                mm.Polygons.Last().Edges.Add(new Edge(mm.Polygons.Last().Vertices.Last(),mm.Polygons.Last().Vertices[^2],mm.Polygons.Last()  ));
                                break;
                            }
                            mm.Polygons.Add(new Polygon());
                            mm.Polygons.Last().Vertices.Add(new Vertex(raycastHit.transform.position));
                            break;
                    }


                }
            }
        }
    }
}
