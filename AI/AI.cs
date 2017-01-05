using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Security.Permissions;

namespace Battleships
{

    /*
     * A simple AI, if there's no known "targets" it will choose one at random.
     * If there's a better option (3 in the known board), it will choose one of those at random
     * to avoid predictability. 
     * 
     */

    public class AI
    {
        List<List<int>> knownBoard = new List<List<int>>();
        /* Will have integers as identifiers.
         * 0 - unknown
         * 1 - hit
         * 2 - hit and ship
         * 3 - next to ship (might also be ship)
         * 4 - dead ship
         * 5 - next to ship, aka nothing
         * */
        List<Tuple<int, int>> targetList = new List<Tuple<int, int>>();

        Storage xmlStorage = new Storage();

        public AI()
        {
            initKnownBoard();
        }


        public void loadKnownBoard()
        {

            XDocument db = XDocument.Load(xmlStorage.getPath());

            var tmp = from node in db.Root.Element("AI-Board").Elements("Node") select node;

            int x, y, value;

            foreach (XElement node in tmp)
            {
                x = Int32.Parse(node.Element("X").Value);
                y = Int32.Parse(node.Element("Y").Value);
                value = Int32.Parse(node.Element("Value").Value);

                knownBoard[y][x] = value;
            }
            printBoard();
        }

        public void playTurn(Board targetBoard)
        {
            Console.WriteLine("Playing AI turn");
            Tuple<int, int> target;
            do
            {
                target = chooseFireCoords();
                Console.WriteLine("AI-Target: " + target);
            } while (!targetBoard.fire(target.Item1, target.Item2));

            if (targetBoard.isShip(target.Item1, target.Item2))
            {
                updateKnownBoard(target.Item1, target.Item2, 2);    //Hit a ship

                if (target.Item1 < 9)
                {
                    if (knownBoard[target.Item2][target.Item1 + 1] == 2)
                    {
                        Tuple<int, int> start = new Tuple<int, int>(target.Item1, target.Item2);
                        Tuple<int, int> end = new Tuple<int, int>(target.Item1 + 1, target.Item2);
                        damagedShipAt(start, end);
                    }
                }
                if (target.Item1 > 0)
                {
                    if (knownBoard[target.Item2][target.Item1 - 1] == 2)
                    {
                        Tuple<int, int> end = new Tuple<int, int>(target.Item1, target.Item2);
                        Tuple<int, int> start = new Tuple<int, int>(target.Item1 - 1, target.Item2);
                        damagedShipAt(start, end);
                    }
                }
                if (target.Item2 < 9)
                {
                    if (knownBoard[target.Item2 + 1][target.Item1] == 2)
                    {
                        Tuple<int, int> start = new Tuple<int, int>(target.Item1, target.Item2);
                        Tuple<int, int> end = new Tuple<int, int>(target.Item1, target.Item2 + 1);
                        damagedShipAt(start, end);

                    }
                }
                if (target.Item2 > 0)
                {
                    if (knownBoard[target.Item2 - 1][target.Item1] == 2)
                    {
                        Tuple<int, int> end = new Tuple<int, int>(target.Item1, target.Item2);
                        Tuple<int, int> start = new Tuple<int, int>(target.Item1, target.Item2 - 1);
                        damagedShipAt(start, end);
                    }
                }
                updateKnownBoard(target.Item1 + 1, target.Item2, 3);  //Update the surrounding nodes
                updateKnownBoard(target.Item1 - 1, target.Item2, 3);
                updateKnownBoard(target.Item1, target.Item2 + 1, 3);
                updateKnownBoard(target.Item1, target.Item2 - 1, 3);

                if (targetBoard.getShip(target.Item1, target.Item2).getDead())
                {
                    deadShipAt(targetBoard.getShip(target.Item1, target.Item2).getStart(), targetBoard.getShip(target.Item1, target.Item2).getEnd());
                    updateKnownBoard(target.Item1, target.Item2, 4);
                }


            }
            else
            {
                updateKnownBoard(target.Item1, target.Item2, 1);
            }

        }

        private void damagedShipAt(Tuple<int, int> start, Tuple<int, int> end)
        {
            if (start.Item1 == end.Item1)
            {
                for (int i = start.Item2; i < 10; ++i)
                {
                    if (knownBoard[i][start.Item1] == 2)
                    {
                        updateKnownBoard(start.Item1 + 1, i, 5);
                        updateKnownBoard(start.Item1 - 1, i, 5);
                    }
                    else if (knownBoard[i][start.Item1] == 0)
                    {
                        break;
                    }
                }
            }
            else if (start.Item2 == end.Item2)
            {
                for (int i = start.Item1; i < 10; ++i)
                {
                    if (knownBoard[start.Item2][i] == 2)
                    {
                        updateKnownBoard(i, start.Item2 + 1, 5);
                        updateKnownBoard(i, start.Item2 - 1, 5);
                    }
                    else if (knownBoard[start.Item2][i] == 0)
                    {
                        break;
                    }
                }
            }

        }

