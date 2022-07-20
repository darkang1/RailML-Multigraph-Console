using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RailML_Multigraph_Console
{
    public class NetRelations
    {
        [XmlElement("netRelation")]
        public List<NetRelation> netRelations = new List<NetRelation>();
    }
    public class NetRelation : ParsedElement
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        // Either 0 or 1
        [XmlAttribute("positionOnA")]
        public int PositionOnA { get; set; }

        // Either 0 or 1
        [XmlAttribute("positionOnB")]
        public int PositionOnB { get; set; }

        // Either None or Both or ? (maybe more)
        [XmlAttribute("navigability")]
        public string Navigability { get; set; }

        [XmlElement("elementA")]
        public ElementA ElementA { get; set; } = new ElementA();

        [XmlElement("elementB")]
        public ElementB ElementB { get; set; } = new ElementB();

        public override string GetID()
        {
            return ID;
        }
    }

    public class ElementA
    {
        [XmlAttribute("ref")]
        public string ElementRef { get; set; }

        [XmlIgnore]
        public NetElement ActualNetElementRef { get; set; } = new NetElement();
    }
    public class ElementB
    {
        [XmlAttribute("ref")]
        public string ElementRef { get; set; }

        [XmlIgnore]
        public NetElement ActualNetElementRef { get; set; } = new NetElement();
    }

}
