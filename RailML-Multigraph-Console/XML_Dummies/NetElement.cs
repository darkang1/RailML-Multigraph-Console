using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RailML_Multigraph_Console
{
    public class NetElements
    {
        [XmlElement("netElement")]
        public List<NetElement> netElements = new List<NetElement>();
    }

    public class NetElement : ParsedElement
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlElement("relation")]
        public List<Relation> Relations { get; set; } = new List<Relation>();

        [XmlElement("associatedPositioningSystem")]
        public AssociatedPositioningSystem AssociatedPositioningSystem { get; set; } = new AssociatedPositioningSystem();

        public override string GetID()
        {
            return ID;
        }

    }

    public class Relation
    {
        [XmlAttribute("ref")]
        public string ElementRef { get; set; }

        [XmlIgnore]
        public NetRelation ActualNetRelationRef { get; set; } = new NetRelation();

    }

    public class AssociatedPositioningSystem
    {
        [XmlElement("intrinsicCoordinate")]
        public List<IntrisicCoordinate> IntrinsicCoordinates { get; set; } = new List<IntrisicCoordinate>();

    }

    public class IntrisicCoordinate
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlAttribute("intrinsicCoord")]
        public string IntrinsicCoord { get; set; }

        [XmlElement("geometricCoordinate")]
        public GeometricCoordinate GeometricCoordinate { get; set; } = new GeometricCoordinate();

    }

    public class GeometricCoordinate
    {
        [XmlAttribute("positioningSystemRef")]
        public string PositioningSystemRef { get; set; }

        [XmlAttribute("x")]
        public double X { get; set; }

        [XmlAttribute("y")]
        public double Y { get; set; }

    }

}
