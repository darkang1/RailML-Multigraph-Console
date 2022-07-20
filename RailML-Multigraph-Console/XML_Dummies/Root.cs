using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RailML_Multigraph_Console
{

    [XmlRoot(ElementName = "railML", Namespace = "https://www.railml.org/schemas/3.1")]
    public class RailML
    {
        [XmlElement("infrastructure")]
        public Infrastructure Infrastructure { get; set; } = new Infrastructure();
    }

    public class Infrastructure
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlElement("topology")]
        public Topology Topology { get; set; } = new Topology();

        [XmlElement("functionalInfrastructure")]
        public FunctionalInfrastructure FunctionalInfrastructure { get; set; } = new FunctionalInfrastructure();

        [XmlElement("infrastructureVisualizations")]
        public InfrastructureVisualization InfrastructureVisualization { get; set; } = new InfrastructureVisualization();
    }

    public class Topology
    {
        [XmlElement("netElements")]
        public NetElements NetElements { get; set; } = new NetElements();

        [XmlElement("netRelations")]
        public NetRelations NetRelations { get; set; } = new NetRelations();
    }

    public abstract class ParsedElement
    {
        public abstract string GetID();

    }
}