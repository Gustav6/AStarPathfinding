using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    public class AStar
    {
        private static HashSet<Tile> closedNodes;
        private static List<Tile> openNodes;
        private static Tile[,] map;
        private static Tile targetNode;
        private static Tile startingNode;

        private static bool foundPath;
        private static float timer;

        public static List<Tile> Path()
        {
            List<Tile> result = new();

            if (foundPath)
            {
                Tile currentTile = targetNode.parent;

                while (currentTile != startingNode)
                {
                    result.Add(currentTile);

                    currentTile = currentTile.parent;
                }
            }

            return result;
        }

        public static void FindPath(Tile[,] _map, Tile _target, Tile _start)
        {
            // Variables needed to run the method
            if (_target == null || _start == null || _map == null) 
                return;

            targetNode = _target;
            startingNode = _start;

            foundPath = false;

            for (int x = 0; x < _map.GetLength(0); x++)
            {
                for (int y = 0; y < _map.GetLength(1); y++)
                {
                    if (_map[x, y] != targetNode && _map[x, y] != startingNode)
                    {
                        _map[x, y].ResetTile();
                    }
                }
            }

            map = _map;
            int positionX = startingNode.MapPositionX, positionY = startingNode.MapPositionY;

            closedNodes = new HashSet<Tile>()
            {
                { map[positionX, positionY] }
            };

            openNodes = new List<Tile>();

            #region Set cost for starting nodes
            foreach (Tile tile in GetAdjacentNodes(positionX, positionY))
            {
                if (!tile.IsSolid)
                {
                    SetCosts(map[positionX, positionY], tile, false);
                    tile.color = Color.Green;
                    openNodes.Add(tile);
                }
            }

            foreach (Tile tile in GetVerticalNodes(positionX, positionY))
            {
                if (!tile.IsSolid)
                {
                    SetCosts(map[positionX, positionY], tile, true);
                    tile.color = Color.Green;
                    openNodes.Add(tile);
                }
            }
            #endregion

            Tile temp = null;
            float fCost = 0;
            float hCost = 0;

            while (!foundPath)
            {
                for (int i = 0; i < openNodes.Count; i++)
                {
                    if (temp == null)
                    {
                        temp = openNodes[i];

                        fCost = temp.fCost;
                        hCost = temp.hCost;

                    }
                    else
                    {
                        if (openNodes[i].fCost < fCost)
                        {
                            fCost = openNodes[i].fCost;
                            hCost = openNodes[i].hCost;

                            temp = openNodes[i];
                        }
                        else if (openNodes[i].fCost == fCost)
                        {
                            // Runs when the total cost is the same
                            // Then get the node with the least cost from the target

                            if (openNodes[i].hCost < hCost)
                            {
                                // Is closer to the target
                                temp = openNodes[i];
                            }
                        }
                    }
                }

                if (!foundPath)
                {
                    CheckOpenNodes(temp);
                }

                closedNodes.Add(temp);
                openNodes.Remove(temp);
                temp = null;
            }

            for (int i = 0; i < Path().Count; i++)
            {
                Path()[i].color = Color.LightBlue;
            }
        }

        private static void CheckOpenNodes(Tile temp)
        {
            int x = temp.MapPositionX, y = temp.MapPositionY;

            List<Tile> neighbors = new();

            neighbors.AddRange(GetAdjacentNodes(x, y));
            neighbors.AddRange(GetVerticalNodes(x, y));

            foreach (Tile tile in neighbors)
            {
                if (tile == targetNode)
                {
                    foundPath = true;
                    targetNode.parent = temp;
                    break;
                }
            }

            #region Set costs for neigboring nodes
            if (!foundPath)
            {
                foreach (Tile tile in GetAdjacentNodes(x, y))
                {
                    if (!closedNodes.Contains(tile) && !tile.IsSolid)
                    {
                        if (NewPathShorter(tile, temp))
                        {
                        }
                        if (!openNodes.Contains(tile))
                        {
                            openNodes.Add(tile);
                            tile.color = Color.Green;
                            SetCosts(map[x, y], tile, false);
                        }
                    }
                }

                foreach (Tile tile in GetVerticalNodes(x, y))
                {
                    if (!closedNodes.Contains(tile) && !tile.IsSolid)
                    {
                        if (NewPathShorter(tile, temp))
                        {
                        }
                        if (!openNodes.Contains(tile))
                        {
                            openNodes.Add(tile);
                            tile.color = Color.Green;
                            SetCosts(map[x, y], tile, true);
                        }
                    }
                }
            }
            #endregion

            temp.color = Color.Red;
        }

        private static List<Tile> GetAdjacentNodes(int positionX, int positionY)
        {
            List<Tile> result = new();
            Tile NeighboringNode;

            #region Adjacent
            if (InBounds(map, positionX + 1, positionY))
            {
                NeighboringNode = map[positionX + 1, positionY];

                result.Add(NeighboringNode);
            }
            if (InBounds(map, positionX - 1, positionY))
            {
                NeighboringNode = map[positionX - 1, positionY];

                result.Add(NeighboringNode);
            }
            if (InBounds(map, positionX, positionY + 1))
            {
                NeighboringNode = map[positionX, positionY + 1];

                result.Add(NeighboringNode);
            }
            if (InBounds(map, positionX, positionY - 1))
            {
                NeighboringNode = map[positionX, positionY - 1];

                result.Add(NeighboringNode);
            }
            #endregion

            return result;
        }

        private static List<Tile> GetVerticalNodes(int positionX, int positionY)
        {
            List<Tile> result = new ();
            Tile NeighboringNode;

            #region Vertical
            if (InBounds(map, positionX + 1, positionY + 1))
            {
                NeighboringNode = map[positionX + 1, positionY + 1];

                result.Add(NeighboringNode);
            }
            if (InBounds(map, positionX + 1, positionY - 1))
            {
                NeighboringNode = map[positionX + 1, positionY - 1];

                result.Add(NeighboringNode);
            }
            if (InBounds(map, positionX - 1, positionY - 1))
            {
                NeighboringNode = map[positionX - 1, positionY - 1];

                result.Add(NeighboringNode);
            }
            if (InBounds(map, positionX - 1, positionY + 1))
            {
                NeighboringNode = map[positionX - 1, positionY + 1];

                result.Add(NeighboringNode);
            }
            #endregion

            return result;
        }

        private static bool NewPathShorter(Tile neighboringNode, Tile temp)
        {
            int x = startingNode.MapPositionX, y = startingNode.MapPositionY;
            List<Tile> hasVisited = new(), newPath = new();
            Tile closestNode = startingNode;
            bool foundTarget = false;
            int totalCost = 0;

            while (!foundTarget)
            {
                int tempCost = 0;

                List<Tile> adjacentNodes = new(), verticalNodes = new();
                adjacentNodes.AddRange(GetAdjacentNodes(x, y));
                verticalNodes.AddRange(GetVerticalNodes(x, y));

                // "Walk" towards target to calculate final distance

                #region Check adjacent nodes
                for (int j = 0; j < adjacentNodes.Count; j++)
                {
                    if (!hasVisited.Contains(adjacentNodes[j]))
                    {
                        float xDistance = MathF.Abs(adjacentNodes[j].Position.X - neighboringNode.Position.X);
                        float yDistance = MathF.Abs(adjacentNodes[j].Position.Y - neighboringNode.Position.Y);

                        float tempXDistance = MathF.Abs(closestNode.Position.X - neighboringNode.Position.X);
                        float tempYDistance = MathF.Abs(closestNode.Position.Y - neighboringNode.Position.Y);

                        if (xDistance < tempXDistance || yDistance < tempYDistance)
                        {
                            tempCost = 10;
                            closestNode = adjacentNodes[j];

                            if (closestNode != startingNode)
                            {
                                hasVisited.Add(closestNode);
                            }
                        }
                    }
                }
                #endregion

                #region Check vertical nodes

                for (int j = 0; j < verticalNodes.Count; j++)
                {
                    if (!hasVisited.Contains(verticalNodes[j]))
                    {
                        float xDistance = MathF.Abs(verticalNodes[j].Position.X - neighboringNode.Position.X);
                        float yDistance = MathF.Abs(verticalNodes[j].Position.Y - neighboringNode.Position.Y);

                        float tempXDistance = MathF.Abs(closestNode.Position.X - neighboringNode.Position.X);
                        float tempYDistance = MathF.Abs(closestNode.Position.Y - neighboringNode.Position.Y);

                        if (xDistance < tempXDistance || yDistance < tempYDistance)
                        {
                            tempCost = 14;
                            closestNode = verticalNodes[j];

                            if (closestNode != startingNode)
                            {
                                hasVisited.Add(closestNode);
                            }
                        }
                    }
                }
                #endregion

                if (closestNode == neighboringNode)
                {
                    foundTarget = true;
                }
                else if (closestNode != null)
                {
                    x = closestNode.MapPositionX;
                    y = closestNode.MapPositionY;
                }

                totalCost += tempCost;
            }

            if (totalCost < neighboringNode.gCost)
            {
                for (int i = 0; i < newPath.Count; i++)
                {
                    if (i > 0)
                    {
                        newPath[i].parent = newPath[i - 1];
                    }
                }

                return true;
            }

            return false;
        }

        private static void SetCosts(Tile currentNode, Tile neighboringNode, bool isVertical)
        {
            #region BaseCost
            // Set cost for current node
            int baseGCost;

            if (isVertical) // GCost for vertical nodes ≈ SQRT(2) * 10
            {
                baseGCost = 14;
            }
            else // GCost for adjacent nodes = 1 * 10
            {
                baseGCost = 10;
            }
            #endregion

            #region Set node values
            neighboringNode.hCost = GetHCost(neighboringNode, targetNode);
            neighboringNode.gCost = currentNode.gCost + baseGCost;
            neighboringNode.fCost = neighboringNode.gCost + neighboringNode.hCost;

            neighboringNode.parent = currentNode;
            #endregion

            // For debugging
            neighboringNode.fontSize = Game1.font.MeasureString(neighboringNode.fCost.ToString());
        }

        private static int GetHCost(Tile NeighboringNode, Tile target)
        {
            // The cost from the current node to the target
            int totalHCost = 0;

            int x = NeighboringNode.MapPositionX, y = NeighboringNode.MapPositionY;
            List<Tile> hasVisited = new();
            bool foundTarget = false;

            while (!foundTarget)
            {
                int tempCost = 0;
                Tile closestNode = null;

                List<Tile> adjacentNodes = new(), verticalNodes = new();
                adjacentNodes.AddRange(GetAdjacentNodes(x, y));
                verticalNodes.AddRange(GetVerticalNodes(x, y));

                // "Walk" towards target to calculate final distance

                #region Check adjacent nodes
                for (int j = 0; j < adjacentNodes.Count; j++)
                {
                    if (closestNode == null)
                    {
                        closestNode = adjacentNodes[j];
                        hasVisited.Add(closestNode);
                        tempCost = 10;
                    }
                    else
                    {
                        if (!hasVisited.Contains(adjacentNodes[j]))
                        {
                            float xDistance = MathF.Abs(adjacentNodes[j].Position.X - target.Position.X);
                            float yDistance = MathF.Abs(adjacentNodes[j].Position.Y - target.Position.Y);

                            float tempXDistance = MathF.Abs(closestNode.Position.X - target.Position.X);
                            float tempYDistance = MathF.Abs(closestNode.Position.Y - target.Position.Y);

                            if (xDistance < tempXDistance || yDistance < tempYDistance)
                            {
                                tempCost = 10;
                                closestNode = adjacentNodes[j];
                                hasVisited.Add(closestNode);
                            }
                        }
                    }
                }
                #endregion

                #region Check vertical nodes

                for (int j = 0; j < verticalNodes.Count; j++)
                {
                    if (closestNode == null && openNodes.Contains(adjacentNodes[j]))
                    {
                        closestNode = verticalNodes[j];
                        hasVisited.Add(closestNode);
                        tempCost = 14;
                    }
                    else
                    {
                        if (!hasVisited.Contains(verticalNodes[j]))
                        {
                            float xDistance = MathF.Abs(verticalNodes[j].Position.X - target.Position.X);
                            float yDistance = MathF.Abs(verticalNodes[j].Position.Y - target.Position.Y);

                            float tempXDistance = MathF.Abs(closestNode.Position.X - target.Position.X);
                            float tempYDistance = MathF.Abs(closestNode.Position.Y - target.Position.Y);

                            if (xDistance < tempXDistance || yDistance < tempYDistance)
                            {
                                tempCost = 14;
                                closestNode = verticalNodes[j];
                                hasVisited.Add(closestNode);
                            }
                        }
                    }
                }
                #endregion

                if (closestNode == target)
                {
                    foundTarget = true;
                }
                else if (closestNode != null)
                {
                    x = closestNode.MapPositionX;
                    y = closestNode.MapPositionY;
                }

                totalHCost += tempCost;
            }

            return totalHCost;
        }

        private static bool InBounds(Tile[,] map, int x, int y)
        {
            return 0 <= y && y < map.GetLength(1) && 0 <= x && x < map.GetLength(0);
        }
    }
}
