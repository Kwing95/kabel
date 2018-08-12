using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kabel {

    abstract class Moveset {

        abstract public Vertex move(Vertex position, CartesianGraph graph);

    }

    class ManualMove : Moveset {

        public override Vertex move(Vertex position, CartesianGraph graph) {
            List<Vertex> options = graph.getPaths(position);
            printOptions(options);
            int selection = -1;
            while (selection < 0 || selection > 9)
                selection = Console.ReadLine()[0] - '0';
            return options[selection];
        }

        public void printOptions(List<Vertex> options) {
            for (int i = 0; i < options.Count(); ++i) {
                // 0: (2, 3)
                Console.Write(i + ": ");
                options[i].print(true);
            }
        }

    }

    class RandomMove : Moveset {

        public override Vertex move(Vertex position, CartesianGraph graph) {

            int numOptions = graph.getPaths(position).Count();

            if (numOptions == 1) {
                lastVisited = position;
                return graph.getPaths(position)[0];
            }

            Random rand = new Random();
            int rng = rand.Next(0, numOptions - 1);

            if (isSame(graph.getPaths(position)[rng], lastVisited))
                rng = numOptions - 1;

            lastVisited = position;
            return graph.getPaths(position)[rng];

        }

        private bool isSame(Vertex v1, Vertex v2) {
            if (v2 == null)
                return false;
            return v1.x == v2.x && v1.y == v2.y;
        }

        private Vertex lastVisited;

    }

    class ScriptedMove : Moveset {

        public ScriptedMove(List<Vertex> patternIn) {
            current = 0;
            pattern = patternIn;
        }

        public override Vertex move(Vertex position, CartesianGraph graph) {
            current = (current + 1) % pattern.Count();
            return pattern[current];
        }

        private List<Vertex> pattern;
        private int current;

    }

}
