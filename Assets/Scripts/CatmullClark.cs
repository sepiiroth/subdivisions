using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Centroid
{
    public Vector3 position;
    public int faceIndex;

    public Centroid(Vector3 position, int faceIndex)
    {
        this.position = position;
        this.faceIndex = faceIndex;
    }
}
class EdgePoint
{
    public Vector3 position;
    public List<Centroid> Centroids;
    public List<Vector3> vertices;

    public override bool Equals(object obj)
    {
        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        EdgePoint compare = (EdgePoint) obj;

        return compare.position == this.position;
    }

    public EdgePoint(Vector3 position)
    {
        this.position = position;
        this.vertices = new List<Vector3>();
        this.Centroids = new List<Centroid>();
    }
}

class NewVertex
{
    public Vector3 position;
    public List<EdgePoint> EdgePoints;
    public List<Centroid> Centroids;
    
    public override bool Equals(object obj)
    {
        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        NewVertex compare = (NewVertex) obj;

        return compare.position == this.position;
    }
    
    public NewVertex(Vector3 position)
    {
        this.position = position;
        this.EdgePoints = new List<EdgePoint>();
        this.Centroids = new List<Centroid>();
    }
}

public class CatmullClark : MonoBehaviour
{
    public GameObject cube;
    private MeshFilter mesh;

    public GameObject centroidsPrefabs;
    public GameObject linePrefabs;
    
    private List<List<int>> faces;
    private List<Vector3> vertices;

    private List<Centroid> centroids;
    private List<EdgePoint> edgePoints;
    private List<NewVertex> vertexPoints;

    private int x = 0;

    private Vector3 barycentreMesh;

    // Start is called before the first frame update

