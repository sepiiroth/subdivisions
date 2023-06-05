using System.Collections.Generic;
using System.Numerics;

namespace Structure
{
    public struct Vertex
    {
        public Vector3 vec3;
    }

    public struct Edge
    {
        public List<Vertex> points;
        public List<Polygon> Polygons;
    }

    public struct Polygon
    {
        public List<Vertex> Vertices;
        public List<Edge> Edges;
    }
}