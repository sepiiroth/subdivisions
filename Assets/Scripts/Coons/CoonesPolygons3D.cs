using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoonesPolygons3D
{
    public List<CoonesEdge> edgesList = new List<CoonesEdge>();
    public List<CoonesVertice> verticesList = new List<CoonesVertice>();

    public CoonesPolygons2D polygon2D;

    public CoonesPolygons3D(CoonesPolygons2D polygon2D)
    {
        this.polygon2D = polygon2D;
    }
}