    public int nbIter = 1;
    void Start()
    {
        for (x = 0; x < nbIter; x++)
        {
            mesh = cube.GetComponent<MeshFilter>();
            barycentreMesh = GetBarycentreMesh();

            centroids = new List<Centroid>();
            edgePoints = new List<EdgePoint>();
            vertexPoints = new List<NewVertex>();
        
            faces = new List<List<int>>();
            for (int i = 0; i < mesh.mesh.triangles.Length/3; i++)
            {
                var temp = new List<int>();
                for (int j = 0; j < 3; j++)
                {
                    temp.Add(mesh.mesh.triangles[j + i * 3]);
                }
                faces.Add(temp);
            }
        
            vertices = new List<Vector3>(mesh.mesh.vertices);

            for (int i = 0; i < faces.Count; i++)
            {
                ComputeCentroids(i);
            }

            foreach (var f in faces)
            {
                for (int i = 0; i < 3; i++)
                {
                    ComputeEdgePoints(f[i], f[(i +1)%3]);
                }
            }
        
            foreach (var f in faces)
            {
                for (int i = 0; i < 3; i++)
                {
                    ComputeVertexPoints(f[i]);
                }
            }
        
            Join();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ComputeCentroids(int faceIndex)
    {
        var face = faces[faceIndex];
        var barycentre = Vector3.zero;
        foreach (var vi in face)
        {
            barycentre += vertices[vi];
        }

        barycentre = barycentre / face.Count;

        var tempCent = new Centroid(barycentre, faceIndex);
        centroids.Add(tempCent);
        
        
    }

    void ComputeEdgePoints(int v1, int v2)
    {
        var vecV1 = vertices[v1];
        var vecV2 = vertices[v2];

        var facesAdj = faces.Where(x => x.Any(y => vertices[y] == vecV1) && x.Any(y => vertices[y] == vecV2)).ToList();

        var indexesFaces1 = faces.IndexOf(facesAdj[0]);
        var indexesFaces2 = faces.IndexOf(facesAdj[1]);

        var ep = vecV1 + vecV2 + centroids.First(x => x.faceIndex == indexesFaces1).position + centroids.First(x => x.faceIndex == indexesFaces2).position;
        ep /= 4;

        EdgePoint temp = new EdgePoint(ep);
        temp.Centroids.Add(centroids.First(x => x.faceIndex == indexesFaces1));
        temp.Centroids.Add(centroids.First(x => x.faceIndex == indexesFaces2));
        temp.vertices.Add(vecV1);
        temp.vertices.Add(vecV2);

        if (edgePoints.Contains(temp))
        {
            return;
        }
        
        edgePoints.Add(temp);

    }

    void ComputeVertexPoints(int vertexIndex)
    {
        var vecV = vertices[vertexIndex];

        var facesIndex = faces.Select((x, i) => x.Any(y => vertices[y] == vecV) ? i : -1).Where(i => i >= 0).ToList();

        var cent = centroids.Where(k => facesIndex.Contains(k.faceIndex)).ToList();
        var Q = Vector3.zero;

        foreach (var c in cent)
        {
            Q += c.position;
        }

        Q /= cent.Count;

        var R = Vector3.zero;
        var rEdgePoint = edgePoints.Where(x => x.vertices.Contains(vecV)).ToList();
        
        foreach (var rEP in rEdgePoint)
        {
            R += rEP.position;
        }

        R /= rEdgePoint.Count;
        
        var n = rEdgePoint.Count;

        var vp = (1f / n) * Q + (2f / n) * R + ((n - 3f) / n) * vecV;

        var tempVP = new NewVertex(vp);
        tempVP.EdgePoints.AddRange(rEdgePoint);
        tempVP.Centroids.AddRange(cent);

        if (vertexPoints.Contains(tempVP))
        {
            return;
        }
        
        vertexPoints.Add(tempVP);
        
    }

    void Join()
    {

        List<int> triangles = new List<int>();
        List<Vector3> vectors = new List<Vector3>();

        var index = 0;

        foreach (var c in centroids)
        {
            var ep = edgePoints.Where(x => x.Centroids.Contains(c)).ToList();

            var v = new List<Vector3>();
            
            ep.ForEach(x => v.AddRange(x.vertices));
            v = v.Distinct().ToList();

            foreach (var tempV in v)
            {
                var tempList = ep.Where(x => x.vertices.Contains(tempV)).ToList();

                var vp = vertexPoints.First(x => x.EdgePoints.Contains(tempList[0]) && x.EdgePoints.Contains(tempList[1]));
                
                triangles.Add(index);
                index++;
                triangles.Add(index);
                index++;
                triangles.Add(index);
                index++;
                
                var firstTriangle = new List<Vector3>();
                
                firstTriangle.Add(vp.position);
                firstTriangle.Add(c.position);
                firstTriangle.Add(tempList[0].position);

                var cross1 = Vector3.Cross((firstTriangle[1] - firstTriangle[0]),
                    (firstTriangle[2] - firstTriangle[0]));

                if (Vector3.Dot((barycentreMesh - firstTriangle[0]), cross1) > 0)
                {
                    (firstTriangle[1], firstTriangle[2]) = (firstTriangle[2], firstTriangle[1]);
                }
                
                
                vectors.AddRange(firstTriangle);

                triangles.Add(index);
                index++;
                triangles.Add(index);
                index++;
                triangles.Add(index);
                index++;
                
                var secondTriangle = new List<Vector3>();
                
                secondTriangle.Add(vp.position);
                secondTriangle.Add(c.position);
                secondTriangle.Add(tempList[1].position);

                var cross2 = Vector3.Cross((secondTriangle[1] - secondTriangle[0]),
                    (secondTriangle[2] - secondTriangle[0]));

                if (Vector3.Dot((barycentreMesh - secondTriangle[0]), cross2) > 0)
                {
                    (secondTriangle[1], secondTriangle[2]) = (secondTriangle[2], secondTriangle[1]);
                }
                
                vectors.AddRange(secondTriangle);
            }
            
            
        }

        mesh.mesh.vertices = vectors.ToArray();
        mesh.mesh.triangles = triangles.ToArray();
        
        mesh.mesh.RecalculateBounds();
        mesh.mesh.RecalculateNormals();
    }

    Vector3 GetBarycentreMesh()
    {
        var res = Vector3.zero;
        foreach (var v in mesh.mesh.vertices)
        {
            res += v;
        }

        res /= mesh.mesh.vertices.Length;
        return res;
    }
}
