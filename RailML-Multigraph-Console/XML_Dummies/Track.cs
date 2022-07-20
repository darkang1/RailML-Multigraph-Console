using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RailML_Multigraph_Console
{
    public class Tracks
    {
        [XmlElement("track")]
        public List<Track> tracks = new List<Track>();
    }

    public class Track : ParsedElement
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlAttribute("mainDirection")]
        // Either 'both' or ?
        public string MainDirection { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("infrastructureManagerRef")]
        // Either 'left' or ?
        public string InfrastructureManagerRef { get; set; }

        [XmlElement("name")]
        public Name Name { get; set; } = new Name();

        [XmlElement("linearLocation")]
        public LinearLocation LinearLocation { get; set; } = new LinearLocation();

        [XmlElement("designator")]
        public Designator Designator { get; set; } = new Designator();

        [XmlElement("trackBegin")]
        public TrackBegin TrackBegin { get; set; } = new TrackBegin();

        [XmlElement("trackEnd")]
        public TrackEnd TrackEnd { get; set; } = new TrackEnd();

        [XmlElement("length")]
        public Length Length { get; set; } = new Length();

        public override string GetID()
        {
            return ID;
        }
    }

    public class LinearLocation
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlAttribute("applicationDirection")]
        // Either 'both' or ?
        public string ApplicationDirection { get; set; }

        [XmlElement("associatedNetElement")]
        public AssociatedNetElement AssociatedNetElement { get; set; } = new AssociatedNetElement();
    }

    public class AssociatedNetElement
    {
        [XmlAttribute("keepsOrientation")]
        public bool KeepsOrientation { get; set; }

        [XmlAttribute("netElementRef")]
        public string NetElementRef { get; set; }

        [XmlIgnore]
        public NetElement ActualNetElementRef { get; set; } = new NetElement();
    }

    public class TrackBegin
    {
        [XmlAttribute("ref")]
        // Can be different element type!!!
        public string ElementRef { get; set; }
    }

    public class TrackEnd
    {
        [XmlAttribute("ref")]
        // Can be different element type!!!
        public string ElementRef { get; set; }
    }

    public class Length
    {
        [XmlAttribute("value")]
        public double Value { get; set; }

        [XmlAttribute("type")]
        // Either 'physical' or ?
        public string Type { get; set; }
    }
}
