using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AddPointToCanvas : MonoBehaviour
{
    public GameObject prefabsVertex;
    public GameObject prefabsEdge;
    public MathManager mm;

    private int mode = 0; // 0 : Vertex; 1 : Edge; 2 : Polygon

    private Vertex v1, v2;
    private Edge e;
    private Polygon p;
    
    // Start is called before the first frame update
    void Start()
    {
        mm = MathManager.Instance();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyUp(KeyCode.V))
        {
            mode = 0;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            mode = 1;
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            mode = 2;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit raycastHit;
            GameObject line;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    switch (mode)
                    {
                        case 0:
                            e = null;
                            if (raycastHit.transform.gameObject.CompareTag("Background"))
                            {
                                var v = Instantiate(prefabsVertex, raycastHit.point, Quaternion.Euler(0, 0, 0));
                                v.GetComponent<Vertex>().SetVertex(raycastHit.point);
                                mm.Vertices.Add(v.GetComponent<Vertex>());
                            }
                            break;
                        case 1:
                            if (raycastHit.transform.gameObject.CompareTag("Vertex"))
                            {
                                
                                if (v1 == null)
                                {
                                    v1 = raycastHit.transform.gameObject.GetComponent<Vertex>();
                                    if (e == null)
                                    {
                                        p = new Polygon();
                                        mm.Polygons.Add(p);
                                        Debug.Log("oui");
                                    }
                                }
                                else
                                {
                                    v2 = raycastHit.transform.gameObject.GetComponent<Vertex>();
                                
                                    //Creation GO Edge
                                    line = Instantiate(prefabsEdge, new Vector3(0,0,-0.1f),
                                        Quaternion.Euler(0, 0, 0));
                                    line.GetComponent<LineRenderer>().SetPosition(0, v1.vec3);
                                    line.GetComponent<LineRenderer>().SetPosition(1, v2.vec3);
                                
                                    line.GetComponent<Edge>().SetEdge(v1, v2, p);
                                    mm.Edges.Add(line.GetComponent<Edge>());
                                    
                                    p.Edges.Add(line.GetComponent<Edge>());
                                    p.Vertices.AddRange(line.GetComponent<Edge>().points);
                                    e = line.GetComponent<Edge>();
                                    
                                    v1 = null;
                                    v2 = null;
                                    
                                    
                                }
                            }
                            break;
                    }
                    
                    
                    /*switch (raycastHit.transform.gameObject.tag)
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
                                line = Instantiate(prefabsEdge, new Vector3(0,0,-0.1f),
                                    Quaternion.Euler(0, 0, 0));
                                line.GetComponent<LineRenderer>().SetPosition(0, mm.Polygons.Last().Vertices.Last().vec3);
                                line.GetComponent<LineRenderer>().SetPosition(1, mm.Polygons.Last().Vertices[^2].vec3);
                            }
                            break;
                        case "Vertex":
                            if (mm.Polygons.Last().Vertices.Count == 1)
                            {
                                mm.Polygons.Last().Vertices.Add(new Vertex(raycastHit.transform.position));
                                mm.Polygons.Last().Edges.Add(new Edge(mm.Polygons.Last().Vertices.Last(),mm.Polygons.Last().Vertices[^2],mm.Polygons.Last()  ));
                                break;
                            }

                            if (mm.Polygons.Count > 1)
                            {
                                if (mm.Polygons.Last().Vertices.Count < 2)
                                {
                                    break;
                                }
                                else
                                {
                                    mm.Polygons.Last().Edges.Add(new Edge(new Vertex(raycastHit.transform.position),mm.Polygons.Last().Vertices[0],mm.Polygons.Last()  ));
                                    //
                                    mm.Polygons.Last().Edges.Add(new Edge(mm.Polygons.Last().Vertices.Last(),new Vertex(raycastHit.transform.position),mm.Polygons.Last()  ));
                                    line = Instantiate(prefabsEdge, new Vector3(0,0,-0.1f),
                                        Quaternion.Euler(0, 0, 0));
                                    line.GetComponent<LineRenderer>().SetPosition(0, mm.Polygons.Last().Vertices.Last().vec3);
                                    line.GetComponent<LineRenderer>().SetPosition(1, raycastHit.transform.position);
                                    mm.Polygons.Add(new Polygon());
                                    mm.Polygons.Last().Vertices.Add(new Vertex(raycastHit.transform.position));
                                    break;
                                }
                            }
                            mm.Polygons.Last().Edges.Add(new Edge(mm.Polygons.Last().Vertices.Last(),mm.Polygons.Last().Vertices[0],mm.Polygons.Last()  ));
                            line = Instantiate(prefabsEdge, new Vector3(0,0,-0.1f),
                                Quaternion.Euler(0, 0, 0));
                            line.GetComponent<LineRenderer>().SetPosition(0, mm.Polygons.Last().Vertices.Last().vec3);
                            line.GetComponent<LineRenderer>().SetPosition(1, mm.Polygons.Last().Vertices[0].vec3);
                            mm.Polygons.Add(new Polygon());
                            mm.Polygons.Last().Vertices.Add(new Vertex(raycastHit.transform.position));
                            break;
                    }


                }
            }
        }*/
    }
}
