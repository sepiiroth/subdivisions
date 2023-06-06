using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoonesPolygons2D
{
    public List<CoonesEdge> edgesList = new List<CoonesEdge>();
    public List<CoonesVertice> verticesList = new List<CoonesVertice>();
    
    public CoonesPolygons2D(List<CoonesVertice> listVertices)
    {
        verticesList = listVertices;        
    }
}
