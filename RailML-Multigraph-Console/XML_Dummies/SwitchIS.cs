using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RailML_Multigraph_Console
{
    public class SwitchesIS
    {
        [XmlElement("switchIS")]
        public List<SwitchIS> switchesIS = new List<SwitchIS>();
    }

    public class SwitchIS : ParsedElement
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlAttribute("continueCourse")]
        // Either 'right' or ?
        public string ContinueCourse { get; set; }

        [XmlAttribute("branchCourse")]
        // Either 'left' or ?
        public string BranchCourse { get; set; }

        // Either 'ordinarySwitch' or ?
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("name")]
        public Name Name { get; set; } = new Name();

        [XmlElement("spotLocation")]
        public SpotLocation SpotLocation { get; set; } = new SpotLocation();

        [XmlElement("designator")]
        public Designator Designator { get; set; } = new Designator();

        [XmlElement("leftBranch")]
        public LeftBranch LeftBranch { get; set; } = new LeftBranch();

        [XmlElement("rightBranch")]
        public RightBranch RightBranch { get; set; } = new RightBranch();

        public override string GetID()
        {
            return ID;
        }
    }

    public class LeftBranch
    {
        [XmlAttribute("netRelationRef")]
        public string NetRelationRef { get; set; }

        [XmlIgnore]
        public NetRelation ActualNetRelationRef { get; set; } = new NetRelation();

        [XmlAttribute("branchingSpeed")]
        public double BranchingSpeed { get; set; }

        [XmlAttribute("joiningSpeed")]
        public double JoiningSpeed { get; set; }

        [XmlAttribute("radius")]
        public double Radius { get; set; }
    }

    public class RightBranch
    {
        [XmlAttribute("netRelationRef")]
        public string NetRelationRef { get; set; }

        [XmlIgnore]
        public NetRelation ActualNetRelationRef { get; set; } = new NetRelation();

        [XmlAttribute("branchingSpeed")]
        public double BranchingSpeed { get; set; }

        [XmlAttribute("joiningSpeed")]
        public double JoiningSpeed { get; set; }

        [XmlAttribute("radius")]
        public double Radius { get; set; }
    }

}
