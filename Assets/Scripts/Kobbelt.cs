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

        meshFilter.mesh = newMesh;
    }

    private Mesh Subdiv(Mesh mesh, int iteration)
    {
        Mesh meshsub = mesh;

        for (int i = 0; i < iteration; i++)
        {
            meshsub = KobbeltSubdivision(meshsub);
        }

        return meshsub;
    }

    private Mesh KobbeltSubdivision(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        int nbTriangles = triangles.Length / 3;

        Vector3[] subVertices = new Vector3[vertices.Length + nbTriangles];
        int[] subTriangles = new int[triangles.Length + (9 * nbTriangles)];
        Array.Copy(vertices, subVertices, vertices.Length);
        Array.Copy(triangles, subTriangles, triangles.Length);

        for (int i = 0; i < nbTriangles; i++)
        {
            int indexTriangle = i * 3;
            int vertexIndex1 = triangles[indexTriangle];
            int vertexIndex2 = triangles[indexTriangle + 1];
            int vertexIndex3 = triangles[indexTriangle + 2];

            Vector3 vertex1 = vertices[vertexIndex1];
            Vector3 vertex2 = vertices[vertexIndex2];
            Vector3 vertex3 = vertices[vertexIndex3];

            Vector3 center = (vertex1 + vertex2 + vertex3) / 3;
            subVertices[vertices.Length + i] = center;

            int centerIndex = vertices.Length + i;
            Pertubate(mesh);

            subTriangles[triangles.Length + (9 * i)] = vertexIndex1;
            subTriangles[triangles.Length + (9 * i) + 1] = vertexIndex2;
            subTriangles[triangles.Length + (9 * i) + 2] = centerIndex;

            subTriangles[triangles.Length + (9 * i) + 3] = vertexIndex2;
            subTriangles[triangles.Length + (9 * i) + 4] = vertexIndex3;
            subTriangles[triangles.Length + (9 * i) + 5] = centerIndex;

            subTriangles[triangles.Length + (9 * i) + 6] = vertexIndex3;
            subTriangles[triangles.Length + (9 * i) + 7] = vertexIndex1;
            subTriangles[triangles.Length + (9 * i) + 8] = centerIndex;
        }

        mesh.vertices = subVertices;
        mesh.triangles = subTriangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }

    Mesh Pertubate(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] perturbedVertices = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            HashSet<Vector3> neighbors = GetNeighbors(mesh, vertices[i]);
            float n = neighbors.Count;
            float alpha = ((4 - 2 * Mathf.Cos((2 * Mathf.PI) / n))) / 9;
            Vector3 sum = Vector3.zero;

            foreach (var neighbor in neighbors)
            {
                sum += neighbor;
            }
            //Debug.Log("<color=red>" + sum + "</color>");
            Debug.Log("<color=blue>" + n + "</color>");
            Debug.Log("<color=green>" + alpha + "</color>");
            perturbedVertices[i] = ((1 - alpha) * vertices[i]) + (((alpha) / n) * sum);
        }

        mesh.vertices = perturbedVertices;
        mesh.RecalculateBounds();

        return mesh;
    }


    HashSet<Vector3> GetNeighbors(Mesh mesh, Vector3 vertice)
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



}

