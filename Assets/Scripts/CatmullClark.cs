using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

struct Centroid
{
    public int id;
    public Vector3 position;
    public int faceIndex;
}
struct EdgePoint
{
    public int id;
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
}

struct NewVertex
{
    public int id;
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

    private int vertexIndex = 0;
    private int x = 0;

    private List<Vector3> newVerticesList = new List<Vector3>();
    // Start is called before the first frame update

    public int nbIter = 1;
    void Start()
    {
        for (x = 0; x < nbIter; x++)
        {
            vertexIndex = 0;
            mesh = cube.GetComponent<MeshFilter>();

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
                //Instantiate(centroidsPrefabs, new Vector3(v.x * cube.transform.lossyScale.x,  v.y * cube.transform.lossyScale.y, v.z * cube.transform.lossyScale.z) + cube.transform.position , Quaternion.Euler(0, 0, 0));
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

        var tempCent = new Centroid();
        tempCent.position = barycentre;
        tempCent.faceIndex = faceIndex;
        tempCent.id = vertexIndex;
        vertexIndex++;
        newVerticesList.Add(barycentre);

        centroids.Add(tempCent);

        /*
        if (x >= 1)
        {
            var go = Instantiate(centroidsPrefabs, new Vector3(barycentre.x * cube.transform.lossyScale.x,  barycentre.y * cube.transform.lossyScale.y, barycentre.z * cube.transform.lossyScale.z) + cube.transform.position , Quaternion.Euler(0, 0, 0));
            go.GetComponent<MeshRenderer>().material.color = Color.blue;
        }*/
        
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

        EdgePoint temp = new EdgePoint();
        temp.position = ep;
        temp.Centroids = new List<Centroid>();
        temp.Centroids.Add(centroids.First(x => x.faceIndex == indexesFaces1));
        temp.Centroids.Add(centroids.First(x => x.faceIndex == indexesFaces2));
        temp.vertices = new List<Vector3>();
        temp.vertices.Add(vecV1);
        temp.vertices.Add(vecV2);
        temp.id = vertexIndex;

        if (edgePoints.Contains(temp))
        {
            return;
        }
        
        vertexIndex++;
        newVerticesList.Add(ep);
        edgePoints.Add(temp);
        /*if (x >= 1)
        {
            var go =Instantiate(centroidsPrefabs, new Vector3(ep.x * cube.transform.lossyScale.x,  ep.y * cube.transform.lossyScale.y, ep.z * cube.transform.lossyScale.z) + cube.transform.position , Quaternion.Euler(0, 0, 0));
            go.GetComponent<MeshRenderer>().material.color = Color.red;
        }*/
        
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

        var tempVP = new NewVertex();
        tempVP.position = vp;
        tempVP.EdgePoints = new List<EdgePoint>();
        tempVP.EdgePoints.AddRange(rEdgePoint);
        tempVP.Centroids = new List<Centroid>();
        tempVP.Centroids.AddRange(cent);
        tempVP.id = this.vertexIndex;

        if (vertexPoints.Contains(tempVP))
        {
            return;
        }
        
        this.vertexIndex++;
        newVerticesList.Add(vp);
        vertexPoints.Add(tempVP);

        /*
        if (x >= 1)
            Instantiate(centroidsPrefabs, new Vector3(vp.x * cube.transform.lossyScale.x,  vp.y * cube.transform.lossyScale.y, vp.z * cube.transform.lossyScale.z) + cube.transform.position , Quaternion.Euler(0, 0, 0));
        */
    }

    void Join()
    {
        /*foreach (var ep in edgePoints)
        {
            foreach (var cent in ep.Centroids)
            {
                var lr1 = Instantiate(linePrefabs, Vector3.zero, Quaternion.Euler(0, 0, 0));
                lr1.GetComponent<LineRenderer>().SetPosition(0,new Vector3(cent.position.x * cube.transform.lossyScale.x,  cent.position.y * cube.transform.lossyScale.y, cent.position.z * cube.transform.lossyScale.z) + cube.transform.position);
                lr1.GetComponent<LineRenderer>().SetPosition(1,new Vector3(ep.position.x * cube.transform.lossyScale.x,  ep.position.y * cube.transform.lossyScale.y, ep.position.z * cube.transform.lossyScale.z) + cube.transform.position);
            }
        }
        
        foreach (var vp in vertexPoints)
        {
            foreach (var ep in vp.EdgePoints)
            {
                var lr1 = Instantiate(linePrefabs, Vector3.zero, Quaternion.Euler(0, 0, 0));
                lr1.GetComponent<LineRenderer>().SetPosition(0,new Vector3(vp.position.x * cube.transform.lossyScale.x,  vp.position.y * cube.transform.lossyScale.y, vp.position.z * cube.transform.lossyScale.z) + cube.transform.position);
                lr1.GetComponent<LineRenderer>().SetPosition(1,new Vector3(ep.position.x * cube.transform.lossyScale.x,  ep.position.y * cube.transform.lossyScale.y, ep.position.z * cube.transform.lossyScale.z) + cube.transform.position);
            }
        }*/
        
        List<int> triangles = new List<int>();

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
                
                /*var lr1 = Instantiate(linePrefabs, Vector3.zero, Quaternion.Euler(0, 0, 0));
                lr1.GetComponent<LineRenderer>().SetPosition(0,new Vector3(vp.position.x * cube.transform.lossyScale.x,  vp.position.y * cube.transform.lossyScale.y, vp.position.z * cube.transform.lossyScale.z) + cube.transform.position);
                lr1.GetComponent<LineRenderer>().SetPosition(1,new Vector3(c.position.x * cube.transform.lossyScale.x,  c.position.y * cube.transform.lossyScale.y, c.position.z * cube.transform.lossyScale.z) + cube.transform.position);

                lr1 = Instantiate(linePrefabs, Vector3.zero, Quaternion.Euler(0, 0, 0));
                lr1.GetComponent<LineRenderer>().SetPosition(0,new Vector3(tempList[0].position.x * cube.transform.lossyScale.x,  tempList[0].position.y * cube.transform.lossyScale.y, tempList[0].position.z * cube.transform.lossyScale.z) + cube.transform.position);
                lr1.GetComponent<LineRenderer>().SetPosition(1,new Vector3(c.position.x * cube.transform.lossyScale.x,  c.position.y * cube.transform.lossyScale.y, c.position.z * cube.transform.lossyScale.z) + cube.transform.position);

                lr1 = Instantiate(linePrefabs, Vector3.zero, Quaternion.Euler(0, 0, 0));
                lr1.GetComponent<LineRenderer>().SetPosition(0,new Vector3(tempList[1].position.x * cube.transform.lossyScale.x,  tempList[1].position.y * cube.transform.lossyScale.y, tempList[1].position.z * cube.transform.lossyScale.z) + cube.transform.position);
                lr1.GetComponent<LineRenderer>().SetPosition(1,new Vector3(c.position.x * cube.transform.lossyScale.x,  c.position.y * cube.transform.lossyScale.y, c.position.z * cube.transform.lossyScale.z) + cube.transform.position);

                lr1 = Instantiate(linePrefabs, Vector3.zero, Quaternion.Euler(0, 0, 0));
                lr1.GetComponent<LineRenderer>().SetPosition(0,new Vector3(tempList[0].position.x * cube.transform.lossyScale.x,  tempList[0].position.y * cube.transform.lossyScale.y, tempList[0].position.z * cube.transform.lossyScale.z) + cube.transform.position);
                lr1.GetComponent<LineRenderer>().SetPosition(1,new Vector3(vp.position.x * cube.transform.lossyScale.x,  vp.position.y * cube.transform.lossyScale.y, vp.position.z * cube.transform.lossyScale.z) + cube.transform.position);

                lr1 = Instantiate(linePrefabs, Vector3.zero, Quaternion.Euler(0, 0, 0));
                lr1.GetComponent<LineRenderer>().SetPosition(0,new Vector3(tempList[1].position.x * cube.transform.lossyScale.x,  tempList[1].position.y * cube.transform.lossyScale.y, tempList[1].position.z * cube.transform.lossyScale.z) + cube.transform.position);
                lr1.GetComponent<LineRenderer>().SetPosition(1,new Vector3(vp.position.x * cube.transform.lossyScale.x,  vp.position.y * cube.transform.lossyScale.y, vp.position.z * cube.transform.lossyScale.z) + cube.transform.position);
                */

                //var firstTriangle = new List<Vector3>();
                
                triangles.Add(vp.id);
                triangles.Add(c.id);
                triangles.Add(tempList[0].id);

                //Vector3 bc1 = getBarycentreVector3(firstTriangle);

                //firstTriangle.Sort((x,y) => Vector3.Angle((bc1 - firstTriangle[0]), (bc1 - x)) <= Vector3.Angle((bc1 - firstTriangle[0]), (bc1 - y)) ? 1 : 0 );
                
                /*triangles.Add(vp.id);
                triangles.Add(tempList[0].id);
                triangles.Add(c.id);*/
                
                triangles.Add(vp.id);
                triangles.Add(tempList[1].id);
                triangles.Add(c.id);
                
                /*triangles.Add(vp.id);
                triangles.Add(tempList[1].id);
                triangles.Add(c.id);*/

            }
            
            
        }
        
        /*foreach (var vp in vertexPoints)
        {
            foreach (var ep in vp.EdgePoints)
            {
                var tempCent = vp.Centroids.First(x => ep.Centroids.Contains(x));
                triangles.Add(index);
                index++;
                triangles.Add(index);
                index++;
                triangles.Add(index);
                index++;
                vertex.Add(vp.position);
                vertex.Add(ep.position);
                vertex.Add(tempCent.position);
            }
        }*/
        
        Debug.Log(newVerticesList.Count);

        mesh.mesh.vertices = newVerticesList.ToArray();
        mesh.mesh.triangles = triangles.ToArray();
        
        mesh.mesh.RecalculateBounds();
        mesh.mesh.RecalculateNormals();
    }

    Vector3 getBarycentreVector3(List<Vector3> vectors)
    {
        var res = Vector3.zero;
        vectors.ForEach(x => res += x);

        return res / (float)vectors.Count;
    }
}
