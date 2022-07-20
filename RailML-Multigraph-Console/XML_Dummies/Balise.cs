using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RailML_Multigraph_Console
{
    public class Balises
    {
        [XmlElement("balise")]
        public List<Balise> balises = new List<Balise>();
    }
    public class Balise : ParsedElement
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlAttribute("isBaliseGroup")]
        public bool IsBaliseGroup { get; set; }

        // Either 'fixed' or ?
        [XmlAttribute("baliseGroupType")]
        public string BaliseGroupType { get; set; }

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
