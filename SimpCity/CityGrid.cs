using System;
using System.Collections.Generic;

namespace SimpCity {
    /// <summary>
    /// A simple abstraction of X, Y coordinate
    /// </summary>
    public class CityGridPosition {
        public int X { get; set; }
        public int Y { get; set; }

        public CityGridPosition(int x, int y) {
            X = x;
            Y = y;
        }

        public CityGridPosition Clone() {
            return new CityGridPosition(X, Y);
        }

        public override string ToString() {
            return string.Format("X={0}, Y={1}", X, Y);
        }
    }

    /// <summary>
    /// Represents a city of specified grid dimensions.
    /// </summary>
    public class CityGrid {
        // For information about the multidimensional array syntax, see:
        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/multidimensional-
        /// <summary>
        /// A multidimensional array of rows, columns
        /// </summary>
        protected internal readonly CityGridBuilding[,] grid;
        private readonly IDictionary<CityGridBuilding, CityGridPosition> itemPosition;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public CityGrid(int width, int height) {
            grid = new CityGridBuilding[width, height];
            itemPosition = new Dictionary<CityGridBuilding, CityGridPosition>();
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Checks if the position is within boundary.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsWithin(CityGridPosition pos) {
            return pos.X >= 0 && pos.X < Width
                && pos.Y >= 0 && pos.Y < Height;
        }

        /// <summary>
        /// Adds an item into the specified grid  position.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">When the item already has a spot in the grid</exception>
        /// <exception cref="System.IndexOutOfRangeException">When the position is out of bounds</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">When the position is already occupied</exception>
        public void Add(CityGridBuilding item, CityGridPosition pos) {
            if (itemPosition.ContainsKey(item)) {
                throw new System.InvalidOperationException("Item already has a spot in the grid at: " + itemPosition[item]);
            }
            if (!IsWithin(pos)) {
                throw new System.IndexOutOfRangeException("Position not in grid boundary: " + pos);
            }
            if (grid[pos.X, pos.Y] != null) {
                throw new System.ArgumentOutOfRangeException("The position is already occupied: " + pos);
            }
            grid[pos.X, pos.Y] = item;
            itemPosition[item] = pos.Clone();
        }

        /// <summary>
        /// Retrives item at the given current position.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">When the position is out of bounds</exception>
        public CityGridBuilding Get(CityGridPosition pos) {
            if (!IsWithin(pos)) {
                throw new System.ArgumentOutOfRangeException("Position not in grid boundary: " + pos);
            }
            return grid[pos.X, pos.Y];
        }

        /// <summary>
        /// Retrives  current position of the given item.
        /// </summary>
        public CityGridPosition PositionOf(CityGridBuilding item) {
            return itemPosition.ContainsKey(item) ? itemPosition[item].Clone() : null;
        }
    }
}