        private void deadShipAt(Tuple<int, int> start, Tuple<int, int> end)
        {
            int xMin = Math.Min(start.Item1, end.Item1);
            int yMin = Math.Min(start.Item2, end.Item2);
            int xMax = Math.Max(start.Item1, end.Item1);
            int yMax = Math.Max(start.Item2, end.Item2);
            if (start.Item1 == end.Item1)
            {
                for (int i = yMin; i <= yMax; ++i)
                {
                    updateKnownBoard(xMin, i, 4);
                    updateKnownBoard(xMin + 1, i, 5);
                    updateKnownBoard(xMin - 1, i, 5);
                    updateKnownBoard(xMin, i + 1, 5);
                    updateKnownBoard(xMin, i - 1, 5);

                }
            }
            else if (start.Item2 == end.Item2)
            {
                for (int i = xMin; i <= xMax; ++i)
                {
                    updateKnownBoard(i, yMin, 4);
                    updateKnownBoard(i, yMin + 1, 5);
                    updateKnownBoard(i, yMin - 1, 5);
                    updateKnownBoard(i + 1, yMin, 5);
                    updateKnownBoard(i - 1, yMin, 5);
                }
            }


        }

        void initKnownBoard()
        {
            for (int y = 0; y < 10; ++y)
            {
                List<int> tmpList = new List<int>();
                for (int x = 0; x < 10; ++x)
                {
                    tmpList.Add(0);
                }
                knownBoard.Add(tmpList);    //add the inner List to the outer List
            }

        }

        void updateKnownBoard(int x, int y, int newValue)
        {

            //If hit, update the node to show hit ship, and update surrounding nodes to "next to ship"
            if (x < 10 && x >= 0 && y < 10 && y >= 0 && newValue <= 5 && newValue >= 0)
            {
                switch (this.knownBoard[y][x])
                {
                    case 0:
                        this.knownBoard[y][x] = newValue;
                        Console.WriteLine("[AI.updateKnownBoard] Updating x: " + x + ", y: " + y + " to " + newValue);
                        printBoard();
                        xmlStorage.updateKnownNodes(x, y, newValue);
                        break;
                    case 2:
                        if (newValue == 4)
                        {
                            this.knownBoard[y][x] = newValue;
                            Console.WriteLine("[AI.updateKnownBoard] Updating x: " + x + ", y: " + y + " to " + newValue);
                            printBoard();
                            xmlStorage.updateKnownNodes(x, y, newValue);
                        }
                        break;
                    case 3:
                        if (newValue != 0)
                        {
                            this.knownBoard[y][x] = newValue;
                            Console.WriteLine("[AI.updateKnownBoard] Updating x: " + x + ", y: " + y + " to " + newValue);
                            printBoard();
                            xmlStorage.updateKnownNodes(x, y, newValue);
                        }
                        break;
                    case 5:
                        if (newValue == 4)
                        {
                            this.knownBoard[y][x] = newValue;
                            Console.WriteLine("[AI.updateKnownBoard] Updating x: " + x + ", y: " + y + " to " + newValue);
                            xmlStorage.updateKnownNodes(x, y, newValue);
                        }
                        break;
                    default:
                        Console.WriteLine("[AI.updateKnownBoard] Nothing updated");
                        break;
                }
            }
            else
            {
                Console.WriteLine("[AI.updateKnownBoard] Coords out of range");
            }
            Console.WriteLine("[AI.updateKnownBoard] Board updated");
        }

