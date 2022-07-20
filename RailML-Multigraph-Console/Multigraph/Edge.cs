using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RailML_Multigraph_Console
{
    public enum EdgeType
    {
        Unassigned = 0,
        AdjacentTrackSection = 1,
        SignalSequence = 6,
        AssignmentToATrackSection = 12,
        LinkingOfABaliseGroupWithASignal = 17,
        AssignmentToALine = 14,
        AssignmentToAMacroElement = 13,
        BaliseGroupSequence = 15,
        AdjacentMacroElements = 11
    }
    
    [DataContract]
    public class Edge
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public Vertex FromVertex { get; set; }

        [DataMember]
        public Vertex ToVertex { get; set; }

        [DataMember]
        public EdgeType Type { get; set; }

        [DataMember]
        public bool IsDirected { get; set; } = true;

        public Edge()
        {
            this.Type = EdgeType.Unassigned;
        }

        private string GenerateID(Vertex from, Vertex to)
        {
            if (from != null && to != null)
            {
                if (!String.IsNullOrWhiteSpace(from.ID) && !String.IsNullOrWhiteSpace(to.ID))
                {
                    string combinedID = from.ID + to.ID;
                    return combinedID;
                }
                else
                    return String.Empty;
            }
            else
                return String.Empty;
        }

        public Edge(Vertex from, Vertex to, EdgeType type = EdgeType.Unassigned, bool isDirected = true)
        {
            if(from != null && to != null)
            {
                string generatedID = GenerateID(from, to);
                if (!String.IsNullOrWhiteSpace(generatedID))
                {
                    this.ID = generatedID;
                }
                else
                {
                    throw new ArgumentException("Vertices with invalid ID were passed to Edge constructor!");
                }
                this.FromVertex = from;
                this.ToVertex = to;
                this.Type = type;
                this.IsDirected = isDirected;
            }
        }

        public override bool Equals(object obj)
        {
            Edge e = obj as Edge;
            return e != null && e.FromVertex == this.FromVertex && e.ToVertex == this.ToVertex && e.Type == this.Type && e.IsDirected == this.IsDirected;
        }

        public override int GetHashCode()
        {
            return (this.FromVertex == null ? 0 : this.FromVertex.GetHashCode())
               ^ (this.ToVertex == null ? 0 : this.ToVertex.GetHashCode());
        }

    }

}
