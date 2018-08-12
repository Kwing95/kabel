using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kabel {

    public class Vertex {
        public int x;
        public int y;
        public List<Vertex> edges = new List<Vertex>();
        public Vertex() { }
        public Vertex(int xIn, int yIn) {
            x = xIn;
            y = yIn;
        }
        public void print(bool newline) {
            if(newline)
                Console.WriteLine("(" + x + ", " + y + ")");
            else
                Console.Write("(" + x + ", " + y + ")");
        }
    }

    public class CartesianGraph {

        /* Converts text dump to a graph via this format:

        <MAP>

        O-O   O
        |X \ /
        O O O

        </MAP>

        */

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
                    Vertex temp = new Vertex(c / 2, r / 2);
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

                if (mapData[i] == '-' || mapData[i] == '|' || mapData[i] == '\\') {
                    addEdge(edgeH, edgeV, c, r, false);
                }

                // If NE/SW edge
                if (edgeH && edgeV) {
                    if (mapData[i] == '/') {
                        addEdge(edgeH, edgeV, c, r, true);
                    } else if(mapData[i] == 'X') {
                        addEdge(edgeH, edgeV, c, r, true);
                        addEdge(edgeH, edgeV, c, r, false);
                    }
                }
            }
        }

        public List<Vertex> getPaths(int x, int y) {
            return findVertex(x, y).edges;
        }

        public List<Vertex> getPaths(Vertex v) {
            return v.edges;
        }

        private List<Vertex> vertices = new List<Vertex>();

        // Adds edges between two points
        private void addEdge(bool edgeH, bool edgeV, int c, int r, bool ne) {
            Vertex point1 = new Vertex();
            Vertex point2 = new Vertex();
            point1 = ne ? findVertex((c - 1) / 2, (r + 1) / 2) :
                findVertex(c / 2, r / 2);
            point2 = ne ? findVertex((c + 1) / 2, (r - 1) / 2) :
                findVertex((c + (edgeH ? 1 : 0)) / 2, (r + (edgeV ? 1 : 0)) / 2);
            point1.edges.Add(point2);
            point2.edges.Add(point1);
        }

        // Manages the row and column of the current character
        // r = -1 is "standby" for start of grid not read yet
        // c = -1 because \n counts as column 0 for each line
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

        // Returns a reference to a vertex at the specified (x, y)
        public Vertex findVertex(int x, int y) {
            for (int i = 0; i < vertices.Count(); ++i)
                if (vertices[i].x == x && vertices[i].y == y)
                    return vertices[i];
            Console.WriteLine("ERROR: Vertex not found at coordinates.");
            Vertex retVal = new Vertex(-1, 0);
            return retVal;
        }

    }


}