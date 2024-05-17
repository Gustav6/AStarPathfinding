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
        private static bool foundPath;
        private static Tile target;

        public static Tile[] Path()
        {
            Tile[] result = Array.Empty<Tile>();

            return result;
        }

        public static void FindPath(Tile[,] _map, Tile _target, Tile start)
        {
            // Variables needed to run the method
            if (_target == null || start == null || _map == null) 
                return;

            target = _target;

            for (int x = 0; x < _map.GetLength(0); x++)
            {
                for (int y = 0; y < _map.GetLength(1); y++)
                {
                    if (_map[x, y] != target && _map[x, y] != start)
                    {
                        _map[x, y].ResetTile();
                    }
                }
            }

            foundPath = false;
            map = _map;
            int positionX = start.MapPositionX, positionY = start.MapPositionY;

            closedNodes = new HashSet<Tile>()
            {
                { map[positionX, positionY] }
            };

            openNodes = new List<Tile>();

            #region Set cost for starting nodes

            bool canAddVertical = true;

            foreach (Tile tile in GetAdjacentNodes(positionX, positionY))
            {
                if (tile.fCost == 0 && !tile.IsSolid)
                {
                    SetCosts(map[positionX, positionY], tile, target, false);
                    openNodes.Add(tile);
                }
                else if (tile.IsSolid)
                {
                    canAddVertical = false;
                }
            }

            if (canAddVertical)
            {
                foreach (Tile tile in GetVerticalNodes(positionX, positionY))
                {
                    if (tile.fCost == 0 && !tile.IsSolid)
                    {
                        SetCosts(map[positionX, positionY], tile, target, true);
                        openNodes.Add(tile);
                    }
                }
            }
            #endregion

            //while (!foundPath)
            //{
            //    float fCost = openNodes.First().fCost;
            //    float hCost = openNodes.First().hCost;

            //    Tile temp = openNodes.First();

            //    for (int i = 0; i < openNodes.Count; i++)
            //    {
            //        #region Found path
            //        if (openNodes[i].hCost == 0)
            //        {
            //            // Found path when cost from target is 0
            //            foundPath = true;
            //            break;
            //        }
            //        #endregion

            //        #region Get node with lowest f cost
            //        if (openNodes[i].fCost < fCost)
            //        {
            //            fCost = openNodes[i].fCost;
            //            hCost = openNodes[i].hCost;

            //            temp = openNodes[i];
            //        }
            //        else if (openNodes[i].fCost == fCost)
            //        {
            //            // Runs when the total cost is the same
            //            // Then get the node with the least cost from the target

            //            if (openNodes[i].hCost < hCost)
            //            {
            //                // Is closer to the target
            //                temp = openNodes[i];
            //            }
            //        }
            //        #endregion
            //    }

            //    if (!foundPath)
            //    {
            //        CheckOpenNodes(target, temp);
            //    }
            //}
        }

        public static void Temp()
        {
            float fCost = openNodes.First().fCost;
            float hCost = openNodes.First().hCost;

            Tile temp = openNodes.First();

            for (int i = 0; i < openNodes.Count; i++)
            {
                #region Found path
                if (openNodes[i].hCost == 0)
                {
                    // Found path when cost from target is 0
                    foundPath = true;
                    break;
                }
                #endregion

                #region Get node with lowest f cost
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
                #endregion
            }

            if (!foundPath)
            {
                CheckOpenNodes(target, temp);
            }
        }

        private static void CheckOpenNodes(Tile target, Tile temp)
        {
            int x = temp.MapPositionX, y = temp.MapPositionY;
            bool neighborIsTarget = false;

            List<Tile> neighbors = new();

            neighbors.AddRange(GetAdjacentNodes(x, y));
            neighbors.AddRange(GetVerticalNodes(x, y));

            foreach (Tile tile in neighbors)
            {
                if (tile == target)
                {
                    neighborIsTarget = true;
                    foundPath = true;
                }
            }

            #region Set costs for neigboring nodes
            if (!neighborIsTarget)
            {
                bool canAddVertical = true;

                foreach (Tile tile in GetAdjacentNodes(x, y))
                {
                    if (!closedNodes.Contains(tile))
                    {
                        if (tile.fCost == 0 && !tile.IsSolid)
                        {
                            openNodes.Add(tile);
                            SetCosts(map[x, y], tile, target, false);
                        }
                        else if (tile.IsSolid)
                        {
                            canAddVertical = false;
                        }
                    }
                }

                if (canAddVertical)
                {
                    foreach (Tile tile in GetVerticalNodes(x, y))
                    {
                        if (!closedNodes.Contains(tile))
                        {
                            if (tile.fCost == 0 && !tile.IsSolid)
                            {
                                openNodes.Add(tile);
                                SetCosts(map[x, y], tile, target, true);
                            }
                        }
                    }
                }
            }
            #endregion

            closedNodes.Add(temp);
            openNodes.Remove(temp);

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

        private static void SetCosts(Tile currentNode, Tile NeighboringNode, Tile target, bool isVertical)
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

            // The cost from the current node to the target
            int totalCost = 0;

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

                totalCost += tempCost;
            }

            NeighboringNode.hCost = totalCost;
            NeighboringNode.gCost = currentNode.gCost + baseGCost;
            NeighboringNode.fCost = NeighboringNode.gCost + NeighboringNode.hCost;

            NeighboringNode.fontSize = Game1.font.MeasureString(NeighboringNode.fCost.ToString());
        }

        private static bool InBounds(Tile[,] map, int x, int y)
        {
            return 0 <= y && y < map.GetLength(1) && 0 <= x && x < map.GetLength(0);
        }
    }
}
