using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kabel {
    class Program {
        static void Main(string[] args) {

            CartesianGraph graph = new CartesianGraph("level1.txt");
            PlayerUnit player = new Kabel.PlayerUnit(0, 0, 100, 100, 100, new ManualMove(), graph);
            EnemyUnit randy = new Kabel.EnemyUnit(0, 1, new RandomMove(), graph);
            List<Vertex> path = new List<Vertex>();
            Vertex v1 = new Vertex(1, 1);
            Vertex v2 = new Kabel.Vertex(2, 2);
            Vertex v3 = new Kabel.Vertex(2, 1);
            path.Add(v1);
            path.Add(v2);
            path.Add(v3);
            EnemyUnit cycle = new Kabel.EnemyUnit(1, 1, new ScriptedMove(path), graph);
            int round = 0;

            while (true) {
                Console.Write("Current position: ");
                player.position.print(true);
                Console.Write("Randy: ");
                randy.position.print(true);
                Console.Write("Cycle: ");
                cycle.position.print(true);
                player.position = player.moveset.move(player.position, graph);
                randy.position = randy.moveset.move(randy.position, graph);
                cycle.position = cycle.moveset.move(cycle.position, graph);
                ++round;
            }
            

            Console.WriteLine("Hello World!");

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
