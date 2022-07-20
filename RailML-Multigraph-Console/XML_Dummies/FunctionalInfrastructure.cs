using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RailML_Multigraph_Console
{
    public class FunctionalInfrastructure
    {
        [XmlElement("balises")]
        public Balises Balises { get; set; } = new Balises();

        [XmlElement("bufferStops")]
        public BufferStops BufferStops { get; set; } = new BufferStops();

        [XmlElement("switchesIS")]
        public SwitchesIS SwitchesIS { get; set; } = new SwitchesIS();

        [XmlElement("tracks")]
        public Tracks Tracks { get; set; } = new Tracks();

    }





    public class Name
    {
        [XmlAttribute("name")]
        public string Name_ { get; set; }

        // 'en' by default
        [XmlAttribute("language")]
        public string Language { get; set; }
    }

    public class SpotLocation
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlAttribute("netElementRef")]
        public string NetElementRef { get; set; }

        [XmlIgnore]
        public NetElement ActualNetElementRef { get; set; } = new NetElement();

        // Either 'normal' or 'reverse' or ?
        [XmlAttribute("applicationDirection")]
        public string ApplicationDirection { get; set; }

        [XmlAttribute("intrinsicCoord")]
        public double IntrinsicCoord { get; set; }

    }

    public class Designator
    {
        // Usually '_Example' by default
        [XmlAttribute("register")]
        public string Register { get; set; }

        // Balise name ?
        [XmlAttribute("entry")]
        public string Entry { get; set; }

    }
}
