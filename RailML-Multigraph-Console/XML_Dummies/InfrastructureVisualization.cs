using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RailML_Multigraph_Console
{
    public class InfrastructureVisualization
    {
        [XmlElement("visualization")]
        public Visualization Visualization { get; set; } = new Visualization();
    }

    public class Visualization
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlAttribute("positioningSystemRef")]
        public string PositioningSystemRef { get; set; }

        [XmlElement("name")]
        public Name Name { get; set; } = new Name();

        [XmlElement("spotElementProjection")]
        public List<SpotElementProjection> SpotElementProjection { get; set; } = new List<SpotElementProjection>();

        [XmlElement("linearElementProjection")]
        public List<LinearElementProjection> LinearElementProjection { get; set; } = new List<LinearElementProjection>();

    }

    public class SpotElementProjection : ParsedElement
    {
        [XmlAttribute("refersToElement")]
        public string RefersToElement { get; set; }

        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlElement("name")]
        public Name Name { get; set; } = new Name();

        [XmlElement("coordinate")]
        public Coordinate Coordinate { get; set; }

        public SpotElementProjection()
        {

        }

        public SpotElementProjection(SpotElementProjection spotElementProjection)
        {
            this.RefersToElement = spotElementProjection.RefersToElement;
            this.ID = spotElementProjection.ID;
            this.Name = spotElementProjection.Name;
            this.Coordinate = spotElementProjection.Coordinate;
        }

        public override string GetID()
        {
            return ID;
        }

    }

    public class LinearElementProjection : ParsedElement
    {
        [XmlAttribute("refersToElement")]
        public string RefersToElement { get; set; }

        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlElement("name")]
        public Name Name { get; set; } = new Name();

        [XmlElement("coordinate")]
        public List<Coordinate> Coordinates { get; set; } = new List<Coordinate>();

        public LinearElementProjection()
        {

        }

        public LinearElementProjection(LinearElementProjection linearElementProjection)
        {
            this.RefersToElement = linearElementProjection.RefersToElement;
            this.ID = linearElementProjection.ID;
            this.Name = linearElementProjection.Name;
            foreach(Coordinate coordinate in linearElementProjection.Coordinates)
            {
                this.Coordinates.Add(new Coordinate(coordinate));
            }           
        }

        public override string GetID()
        {
            return ID;
        }

    }

    public class Coordinate
    {
        [XmlAttribute("x")]
        public double X { get; set; }

        [XmlAttribute("y")]
        public double Y { get; set; }

        public Coordinate()
        {

        }

        public Coordinate(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Coordinate(Coordinate coordinate)
        {
            this.X = coordinate.X;
            this.Y = coordinate.Y;
        }
    }
}
