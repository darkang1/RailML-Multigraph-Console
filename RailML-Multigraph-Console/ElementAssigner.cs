using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailML_Multigraph_Console
{
    public static class ElementAssigner
    {
        public static void AssignElementsReferences(RailML xmlData)
        {
            AssignNetElements(xmlData);
            AssignNetRelations(xmlData);
            AssignBalises(xmlData);
            AssignBufferStops(xmlData);
            AssignSwitchesIS(xmlData);
            AssignTracks(xmlData);

            Console.WriteLine("Element assignment process finished!");
        }

        public static void AssignNetElements(RailML xmlData)
        {
            try
            {
                foreach (NetElement netElement in xmlData.Infrastructure.Topology.NetElements.netElements ?? Enumerable.Empty<NetElement>())
                {
                    foreach (NetRelation netRelation in xmlData.Infrastructure.Topology.NetRelations.netRelations ?? Enumerable.Empty<NetRelation>())
                    {
                        foreach(Relation relation in netElement.Relations)

                            if (relation.ElementRef == netRelation.ID)
                            {
                                relation.ActualNetRelationRef = netRelation;
                                break;
                            }
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ElementAssigner] {ex.Message}");
            }

        }

        public static void AssignNetRelations(RailML xmlData)
        {
            try
            {
                foreach(NetRelation netRelation in xmlData.Infrastructure.Topology.NetRelations.netRelations ?? Enumerable.Empty<NetRelation>())
                {
                    bool isElementAAssigned = false;
                    bool isElementBAssigned = false;
                    foreach (NetElement netElement in xmlData.Infrastructure.Topology.NetElements.netElements ?? Enumerable.Empty<NetElement>())
                    {
                        // Optimization technique to get rid of redundant iterations if both Elements were assigned with proper NetElements
                        if (!isElementAAssigned || !isElementBAssigned)
                        {
                            // For ElementA
                            if (netRelation.ElementA.ElementRef == netElement.ID)
                            {
                                netRelation.ElementA.ActualNetElementRef = netElement;
                                isElementAAssigned = true;
                            }
                            // For ElementB
                            if (netRelation.ElementB.ElementRef == netElement.ID)
                            {
                                netRelation.ElementB.ActualNetElementRef = netElement;
                                isElementBAssigned = true;
                            }
                        }
                        else
                            break;

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ElementAssigner] {ex.Message}");
            }
        }

        public static void AssignBalises(RailML xmlData)
        {
            try
            {
                foreach (Balise balise in xmlData.Infrastructure.FunctionalInfrastructure.Balises.balises ?? Enumerable.Empty<Balise>())
                {
                    foreach (NetElement netElement in xmlData.Infrastructure.Topology.NetElements.netElements ?? Enumerable.Empty<NetElement>())
                    {
                        if (balise.SpotLocation.NetElementRef == netElement.ID)
                        {
                            balise.SpotLocation.ActualNetElementRef = netElement;
                            break;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ElementAssigner] {ex.Message}");
            }
        }

        public static void AssignBufferStops(RailML xmlData)
        {
            try
            {
                foreach (BufferStop bufferStop in xmlData.Infrastructure.FunctionalInfrastructure.BufferStops.bufferStops ?? Enumerable.Empty<BufferStop>())
                {
                    foreach (NetElement netElement in xmlData.Infrastructure.Topology.NetElements.netElements ?? Enumerable.Empty<NetElement>())
                    {
                        if (bufferStop.SpotLocation.NetElementRef == netElement.ID)
                        {
                            bufferStop.SpotLocation.ActualNetElementRef = netElement;
                            break;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ElementAssigner] {ex.Message}");
            }
        }

        public static void AssignSwitchesIS(RailML xmlData)
        {
            try
            {
                foreach (SwitchIS switchIS in xmlData.Infrastructure.FunctionalInfrastructure.SwitchesIS.switchesIS ?? Enumerable.Empty<SwitchIS>())
                {
                    // Assigning netElements
                    foreach (NetElement netElement in xmlData.Infrastructure.Topology.NetElements.netElements ?? Enumerable.Empty<NetElement>())
                    {
                        if (switchIS.SpotLocation.NetElementRef == netElement.ID)
                        {
                            switchIS.SpotLocation.ActualNetElementRef = netElement;
                            break;
                        }

                    }

                    bool isLeftBranchAssigned = false;
                    bool isRightBranchAssigned = false;

                    // Assigning netRelations
                    foreach (NetRelation netRelation in xmlData.Infrastructure.Topology.NetRelations.netRelations ?? Enumerable.Empty<NetRelation>())
                    {
                        // Optimization technique to get rid of redundant iterations if both Branches were already assigned with proper NetRelations
                        if (!isLeftBranchAssigned || !isRightBranchAssigned)
                        {
                            // For LeftBranch
                            if (switchIS.LeftBranch.NetRelationRef == netRelation.ID)
                            {
                                switchIS.LeftBranch.ActualNetRelationRef = netRelation;
                                isLeftBranchAssigned = true;
                            }
                            // For RightBranch
                            if (switchIS.RightBranch.NetRelationRef == netRelation.ID)
                            {
                                switchIS.RightBranch.ActualNetRelationRef = netRelation;
                                isRightBranchAssigned = true;
                            }
                        }
                        else
                            break;
                        
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ElementAssigner] {ex.Message}");
            }
        }

        public static void AssignTracks(RailML xmlData)
        {
            // Later will be needed to extend functionality to assigning TrackBegin and TrackEnd
            // Currently it is too complicated due to the fact that TrackBeging/TrackEnd can be different object types
            // P.S 'infrastructureManagerRef' is omitted since this element contains just ID and nothing more
            try
            {
                foreach (Track track in xmlData.Infrastructure.FunctionalInfrastructure.Tracks.tracks ?? Enumerable.Empty<Track>())
                {
                    foreach (NetElement netElement in xmlData.Infrastructure.Topology.NetElements.netElements ?? Enumerable.Empty<NetElement>())
                    {
                        if (track.LinearLocation.AssociatedNetElement.NetElementRef == netElement.ID)
                        {
                            track.LinearLocation.AssociatedNetElement.ActualNetElementRef = netElement;
                            break;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ElementAssigner] {ex.Message}");
            }

        }
    }
}
