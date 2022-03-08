﻿/*    
    Obra original:
        Copyright (c) 2018 Packt
        Unity 2018 Artificial Intelligence Cookbook - Second Edition, by Jorge Palacios
        https://github.com/PacktPublishing/Unity-2018-Artificial-Intelligence-Cookbook-Second-Edition
        MIT License

    Modificaciones:
        Copyright (C) 2020-2022 Federico Peinado
        http://www.federicopeinado.com

        Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
        Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
        Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Navegacion
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;


    public struct NodeRecord{
        public GameObject node;
        public Vertex connection;
        public float costSoFar;
        public float estimatedTotalCost;
    }

    /// <summary>
    /// Abstract class for graphs
    /// </summary>
    public abstract class Graph : MonoBehaviour
    {

        public GameObject vertexPrefab;
        protected List<Vertex> vertices;
        protected List<List<Vertex>> neighbors;
        protected List<List<float>> costs;
        //protected Dictionary<int, int> instIdToId;

        //// this is for informed search like A*
        public delegate float Heuristic(Vertex a, Vertex b);
        // Used for getting path in frames
        public List<Vertex> path;
        //public bool isFinished;

        public virtual void Awake() {
            GameManager.instance.SetGraph(this);            
            Load();
        }

        public virtual void Start(){
        
        }

        public virtual void Load() { }

        public virtual int GetSize()
        {
            if (ReferenceEquals(vertices, null))
                return 0;
            return vertices.Count;
        }

        public virtual Vertex GetNearestVertex(Vector3 position){
            return null;
        }

        public virtual Edge[] GetEdges(Vertex v){
            if (ReferenceEquals(neighbors, null) || neighbors.Count == 0)
                return new Edge[0];
            if (v.id < 0 || v.id >= neighbors.Count)
                return new Edge[0];
            int numEdges = neighbors[v.id].Count;
            Edge[] edges = new Edge[numEdges];
            List<Vertex> vertexList = neighbors[v.id];
            List<float> costList = costs[v.id];
            for (int i = 0; i < numEdges; i++)
            {
                edges[i] = new Edge();
                edges[i].cost = costList[i];
                edges[i].vertex = vertexList[i];
            }
            return edges;
        }


        public virtual Vertex[] GetNeighbours(Vertex v)
        {
            if (ReferenceEquals(neighbors, null) || neighbors.Count == 0)
                return new Vertex[0];
            if (v.id < 0 || v.id >= neighbors.Count)
                return new Vertex[0];
            return neighbors[v.id].ToArray();
        }

        // Encuentra caminos óptimos
        public List<Vertex> GetPathBFS(GameObject srcO, GameObject dstO)
        {
            if (srcO == null || dstO == null)
                return new List<Vertex>();
            Vertex[] neighbours;
            Queue<Vertex> q = new Queue<Vertex>();
            Vertex src = GetNearestVertex(srcO.transform.position);
            Vertex dst = GetNearestVertex(dstO.transform.position);
            Vertex v;
            int[] previous = new int[vertices.Count];
            for (int i = 0; i < previous.Length; i++)
                previous[i] = -1;
            previous[src.id] = src.id; // El vértice que tenga de previo a sí mismo, es el vértice origen
            q.Enqueue(src);
            while (q.Count != 0)
            {
                v = q.Dequeue();
                if (ReferenceEquals(v, dst))
                {
                    return BuildPath(src.id, v.id, ref previous);
                }

                neighbours = GetNeighbours(v);
                foreach (Vertex n in neighbours)
                {
                    if (previous[n.id] != -1)
                        continue;
                    previous[n.id] = v.id; // El vecino n tiene de 'padre' a v
                    q.Enqueue(n);
                }
            }
            return new List<Vertex>();
        }

        // No encuentra caminos óptimos
        public List<Vertex> GetPathDFS(GameObject srcO, GameObject dstO)
        {
            if (srcO == null || dstO == null)
                return new List<Vertex>();
            Vertex src = GetNearestVertex(srcO.transform.position);
            Vertex dst = GetNearestVertex(dstO.transform.position);
            Vertex[] neighbours;
            Vertex v;
            int[] previous = new int[vertices.Count];
            for (int i = 0; i < previous.Length; i++)
                previous[i] = -1;
            previous[src.id] = src.id;
            Stack<Vertex> s = new Stack<Vertex>();
            s.Push(src);
            while (s.Count != 0)
            {
                v = s.Pop();
                if (ReferenceEquals(v, dst))
                {
                    return BuildPath(src.id, v.id, ref previous);
                }

                neighbours = GetNeighbours(v);
                foreach (Vertex n in neighbours)
                {
                    if (previous[n.id] != -1)
                        continue;
                    previous[n.id] = v.id;
                    s.Push(n);
                }
            }
            return new List<Vertex>();
        }

        public List<Vertex> GetPathAstar(GameObject srcObj, GameObject dstObj, Heuristic h = null) {
            // AQUÍ HAY QUE PONER LA IMPLEMENTACIÓN DEL ALGORITMO A*
            // ...

            if(srcObj == null || dstObj == null)
                return new List<Vertex>();
            if (ReferenceEquals(h, null))
                h = EuclidDist;
            Vertex src = GetNearestVertex(srcObj.transform.position);
            Vertex dst = GetNearestVertex(dstObj.transform.position);
            BinaryHeap<Edge> frontier = new BinaryHeap<Edge>();
            Edge[] edges;
            Edge node, child;
            int size = vertices.Count;
            float[] distValue = new float[size];
            int[] previous = new int[size];
            // next steps

            node = new Edge(src, 0);
            frontier.Add(node);
            distValue[src.id] = 0;
            previous[src.id] = src.id;

            for (int i = 0; i < size; i++){
                if (i == src.id)
                    continue;
                distValue[i] = Mathf.Infinity;
                previous[i] = -1;
            }

            while (frontier.Count != 0)
            {
                node = frontier.Remove();
                int nodeId = node.vertex.id;
                if (ReferenceEquals(node.vertex, dst))
                {
                    return BuildPath(src.id, node.vertex.id, ref previous);
                }

                edges = GetEdges(node.vertex);

                foreach (Edge e in edges)
                {
                    int eId = e.vertex.id;
                    if (previous[eId] != -1)
                        continue;
                    float cost = distValue[nodeId] + e.cost;
                    // key point
                    cost += h(node.vertex, e.vertex);
                    // next step

                    if (cost < distValue[e.vertex.id])
                    {
                        distValue[eId] = cost;
                        previous[eId] = nodeId;
                        frontier.Remove(e);
                        child = new Edge(e.vertex, cost);
                        frontier.Add(child);
                    }
                }
            }
            return new List<Vertex>();
        }

        public List<Vertex> Smooth(List<Vertex> path){
            // AQUÍ HAY QUE PONER LA IMPLEMENTACIÓN DEL ALGORITMO DE SUAVIZADO
            // ...

            return null; //newPath
        }

        // Reconstruir el camino, dando la vuelta a la lista de nodos 'padres' /previos que hemos ido anotando
        private List<Vertex> BuildPath(int srcId, int dstId, ref int[] prevList)
        {
            List<Vertex> path = new List<Vertex>();
            int prev = dstId;
            do
            {
                path.Add(vertices[prev]);
                prev = prevList[prev];
            } while (prev != srcId);
            return path;
        }

        // Sí me parece razonable que la heurística trabaje con la escena de Unity
        // Heurística de distancia euclídea
        public float EuclidDist(Vertex a, Vertex b)
        {
            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;
            return Vector3.Distance(posA, posB);
        }

        // Heurística de distancia Manhattan
        public float ManhattanDist(Vertex a, Vertex b)
        {
            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;
            return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
        }
    }
}