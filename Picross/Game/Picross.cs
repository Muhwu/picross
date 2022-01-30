using System.Linq;

namespace Picross.Game
{
    public class Picross
    {
        private Board _board;
        
        public Picross()
        {
            _board = new Board(15, 15);
        }

        public void Solve()
        {
            _board.SolveIteration();
            _board.SolveIteration();
            _board.Print();
        }
    }
}