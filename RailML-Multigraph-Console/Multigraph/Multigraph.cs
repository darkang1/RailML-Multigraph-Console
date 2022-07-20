using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RailML_Multigraph_Console
{
    public class Multigraph
    {
        // Vertices
        [DataMember]
        public Dictionary<string, Vertex> Vertices { get; protected set; } = new Dictionary<string, Vertex>();
        // Edges
        [DataMember]
        public Dictionary<string, Edge> Edges { get; private set; } = new Dictionary<string, Edge>();
        // Layers
        [DataMember]
        public Dictionary<string, Layer> Layers { get; protected set; } = new Dictionary<string, Layer>();
        // Coordinate Elements
        public Dictionary<string, SpotElementProjection> SpotElements { get; private set; } = new Dictionary<string, SpotElementProjection>();
        public Dictionary<string, LinearElementProjection> LinearElements { get; private set; } = new Dictionary<string, LinearElementProjection>();


        #region Vertex Functions
        public void AddElements(HashSet<ParsedElement> vertices)
        {
            if (vertices != null)
            {
                foreach (ParsedElement v in vertices)
                {
                    // [M_TrackSection]
                    if (v is NetElement)
                    {
                        Vertices.TryAdd(v.GetID(), new M_TrackSection(v));
                    }
                    // [M_BaliseGroup]
                    else if (v is Balise)
                    {
                        Vertices.TryAdd(v.GetID(), new M_BaliseGroup(v));
                    }
                    // [M_Point]
                    else if (v is SwitchIS)
                    {
                        Vertices.TryAdd(v.GetID(), new M_Point(v));
                    }
                    // [M_Track]
                    else if (v is Track)
                    {
                        Vertices.TryAdd(v.GetID(), new M_Track(v));
                    }
                    else if (v is SpotElementProjection)
                    {
                        var t = (SpotElementProjection)v;
                        SpotElements.TryAdd(t.RefersToElement, (SpotElementProjection)v);
                    }
                    else if (v is LinearElementProjection)
                    {
                        var t = (LinearElementProjection)v;
                        LinearElements.TryAdd(t.RefersToElement, (LinearElementProjection)v);
                    }
                }

            }
        }

        public bool RemoveVertex(string id)
        {
            return Vertices.Remove(id);
        }

        public void AutoAssignElementsCoordinates()
        {
            if (Vertices != null)
            {
                foreach (Vertex v in Vertices.Values ?? Enumerable.Empty<Vertex>())
                {
                    // For TrackSections (NetElements)
                    // Here is differnet proccess since NetElements already contain their coordiantes
                    // Hopefully, they are always the same as the ones in LinearElementProjection (At least they are by observations)
                    if (v is M_TrackSection)
                    {
                        M_TrackSection ts = (M_TrackSection)v;
                        foreach (IntrisicCoordinate coordinate in ts.ThisNetElement.AssociatedPositioningSystem.IntrinsicCoordinates ?? Enumerable.Empty<IntrisicCoordinate>())
                        {
                            v.Coordinates.Add(new Coordinate(coordinate.GeometricCoordinate.X, coordinate.GeometricCoordinate.Y));
                        }
                    }
                    // For Balises
                    else if (v is M_BaliseGroup)
                    {
                        if (SpotElements.TryGetValue(v.ID, out SpotElementProjection sep))
                        {
                            v.Coordinates.Add(sep.Coordinate);
                        }
                    }
                    // For SwitchesIS
                    else if (v is M_Point)
                    {
                        if (SpotElements.TryGetValue(v.ID, out SpotElementProjection sep))
                        {
                            v.Coordinates.Add(sep.Coordinate);
                        }
                    }
                    // For Tracks
                    // Here is different procces since LinerElementProjection doesn't hold any information regarding Track
                    // To be implemented later
                }

                Console.WriteLine("Coordinates assignment process finished!");
            }
        }

        public bool UpdateAnyVertexName(string id, string newName)
        {
            if (Vertices != null && Vertices.TryGetValue(id, out Vertex v))
            {
                return v.UpdateName(newName);
            }
            return false;
        }

        public bool UpdateAnyVertexColor(string id, VertexColors newColor)
        {
            if (Vertices != null && Vertices.TryGetValue(id, out Vertex v))
            {
                return v.UpdateColor(newColor);
            }
            return false;
        }

        public bool UpdateAnyVertexSpotCoordinate(string id, double newX, double newY)
        {
            if (Vertices != null && Vertices.TryGetValue(id, out Vertex v))
            {
                return v.UpdateSpotCoordinates(newX, newY);
            }
            return false;
        }

        #endregion

        #region Edge Functions
        public bool AddEdge(Vertex from, Vertex to, EdgeType type = EdgeType.Unassigned, bool isDirected = true)
        {
            Edge ne = new Edge(from, to, type, isDirected);
            return Edges.TryAdd(ne.ID, ne);
        }

        public bool RemoveEdge(string id)
        {
            return Edges.Remove(id);
        }

        public void AssignEdgesForAllVertices()
        {
            foreach (Vertex v in Vertices.Values ?? Enumerable.Empty<Vertex>())
            {
                AutoAssignEdge(v);
            }
        }

        public bool AutoAssignEdge(Vertex v)
        {
            if (v != null)
            {
                // [NetRelation]
                if (v is M_TrackSection)
                {
                    M_TrackSection t = (M_TrackSection)v;
                    if (t.ThisNetElement != null)
                    {
                        foreach (Relation relation in t.ThisNetElement.Relations ?? Enumerable.Empty<Relation>())
                        {
                            if (relation.ActualNetRelationRef != null)
                            {
                                if (Vertices.TryGetValue(relation.ActualNetRelationRef.ElementB.ActualNetElementRef.ID, out Vertex m_ts))
                                {
                                    //var converted_m_ts = (M_TrackSection)m_ts;
                                    if (!m_ts.ID.Equals(t.ThisNetElement.ID))
                                    {
                                        Edge ne = new Edge(t, m_ts, type: EdgeType.AdjacentTrackSection);
                                        Edges.TryAdd(ne.ID, ne);

                                    }
                                }
                            }
                        }
                        return true;
                    }

                }
                else if (v is M_BaliseGroup)
                {
                    M_BaliseGroup b = (M_BaliseGroup)v;
                    if (b.ThisBalise != null)
                    {
                        if (b.ThisBalise.SpotLocation.ActualNetElementRef != null)
                        {
                            if (Vertices.TryGetValue(b.ThisBalise.SpotLocation.ActualNetElementRef.ID, out Vertex m_ts))
                            {
                                if (!m_ts.ID.Equals(b.ThisBalise.ID))
                                {
                                    Edge ne = new Edge(b, m_ts, type: EdgeType.AssignmentToATrackSection);
                                    if (Edges.TryAdd(ne.ID, ne))
                                        return true;
                                    else
                                        return false;
                                }
                                else
                                    return false;
                            }
                        }
                    }
                }
                // [SwitchIS]
                else if (v is M_Point)
                {
                    M_Point p = (M_Point)v;
                    if (p.ThisSwitchIS != null)
                    {
                        if (p.ThisSwitchIS.SpotLocation.ActualNetElementRef != null)
                        {
                            if (Vertices.TryGetValue(p.ThisSwitchIS.SpotLocation.ActualNetElementRef.ID, out Vertex m_ts))
                            {
                                if (!m_ts.ID.Equals(p.ThisSwitchIS.ID))
                                {
                                    Edge ne = new Edge(p, m_ts, type: EdgeType.AssignmentToATrackSection);
                                    if (Edges.TryAdd(ne.ID, ne))
                                        return true;
                                    else
                                        return false;
                                }
                                else
                                    return false;

                            }
                        }
                    }
                }
                // Later needs to be implemented Track association with OperationalPosts, Lines
                else if (v is M_Track)
                {
                    M_Track t = (M_Track)v;
                    if (t.ThisTrack != null)
                    {
                        if (t.ThisTrack.LinearLocation.AssociatedNetElement.ActualNetElementRef != null)
                        {
                            if (Vertices.TryGetValue(t.ThisTrack.LinearLocation.AssociatedNetElement.ActualNetElementRef.ID, out Vertex m_ts))
                            {
                                if (!m_ts.ID.Equals(t.ThisTrack.ID))
                                {
                                    Edge ne = new Edge(t, m_ts, type: EdgeType.AssignmentToAMacroElement);
                                    if (Edges.TryAdd(ne.ID, ne))
                                        return true;
                                    else
                                        return false;
                                }
                                else
                                    return false;
                            }
                        }
                    }
                }
                else
                    throw new InvalidCastException();

            }
            return false;
        }

        #endregion

        #region Tasks Functions

        // UNFINISHED
        public void Task1_FindAllTrainPaths(string startNodeID, string finalNodeID)
        {
            // Here we are going to be working only with vertices of type M_TrackSection,
            // since they represent train path
            if (Vertices == null)
                return;

            bool[] isVisited = new bool[Vertices.Count];
            List<Edge> pathList = new List<Edge>();

            if (Vertices.ContainsKey(startNodeID))
            {

            }

        }

        #endregion

        #region Layers
        public bool AutoGenerateBasicLayers()
        {
            //foreach(M_TrackSection netElem in TrackSections.Values ?? Enumerable.Empty<M_TrackSection>())
            //{

            //}
            return true;
        }

        // Creates new empty layer
        public bool CreateLayer(string layerName)
        {
            Layer layer = new Layer(layerName);
            return Layers.TryAdd(layerName, layer);
        }

        // Creates new layer with vertices
        public bool CreateLayer(string layerName, Dictionary<string, Vertex> vertices)
        {
            Layer layer = new Layer(layerName, vertices);
            return Layers.TryAdd(layerName, layer);
        }

        #endregion

        #region Printing Functions
        public void DisplayAllVertices()
        {
            Console.WriteLine("\n\t======Mulitgraph Vertices======");

            // Initializing a dictionary with all possible vertex types
            Dictionary<Type, string> vertexType = new Dictionary<Type, string>(){ 
                {typeof(M_TrackSection), "TrackSection"},
                {typeof(M_BaliseGroup), "Balise" },
                {typeof(M_Point), "SwitchIS" },
                {typeof(M_Track), "Track" }
            };

            if (Vertices != null && Vertices.Count > 0)
            {
                for (int i = 0; i < Vertices.Count; i++)
                {
                    if (Vertices.Values.ElementAt(i) != null)
                    {
                        Type vType = Vertices.Values.ElementAt(i).GetType();

                        Console.WriteLine("|-------------------------------|");
                        Console.WriteLine($"[{i + 1}] ID: {Vertices.Values.ElementAt(i).ID}");
                        Console.WriteLine($"Name: {Vertices.Values.ElementAt(i).Name}");
                        Console.WriteLine($"Color: {Vertices.Values.ElementAt(i).Color}");
                        Console.Write($"Coordinates: ");
                        foreach (Coordinate c in Vertices.Values.ElementAt(i).Coordinates ?? Enumerable.Empty<Coordinate>())
                        {
                            Console.Write($"({c.X}, {c.Y}) ");
                        }

                        Console.Write($"\nType: ");
                        if (vertexType.TryGetValue(vType, out var typeName))
                            Console.Write(typeName);

                        Console.WriteLine();
                        Console.WriteLine("|-------------------------------|\n");
                    }
                }
            }
            else
                Console.WriteLine("Vertices list is empty!\n");
        }
        
        public void DisplayAllEdges()
        {
            Console.WriteLine("\n\t======Mulitgraph Edges======");
            if (Edges != null && Edges.Count > 0)
            {
                Console.WriteLine("|-------------------------------|");
                foreach (Edge edge in Edges.Values ?? Enumerable.Empty<Edge>())
                {
                    Console.WriteLine($"[<{edge.FromVertex.ID}, {edge.ToVertex.ID}>: {edge.Type}]");
                }
                Console.WriteLine("|-------------------------------|\n");
            }
            else
                Console.WriteLine("Edge list is empty!\n");
        }

        #endregion
    }
}
