using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Kobbelt : MonoBehaviour
{
    public int nbIteration = 1;

    private void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh baseMesh = meshFilter.mesh;

        Mesh newMesh = Subdiv(baseMesh, nbIteration);

        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();

        baseMesh = newMesh;
    }


    private Mesh Subdiv(Mesh mesh,int iteration)
    {
        Mesh meshsub = mesh;

        for(int i = 0;i< iteration; i++)
        {
            meshsub = KobbeltSubdivision(meshsub); //fonction de subdivision de kobbelt qui renvoie le mesh
        }

        return meshsub;
    }

    #region "v1"
    private Mesh KobbeltSubdivision(Mesh mesh)
    {
        int[] triangles = mesh.triangles;
        int nbTriangles = triangles.Length / 3;

        Vector3[] vertices = mesh.vertices;
        int nbVertices = vertices.Length;

       

        for (int i = 0; i < nbTriangles; i++)
        {
            int indexTriangle = i * 3;
            int vertice1 = triangles[indexTriangle];
            int vertice2 = triangles[indexTriangle + 1];
            int vertice3 = triangles[indexTriangle + 2];

            Vector3 centre = (vertices[vertice1] + vertices[vertice2] + vertices[vertice3]) / 3;
            Vector3[] subVertices = mesh.vertices;
            Array.Resize(ref subVertices, subVertices.Length + 1);
            subVertices[subVertices.Length - 1] = centre;

            mesh.vertices = subVertices;

            int[] subTriangles = mesh.triangles;
            Array.Resize(ref subTriangles, subTriangles.Length + 9);


            Pertubate(mesh);
            //creation des nouveaux triangles
            subTriangles[subTriangles.Length - 1] = vertice1;
            subTriangles[subTriangles.Length - 2] = vertice2;
            subTriangles[subTriangles.Length - 3] = subVertices.Length - 1;

            subTriangles[subTriangles.Length - 4] = vertice2;
            subTriangles[subTriangles.Length - 5] = vertice3;
            subTriangles[subTriangles.Length - 6] = subVertices.Length - 1;

            subTriangles[subTriangles.Length - 7] = vertice3;
            subTriangles[subTriangles.Length - 8] = vertice1;
            subTriangles[subTriangles.Length - 9] = subVertices.Length - 1;

            mesh.triangles = subTriangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }



        return mesh;
    }


    Mesh Pertubate(Mesh mesh)
    {
        Vector3[] subVertices = mesh.vertices;
        Vector3[] pertubateVertices = subVertices;

        for (int i = 0; i < subVertices.Length - 1; i++)
        {

            HashSet<Vector3> neighbords = GetNeighbords(mesh, subVertices[i]);
            float n = neighbords.Count; // nb voisin
            float alpha = ((4 - 2 * Mathf.Cos((2 * Mathf.PI) / n))) / 9; // avoir la liste des voisins
            Vector3 somme = Vector3.zero;

            foreach (var vec in neighbords)
            {
                somme += vec;
            }
            //Debug.Log("<color=red>" + somme + "</color>");
            Debug.Log("<color=blue>" + n + "</color>");
            Debug.Log("<color=green>" + alpha + "</color>");
            pertubateVertices[i] = ((1 - alpha) * subVertices[i]) + (((alpha) / n) * somme);

        }

        mesh.vertices = pertubateVertices;
        mesh.RecalculateBounds();
        return mesh;
    }

    //Vector3[] GetNeighbords(Mesh mesh, int verticeIndex)
    //{
    //    HashSet<Vector3> neighborVertices = new HashSet<Vector3>();

    //    int[] triangles = mesh.triangles;
    //    int triangleCount = triangles.Length / 3;

    //    for (int i = 0; i < triangleCount; i++)
    //    {
    //        int triangleIndex = i * 3;

    //        if (triangles[triangleIndex] == verticeIndex || triangles[triangleIndex + 1] == verticeIndex || triangles[triangleIndex + 2] == verticeIndex)
    //        {
    //            if (triangles[triangleIndex] == verticeIndex)
    //            {
    //                neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 1]]);
    //                neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 2]]);
    //            }
    //            else if (triangles[triangleIndex + 1] == verticeIndex)
    //            {
    //                neighborVertices.Add(mesh.vertices[triangles[triangleIndex]]);
    //                neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 2]]);
    //            }
    //            else if (triangles[triangleIndex + 2] == verticeIndex)
    //            {
    //                neighborVertices.Add(mesh.vertices[triangles[triangleIndex]]);
    //                neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 1]]);
    //            }
    //        }

    //    }


    //    return neighborVertices.ToArray();

    //}

    HashSet<Vector3> GetNeighbords(Mesh mesh, Vector3 vertice)
    {
        HashSet<Vector3> neighborVertices = new HashSet<Vector3>();

        int[] triangles = mesh.triangles;
        int triangleCount = triangles.Length / 3;

        for (int i = 0; i < triangleCount; i++)
        {
            int triangleIndex = i * 3;

            if (mesh.vertices[triangles[triangleIndex]] == vertice 
                || mesh.vertices[triangles[triangleIndex + 1]] == vertice 
                || mesh.vertices[triangles[triangleIndex + 2]] == vertice)
            {
                if (mesh.vertices[triangles[triangleIndex]] == vertice)
                {
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 1]]);
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 2]]);
                }
                else if (mesh.vertices[triangles[triangleIndex + 1]] == vertice)
                {
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex]]);
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 2]]);
                }
                else if (mesh.vertices[triangles[triangleIndex + 2]] == vertice)
                {
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex]]);
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 1]]);
                }
            }

        }


        return neighborVertices;

    }

    #endregion


}

