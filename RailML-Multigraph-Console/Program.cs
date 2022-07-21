using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;

namespace RailML_Multigraph_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(RailML));
                //Main testing file: @"C:\Users\Neo\Desktop\Diploma\RailAiD-Saves\mult-lines-description.railml"
                TextReader reader = new StreamReader(@"C:\Users\Neo\Desktop\Diploma\RailAiD-Saves\mult-lines-description.railml");
                RailML xmlData = (RailML)deserializer.Deserialize(reader);
                reader.Close();

                ElementAssigner.AssignElementsReferences(xmlData);
                HashSet<ParsedElement> parsedElements = new HashSet<ParsedElement>();
                
                // NetElements
                foreach(ParsedElement pe in xmlData.Infrastructure.Topology.NetElements.netElements.ToHashSet())
                    parsedElements.Add(pe);
                // NetRelations
                foreach (ParsedElement pe in xmlData.Infrastructure.Topology.NetRelations.netRelations.ToHashSet())
                    parsedElements.Add(pe);
                // Balises
                foreach (ParsedElement pe in xmlData.Infrastructure.FunctionalInfrastructure.Balises.balises.ToHashSet())
                    parsedElements.Add(pe);
                // BufferStops
                foreach (ParsedElement pe in xmlData.Infrastructure.FunctionalInfrastructure.BufferStops.bufferStops.ToHashSet())
                    parsedElements.Add(pe);
                // SwitchesIS
                foreach (ParsedElement pe in xmlData.Infrastructure.FunctionalInfrastructure.SwitchesIS.switchesIS.ToHashSet())
                    parsedElements.Add(pe);
                // Tracks
                foreach (ParsedElement pe in xmlData.Infrastructure.FunctionalInfrastructure.Tracks.tracks.ToHashSet())
                    parsedElements.Add(pe);
                // SpotElementProjections
                foreach (ParsedElement pe in xmlData.Infrastructure.InfrastructureVisualization.Visualization.SpotElementProjection.ToHashSet())
                    parsedElements.Add(pe);
                // LinearElementProjections
                foreach (ParsedElement pe in xmlData.Infrastructure.InfrastructureVisualization.Visualization.LinearElementProjection.ToHashSet())
                    parsedElements.Add(pe);

                ///// MULTIGRAPH /////

                Multigraph mg = new Multigraph();

                // Adding all elements to Multigraph
                mg.AddElements(parsedElements);

                // Perform edge and coordinate assignment
                mg.AssignEdgesForAllVertices();
                mg.AutoAssignElementsCoordinates();

                // Updating parameters of some vertex
                mg.UpdateAnyVertexName("swi3", "NEW_SWITCH_NAME");
                mg.UpdateAnyVertexColor("swi3", VertexColors.Blue);
                mg.UpdateAnyVertexSpotCoordinate("swi3", 777.07, 777.77);

                // Removing Edge from Edge and AdjList
                //mg.RemoveVertex("ne3");
                //mg.RemoveEdge("ne2", "ne2ne3"); 

                // Displaying all information stored in Multigraph
                mg.DisplayAllVertices();
                mg.DisplayAllEdges();
                mg.DisplayAdjList();

                // Start tracksection
                string s = "ne1";
                // Destination tracksection
                string d = "ne3";

                Console.WriteLine("[Task1 Results]");
                Console.WriteLine("(All possible train paths from " + s + " to " + d + ")");
                mg.Task1_FindAllTrainPaths(s, d);
                mg.Task2_FindLengthOfAllTravelPaths();

                /////////////////////////


                // SERIALIZING //

                bool isSerializingEnabled = false;
                string formattedXml = XElement.Parse(Serialize(mg)).ToString();

                if (isSerializingEnabled)
                {
                    using (StreamWriter w = File.AppendText("mult-lines-description.railml"))
                    {
                        w.WriteLine();
                        w.WriteLine(formattedXml);

                    }
                }

                /* //////////// TESTING AREA //////////////////

                /////////////////////////////////////////// */

            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("File not found at given location!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Unhandled exception occured]: \n{ex.ToString()}");
            }
        }

        #region XML Functions
        public static string Serialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        public static object Deserialize(string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(toType);
                return deserializer.ReadObject(stream);
            }
        }

        #endregion


        /*
        public static void printBalises(HashSet<Balise> balises)
        {

            Console.WriteLine("\n======Parsed Balises======");
            for (int i = 0; i < balises.Count; i++)
            {
                if (balises.ElementAt(i) != null)
                {
                    Console.WriteLine("|-------------------------------|");
                    Console.WriteLine($"[{i + 1}.] {balises.ElementAt(i).Designator.Entry}");
                    Console.WriteLine($"ID: {balises.ElementAt(i).ID}");
                    Console.WriteLine($"isBaliseGroup: {balises.ElementAt(i).IsBaliseGroup}");
                    Console.WriteLine($"baliseGroupType: {balises.ElementAt(i).BaliseGroupType}\n");

                    Console.WriteLine("[name] Tag");
                    Console.WriteLine($"Name: {balises.ElementAt(i).Name.Name_}");
                    Console.WriteLine($"Language: {balises.ElementAt(i).Name.Language}\n");

                    Console.WriteLine("[spotLocation] Tag");
                    Console.WriteLine($"ID: {balises.ElementAt(i).SpotLocation.ID}");
                    Console.WriteLine($"netElementRef: {balises.ElementAt(i).SpotLocation.NetElementRef} <------- (This is NetElement this balise is related to [string])");
                    Console.WriteLine($"actualNetElementRef: {balises.ElementAt(i).SpotLocation.ActualNetElementRef.ID} <------- (This is NetElement this balise is related to [object])");
                    Console.WriteLine($"applicationDirection: {balises.ElementAt(i).SpotLocation.ApplicationDirection}");
                    Console.WriteLine($"intrinsicCoord: {balises.ElementAt(i).SpotLocation.IntrinsicCoord}\n");

                    Console.WriteLine("[designator] Tag");
                    Console.WriteLine($"register: {balises.ElementAt(i).Designator.Register}");
                    Console.WriteLine($"ID: {balises.ElementAt(i).Designator.Entry}");
                    Console.WriteLine("|-------------------------------|\n");

                }

            }


        }
        */
    }

}


