using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Vertex {
    public int x;
    public int y;
    public List<Vertex> edges = new List<Vertex>();
}

namespace ConsoleApplication1 {
    public class CartesianGraph {

        public CartesianGraph(string inFile) {
            // example said: @"C:\Users\Public\TestFolder\WriteText.txt"
            string text = System.IO.File.ReadAllText(inFile);
            string mapData = getBetween(text, "<MAP>", "</MAP>");
            int c = 0;
            int r = -1;
            // for all vertices
            for (int i = 0; i < mapData.Length; ++i) {
                // Upon new line, x = 0 and ++y, otherwise x += 1
                iterateGrid(mapData[i], ref r, ref c);
                if (mapData[i] == 'O') {
                    if (r == -1) r = 0;
                    Vertex temp = new Vertex();
                    temp.x = c / 2;
                    temp.y = r / 2;
                    vertices.Add(temp);
                }
            }
            r = -1;
            c = 0;
            // for all edges
            for (int i = 0; i < mapData.Length; ++i) {

                iterateGrid(mapData[i], ref r, ref c);
                if (mapData[i] == 'O' && r == -1)
                    r = 0;

                bool edgeH = (c % 2 != 0);
                bool edgeV = (r % 2 != 0);
                char kz = mapData[i];

                Vertex point1, point2;

                if (mapData[i] == '-' || mapData[i] == '|' || mapData[i] == '\\') {
                    point1 = findVertex(c / 2, r / 2);
                    point2 = findVertex((c + (edgeH ? 1 : 0)) / 2, (r + (edgeV ? 1 : 0)) / 2);
                    point1.edges.Add(point2);
                    point2.edges.Add(point1);
                }

                // If NE/SW edge
                if (edgeH && edgeV) {
                    if (mapData[i] == '/') {
                        point1 = findVertex((c - 1) / 2, (r + 1) / 2);
                        point2 = findVertex((c + 1) / 2, (r - 1) / 2);
                        point1.edges.Add(point2);
                        point2.edges.Add(point1);
                    } else if(mapData[i] == 'X') {
                        point1 = findVertex((c - 1) / 2, (r + 1) / 2);
                        point2 = findVertex((c + 1) / 2, (r - 1) / 2);
                        point1.edges.Add(point2);
                        point2.edges.Add(point1);
                        
                        point1 = findVertex(c / 2, r / 2);
                        point2 = findVertex((c + (edgeH ? 1 : 0)) / 2, (r + (edgeV ? 1 : 0)) / 2);
                        point1.edges.Add(point2);
                        point2.edges.Add(point1);
                    }
                }
            }
        }

        public CartesianGraph(string inFile, bool k) {
            // example said: @"C:\Users\Public\TestFolder\WriteText.txt"
            string text = System.IO.File.ReadAllText(inFile);
            string mapData = getBetween(text, "<MAP>", "</MAP>");
            string[] words = mapData.Split(null);
            // for all vertices
            for(int i = 0; i < words.Length; ++i) {
                if (words[i].Length > 0 && words[i][0] == '(') {
                    Vertex temp = parseVertex(words[i]);
                    vertices.Add(temp);
                }
            }
            // for all edges
            for (int i = 0; i < words.Length; ++i) {
                if (words[i].Length > 0 && char.IsNumber(words[i][0])) {
                    parseEdge(words[i]);
                }
            }
        }

        private List<Vertex> vertices = new List<Vertex>();

        private void iterateGrid(char myChar, ref int r, ref int c) {
            if (myChar == '\n') {
                if (r > -1) ++r;
                c = -1;
            } else {
                ++c;
            }
        }

        // Takes a string and returns non-inclusive substring of text between start and end
        private static string getBetween(string strSource, string strStart, string strEnd) {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd)) {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            } else {
                return "";
            }
        }

        // Returns a vertex with x and y values specified by format (XXYY)
        private static Vertex parseVertex(string vertex) {
            Vertex retVal = new Vertex();
            retVal.x = Int32.Parse(vertex.Substring(1, 2));
            retVal.y = Int32.Parse(vertex.Substring(3, 2));
            return retVal;
        }

        // Interprets edge notation 
        private void parseEdge(string edge) {
            int x = Int32.Parse(edge.Substring(0, 2));
            int y = Int32.Parse(edge.Substring(2, 2));
            string direction = edge.Substring(4, 2);
            int xOffset = 0;
            int yOffset = 0;
            Vertex vertex = findVertex(x, y);
            if (vertex.x == -1) {
                Console.WriteLine("ERROR: Couldn't parse edge.");
                return;
            }

            switch (direction[0]) {
                case 'N': yOffset = -1;
                    break;
                case 'S': yOffset = 1;
                    break;
            }
            switch (direction[1]) {
                case 'E': xOffset = 1;
                    break;
                case 'W': xOffset = -1;
                    break;
            }
            Vertex toVertex = findVertex(vertex.x + xOffset, vertex.y + yOffset);
            addEdge(vertex, toVertex);
            addEdge(toVertex, vertex);
        }

        // Adds an edge to a vertex if its values are between [0, 99]
        private void addEdge(Vertex vertex, Vertex edge) {
            if (edge.x >= 0 && edge.x < 100 && edge.y >= 0 && edge.y < 100)
                vertex.edges.Add(edge);
            else
                Console.WriteLine("ERROR: Edge out of bounds.");
        }

        // Returns a reference to a vertex at the specified (x, y)
        private Vertex findVertex(int x, int y) {
            for (int i = 0; i < vertices.Count(); ++i)
                if (vertices[i].x == x && vertices[i].y == y)
                    return vertices[i];
            Console.WriteLine("ERROR: Vertex not found at coordinates.");
            Vertex retVal = new Vertex();
            retVal.x = -1;
            return retVal;
        }

    }


}