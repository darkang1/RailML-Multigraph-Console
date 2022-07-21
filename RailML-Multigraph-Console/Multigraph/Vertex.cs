using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RailML_Multigraph_Console
{
    public enum VertexColors
    {
        DarkRed = 2,
        Lime = 4,
        Pink = 7,
        Yellow = 12,
        Blue = 15,
        LightBrown = 17,
        LightOrange = 16,
        Red = 18
    }

    //[DataContract]
    //[KnownType(typeof(M_TrackSection)), KnownType(typeof(M_BaliseGroup)), KnownType(typeof(M_Point)), KnownType(typeof(M_Track))]
    public class Vertex
    {
        [DataMember]
        public string ID { get; protected set; }
        [DataMember]
        public string Name { get; protected set; }
        [DataMember]
        public VertexColors Color { get; protected set; }
        [DataMember]
        public List<Coordinate> Coordinates { get; protected set; } = new List<Coordinate>();

        public Dictionary<string, Edge> Neighbours { get; protected set; } = new Dictionary<string, Edge>();

        
        // Contains HashSet of Layer(s) ID's vertex belongs to
        [DataMember]
        public HashSet<string> Layers { get; protected set; } = new HashSet<string>();

        public bool UpdateName(string newName)
        {
            this.Name = newName;
            return true;
        }

        public bool UpdateColor(VertexColors newColor)
        {
            this.Color = newColor;
            return true;
        }

        public bool UpdateSpotCoordinates(double newX, double newY)
        {
            bool isSingleCoordinate = this.Coordinates != null && this.Coordinates.Count == 1;
            bool isZeroCoordinates = this.Coordinates != null && this.Coordinates.Count == 0;
            if (isSingleCoordinate)
            {
                this.Coordinates.ElementAt(0).X = newX;
                this.Coordinates.ElementAt(0).Y = newY;
                return true;
            }
            else if (isZeroCoordinates)
            {
                this.Coordinates.Add(new Coordinate(newX, newY));
                return true;
            }
            else
                return false;
        }

        public bool AddLayerReference(string layerName)
        {
            return this.Layers.Add(layerName);

        }

        public bool RemoveLayerReference(string layerName)
        {
            return this.Layers.Remove(layerName);
        }
    }

    // NetElement
    [DataContract]
    public class M_TrackSection : Vertex
    {
        public NetElement ThisNetElement { get; private set; }

        public double Length { get; private set; }
        
        public M_TrackSection()
        {
            this.Color = VertexColors.DarkRed;
        }

        public M_TrackSection(NetElement netElement)
        {
            Setup(netElement);
        }

        public M_TrackSection(ParsedElement netElement)
        {
            if(netElement is NetElement)
            {
                Setup((NetElement)netElement);
            }   
        }

        public M_TrackSection(string id, string name, VertexColors color = VertexColors.DarkRed)
        {
            this.ID = id;
            this.Name = name;
            this.Color = color;
        }

        private void Setup(NetElement netElement)
        {
            this.ID = netElement.ID;
            this.Name = netElement.ID; // Name same as ID because NetElement doens't have name
            this.ThisNetElement = netElement;
            this.Color = VertexColors.DarkRed;
        }

        public void SetLength(double newLength)
        {
            Length = newLength;
        }
    }

    [DataContract]
    public class M_BaliseGroup : Vertex
    {
        public Balise ThisBalise { get; private set; }
        public M_BaliseGroup()
        {
            this.Color = VertexColors.Yellow;
        }

        public M_BaliseGroup(ParsedElement balise)
        {
            if (balise is Balise)
            {
                Setup((Balise)balise);
            }
        }
        public M_BaliseGroup(Balise balise)
        {
            Setup(balise);
        }

        private void Setup(Balise balise)
        {
            this.ID = balise.ID;
            this.Name = balise.Name.Name_;
            this.ThisBalise = balise;
            this.Color = VertexColors.Yellow;
        }

        // To delete later
        public M_BaliseGroup(string id, string name, VertexColors color = VertexColors.Yellow)
        {
            this.ID = id;
            this.Name = name;
            this.Color = color;
        }

        public Balise GetElement()
        {
            Console.WriteLine("M_BaliseGroup GetElement() callled!");
            return ThisBalise;
        }
    }

    // SwitchIS
    [DataContract]
    public class M_Point : Vertex
    {
        public SwitchIS ThisSwitchIS { get; private set; }
        public M_Point()
        {
            this.Color = VertexColors.Pink;
        }

        public M_Point(SwitchIS switchIS)
        {
            Setup(switchIS);
        }

        public M_Point(ParsedElement switchIS)
        {
            if (switchIS is SwitchIS)
            {
                Setup((SwitchIS)switchIS);
            }
        }

        public M_Point(string id, string name, VertexColors color = VertexColors.Pink)
        {
            this.ID = id;
            this.Name = name;
            this.Color = color;
        }

        private void Setup(SwitchIS switchIS)
        {
            this.ID = switchIS.ID;
            this.Name = switchIS.Name.Name_;
            this.ThisSwitchIS = switchIS;
            this.Color = VertexColors.Pink;
        }
    }

    [DataContract]
    public class M_Track : Vertex
    {
        public Track ThisTrack { get; private set; }
        public M_Track()
        {
            this.Color = VertexColors.LightBrown;
        }

        public M_Track(Track track)
        {
            Setup(track);
        }

        public M_Track(ParsedElement track)
        {
            if (track is Track)
            {
                Setup((Track)track);
            }
        }
        private void Setup(Track track)
        {
            this.ID = track.ID;
            this.Name = track.Name.Name_;
            this.ThisTrack = track;
            this.Color = VertexColors.LightBrown;
        }

        public M_Track(string id, string name, VertexColors color = VertexColors.LightBrown)
        {
            this.ID = id;
            this.Name = name;
            this.Color = color;
        }
    }



    public class M_Signal : Vertex
    {
        public M_Signal()
        {
            this.Color = VertexColors.Lime;
        }

        public M_Signal(string id, string name, VertexColors color = VertexColors.Lime)
        {
            this.ID = id;
            this.Name = name;
            this.Color = color;
        }
    }

    public class M_RouteStartSection : Vertex
    {
        public M_RouteStartSection()
        {
            this.Color = VertexColors.Blue;
        }

        public M_RouteStartSection(string id, string name, VertexColors color = VertexColors.Blue)
        {
            this.ID = id;
            this.Name = name;
            this.Color = color;
        }
    }

    public class M_OperationalPost : Vertex
    {
        public M_OperationalPost()
        {
            this.Color = VertexColors.LightOrange;
        }

        public M_OperationalPost(string id, string name, VertexColors color = VertexColors.LightOrange)
        {
            this.ID = id;
            this.Name = name;
            this.Color = color;
        }
    }

    public class M_Line : Vertex
    {
        public M_Line()
        {
            this.Color = VertexColors.Red;
        }

        public M_Line(string id, string name, VertexColors color = VertexColors.Red)
        {
            this.ID = id;
            this.Name = name;
            this.Color = color;
        }
    }

}


