using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace User.Jeffrey.Scripts.MapGenerator
{
    public class MapCell
    {
        private int _indexX;
        private int _indexY;
        private float _mapCellSize;
        
        public bool IsVisited;
        public bool HasKey = false;
        public bool IsExit = false;
        public bool HasUpRoad;
        public bool HasDownRoad;
        public bool HasLeftRoad;
        public bool HasRightRoad;
        
        public void SetMapIndex(int x, int y) { _indexX = x; _indexY = y; }
        public void GetMapIndex(out int x, out int y) { x = _indexX; y = _indexY; }
        public void SetMapCellSize(float mapCellSize) { _mapCellSize = mapCellSize; }

        public Vector2 GetWorldPosition()
        {
            return new Vector2(_indexX * _mapCellSize + _mapCellSize/2.0f, _indexY * _mapCellSize + _mapCellSize/2.0f);
        }
    }
    
    public class MapBuilder
    {
        private MapCell[][] _map;
        private int _mapSize;
        private float _mapCellSize;
        
        private System.Random _random;
        
        public MapCell[][] Build()
        {
            return _map;
        }
        public MapBuilder SetMapSize(int mapSize)
        {   
            _mapSize = mapSize;
            return this;
        }
        public MapBuilder SetMapCellSize(float mapCellSize)
        {   
            _mapCellSize = mapCellSize;
            return this;
        }
        
        public MapBuilder SetSeed(int seed)
        {
            _random = new System.Random(seed);
            return this;
        }
        public MapBuilder Initialize()
        {
            _map = new MapCell[_mapSize][];
            for (int x = 0; x < _mapSize; x++)
            {
                _map[x] = new MapCell[_mapSize];
                for (int y = 0; y < _mapSize; y++)
                {
                    _map[x][y] = new MapCell();
                    _map[x][y].SetMapIndex(x, y);
                    _map[x][y].SetMapCellSize(_mapCellSize);
                }
            }
            return this;
        }

        public MapBuilder GenerateKey()
        {
            int keyCount = 0;
            do
            {   
                var randomX = _random.Next(1, 9);
                var randomY = _random.Next(1, 9);
                var randomCell = _map[randomX][randomY];
                if (!randomCell.HasKey)
                {
                    randomCell.HasKey = true;
                    keyCount++;
                }
            } while (keyCount <= 3);
            
            return this;
        }
        
        public MapBuilder RemoveRandomWall()
        {
            /*
             * 00 01 02 03 04
             * 10 11 12 13 14
             * 20 21 22 23 24
             * 30 31 32 33 34
             * 40 41 42 43 44
             */

            for (int i = 0; i < 3; i++)
            {
                var randomX = _random.Next(1, 9);
                var randomY = _random.Next(1, 9);
                var randomCell = _map[randomX][randomY];
            
                RemoveWalls(randomCell,_map[randomX][randomY + 1]);
                RemoveWalls(randomCell,_map[randomX][randomY - 1]);
                RemoveWalls(randomCell,_map[randomX + 1][randomY]);
                RemoveWalls(randomCell,_map[randomX - 1][randomY]);
            }
            return this;
        }
        
        public MapBuilder GenerateMap()
        {   
            var current = _map[0][0];
            //DFS 
            Stack<MapCell> stack = new Stack<MapCell>();
            current.IsVisited = true;
        
            int visitedCount = 1;
            int totalCells = _mapSize * _mapSize;

            while (visitedCount < totalCells)
            {
                List<MapCell> neighbors = GetUnvisitedNeighbors(current);

                if (neighbors.Count > 0)
                {
                    MapCell next = neighbors[_random.Next(0, neighbors.Count)];
                
                    RemoveWalls(current, next);
                
                    stack.Push(current);
                    current = next;
                    current.IsVisited = true;
                    visitedCount++;
                }
                else if (stack.Count > 0)
                {
                    current = stack.Pop();
                }
            }
            current.IsExit = true;
            Debug.Log("Map Generated！");
            return this;
        }

        private List<MapCell> GetUnvisitedNeighbors(MapCell cell)
        {
            List<MapCell> neighbors = new List<MapCell>();
            cell.GetMapIndex(out int x, out int y);

            if (x > 0 && !_map[x - 1][y].IsVisited) neighbors.Add(_map[x - 1][y]);//left
            if (x < _mapSize - 1 && !_map[x + 1][y].IsVisited) neighbors.Add(_map[x + 1][y]);//right
            if (y > 0 && !_map[x][y - 1].IsVisited) neighbors.Add(_map[x][y - 1]); // down
            if (y < _mapSize - 1 && !_map[x][y + 1].IsVisited) neighbors.Add(_map[x][y + 1]); // up

            return neighbors;
        }

        private void RemoveWalls(MapCell a, MapCell b)
        {   
            a.GetMapIndex(out int ax, out int ay);
            b.GetMapIndex(out int bx, out int by);
            if (ax < bx) { a.HasRightRoad = true; b.HasLeftRoad = true; }
            else if (ax > bx) { a.HasLeftRoad = true; b.HasRightRoad = true; }
            else if (ay < by) { a.HasUpRoad = true; b.HasDownRoad = true; }
            else if (ay > by) { a.HasDownRoad = true; b.HasUpRoad = true; }
        }
    }
}