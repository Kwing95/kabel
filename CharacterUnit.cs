using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kabel { 

    class CharacterUnit {

        public Vertex position;
        public int head, body, limb;
        public Moveset moveset;

    }

    class PlayerUnit : CharacterUnit {

        public PlayerUnit(int xIn, int yIn, int headIn, int bodyIn, int limbIn, Moveset moveIn, CartesianGraph graph) {
            position = graph.findVertex(xIn, yIn);
            head = headIn;
            body = bodyIn;
            limb = limbIn;
            moveset = moveIn;
        }

        public List<Vertex> getMoves(CartesianGraph g) {
            return g.getPaths(position);
        }

    }

    class EnemyUnit : CharacterUnit {

        public EnemyUnit(int xIn, int yIn, Moveset moveIn, CartesianGraph graph) {
            position = graph.findVertex(xIn, yIn);
            head = body = limb = 100;
            moveset = moveIn;
        }
        
    }

}
