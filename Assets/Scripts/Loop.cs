using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Vertice {
    public int index;
    public Vector3 position;
    public Vertice newPosition;

    public Vertice(int i, Vector3 pos) {
        this.index = i;
        this.position = pos;
    }
}

public class Edge {
    public int index;
    public Vertice newVertice;
    public Vector3 v1Position;
    public Vector3 v2Position;

    public List<Vertice> vertices = new List<Vertice>(); 
    public List<Polygon> adjacentPolygons = new List<Polygon>();

    public Edge(int i, Vertice v1, Vertice v2) {
        this.index = i;
        this.v1Position = v1.position;
        this.v2Position = v2.position;
        this.vertices.Add(v1);
        this.vertices.Add(v2);
    }
}

public class Polygon {
    public int index;
    public List<Edge> edges = new List<Edge>();

    public Polygon(int i, Edge e1, Edge e2, Edge e3) {
        this.index = i;
        this.edges.Add(e1);
        this.edges.Add(e2);
        this.edges.Add(e3);
    }
}

public class Loop : MonoBehaviour {
    private MeshFilter meshFilter;
    public GameObject sphere;
    public int iteration;

    private void Start() {
        LoopSubdivide(iteration);
    }

    private void LoopSubdivide(int fois) {

        Dictionary<int, Vertice> verticeDict = new Dictionary<int, Vertice>();
        List<Edge> edges = new List<Edge>();
        List<Polygon> polygons = new List<Polygon>();
        List<Vertice> new_vertices = new List<Vertice>();
        
        meshFilter = GetComponent<MeshFilter>();

        for(int x = 0; x < fois; x++) {
            verticeDict.Clear();
            edges.Clear();
            polygons.Clear();
            new_vertices.Clear();

            if (meshFilter != null) {

                Mesh mesh = meshFilter.mesh;
                
                for(int i = 0; i < mesh.vertices.Length; i++) {
                    verticeDict[i] = new Vertice(i, mesh.vertices[i]);
                }

                for(int i = 0; i < mesh.triangles.Length; i+=3) {
                    Vertice v1 = verticeDict[mesh.triangles[i]];
                    Vertice v2 = verticeDict[mesh.triangles[i+1]];
                    Vertice v3 = verticeDict[mesh.triangles[i+2]];

                    Edge e1 = new Edge(edges.Count, v1, v2);
                    Edge e2 = new Edge(edges.Count, v2, v3);
                    Edge e3 = new Edge(edges.Count, v3, v1);

                    Edge existingE1 = FindEquivalentEdge(edges, e1);
                    Edge existingE2 = FindEquivalentEdge(edges, e2);
                    Edge existingE3 = FindEquivalentEdge(edges, e3);

                    Polygon newPolygon = new Polygon(i, e1, e2, e3);
                    polygons.Add(newPolygon);

                    e1.adjacentPolygons.Add(newPolygon);
                    e2.adjacentPolygons.Add(newPolygon);
                    e3.adjacentPolygons.Add(newPolygon);

                    if (existingE1 != null) {
                        existingE1.adjacentPolygons.Add(newPolygon);
                        e1.adjacentPolygons.AddRange(existingE1.adjacentPolygons.Where(p => p.index != newPolygon.index));
                    }

                    if (existingE2 != null) {
                        existingE2.adjacentPolygons.Add(newPolygon);
                        e2.adjacentPolygons.AddRange(existingE2.adjacentPolygons.Where(p => p.index != newPolygon.index));
                    }

                    if (existingE3 != null) {
                        existingE3.adjacentPolygons.Add(newPolygon);
                        e3.adjacentPolygons.AddRange(existingE3.adjacentPolygons.Where(p => p.index != newPolygon.index));
                    }

                    edges.Add(e1);
                    edges.Add(e2);
                    edges.Add(e3);
                }

                foreach (Edge edge in edges) {
                    Vector3 v1 = edge.vertices[0].position;
                    Vector3 v2 = edge.vertices[1].position;
                    if (edge.adjacentPolygons.Count >= 2) {
                        Vector3 vleft = edge.adjacentPolygons[0].edges.Find(e => e.v1Position != edge.v1Position && e.v2Position != edge.v1Position).vertices.Find(v => v.position != edge.vertices[0].position && v.position != edge.vertices[1].position).position;
                        Vector3 vright = edge.adjacentPolygons[1].edges.Find(e => e.v1Position != edge.v1Position && e.v2Position != edge.v1Position).vertices.Find(v => v.position != edge.vertices[0].position && v.position != edge.vertices[1].position).position;

                        Vector3 newPos = 3f/8f * (v1 + v2) + 1f/8f * (vleft + vright);
                        edge.newVertice = new Vertice(new_vertices.Count, newPos);
                    } else {
                        // Utiliser le milieu quand l'un des deux est
                        Vector3 vleft = edge.adjacentPolygons[0].edges.Find(e => e.v1Position != edge.v1Position && e.v2Position != edge.v1Position).vertices.Find(v => v.position != edge.vertices[0].position && v.position != edge.vertices[1].position).position;
                        Vector3 vright = GetMidPoint(edge.v1Position, edge.v2Position);
                        

                        Vector3 newPos = 3f/8f * (v1 + v2) + 1f/8f * (vleft + vright);
                        edge.newVertice = new Vertice(new_vertices.Count, newPos);
                    }
                    new_vertices.Add(edge.newVertice);
                }

                foreach(KeyValuePair<int, Vertice> element in verticeDict) {
                    float n = 0;
                    float alpha = 0.0f;
                    List<Vector3> tempoDedans = new List<Vector3>();
                    List<Vector3> tempoDehors = new List<Vector3>();

                    foreach(Edge ed in edges) {
                        if(ed.vertices[0].position == element.Value.position || ed.vertices[1].position == element.Value.position) {
                            if (ed.adjacentPolygons.Count == 1) { 
                                if(ed.vertices[0].position == element.Value.position) {
                                    tempoDehors.Add(ed.vertices[1].position);
                                }
                                if(ed.vertices[1].position == element.Value.position) {
                                    tempoDehors.Add(ed.vertices[0].position);
                                }
                            } else {
                                if(ed.vertices[0].position == element.Value.position) {
                                    n++;
                                    tempoDedans.Add(ed.vertices[1].position);
                                }
                                if(ed.vertices[1].position == element.Value.position) {
                                    n++;
                                    tempoDedans.Add(ed.vertices[0].position);
                                }
                            }
                        }
                    }

                    n /= 2;

                    /*if(n == 2) {
                        Vector3 neighbor1 = tempoDehors[0];
                        Vector3 neighbor2 = tempoDehors[1];
                        Vector3 newPos = 3f/4f * element.Value.position + 1f/8f * (neighbor1 + neighbor2);
                        Vertice vert = new Vertice(new_vertices.Count, newPos);
                        Debug.Log(vert);
                        //element.Value.newPosition = vert;
                        //new_vertices.Add(vert);
                    }*/
                    if(n <= 3) {
                        alpha = 3/16;
                        List<Vector3> uniqueVectors = new List<Vector3>();
                        foreach (Vector3 vector in tempoDedans) {
                            if (!uniqueVectors.Contains(vector))
                            {
                                uniqueVectors.Add(vector);
                            }
                        }

                        Vector3 somme = new Vector3(0, 0, 0);
                        foreach(Vector3 vec in uniqueVectors) {
                            somme += vec;
                        }
                        
                        Vertice vert = new Vertice(new_vertices.Count, (1f - n * alpha) * element.Value.position + alpha * somme);
                        element.Value.newPosition = vert;
                        new_vertices.Add(vert);
                    } else if(n > 3) {
                        alpha = (1f/n)*((5f/8f)-Mathf.Pow((3f/8f)+(1f/4f)*Mathf.Cos(2*Mathf.PI/n), 2));
                        List<Vector3> uniqueVectors = new List<Vector3>();

                        foreach (Vector3 vector in tempoDedans) {
                            if (!uniqueVectors.Contains(vector))
                            {
                                uniqueVectors.Add(vector);
                            }
                        }

                        Vector3 somme = new Vector3(0, 0, 0);
                        foreach(Vector3 vec in uniqueVectors) {
                            somme += vec;
                        }
                        
                        Vertice vert = new Vertice(new_vertices.Count, (1f - n * alpha) * element.Value.position + alpha * somme);
                        element.Value.newPosition = vert;
                        new_vertices.Add(vert);
                    }
                }

                List<Vertice> newVertices = new List<Vertice>();
                List<Edge> newEdges = new List<Edge>();
                List<Polygon> newPolygons = new List<Polygon>();

                foreach (Polygon poly in polygons) { // 12
                    for (int i = 0; i < 3; i++) {
                        Edge oldEdge = poly.edges[i];
                        Vertice vert1 = oldEdge.vertices[0].newPosition;
                        newVertices.Add(vert1);
                        Vertice vert2 = oldEdge.newVertice;
                        newVertices.Add(vert2);

                        Edge newEdge1 = new Edge(newEdges.Count, vert1, vert2);
                        newEdges.Add(newEdge1);
                        
                        Vertice vert3 = poly.edges[(i+2)%3].newVertice;
                        newVertices.Add(vert3);

                        Edge newEdge2 = new Edge(newEdges.Count, vert2, vert3); 
                        newEdges.Add(newEdge2);

                        Edge connectingEdge = new Edge(newEdges.Count, vert3, vert1);
                        newEdges.Add(connectingEdge);

                        Polygon newPoly = new Polygon(newPolygons.Count, newEdge1, newEdge2, connectingEdge);
                        newPolygons.Add(newPoly);
                    }

                    Vertice centerVert1 = poly.edges[0].newVertice;
                    Vertice centerVert2 = poly.edges[1].newVertice;
                    Vertice centerVert3 = poly.edges[2].newVertice;

                    newVertices.Add(centerVert1);
                    newVertices.Add(centerVert2);
                    newVertices.Add(centerVert3);

                    Edge edge1 = new Edge(newEdges.Count, centerVert1, centerVert2);
                    newEdges.Add(edge1);
                    Edge edge2 = new Edge(newEdges.Count, centerVert2, centerVert3);
                    newEdges.Add(edge2);
                    Edge edge3 = new Edge(newEdges.Count, centerVert3, centerVert1);
                    newEdges.Add(edge3);

                    Polygon centerPoly = new Polygon(newPolygons.Count, edge1, edge2, edge3);
                    newPolygons.Add(centerPoly);
                }

                edges = newEdges;
                polygons = newPolygons;
                
                Vector3[] newVerticesTab = new Vector3[newVertices.Count]; 
                for (int i = 0; i < newVertices.Count; i++) {
                    newVerticesTab[i] = newVertices[i].position;
                }

                int[] newTriangles = new int[newVertices.Count];  
                for (int i = 0; i < newVertices.Count; i+=3) {
                    newTriangles[i] = i;
                    newTriangles[i+ 1] = i+1;
                    newTriangles[i+ 2] = i+2;
                }

                Vector2[] uvs = new Vector2[newVerticesTab.Length];
                for (int i = 0; i < uvs.Length; i++) {
                    uvs[i] = new Vector2(newVerticesTab[i].x, newVerticesTab[i].z);
                }

                meshFilter.mesh.Clear();
                meshFilter.mesh.vertices = newVerticesTab;
                meshFilter.mesh.triangles = newTriangles;
                meshFilter.mesh.uv = uvs;
                meshFilter.mesh.RecalculateBounds();
                meshFilter.mesh.RecalculateNormals();
            }
        }
    }

    Edge FindEquivalentEdge(List<Edge> edges, Edge edge) {
        foreach (Edge existingEdge in edges) {
            if ((existingEdge.v1Position == edge.v1Position && existingEdge.v2Position == edge.v2Position) || (existingEdge.v1Position == edge.v2Position && existingEdge.v2Position == edge.v1Position)) {
                return existingEdge;
            }
        }
        return null;
    }

    Vector3 GetMidPoint(Vector3 a, Vector3 b) {
    float x = (a.x + b.x) / 2f;
    float y = (a.y + b.y) / 2f;
    float z = (a.z + b.z) / 2f;
    return new Vector3(x, y, z);
    }
}
