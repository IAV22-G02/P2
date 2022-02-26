using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using UnityEngine;

namespace Maze {
    public class MazeGenerator {
        private CellState[,] _cells;
        private int _width;
        private int _height;
        private System.Random _rng;

        public MazeGenerator(){
            _rng = new System.Random();
        }

        public void setSize(int width, int height){
            _width = width;
            _height = height;
        }

        public void Generate(){
            _cells = new CellState[_width, _height];
            for (var x = 0; x < _width; x++)
                for (var y = 0; y < _height; y++)
                    _cells[x, y] = CellState.Initial;
            VisitCell(_rng.Next(_width), _rng.Next(_height));
        }

        public CellState this[int x, int y] {
            get { return _cells[x, y]; }
            set { _cells[x, y] = value; }
        }

        public IEnumerable<RemoveWallAction> GetNeighbours(Point p) {
            if (p.X > 0) yield return new RemoveWallAction { Neighbour = new Point(p.X - 1, p.Y), Wall = CellState.Left };
            if (p.Y > 0) yield return new RemoveWallAction { Neighbour = new Point(p.X, p.Y - 1), Wall = CellState.Top };
            if (p.X < _width - 1) yield return new RemoveWallAction { Neighbour = new Point(p.X + 1, p.Y), Wall = CellState.Right };
            if (p.Y < _height - 1) yield return new RemoveWallAction { Neighbour = new Point(p.X, p.Y + 1), Wall = CellState.Bottom };
        }

        public void VisitCell(int x, int y) {
            this[x, y] |= CellState.Visited;
            foreach (var p in GetNeighbours(new Point(x, y)).Shuffle(_rng).Where(z => !(this[z.Neighbour.X, z.Neighbour.Y].HasFlag(CellState.Visited)))){
                this[x, y] -= p.Wall;
                this[p.Neighbour.X, p.Neighbour.Y] -= p.Wall.OppositeWall();
                VisitCell(p.Neighbour.X, p.Neighbour.Y);
            }
        }

        public void Display() {
            string filename = Application.dataPath + "/Maps/map.map";

            StreamWriter file = new StreamWriter(filename);
            file.WriteLine("type octile");
            file.WriteLine("height " + (_height * 3 + 1));
            file.WriteLine("width " + (_width * 3 + 1));
            file.WriteLine("map");

            var firstLine = string.Empty;
            for (var y = 0; y < _height; y++)
            {
                var sbTop = new StringBuilder();
                var sbMid = new StringBuilder();
                for (var x = 0; x < _width; x++) {
                    sbTop.Append(this[x, y].HasFlag(CellState.Top) ? "TTT" : "T..");
                    sbMid.Append(this[x, y].HasFlag(CellState.Left) ? "T.." : "...");
                }
                if (firstLine == string.Empty)
                    firstLine = sbTop.ToString();



                file.WriteLine(sbTop + "T");
                file.WriteLine(sbMid + "T");
                file.WriteLine(sbMid + "T");
            }
            file.WriteLine(firstLine + "T");

            file.Close();
        }
    }

    public static class Extensions{
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, System.Random rng)
        {
            var e = source.ToArray();
            for (var i = e.Length - 1; i >= 0; i--)
            {
                var swapIndex = rng.Next(i + 1);
                yield return e[swapIndex];
                e[swapIndex] = e[i];
            }
        }

        public static CellState OppositeWall(this CellState orig){
            return (CellState)(((int)orig >> 2) | ((int)orig << 2)) & CellState.Initial;
        }

        public static bool HasFlag(this CellState cs, CellState flag){
            return ((int)cs & (int)flag) != 0;
        }
    }

    [Flags]
    public enum CellState {
        Top = 1,
        Right = 2,
        Bottom = 4,
        Left = 8,
        Visited = 128,
        Initial = Top | Right | Bottom | Left,
    }

    public struct RemoveWallAction{
        public Point Neighbour;
        public CellState Wall;
    }
}