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
            _board.GenerateRows(new[] {1, 2, 3, 4}, string.Concat(Enumerable.Repeat("0", 15)));
            //_board.SolveIteration();
            //_board.Print();
        }
    }
}