﻿using System;
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

        // Experimental adjacency list (edges)
        // Key is ID of each vertex from Vertices dictionary
        // Value is Dictionary of related edges to a specific vertex
        [DataMember]
        public Dictionary<string, Dictionary<string, Edge>> AdjList { get; private set; } = new Dictionary<string, Dictionary<string, Edge>>();

        // Layers
        [DataMember]
        public Dictionary<string, Layer> Layers { get; protected set; } = new Dictionary<string, Layer>();

        // Relations, Coordinate Elements, and all possible Travel Paths
        public Dictionary<string, NetRelation> NetRelations { get; private set; } = new Dictionary<string, NetRelation>();
        public Dictionary<string, SpotElementProjection> SpotElements { get; private set; } = new Dictionary<string, SpotElementProjection>();
        public Dictionary<string, LinearElementProjection> LinearElements { get; private set; } = new Dictionary<string, LinearElementProjection>();
        public Dictionary<string, HashSet<Vertex>> TravelPaths { get; private set; } = new Dictionary<string, HashSet<Vertex>>();

        public bool CanCreateNonExistingLayerOnVertexAdd { get; private set; } = false;


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
                        AdjList.TryAdd(v.GetID(), new Dictionary<string, Edge>());
                    }
                    // [M_BaliseGroup]
                    else if (v is Balise)
                    {
                        Vertices.TryAdd(v.GetID(), new M_BaliseGroup(v));
                        AdjList.TryAdd(v.GetID(), new Dictionary<string, Edge>());
                    }
                    // [M_Point]
                    else if (v is SwitchIS)
                    {
                        Vertices.TryAdd(v.GetID(), new M_Point(v));
                        AdjList.TryAdd(v.GetID(), new Dictionary<string, Edge>());
                    }
                    // [M_Track]
                    else if (v is Track)
                    {
                        Vertices.TryAdd(v.GetID(), new M_Track(v));
                        AdjList.TryAdd(v.GetID(), new Dictionary<string, Edge>());
                    }
                    else if (v is NetRelation)
                    {
                        var t = (NetRelation)v;
                        NetRelations.TryAdd(t.ID, t);
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
            RemoveVertexFromAdjList(id);
            RemoveVertexFromEdgeList(id);
            RemoveVertexFromLayers(id);
            return Vertices.Remove(id);
        }

        public void RemoveVertexFromLayers(string id)
        {
            if(Vertices.TryGetValue(id, out Vertex v))
            {
                // Looking inside each layer Vertex is related to and from that Layer reference remove Vertex from Layer
                foreach(Layer l in v.Layers.Values)
                {
                    if (Layers.TryGetValue(l.Name, out Layer layer))
                    {
                        // Removing vertex from Layer it is related to
                        layer.Vertices.Remove(id);
                        // Removing Layer reference inside Vertex, just in case
                        v.RemoveLayerReference(l);
                    }
                }
            }
        }

        public void RemoveVertexFromEdgeList(string id)
        {
            foreach (Edge e in Edges.Values)
            {
                if (e.FromVertex.ID == id || e.ToVertex.ID == id)
                {
                    Edges.Remove(e.ID);
                }
            }
        }

        public void RemoveVertexFromAdjList(string vertexId)
        {
            Dictionary<string, Edge> adjListValues;
            if (AdjList.TryGetValue(vertexId, out adjListValues))
            {
                // Removing the same edges from related vertices adjacency lists
                foreach(var edge in adjListValues.Values)
                {
                    if (vertexId == edge.FromVertex.ID)
                        RemoveEdge(edge.ToVertex.ID, edge.ID);
                    else if (vertexId == edge.ToVertex.ID)
                        RemoveEdge(edge.FromVertex.ID, edge.ID);
                    else
                        throw new InvalidOperationException("This vertex contains an Edge which is not related to it!");                    
                }
                AdjList.Remove(vertexId);
            }               
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

        public bool AddEdge(Edge ne)
        {
            AddEdgeToAdjList(ne);
            return Edges.TryAdd(ne.ID, ne);
        }

        public bool AddEdge(Vertex from, Vertex to, EdgeType type = EdgeType.Unassigned, bool isDirected = true)
        {
            Edge ne = new Edge(from, to, type, isDirected);
            return AddEdge(ne);
        }

        public void AddEdgeToAdjList(Edge ne)
        {
            Dictionary<string, Edge> adjListValues;

            // If Edge is directed, adding it only to "FromVertex" adjacency list
            if (AdjList.TryGetValue(ne.FromVertex.ID, out adjListValues))
            {
                adjListValues.TryAdd(ne.ID, ne);
            }
            else
            {
                AdjList.Add(ne.FromVertex.ID, new Dictionary<string, Edge>());
            }
            // If Edge is NOT directed, then also adding it to "ToVertex" adjacency lists
            if (!ne.IsDirected)
            {
                if (AdjList.TryGetValue(ne.ToVertex.ID, out adjListValues))
                {
                    adjListValues.TryAdd(ne.ID, ne);
                }
                else
                {
                    AdjList.Add(ne.ToVertex.ID, new Dictionary<string, Edge>());
                }
            }
        }

        public bool RemoveEdge(string edgeID)
        {
            return Edges.Remove(edgeID);
        }

        public bool RemoveEdge(string vertexId, string edgeID)
        {
            RemoveEdgeFromAdjList(vertexId, edgeID);
            return Edges.Remove(edgeID);
        }

        public void RemoveEdgeFromAdjList(string vertexId, string edgeID)
        {
            Dictionary<string, Edge> adjListValues;

            // If Edge is directed, removeing it only to "FromVertex" adjacency list
            if (AdjList.TryGetValue(vertexId, out adjListValues))
            {
                Edge edgeToDelete;
                if (adjListValues.TryGetValue(edgeID, out edgeToDelete))
                {
                    // If Edge is NOT directed, then also removing it to "ToVertex" adjacency lists
                    if (!edgeToDelete.IsDirected)
                    {
                        Dictionary<string, Edge> adjListValues2;
                        if (AdjList.TryGetValue(edgeToDelete.ToVertex.ID, out adjListValues2))
                        {
                            // Removing Edge from the ToVertex AdjList
                            adjListValues2.Remove(edgeID);
                        }
                    }
                    // Removing Edge from the FromVertex AdjList
                    adjListValues.Remove(edgeID);
                }
            }
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
                                        Edge ne = new Edge(t, m_ts, type: EdgeType.AdjacentTrackSection, isDirected: false);
                                        AddEdge(ne);
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
                                    Edge ne = new Edge(b, m_ts, type: EdgeType.AssignmentToATrackSection, isDirected: false);
                                    return AddEdge(ne);
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
                                    Edge ne = new Edge(p, m_ts, type: EdgeType.AssignmentToATrackSection, isDirected: false);
                                    return AddEdge(ne);
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
                                    Edge ne = new Edge(t, m_ts, type: EdgeType.AssignmentToAMacroElement, isDirected: false);
                                    return AddEdge(ne);
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

        public void Task1_FindAllTrainPaths(string startNodeID, string finalNodeID)
        {
            // Here we are going to be working only with vertices of type M_TrackSection and its included NetRelations,
            // since M_TrackSection represents the train path and its included NetRelation contains navigability of those sections
            if (Vertices == null)
                return;

            // Hashset of all visited nodes
            HashSet<Vertex> visited = new HashSet<Vertex>();
            // Hashset to keep track of the vertices along the path
            HashSet<Vertex> pathList = new HashSet<Vertex>();

            // Simple check to verify if the starting node with provided ID exists
            Vertex startNode;
            if(Vertices.TryGetValue(startNodeID, out startNode))
            {
                // Adding start node to the pathlist
                pathList.Add(startNode);
                // Calling recursive funciton to find all possible paths
                Task1_FindAllTrainPathsUtil(startNodeID, finalNodeID, visited, pathList);
            }
            //else
                //throw new ArgumentException("Invalid Vertex ID was passsed to train path traversing function!");         
        }

        public void Task1_FindAllTrainPathsUtil(string currNodeID, string finalNodeID, HashSet<Vertex> visited, HashSet<Vertex> pathList)
        {
            // If we reached the final node, print the results
            if (currNodeID.Equals(finalNodeID))
            {
                Task1_PrintResults(pathList);
                AddTravelPath(new HashSet<Vertex>(pathList, pathList.Comparer));
                return;
            }
         
            Vertex currNode;
            if(!Vertices.TryGetValue(currNodeID, out currNode))
                throw new ArgumentException("Invalid Vertex ID was passsed to train path traversing function!");

            // Marking current vertex as "visited"
            visited.Add(currNode);

            Dictionary<string, Edge> currNodeAdjList;
            if (!AdjList.TryGetValue(currNodeID, out currNodeAdjList))
                throw new ArgumentException("Invalid Vertex ID was passsed to train path traversing function!");

            // Recur for all vertices adjacent to the current one
            foreach (var edge in currNodeAdjList.Values)
            {
                // Checking if both Vertices from Edge object are TrackSections, since they represent railway sections
                bool areBothVerticesTrackSections = edge.FromVertex is M_TrackSection && edge.ToVertex is M_TrackSection;
                // Depending on at which possition in Edge object is our current Vertex (FromVertex or ToVertex), setting the next vertex object to parse accordingly
                Vertex nextVertex = (currNodeID == edge.FromVertex.ID) ? edge.ToVertex : ((currNodeID == edge.ToVertex.ID) ? edge.FromVertex : edge.ToVertex);

                if (areBothVerticesTrackSections)
                {
                    if (!visited.Contains(nextVertex))
                    {
                        // Extracting NetElement from M_TrackSection to get its NetRelations
                        // for proper handling of the Tracksection directions
                        M_TrackSection ts = (M_TrackSection)edge.FromVertex;
                        NetElement refNetElem = ts.ThisNetElement;

                        // Looking at each Relation of the current node to find related to our current traversing case
                        foreach (var rel in refNetElem.Relations)
                        {
                            NetRelation netRel = rel.ActualNetRelationRef;

                            // Verifying whether it's proper NetRelation by the following parameters:
                            bool isFromVertexAndElementAEqual = netRel.ElementA.ElementRef.Equals(edge.FromVertex.ID);
                            bool isToVertexAndElementBEqual = netRel.ElementB.ElementRef.Equals(edge.ToVertex.ID);
                            bool isNavigabilityEnabled = !netRel.Navigability.Equals("None"); // !!! Later this check needs to be modified, since there can be "AB" and "BA" cases which should be handled differently

                            // If both vertices correlate with NetRelation vertices in proper order and Navigability parameter is not disbaled
                            // Navigability parameter shows whether we can perform "train transition" on the current NetRelation of two different TrackSections
                            if (isFromVertexAndElementAEqual && isToVertexAndElementBEqual && isNavigabilityEnabled)
                            {
                                // Adding ToVertex to our pathlist
                                pathList.Add(nextVertex);
                                // Recursivly call function to find all possible paths from ToVertex
                                Task1_FindAllTrainPathsUtil(nextVertex.ID, finalNodeID, visited, pathList);
                                // Removing current node from the pathlist
                                pathList.Remove(nextVertex);
                            }
                        }
                    }
                }
            }
            visited.Remove(currNode);
        }

        public Dictionary<string, double> Task2_FindLengthOfAllTravelPaths()
        {
            if(TravelPaths.Count == 0)
                Console.WriteLine("TravelPaths list is empty!");

            // Represents total path length for a given path ID
            Dictionary<string, double> pathsWithLength = new Dictionary<string, double>();

            // Iterating through each TravelPath
            foreach (var path in TravelPaths)
            {
                // Storing lengthes of all vertices that belongs to the current path
                Dictionary<string, double> verticesWithLength = new Dictionary<string, double>();
                foreach (var vertex in path.Value)
                {
                    // Verifying that we have only M_TrackSection objects
                    if (vertex is not M_TrackSection)
                        throw new ArgumentException("Invalid vertices were detected in TravelPaths list!");

                    // Extracting NetElement from current TrackSection vertex to get its GeometricCoordinates
                    NetElement currNetElem = (vertex as M_TrackSection).ThisNetElement;
                    Dictionary<string, GeometricCoordinate> gpsCoordinates = new Dictionary<string, GeometricCoordinate>();
                    foreach (var intrisicCoordinate in currNetElem.AssociatedPositioningSystem.IntrinsicCoordinates)
                        gpsCoordinates.TryAdd(intrisicCoordinate.ID, intrisicCoordinate.GeometricCoordinate);

                    // Calculating length between two geometric points and adding them all up to get total length
                    double totalTrackLength = 0.0;
                    for (int i = 0; i < gpsCoordinates.Count; i++)
                    {
                        if (i > 0)
                        {
                            double x1 = gpsCoordinates.ElementAtOrDefault(i - 1).Value.X;
                            double y1 = gpsCoordinates.ElementAtOrDefault(i - 1).Value.Y;

                            double x2 = gpsCoordinates.ElementAtOrDefault(i).Value.X;
                            double y2 = gpsCoordinates.ElementAtOrDefault(i).Value.Y;

                            // Here we are using distance formula derived from Pythagorean theorem
                            double length = CalcTrackSectionLength(x1, y1, x2, y2);
                            totalTrackLength += length;
                        }
                    }
                    // Setting length to the current TrackSection vertex, as required in the Task
                    (vertex as M_TrackSection).SetLength(totalTrackLength);
                    verticesWithLength.TryAdd(vertex.ID, (vertex as M_TrackSection).Length);
                }
                // Calculating total path length from obtained vertices length values
                double totalTrackPathLength = 0.0;
                foreach (double trackLength in verticesWithLength.Values)
                    totalTrackPathLength += trackLength;

                // Adding path with calculated total length to dictionary
                pathsWithLength.TryAdd(path.Key, totalTrackPathLength);
            }

            Task2_PrintResults(pathsWithLength);
            return pathsWithLength;
        }

        public void Task1_PrintResults(HashSet<Vertex> pathList)
        {
            Console.WriteLine();
            foreach (var elem in pathList)
                Console.Write($"{elem.ID} ");
        }

        public void Task2_PrintResults(Dictionary<string, double> results)
        {
            Console.WriteLine("\n|---------------------------------------------|\n");
            Console.WriteLine("[Task2 Results]");
            Console.WriteLine("(Calculated length for all train paths from geometric positioning coordinates)\n");
            if(results.Count < 1)
                Console.WriteLine("TravelPaths list is empty!");
            
            foreach(var item in results)
            {
                Console.WriteLine($"ID: {item.Key}");
                Console.WriteLine($"Length: {Math.Round(item.Value, 3)} units\n");
            }
            Console.WriteLine("|---------------------------------------------|\n");
        }

        public bool AddTravelPath(HashSet<Vertex> path)
        {
            string pathID = GenerateIDForTravelPath(path);
            return TravelPaths.TryAdd(pathID, path);
        }

        private string GenerateIDForTravelPath(HashSet<Vertex> path)
        {
            if (path != null)
            {
                string combinedID = String.Empty;
                foreach (Vertex v in path)
                {
                    if (combinedID != String.Empty)
                        combinedID += "+" + v.ID;
                    else
                        combinedID += v.ID;

                }
                return combinedID;
            }
            else
                return String.Empty;
        }

        private double CalcTrackSectionLength(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) +
                          Math.Pow(y2 - y1, 2) * 1.0);
        }

        #endregion

        #region Layers

        public void AutoAssignBasicVerticesToLayers()
        {
            bool toRestoreSettings = CanCreateNonExistingLayerOnVertexAdd;
            
            // We need this setting to be true in order to create basic Layers without a problem
            CanCreateNonExistingLayerOnVertexAdd = true;
            foreach (Vertex v in Vertices.Values)
            {
                // [M_TrackSection]
                if (v is M_TrackSection)
                    AddVertexToLayer(v, "TrackSection");
                // [M_BaliseGroup]
                else if (v is M_BaliseGroup)
                    AddVertexToLayer(v, "Balises");
                // [M_Point]
                else if (v is M_Point)
                    AddVertexToLayer(v, "SwitchesIS");
                // [M_Track]
                else if (v is M_Track)
                    AddVertexToLayer(v, "Tracks");

                // More to be added with the addition of new Vertex types
            }
            CanCreateNonExistingLayerOnVertexAdd = toRestoreSettings;
        }

        public bool AddEmptyLayer(string name)
        {
            return Layers.TryAdd(name, new Layer(name));
        }

        // If Layer with provided layerName doesn't exist, creates a new layer with this name
        public void AddVertexToLayer(Vertex v, string layerName)
        {   
            // If Layer exists
            if (Layers.TryGetValue(layerName, out Layer existingLayer))
            {
                v.AddLayerReference(existingLayer);
                existingLayer.Vertices.TryAdd(v.ID, v);
            }
            else
            {
                if (!CanCreateNonExistingLayerOnVertexAdd)
                {
                    Console.Error.WriteLine("Cannot create non-existing layer! (Modify accroding settings in Multigraph)");
                    return;
                }
                   

                if (Layers.TryAdd(layerName, new Layer(layerName)))
                {
                    if(Layers.TryGetValue(layerName, out Layer createdLayer))
                    {
                        v.AddLayerReference(createdLayer);
                        createdLayer.Vertices.TryAdd(v.ID, v);
                    }
                }
            }
        }

        public void AddVertexToLayer(string vertexID, string layerName)
        {
            if(Vertices.TryGetValue(vertexID, out Vertex v))
                AddVertexToLayer(v, layerName);
        }

        public bool RemoveLayer(string layerName)
        {
            if(Layers.TryGetValue(layerName, out Layer layer))
            {
                // Removing Layer reference from each Vertex inside Layer to be removed
                foreach(Vertex v in layer.Vertices.Values)
                {
                    v.RemoveLayerReference(layer);
                }
                return Layers.Remove(layerName);
            }
            return false;
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

                        Console.Write($"\nLayers: ");
                        foreach (string layerName in Vertices.Values.ElementAt(i).Layers.Keys)
                        {
                            Console.Write($"{layerName} ");
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
                    Console.WriteLine($"[<{edge.FromVertex.ID}, {edge.ToVertex.ID}>: {edge.Type}]");

                Console.WriteLine("|-------------------------------|\n");
            }
            else
                Console.WriteLine("Edge list is empty!\n");
        }

        public void DisplayAdjList()
        {
            Console.WriteLine("\n\t======Mulitgraph Adjacency List======");
            if (AdjList != null && AdjList.Count > 0)
            {
                Console.WriteLine("|---------------------------------------------|");

                foreach(var item in AdjList)
                {
                    Console.WriteLine($"\n[ID: {item.Key}]");
                    if (item.Value.Count > 0)
                    {
                        foreach (var edge in item.Value)
                            Console.WriteLine($"|<{edge.Value.FromVertex.ID}, {edge.Value.ToVertex.ID}>: {edge.Value.Type}|");
                    }
                    else
                        Console.WriteLine("Empty!");

                }
                Console.WriteLine("|---------------------------------------------|\n");
            }
            else
                Console.WriteLine("Adjacency List is empty!\n");
        }

        #endregion
    }
}
