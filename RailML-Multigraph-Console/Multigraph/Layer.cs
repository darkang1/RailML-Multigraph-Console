﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RailML_Multigraph_Console
{
    /// <summary>
    /// 1. Stores selected vertices
    ///    - Layers for parsed RailML elements are generated automatically, based on the element type
    ///    - User can manually create Layers with various types of vertices
    /// 2. Can only add existing vertices in Multigraph
    /// 3. If vertice removed from Layer, DO NOT remove it from Multigraph
    ///      If vertice removed from Multigraph, REMOVE it from Layer
    /// </summary>
    public class Layer
    {
        [DataMember]
        public string ID { get; protected set; }
        [DataMember]
        public string Name { get; protected set; }

        // Data type to be modified later with some class/enum instance
        [DataMember]
        public string Color { get; protected set; }
        [DataMember]
        public Dictionary<string, Vertex> Vertices { get; private set; } = new Dictionary<string, Vertex>();       

        public Layer(string name, string color = "None")
        {
            ID = GenerateID();
            SetName(name);
            Color = color;
        }
        
        // Overloaded constructor in case if we can pass full Dictionary of specific vertices
        public Layer(string name, Dictionary<string, Vertex> vertices, string color = "None")
        {
            ID = GenerateID();
            SetName(name);
            Color = color;

            if(vertices != null)
            {
                foreach (var vertex in vertices)
                {
                    Vertices.TryAdd(vertex.Key, vertex.Value);
                }
            }
          
        }

        private bool SetName(string name)
        {
            if(!String.IsNullOrWhiteSpace(name))
            {
                Name = name;
                return true;
            }
            return false;

        }
        private string GenerateID()
        {
            return Guid.NewGuid().ToString("D");
        }

    }
}