/*
       // For now not used; Most likely to be remove later
        public bool AddEdge(Edge v)
        {
            if (v != null)
            {
                if (Edges.Add(v))
                {
                    if (v.FromVertex == this)
                    {
                        v.ToVertex.Edges.Add(v);
                        return true;
                    }
                    else if (v.ToVertex == this)
                    {
                        v.FromVertex.Edges.Add(v);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else 
                    return false;
            }
            else
                return false;
        }

        // For now not used; Most likely to be remove later
        public bool AddNeighbor(Vertex v)
        {
            if(v != null && v != this && v.ID != this.ID)
            {
                Neighbours.Add(v);
                v.Neighbours.Add(this);
                return true;
            }
            else
                return false;
        }

        // For now not used; Most likely to be remove later
        public bool RemoveNeighbor(Vertex v)
        {
            if (v != null)
            {
                if (Neighbours.Remove(v) && v.Neighbours.Remove(this))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        // For now not used; Most likely to be remove later
        public void DisplayAllNeighbors()
        {
            Console.WriteLine($"\n======{this.ID} Neighbor Vertices======");
                if (Neighbours != null)
                {
                    Console.Write($"By ID: ");

                    foreach (Vertex neighbor in Neighbours ?? Enumerable.Empty<Vertex>())
                    {
                        Console.Write($"{neighbor.ID} ");
                    }
                    Console.WriteLine();
                    Console.WriteLine("|-------------------------------|\n");

                }

        } 


 */