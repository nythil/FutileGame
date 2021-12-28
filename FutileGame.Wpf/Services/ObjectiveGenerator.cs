using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FutileGame.Models;

namespace FutileGame.Services
{
    public interface IObjectiveGenerator
    {
        void Reset(int seed);
        Board Generate(int numRows, int numColumns, int numRounds);
    }

    public class ObjectiveGenerator : IObjectiveGenerator
    {
        private Random _random = new();

        public Board Generate(int numRows, int numColumns, int numRounds)
        {
            var board = new Board(numRows, numColumns);
            var tiles = board.Tiles.ToList();

            for (int i = 0; i < numRounds; i++)
            {
                var index = _random.Next(tiles.Count);
                tiles[index].Check();
                tiles.RemoveAt(index);
            }

            return board;
        }

        public void Reset(int seed)
        {
            _random = new(seed);
        }
    }
}
