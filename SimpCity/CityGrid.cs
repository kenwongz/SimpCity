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

        /// <summary>
        /// Applies an offset to this position.
        /// </summary>
        /// <returns>The new position.</returns>
        public CityGridPosition Offset(CityGridOffset offset) {
            return new CityGridPosition(X + offset.X, Y + offset.Y);
        }

        public CityGridPosition Clone() {
            return new CityGridPosition(X, Y);
        }

        public override string ToString() {
            return string.Format("X={0}, Y={1}", X, Y);
        }
    }

    /// <summary>
    /// A simple abstraction of X, Y offsets
    /// </summary>
    public class CityGridOffset {
        public int X { get; set; }
        public int Y { get; set; }

        public CityGridOffset(int x = 0, int y = 0) {
            X = x;
            Y = y;
        }

        public CityGridOffset Clone() {
            return new CityGridOffset(X, Y);
        }

        public override string ToString() {
            return string.Format("offsetX={0}, offsetY={1}", X, Y);
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
        /// Retrieves an enumerator of orthogonally adjacent offsets.
        /// </summary>
        public static IEnumerable<CityGridOffset> AdjacentOffsets() {
            for (int offY = -1; offY <= 1; offY++) {
                for (int offX = -1; offX <= 1; offX++) {
                    if (offX == 0 && offY == 0) continue;
                    yield return new CityGridOffset(offX, offY);
                }
            }
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
        /// Passively adds an item into the specified grid  position.
        /// Throws if unsuccessful, does nothing otherwise.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">When the item already has a spot in the grid</exception>
        /// <exception cref="System.IndexOutOfRangeException">When the position is out of bounds</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">When the position is already occupied</exception>
        public void PassiveAdd(CityGridBuilding item, CityGridPosition pos) {
            if (itemPosition.ContainsKey(item)) {
                throw new System.InvalidOperationException("Item already has a spot in the grid at: " + itemPosition[item]);
            }
            if (!IsWithin(pos)) {
                throw new System.IndexOutOfRangeException("Position not in grid boundary: " + pos);
            }
            if (grid[pos.X, pos.Y] != null) {
                throw new System.ArgumentOutOfRangeException("The position is already occupied: " + pos);
            }
        }

        /// <summary>
        /// Adds an item into the specified grid  position.
        /// </summary>
        /// <param name="force">Whether the call to <i>PassiveAdd</i> should be skipped.</param>
        /// <exception cref="System.InvalidOperationException">When the item already has a spot in the grid</exception>
        /// <exception cref="System.IndexOutOfRangeException">When the position is out of bounds</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">When the position is already occupied</exception>
        public void Add(CityGridBuilding item, CityGridPosition pos, bool force = false) {
            if (!force) {
                // Propagate any errors forward
                PassiveAdd(item, pos);
            }
            grid[pos.X, pos.Y] = item;
            itemPosition[item] = pos.Clone();
        }

        /// <summary>
        /// Retrives item at the given current position.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">When the position is out of bounds</exception>
        /// <returns>The building item at the position, <i>null</i> if it's empty.</returns>
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

        // TODO: Marked for deprecation
        // We should attempt to encapsulate implementation specifics wherever possible.

        /// <summary>
        /// Retrieves the raw grid in multidimensional array form.
        /// </summary>
        /// <returns></returns>
        public CityGridBuilding[,] GetRawGrid() {
            // TODO: does .Clone() do deep copy on both dimensions?
            return (CityGridBuilding[,])grid.Clone();
        }
    }
}
