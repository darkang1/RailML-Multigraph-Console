using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RailML_Multigraph_Console
{
    public class BufferStops
    {
        [XmlElement("bufferStop")]
        public List<BufferStop> bufferStops = new List<BufferStop>();
    }

    public class BufferStop : ParsedElement
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        // Either 'fixedBufferStop' or ?
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("name")]
        public Name Name { get; set; } = new Name();

        [XmlElement("spotLocation")]
        public SpotLocation SpotLocation { get; set; } = new SpotLocation();

        [XmlElement("designator")]
        public Designator Designator { get; set; } = new Designator();

        public override string GetID()
        {
            return ID;
        }
    }
}
