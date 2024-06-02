using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace AStar
{
    public class AStar
    {
        private static readonly bool canMoveVertical = false;

        public static bool FoundPath { get; private set; }
        private static Tile[,] grid;

        private static Tile targetNode;
        private static Tile startingNode;

        public static void GetPath(Tile[,] grid, Tile start, Tile target)
        {
            FindPath(grid, start, target);

            if (FoundPath)
            {
                Tile currentTile = target.parent;

                while (currentTile != startingNode)
                {
                    currentTile.color = Color.LightBlue;

                    if (currentTile.parent != null)
                    {
                        currentTile = currentTile.parent;
                    }
                }
            }
        }
        private static void FindPath(Tile[,] _grid, Tile start, Tile target)
        {
            // Variables needed to run the method
            if (target == null || start == null || _grid == null || target == start) 
                return;

            startingNode = start;
            targetNode = target;
            grid = _grid;
            FoundPath = false;

            #region Reset map
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y] != targetNode && grid[x, y] != startingNode)
                    {
                        grid[x, y].ResetTile();
                    }
                }
            }
            #endregion

            HashSet<Tile> closedNodes = new();
            List<Tile> openNodes = new()
            {
                startingNode
            };

            Tile currentNode = startingNode;

            while (!FoundPath && openNodes.Count > 0)
            {
                for (int i = 0; i < openNodes.Count; i++)
                {
                    if (currentNode == null)
                    {
                        currentNode = openNodes[i];
                    }
                    else if (currentNode.fCost > openNodes[i].fCost || currentNode.fCost == openNodes[i].fCost && currentNode.hCost > openNodes[i].hCost)
                    {
                        currentNode = openNodes[i];
                    }
                }

                #region Check neighbors
                foreach (Tile neighbor in GetNeighbors(currentNode.GridX, currentNode.GridY))
                {
                    if (neighbor == targetNode)
                    {
                        FoundPath = true;
                        targetNode.parent = currentNode;
                        break;
                    }
                    else if (closedNodes.Contains(neighbor) || neighbor.IsSolid)
                        continue;

                    int newPathCost = currentNode.gCost + GetDistance(currentNode, neighbor);

                    if (newPathCost < neighbor.gCost || !openNodes.Contains(neighbor))
                    {
                        openNodes.Add(neighbor);
                        //neighbor.color = Color.Green;

                        SetCosts(grid[currentNode.GridX, currentNode.GridY], neighbor);
                    }
                }
                #endregion

                if (currentNode != targetNode && currentNode != startingNode)
                {
                    //currentNode.color = Color.Red;
                }

                closedNodes.Add(currentNode);
                openNodes.Remove(currentNode);
                currentNode = null;
            }
        }

        private static void SetCosts(Tile currentNode, Tile neighbor)
        {
            #region Set baseCost
            int baseGCost;

            if (currentNode.GridX == neighbor.GridX || currentNode.GridY == neighbor.GridY)
            {
                baseGCost = 10;
            }
            else
            {
                baseGCost = 14;
            }
            #endregion

            #region Set node values
            neighbor.hCost = GetHCost(neighbor);
            neighbor.gCost = currentNode.gCost + baseGCost;
            neighbor.fCost = neighbor.gCost + neighbor.hCost;

            neighbor.parent = currentNode;
            #endregion

            // For debugging
            neighbor.fontSize = Game1.font.MeasureString(neighbor.fCost.ToString());
        }

        private static int GetHCost(Tile currentNode)
        {
            // The cost from the current node to the target
            int cost = 0;

            List<Tile> hasVisited = new()
            {
                currentNode,
            };

            bool foundTarget = false;
            Tile closestNode = currentNode;

            while (!foundTarget)
            {
                int tempCost;
                Tile prevNode = closestNode;

                // "Walk" towards target to calculate final distance

                #region Get the closest node
                foreach (Tile neighbor in GetNeighbors(closestNode.GridX, closestNode.GridY))
                {
                    if (hasVisited.Contains(neighbor))
                    {
                        continue;
                    }

                    float xDistance = MathF.Abs(neighbor.Position.X - targetNode.Position.X);
                    float yDistance = MathF.Abs(neighbor.Position.Y - targetNode.Position.Y);

                    float tempXDistance = MathF.Abs(closestNode.Position.X - targetNode.Position.X);
                    float tempYDistance = MathF.Abs(closestNode.Position.Y - targetNode.Position.Y);

                    if (xDistance < tempXDistance || yDistance < tempYDistance)
                    {
                        closestNode = neighbor;
                        hasVisited.Add(closestNode);
                    }
                }
                #endregion

                #region Determine if the move is vertical or adjacent
                if (closestNode.GridX == prevNode.GridX || closestNode.GridY == prevNode.GridY)
                {
                    tempCost = 10;
                }
                else
                {
                    tempCost = 14;
                }
                #endregion

                cost += tempCost;

                if (closestNode == targetNode)
                {
                    foundTarget = true;
                }
            }

            return cost;
        }

        private static int GetDistance(Tile NodeA, Tile NodeB)
        {
            int distanceX = Math.Abs(NodeA.GridX - NodeB.GridX);
            int distanceY = Math.Abs(NodeA.GridY - NodeB.GridY);

            if (distanceX > distanceY)
            {
                return 14 * distanceY + 10 * (distanceX - distanceY);
            }
            else
            {
                return 14 * distanceX + 10 * (distanceY - distanceX);
            }
        }

        private static List<Tile> GetNeighbors(int positionX, int positionY)
        {
            List<Tile> result = new();
            Tile NeighboringNode;

            #region Adjacent
            if (InBounds(grid, positionX + 1, positionY))
            {
                NeighboringNode = grid[positionX + 1, positionY];

                result.Add(NeighboringNode);
            }
            if (InBounds(grid, positionX - 1, positionY))
            {
                NeighboringNode = grid[positionX - 1, positionY];

                result.Add(NeighboringNode);
            }
            if (InBounds(grid, positionX, positionY + 1))
            {
                NeighboringNode = grid[positionX, positionY + 1];

                result.Add(NeighboringNode);
            }
            if (InBounds(grid, positionX, positionY - 1))
            {
                NeighboringNode = grid[positionX, positionY - 1];

                result.Add(NeighboringNode);
            }
            #endregion

            #region Vertical
            if (InBounds(grid, positionX + 1, positionY + 1))
            {
                NeighboringNode = grid[positionX + 1, positionY + 1];

                result.Add(NeighboringNode);
            }
            if (InBounds(grid, positionX + 1, positionY - 1))
            {
                NeighboringNode = grid[positionX + 1, positionY - 1];

                result.Add(NeighboringNode);
            }
            if (InBounds(grid, positionX - 1, positionY - 1))
            {
                NeighboringNode = grid[positionX - 1, positionY - 1];

                result.Add(NeighboringNode);
            }
            if (InBounds(grid, positionX - 1, positionY + 1))
            {
                NeighboringNode = grid[positionX - 1, positionY + 1];

                result.Add(NeighboringNode);
            }
            #endregion

            return result;
        }

        private static bool InBounds(Tile[,] map, int x, int y)
        {
            return 0 <= y && y < map.GetLength(1) && 0 <= x && x < map.GetLength(0);
        }
    }
}