//public class Edge
//{
//    public int vertexIndex1;
//    public int vertexIndex2;

//    public Edge(int vertex1, int vertex2)
//    {
//        vertexIndex1 = vertex1;
//        vertexIndex2 = vertex2;
//    }

//    public bool Equals(Edge other)
//    {
//        return (vertexIndex1 == other.vertexIndex1 && vertexIndex2 == other.vertexIndex2) ||
//               (vertexIndex1 == other.vertexIndex2 && vertexIndex2 == other.vertexIndex1);
//    }

//    public override bool Equals(object obj)
//    {
//        if (ReferenceEquals(null, obj)) return false;
//        if (ReferenceEquals(this, obj)) return true;
//        if (obj.GetType() != this.GetType()) return false;
//        return Equals((Edge)obj);
//    }

//    public override int GetHashCode()
//    {
//        unchecked
//        {
//            int hash = 17;
//            hash = hash * 23 + Math.Min(vertexIndex1, vertexIndex2).GetHashCode();
//            hash = hash * 23 + Math.Max(vertexIndex1, vertexIndex2).GetHashCode();
//            return hash;
//        }
//    }
//}


//public class Triangle
//{
//    public int[] vertices = new int[3];
//    public Edge[] edges = new Edge[3];

//    public Triangle(int vertex1, int vertex2, int vertex3)
//    {
//        vertices[0] = vertex1;
//        vertices[1] = vertex2;
//        vertices[2] = vertex3;

//        edges[0] = new Edge(vertex1, vertex2);
//        edges[1] = new Edge(vertex2, vertex3);
//        edges[2] = new Edge(vertex3, vertex1);
//    }

//    public bool ContainsEdge(Edge edge)
//    {
//        return edges[0].Equals(edge) || edges[1].Equals(edge) || edges[2].Equals(edge);
//    }

//    public int GetOtherVertex(Edge edge)
//    {
//        if (edges[0].Equals(edge))
//            return vertices[2];
//        else if (edges[1].Equals(edge))
//            return vertices[0];
//        else if (edges[2].Equals(edge))
//            return vertices[1];
//        else
//            return -1; // Edge does not belong to this triangle
//    }
//}