        /*
         * Works both horizontally and vertically! 
         * */
        List<Tuple<int, int>> findDeadShips(int x, int y)
        {
            Console.WriteLine("[AI.findDeadShips] Entering function");
            bool hitShipFound = false;
            int shipLengthCounter = 0;
            bool deadShipFound = false;
            bool hitFound = false;

            List<Tuple<int, int>> deadShipCoords = new List<Tuple<int, int>>();

            //Check the horizontal row for dead ships
            for (int x_ = 0; x_ < 10; ++x_)
            {
                if (knownBoard[y][x_] == 0)
                {
                    hitShipFound = false;
                    shipLengthCounter = 0;
                    deadShipFound = false;
                    hitFound = false;
                    deadShipCoords.Clear();
                }
                if (knownBoard[y][x_] == 1)
                {
                    Console.WriteLine("[AI.findDeadShips] hitFound!");
                    hitFound = true;
                }
                if (knownBoard[y][x_] == 2 && (hitFound || x_ == 0 || hitShipFound))
                {
                    Console.WriteLine("[AI.findDeadShips] hitShipFound!");
                    hitShipFound = true;
                    hitFound = false;
                    shipLengthCounter++;
                    Tuple<int, int> tmpPos = new Tuple<int, int>(x_, y);
                    deadShipCoords.Add(tmpPos);
                }
                if (hitShipFound && shipLengthCounter > 1 && hitFound)
                {
                    deadShipFound = true;
                    Console.WriteLine("[AI.findDeadShips] Found a dead ship!");
                    return deadShipCoords;
                }
            }

            //Check vertically for dead ships
            Console.WriteLine("Checking vertically for dead ships!");
            hitShipFound = false;
            shipLengthCounter = 0;
            deadShipFound = false;
            hitFound = false;

            for (int y_ = 0; y_ < 10; ++y_)
            {
                if (knownBoard[y_][x] == 0)
                {
                    hitShipFound = false;
                    shipLengthCounter = 0;
                    deadShipFound = false;
                    hitFound = false;
                    deadShipCoords.Clear();
                }
                if (knownBoard[y_][x] == 1)
                {
                    Console.WriteLine("[AI.findDeadShips] hitFound!");
                    hitFound = true;
                }
                if (knownBoard[y_][x] == 2 && (hitFound || y_ == 0 || hitShipFound))
                {
                    Console.WriteLine("[AI.findDeadShips] hitShipFound!");
                    hitShipFound = true;
                    hitFound = false;
                    shipLengthCounter++;
                    Tuple<int, int> tmpPos = new Tuple<int, int>(x, y_);
                    deadShipCoords.Add(tmpPos);
                }
                if (hitShipFound && knownBoard[y_][x] == 1 && shipLengthCounter > 1 && hitFound)
                {
                    deadShipFound = true;
                    Console.WriteLine("[AI.findDeadShips] Found a dead ship!");
                    break;
                }
            }

            if (!deadShipFound)
                deadShipCoords.Clear();

            return deadShipCoords;
        }

        Tuple<int, int> chooseFireCoords()
        {
            Random random = new Random();
            do
            {
                if (searchForTarget(3)) //search for a  "3" in the known board, since they're a prio
                {
                    int randomNumber = random.Next(0, targetList.Count - 1);
                    Tuple<int, int> target = new Tuple<int, int>(targetList.ElementAt(randomNumber).Item1, targetList.ElementAt(randomNumber).Item2);
                    targetList.Clear();
                    return target;
                }
                else
                {
                    if (searchForTarget(0)) //take a random tile not yet fired upon
                    {
                        int randomNumber = random.Next(0, targetList.Count - 1);
                        Tuple<int, int> target = new Tuple<int, int>(targetList.ElementAt(randomNumber).Item1, targetList.ElementAt(randomNumber).Item2);
                        targetList.Clear();
                        return target;
                    }
                }
            } while (true);
        }

        void printBoard()
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (knownBoard[y][x] != 0)
                        Console.Write("[" + knownBoard[y][x] + "]");
                    else
                        Console.Write("[ ]");
                }
                Console.Write("\n");
            }

        }

        bool searchForTarget(int value) //search for value in the known board by looping through it
        {
            bool foundTargets = false;

            for (int y = 0; y < 10; ++y)
            {
                for (int x = 0; x < 10; ++x)
                {
                    if (knownBoard[y][x] == value)
                    {
                        Tuple<int, int> target = new Tuple<int, int>(x, y);
                        targetList.Add(target);
                        foundTargets = true;
                    }
                }
            }

            return foundTargets;
        }

        public Tuple<Tuple<int, int>, Tuple<int, int>> placeShip(Ship s)
        {

            Random random = new Random();
            int randomX = random.Next(0, 10);
            int randomY = random.Next(0, 10);
            int randomDir = random.Next(0, 2);
            //0 means horizontally
            //1 means vertically

            if (randomDir == 0)
            {
                Tuple<int, int> start = new Tuple<int, int>(randomX, randomY);
                Tuple<int, int> end = new Tuple<int, int>(randomX + s.getSize() - 1, randomY);
                Tuple<Tuple<int, int>, Tuple<int, int>> startEnd = new Tuple<Tuple<int, int>, Tuple<int, int>>(start, end);
                return startEnd;
            }
            else if (randomDir == 1)
            {
                Tuple<int, int> start = new Tuple<int, int>(randomX, randomY);
                Tuple<int, int> end = new Tuple<int, int>(randomX, randomY + s.getSize() - 1);
                Tuple<Tuple<int, int>, Tuple<int, int>> startEnd = new Tuple<Tuple<int, int>, Tuple<int, int>>(start, end);
                return startEnd;
            }
            else
            {
                Console.WriteLine("[AI.placeShip] randomDir not within bounds");
            }

            Console.WriteLine("[AI.placeShip] Something fucked up");
            Tuple<Tuple<int, int>, Tuple<int, int>> tmp = new Tuple<Tuple<int, int>, Tuple<int, int>>(null, null);
            return tmp;
        }

    }
}
