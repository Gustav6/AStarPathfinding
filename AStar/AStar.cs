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
        private static Tile[,] nodeMap;

        public static void FindPath(Tile[,] map, Tile target, int startPosX, int startPosY)
        {
            // Variables needed to run the method
            if (map == null || target == null || map[startPosX, startPosY] == null) 
                return;

            bool foundPath = false;
            nodeMap = map;

            closedNodes = new HashSet<Tile>()
            {
                { map[startPosX, startPosY] }
            };

            openNodes = new List<Tile>();

            #region Set cost for starting nodes
            foreach (Tile tile in GetAdjacentNodes(startPosX, startPosY))
            {
                if (!tile.IsSolid)
                {
                    SetCosts(map[startPosX, startPosY], tile, target, false);
                    openNodes.Add(tile);
                }
            }

            foreach (Tile tile in GetVerticalNodes(startPosX, startPosY))
            {
                if (!tile.IsSolid)
                {
                    SetCosts(map[startPosX, startPosY], tile, target, true);
                    openNodes.Add(tile);
                }
            }
            #endregion

            while (!foundPath)
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
                    closedNodes.Add(temp);

                    int positionX = temp.MapPositionX, positionY = temp.MapPositionY;

                    #region Set costs for neigboring nodes
                    foreach (Tile tile in GetAdjacentNodes(positionX, positionY))
                    {
                        //if (tile == target)
                        //{
                        //    foundPath = true;
                        //    break;
                        //}

                        if (!tile.IsSolid)
                        {
                            openNodes.Add(tile);
                            SetCosts(map[startPosX, startPosY], tile, target, false);
                        }
                    }

                    foreach (Tile tile in GetVerticalNodes(positionX, positionY))
                    {
                        //if (tile == target)
                        //{
                        //    foundPath = true;
                        //    break;
                        //}

                        if (!tile.IsSolid)
                        {
                            openNodes.Add(tile);
                            SetCosts(map[startPosX, startPosY], tile, target, true);
                        }
                    }
                }
                #endregion
            }

            if (foundPath)
            {
                Debug.WriteLine("FoundPath");
            }
        }

        private static List<Tile> GetAdjacentNodes(int positionX, int positionY)
        {
            List<Tile> result = new();
            Tile NeighboringNode;

            #region Adjacent
            if (InBounds(nodeMap, positionX + 1, positionY))
            {
                NeighboringNode = nodeMap[positionX + 1, positionY];

                if (!closedNodes.Contains(NeighboringNode))
                {
                    result.Add(NeighboringNode);
                }
            }
            if (InBounds(nodeMap, positionX - 1, positionY))
            {
                NeighboringNode = nodeMap[positionX - 1, positionY];

                if (!closedNodes.Contains(NeighboringNode))
                {
                    result.Add(NeighboringNode);
                }
            }
            if (InBounds(nodeMap, positionX, positionY + 1))
            {
                NeighboringNode = nodeMap[positionX, positionY + 1];

                if (!closedNodes.Contains(NeighboringNode))
                {
                    result.Add(NeighboringNode);
                }
            }
            if (InBounds(nodeMap, positionX, positionY - 1))
            {
                NeighboringNode = nodeMap[positionX, positionY - 1];

                if (!closedNodes.Contains(NeighboringNode))
                {
                    result.Add(NeighboringNode);
                }
            }

            //if (!openNodes.Contains(NeighboringNode) && !closedNodes.Contains(NeighboringNode))
            //{
            //    result.Add(NeighboringNode);
            //}
            #endregion


            return result;
        }

        private static List<Tile> GetVerticalNodes(int positionX, int positionY)
        {
            List<Tile> result = new ();
            Tile NeighboringNode;

            #region Vertical
            if (InBounds(nodeMap, positionX + 1, positionY + 1))
            {
                NeighboringNode = nodeMap[positionX + 1, positionY + 1];

                if (!closedNodes.Contains(NeighboringNode))
                {
                    result.Add(NeighboringNode);
                }
            }
            if (InBounds(nodeMap, positionX + 1, positionY - 1))
            {
                NeighboringNode = nodeMap[positionX + 1, positionY - 1];

                if (!closedNodes.Contains(NeighboringNode))
                {
                    result.Add(NeighboringNode);
                }
            }
            if (InBounds(nodeMap, positionX - 1, positionY - 1))
            {
                NeighboringNode = nodeMap[positionX - 1, positionY - 1];

                if (!closedNodes.Contains(NeighboringNode))
                {
                    result.Add(NeighboringNode);
                }
            }
            if (InBounds(nodeMap, positionX - 1, positionY + 1))
            {
                NeighboringNode = nodeMap[positionX - 1, positionY + 1];

                if (!closedNodes.Contains(NeighboringNode))
                {
                    result.Add(NeighboringNode);
                }
            }

            //if (!openNodes.Contains(NeighboringNode) && !closedNodes.Contains(NeighboringNode))
            //{
            //    result.Add(NeighboringNode);
            //}
            #endregion

            return result;
        }

        private static void SetCosts(Tile currentNode, Tile NeighboringNode, Tile target, bool isVeritcal)
        {
            // Set cost for current node
            int baseGCost;

            if (isVeritcal)
            {
                // Special gCost for vertical nodes, the cost = SQRT(2) but we multiply with 10
                // This is because we want numbers to work with this will be 14.

                baseGCost = 14;
            }
            else
            {
                // The cost for adjacent tiles will be distance of 1 multiplied with 10

                baseGCost = 10;
            }

            // Decide the cost from the current node to the target

            int distanceX, distanceY;

            distanceX = Math.Abs(NeighboringNode.MapPositionX - target.MapPositionX);
            distanceY = Math.Abs(NeighboringNode.MapPositionY - target.MapPositionY);

            if (distanceX > 1 || distanceY > 1)
            {
                // "Walk" towards target to calculate final distance

                int stepReq, totalCost = 0;

                if (distanceX > distanceY)
                    stepReq = distanceX;
                else
                    stepReq = distanceY;

                int x = currentNode.MapPositionX, y = currentNode.MapPositionY;

                // Check neigbors to determine next temp tile and count cost
                for (int i = 0; i < stepReq; i++)
                {
                    int tempCost = 0;
                    Tile closestNode = null;

                    List<Tile> verticalNodes = new();
                    verticalNodes.AddRange(GetVerticalNodes(x, y));

                    #region Set distance from target
                    for (int j = 0; j < verticalNodes.Count; j++)
                    {
                        int xDistance = Math.Abs(verticalNodes[j].MapPositionX - target.MapPositionX);
                        int yDistance = Math.Abs(verticalNodes[j].MapPositionY - target.MapPositionY);

                        if (closestNode == null)
                        {
                            closestNode = verticalNodes.First();
                        }
                        else
                        {
                            int tempXDistance = Math.Abs(closestNode.MapPositionX - target.MapPositionX);
                            int tempYDistance = Math.Abs(closestNode.MapPositionY - target.MapPositionY);

                            if (xDistance > tempXDistance || yDistance > tempYDistance)
                            {
                                tempCost = 14;
                                closestNode = verticalNodes[j];
                            }
                        }
                    }

                    List<Tile> adjacentNodes = new();
                    adjacentNodes.AddRange(GetAdjacentNodes(x, y));

                    for (int j = 0; j < adjacentNodes.Count; j++)
                    {
                        int xDistance = Math.Abs(adjacentNodes[j].MapPositionX - target.MapPositionX);
                        int yDistance = Math.Abs(adjacentNodes[j].MapPositionY - target.MapPositionY);

                        if (closestNode == null)
                        {
                            closestNode = adjacentNodes.First();
                        }
                        else
                        {
                            int tempXDistance = Math.Abs(closestNode.MapPositionX - target.MapPositionX);
                            int tempYDistance = Math.Abs(closestNode.MapPositionY - target.MapPositionY);

                            if (xDistance > tempXDistance || yDistance > tempYDistance)
                            {
                                tempCost = 10;
                                closestNode = adjacentNodes[j];
                            }
                        }
                    }
                    #endregion

                    if (closestNode != null)
                    {
                        x = closestNode.MapPositionX;
                        y = closestNode.MapPositionY;
                    }
                    totalCost += tempCost;
                }

                NeighboringNode.hCost = totalCost;
            }
            else if (distanceX == 1 || distanceY == 1)
            {
                // Check if vertical move can be done
                if (distanceX == distanceY)
                {
                    NeighboringNode.hCost = 14;
                }
                else
                {
                    NeighboringNode.hCost = 10;
                }
            }

            NeighboringNode.gCost = currentNode.gCost + baseGCost;

            NeighboringNode.fCost = NeighboringNode.gCost + NeighboringNode.hCost;
        }

        private static bool InBounds(Tile[,] map, int x, int y)
        {
            return 0 <= y && y < map.GetLength(1) && 0 <= x && x < map.GetLength(0);
        }
    }
}
