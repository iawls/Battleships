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
    public class Board
    {
        List<List<Node>> GameBoard = new List<List<Node>>();
        public Rules rulebook = new Rules();
        List<Ship> shipList = new List<Ship>();
        Storage xmlStorage;
        int boardSize;
        bool isHuman;
        string player;
        public AI ai = new AI();

        public bool isLoading = false;

        //default constructor, defaults to a boardsize of 10x10
        public Board(bool human, string player, Storage xmlStorage)
        {
            this.boardSize = 10;
            this.isHuman = human;
            this.player = player;
            this.xmlStorage = xmlStorage;
            init();
        }

        //Load stored game
        public Board(string player, Storage xmlStorage)
        {
            this.boardSize = 10;
            this.isHuman = xmlStorage.isHuman(player);
            this.player = player;
            this.xmlStorage = xmlStorage;

            //Todo: init from xmlStorage
            initFromSave();
        }

        public void loadGame()
        {
            System.Threading.Thread.Sleep(100);
            initFromSave();
        }

        void initFromSave()
        {
            isLoading = true;

            Console.WriteLine("Initiating from saved file");
            if (File.Exists(xmlStorage.getPath()))
            {

                this.shipList.Clear();

                GameBoard.Clear();

                for (int y = 0; y < boardSize; ++y)
                {
                    List<Node> tmpList = new List<Node>();
                    for (int x = 0; x < boardSize; ++x)
                    {
                        Node n = new Node(false, null, x);        //create a node to insert in the inner List
                        tmpList.Add(n);
                    }
                    GameBoard.Add(tmpList);                    //add the inner List to the outer List
                }
                XDocument db = XDocument.Load(xmlStorage.getPath());

                var shipList = from ship in db.Root.Element("Player")
                                                   .Element(this.player)
                                                   .Element("Ships")
                                                   .Elements("Ship")
                               select ship;

                var hitList = from node in db.Root.Element("Player")
                                      .Element(this.player)
                                      .Element("Hits")
                                      .Elements("Node")
                              select node;

                ai.loadKnownBoard(); //load the internal AI board

                int startX, startY, endX, endY, size, hits;

                //Place all the loaded ships onto the board, and set hits on hit ships
                foreach (XElement ship in shipList)
                {

                    startX = Int32.Parse(ship.Element("startPos").Element("X").Value);
                    startY = Int32.Parse(ship.Element("startPos").Element("Y").Value);
                    endX = Int32.Parse(ship.Element("endPos").Element("X").Value);
                    endY = Int32.Parse(ship.Element("endPos").Element("Y").Value);
                    size = Int32.Parse(ship.Element("size").Value);
                    hits = Int32.Parse(ship.Element("hits").Value);

                    Tuple<int, int> startPos = new Tuple<int, int>(startX, startY);
                    Tuple<int, int> endPos = new Tuple<int, int>(endX, endY);
                    Ship s = new Ship();

                    placeShip(startPos, endPos, s);

                    s.setHits(hits);

                    this.shipList.Add(s);
                }

                int hitX, hitY;


                //Place hit markers
                foreach (XElement hit in hitList)
                {
                    hitX = Int32.Parse(hit.Element("X").Value);
                    hitY = Int32.Parse(hit.Element("Y").Value);

                    this.GameBoard[hitY][hitX].setHit();
                }

            }
            isLoading = false;
            Console.WriteLine("Initiating complete");

        }


        public void printBoard()
        {

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (isShip(x, y) && isHit(x, y))
                        Console.Write("[X]");
                    else if (isHit(x, y))
                        Console.Write("[+]");
                    else if (isShip(x, y))
                        Console.Write("[S]");
                    else
                        Console.Write("[ ]");
                }
                Console.Write("\n");
            }

        }

        void init()
        {
            //init the gameboard
            Console.WriteLine("Initializing the gameboard");


            for (int y = 0; y < boardSize; ++y)
            {
                List<Node> tmpList = new List<Node>();
                for (int x = 0; x < boardSize; ++x)
                {
                    Node n = new Node(false, null, x);        //create a node to insert in the inner List
                    tmpList.Add(n);
                }
                GameBoard.Add(tmpList);                    //add the inner List to the outer List
            }

            Console.WriteLine("Gameboard initialized");

            //init shiplist
            initShipList();

        }


        public bool placeShip(Tuple<int, int> start, Tuple<int, int> end, Ship s)
        {
            if (rulebook.validPlacement(start, end, this))
            {
                //Mark the nodes affected as ships o the board
                for (int y = start.Item1; y <= end.Item1; y++)
                {
                    for (int x = start.Item2; x <= end.Item2; x++)
                    {
                        GameBoard.ElementAt(x).ElementAt(y).setShip(s);
                        s.setStartEnd(start, end);
                    }
                }
                if (!this.isLoading)
                    xmlStorage.addShip(player, start.Item1, start.Item2, end.Item1, end.Item2, s.getSize());
                printBoard();
                return true;
            }
            else
            {
                Console.WriteLine("Not a valid placement! Try again!");
                return false;
            }
        }

        public Ship getShip(int x, int y)
        {
            return GameBoard[y][x].getShip();
        }

        public bool fire(int x, int y)
        {
            if (rulebook.validFire(x, y, this))
            {
                this.GameBoard.ElementAt(y).ElementAt(x).setHit();
                xmlStorage.addHit(player, x, y);

                if (this.GameBoard[y][x].getShip() != null)
                {
                    this.GameBoard[y][x].getShip().hit();
                    int posX = this.GameBoard[y][x].getShip().getStart().Item1;
                    int posY = this.GameBoard[y][x].getShip().getStart().Item2;
                    int hits = this.GameBoard[y][x].getShip().getHits();
                    xmlStorage.alterShip(player, posX, posY, hits); //update the affected ship in the storage
                }

                Console.WriteLine(x + ", " + y + " is hit");
                printBoard();
                return true;

            }
            else
            {
                Console.WriteLine("Not a valid coordinate! Try again!");
                return false;
            }
        }

        public bool isShip(int x, int y)
        {
            if (x < boardSize && y < boardSize)
                return GameBoard[y][x].getShip() != null; //GameBoard.ElementAt(x).ElementAt(y).getShip() != null;
            else
                return false;
        }
        public bool isHit(int x, int y)
        {

            try
            {
                return GameBoard.ElementAt(y).ElementAt(x).getHit();
            }
            catch (ArgumentOutOfRangeException outOfRange)
            {

                Console.WriteLine("Error: {0}", outOfRange.Message);
            }
            return false;

        }
        public int getSize()
        {
            return boardSize;
        }
        public bool getIsHuman()
        {
            return this.isHuman;
        }

        public void initShipList()
        {
            //Change to lower number for easier debug
            for (int i = 0; i < 10; ++i)
            {
                Ship s = new Ship();
                shipList.Add(s);
            }

            //Remove some ships if you want easier debug with the console
            shipList.ElementAt(0).setSize(6);
            shipList.ElementAt(1).setSize(4);
            shipList.ElementAt(2).setSize(4);
            shipList.ElementAt(3).setSize(3);
            shipList.ElementAt(4).setSize(3);
            shipList.ElementAt(5).setSize(3);
            shipList.ElementAt(6).setSize(2);
            shipList.ElementAt(7).setSize(2);
            shipList.ElementAt(8).setSize(2);
            shipList.ElementAt(9).setSize(2);
            //shipList.ElementAt(0).setSize(6);

            Console.WriteLine("Rules initiated");

        }

        public List<Ship> getShipList()
        {
            return this.shipList;
        }

        //call this method when done with GUI initiation to tell the rules where the ships are placed and where they are located (this is a workaround the "no reference passing" problem from getShipList())
        public void setShipList(List<Ship> s)
        {
            this.shipList = s;
        }

        //find the ship that needs to be removed, and removes it. No need to keep track of indexes =)
        public void removeShips(Ship s)
        {
            try
            {
                shipList.Remove(s);
            }
            catch
            {
                Console.WriteLine("ERROR: [Board.removeShips] Couldn't remove ship s");
            }
        }

        //Check if there are any ships that are not placed yet
        public bool shipToBePlaced()
        {
            foreach (Ship s in shipList ?? Enumerable.Empty<Ship>())
            {

                if (!s.getPlaced())
                {
                    return true;
                }
            }

            return false;
        }


    }
}
