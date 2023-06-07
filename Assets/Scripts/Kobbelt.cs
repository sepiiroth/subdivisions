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

    private Mesh KobbeltSubdivision(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Vector3[] subVertices = new Vector3[vertices.Length + triangles.Length / 3];
        Array.Copy(vertices, subVertices, vertices.Length);

        int[] subTriangles = new int[triangles.Length + triangles.Length / 3 * 9];
        Array.Copy(triangles, subTriangles, triangles.Length);

        for (int i = 0; i < triangles.Length / 3; i++)
        {
            int indexTriangle = i * 3;
            int vertice1 = triangles[indexTriangle];
            int vertice2 = triangles[indexTriangle + 1];
            int vertice3 = triangles[indexTriangle + 2];

            Vector3 centre = (vertices[vertice1] + vertices[vertice2] + vertices[vertice3]) / 3;
            subVertices[vertices.Length + i] = centre;

            Pertubate(mesh);

            subTriangles[subTriangles.Length - 9] = vertice1;
            subTriangles[subTriangles.Length - 8] = vertice2;
            subTriangles[subTriangles.Length - 7] = vertices.Length + i;

            subTriangles[subTriangles.Length - 6] = vertice2;
            subTriangles[subTriangles.Length - 5] = vertice3;
            subTriangles[subTriangles.Length - 4] = vertices.Length + i;

            subTriangles[subTriangles.Length - 3] = vertice3;
            subTriangles[subTriangles.Length - 2] = vertice1;
            subTriangles[subTriangles.Length - 1] = vertices.Length + i;
        }

        mesh.vertices = subVertices;
        mesh.triangles = subTriangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }


    Mesh Pertubate(Mesh mesh)
    {
        Vector3[] subVertices = mesh.vertices;
        Vector3[] pertubateVertices = subVertices;
        
        for (int i = 0; i < subVertices.Length-1; i++)
        {

            float n = GetNeighbords(mesh, i).Length; // nb voisin
            Vector3[] neighbords = GetNeighbords(mesh, i);
            float alpha =  ((4 - 2 * Mathf.Cos((2 * Mathf.PI) / n))) / 9; // avoir la liste des voisins
            Vector3 somme = Vector3.zero;

            foreach (var vec in neighbords)
            {
                somme += vec ;
            }
            //Debug.Log("<color=red>" + somme + "</color>");
            Debug.Log("<color=blue>" + n + "</color>");
            Debug.Log("<color=green>" + alpha + "</color>");
            pertubateVertices[i] = ((1 - alpha) * subVertices[i]) + (((alpha) / (n*n)) * somme);

        mesh.RecalculateBounds();
        }

        mesh.vertices = pertubateVertices;
        return mesh;
    }

    Vector3[] GetNeighbords(Mesh mesh, int verticeIndex)
    {
        HashSet<Vector3> neighborVertices = new HashSet<Vector3>();

        int[] triangles = mesh.triangles;
        int triangleCount = triangles.Length / 3;

        for (int i = 0; i < triangleCount; i++)
        {
            int triangleIndex = i * 3;

            if (triangles[triangleIndex] == verticeIndex || triangles[triangleIndex + 1] == verticeIndex || triangles[triangleIndex + 2] == verticeIndex)
            {
                if (triangles[triangleIndex] == verticeIndex)
                {
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 1]]);
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 2]]);
                }
                else if (triangles[triangleIndex + 1] == verticeIndex)
                {
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex]]);
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 2]]);
                }
                else if (triangles[triangleIndex + 2] == verticeIndex)
                {
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex]]);
                    neighborVertices.Add(mesh.vertices[triangles[triangleIndex + 1]]);
                }
            }
            
        }


        return neighborVertices.ToArray();

    }

}

//public class Vertex
//{

//    public Vector3 vec3;

//    public Vertex(Vector3 vec3)
//    {
//        this.vec3 = vec3;
//    }
//}

//public class Edge
//{
//    public List<Vertex> points = new List<Vertex>();

//    public Edge(Vertex v1, Vertex v2)
//    {
//        points.Add(v1);
//        points.Add(v2);
//    }


//}
