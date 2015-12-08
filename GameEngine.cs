using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace Battleships
{
    public class GameEngine
    {
        int turn = 1;
        int action = 0;

        public int getTurn()
        {
            return this.turn;
        }

        public int getAction()
        {
            return this.action;
        }

        int chooseAction()
        {
            do
            {
                Console.WriteLine("Choose an action \n (1): Fire \n (2): Print both boards \n (3): Quit");
                action = int.Parse(Console.ReadLine());

                Console.WriteLine("action: " + action);

                if (action == 1 || action == 2 || action == 3)
                {
                    Console.WriteLine("Breaking loop");
                    return action;
                }
                else
                {
                    Console.WriteLine("Not a valid number, try again");
                }

            } while (true);

        }

        void actionFire(Board p)
        {
            Console.WriteLine("Enter coordinates to fire at");
            Console.Write("x: ");
            int x = int.Parse(Console.ReadLine());
            Console.Write("y: ");
            int y = int.Parse(Console.ReadLine());
            Console.WriteLine("Firing at" + x + ", " + y);
            p.fire(x, y, p);         
        }

        void actionPlaceShip(Board p)
        {
            int x1, y1, x2, y2;
            Console.WriteLine("Enter coordinates to place a ship at");
            Console.Write("x: ");
            x1 = int.Parse(Console.ReadLine());
            Console.Write("y: ");
            y1 = int.Parse(Console.ReadLine());

            Console.Write("x: ");
            x2 = int.Parse(Console.ReadLine());
            Console.Write("y: ");
            y2 = int.Parse(Console.ReadLine());

            Tuple<int, int> posStart = new Tuple<int, int>(x1, y1);
            Tuple<int, int> posEnd = new Tuple<int, int>(x2, y2);

            Ship s = new Ship(posStart, posEnd);
            p.placeShip(posStart, posEnd, s);
        }

        //Place each ship
        void placeShipsPhase(Board p)
        {

            List<Ship> newShipList = new List<Ship>(p.rulebook.getShipList());

            foreach (Ship s in newShipList)
            {
                Console.WriteLine("Place your size " + s.getSize() + " ship");
                int x1, y1, x2, y2;

                do
                {
                    Console.WriteLine("Enter coordinates to place a ship at, cant be longer or shorter than " + s.getSize());
                    Console.Write("startx: ");
                    x1 = int.Parse(Console.ReadLine());
                    Console.Write("starty: ");
                    y1 = int.Parse(Console.ReadLine());

                    Console.Write("endx: ");
                    x2 = int.Parse(Console.ReadLine());
                    Console.Write("endy: ");
                    y2 = int.Parse(Console.ReadLine());

                } while ((x2 - x1 + 1 != s.getSize() && y2 == y1) || (y2 - y1 + 1 != s.getSize() && x2 == x1));

                Tuple<int, int> posStart = new Tuple<int, int>(x1, y1);
                Tuple<int, int> posEnd = new Tuple<int, int>(x2, y2);

                Console.WriteLine("Ship position accepted");

                s.setStartEnd(posStart, posEnd);
            }
            p.rulebook.setShipList(newShipList);

            foreach (Ship s in p.rulebook.getShipList())
            {
                p.placeShip(s.getStart(), s.getEnd(), s);
            }
            p.printBoard();
            Console.WriteLine("All ships placed, leaving phase");


        }

       void GameLoop(Board p1, Board p2)
        {
            //Gameloop starts here
            while (true)
            {
                Console.WriteLine("Player " + turn);
                action = chooseAction();
                //Player 1s turn         
                if (turn == 1)
                {
                    //Fire at Player 2
                    if (action == 1)
                    {
                        actionFire(p2);
                        turn = 2;
                    }
                    else if (action == 2)
                    {
                        p1.printBoard();
                        Console.WriteLine("------------------------------");
                        p2.printBoard();
                    }
                    else if (action == 3)
                    {
                        break;
                    }
                }
                else if (turn == 2)
                {
                    if (action == 1)
                    {
                        actionFire(p1);
                        turn = 1;
                    }
                    else if (action == 2)
                    {
                        p1.printBoard();
                        Console.WriteLine("------------------------------");
                        p2.printBoard();
                    }
                    else if (action == 3)
                    {
                        break;
                    }

                }
                else
                {
                    Console.WriteLine("Something went terribly wrong, turn is invalid. Breaking GameLoop");
                    break;
                }

                Console.WriteLine("Removing dead ships");
                //Remove the dead ships (if any) from the boards

                /* ??-Operator returns the left hand operand as long as it's not null.
                 * It returns the right hand operand in that case.
                 * This is done to prevent the foreach-loop from crashing, since it can't handle it when the list is empty
                 */

                
                var p1NewList = p1.rulebook.getShipList().ToList<Ship>(); //makes a copy of the list, since you cant remove elements from a list in a foreach-loop
                var p2NewList = p2.rulebook.getShipList().ToList<Ship>();


                foreach (Ship s in p1NewList ?? Enumerable.Empty<Ship>())
                {
                    Console.WriteLine("Hits: "+s.getHits());
                    if (s.getDead())
                    {
                        p1.rulebook.removeShips(s);
                        Console.WriteLine("removed a dead ship");
                    }
                    else
                    {
                        Console.WriteLine("No dead ships in p1 board");
                    }
                }

                foreach (Ship s in p2NewList ?? Enumerable.Empty<Ship>())
                {

                    if (s.getDead())
                    {
                        p2.rulebook.removeShips(s);
                        Console.WriteLine("removed a dead ship");
                    }
                    else
                    {
                        Console.WriteLine("No dead ships in p2 board");
                    }
                }

                Console.WriteLine("Checking Win coniditions");
                //check win-conditions
                if (p1.rulebook.lost())
                {
                    Console.WriteLine("Player 2 Wins! Shutting down");
                    break;
                }
                else if (p2.rulebook.lost())
                {
                    Console.WriteLine("Player 1 Wins! Shutting down");
                    break;

                }
            }
        }

        void splashScreenStart()
        {
            Form splashScreen = new SplashScreen();
            splashScreen.Show();
            Thread.Sleep(1000);
            splashScreen.Close();
        }

        static void Main()
        {
            GameEngine GE = new GameEngine();
            GE.splashScreenStart();
            Menu menu = new Menu();
            menu.ShowDialog();
            string menuChoice = menu.buttonEvent;

            if (menuChoice != "EXIT")
            {
               
                if (menuChoice == "PLAYER_VS_PLAYER")
                {
                    Board p1 = new Board(true); //Player 1 is human
                    Board p2 = new Board(true); //PLayer 2 is human
                    GameScreen gameScreen = new GameScreen(GE, p1, p2);
                    gameScreen.ShowDialog();
                }
                else if (menuChoice == "PLAYER_VS_PC")
                {
                    Board p1 = new Board(true); //Player 1 is human
                    Board p2 = new Board(false); //PLayer 2 is PC
                    GameScreen gameScreen = new GameScreen(GE, p1, p2);
                    gameScreen.ShowDialog();
                }
                /*
                Console.WriteLine("Player 1, place your ships");
                GE.placeShipsPhase(p1);
                Console.WriteLine("Player 2, place your ships");
                GE.placeShipsPhase(p2);
                Console.WriteLine("Starting GameLoop");
                GE.GameLoop(p1, p2);
                Console.WriteLine("Press ENTER to close window");
                String stop = Console.ReadLine();
                */
            }
            
        }
    }

    public class Board
    {
        List<List<Node>> GameBoard = new List<List<Node>>();

        public Rules rulebook = new Rules();

        int boardSize;

        bool isHuman = false;

        //default constructor, defaults to a boardsize of 10x10
        public Board(bool human)
        {
            this.boardSize = 10;
            this.isHuman = human;
            init();
        }
        //Construct a board with a boardsize of size x size
        public Board(int size, bool human)
        {
            this.boardSize = size;
            this.isHuman = human;
            init();

        }

        public int getSize()
        {
            return boardSize;
        }
        public void printBoard(){

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

           //init Rules
            rulebook.init();

        }

       
       public void placeShip(Tuple<int, int> start, Tuple<int, int> end, Ship s)
       {
           if(rulebook.validPlacement(start, end, s)){

              for (int y = start.Item1; y <= end.Item1; y++)
              {
                  for (int x = start.Item2; x <= end.Item2; x++)
                  {
                      GameBoard.ElementAt(x).ElementAt(y).setShip(s);
                  }
               }
           }
           else
           {
               Console.WriteLine("Not a valid placement! Try again!");
           }
       }
       public void fire(int x, int y, Board b)
       {
           if (rulebook.validFire(x, y, b))
           {
               b.GameBoard.ElementAt(y).ElementAt(x).setHit();

               if (b.GameBoard[y][x].getShip() != null)
               {
                   b.GameBoard[y][x].getShip().hit();
               }

               Console.WriteLine(x + ", " + y + " is hit");
               printBoard();
           }
           else
           {
               Console.WriteLine("Not a valid coordinate! Try again!");
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
           return GameBoard.ElementAt(y).ElementAt(x).getHit();

       }


    }

    class Node
    {
        public Node(bool h, Ship s, int id)
        {
            this.hit = h;
            this.ship = s;
            this.id = id;
        }
        public void setShip(Ship s)
        {
            this.ship = s;
        }

        public Ship getShip()
        {
            return this.ship;
        }

        public void setHit()
        {
            this.hit = true;
        }
        public bool getHit()
        {
            return this.hit;
        }

        public int getID()
        {
            return this.id;
        }

        bool hit;
        Ship ship;
        int id;

    }
    public class Ship
    {

        public Ship(Tuple<int, int> start, Tuple<int, int> end)
        {
            this.size = Math.Max(end.Item1 - start.Item1, end.Item2 - start.Item2) + 1;
            this.start = start;
            this.end = end;
        }

        //do nothing, ships initiated like this will get values from the GUI
        public Ship(){} 

        public bool getDead()
        {
            return dead;
        }

        public void hit()
        {
            hits++;
            Console.WriteLine("Ship hit!"+this.hits+ " "+ this.size);
            if (hits == size)
            {
                Console.WriteLine("Killed a ship");
                this.dead = true;
            }
        }

        public void setSize(int size)
        {
            this.size = size;
        }

        public int getSize()
        {
            return this.size;
        }

        public void setStartEnd(Tuple<int, int> start, Tuple<int, int> end)
        {
            this.start = start;
            this.end = end;
            this.size = Math.Max(end.Item1 - start.Item1, end.Item2 - start.Item2)+1;
            Console.WriteLine("Size: " + this.size);
        }

        public Tuple<int, int> getStart()
        {
            return start;
        }
        public Tuple<int, int> getEnd()
        {
            return end;
        }
        public int getHits()
        {
            return this.hits;
        }

        Tuple<int, int> start;
        Tuple<int, int> end;
        bool dead = false;
        int size = 1;
        int hits = 0;


       
    }
    public class Rules
    {
        List<Ship> shipList = new List<Ship>();

        public void init()
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
            //shipList.ElementAt(0).setSize(2);
            //shipList.ElementAt(1).setSize(2);
            
            Console.WriteLine("Rules initiated");

        }

        //c# doesn't seem to support passing by reference, so it's passed by value. 
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
            shipList.Remove(s);
        }

        public bool validFire(int x, int y, Board b)
        {
            if(x < 0 || x > 10 || y < 0 || y > 10){
                return false;
            }

            return true;
        }

        public bool validPlacement(Tuple<int, int> start, Tuple<int, int> end, Ship s)
        {
            if((start.Item1 == end.Item1) || (start.Item2 == end.Item2)){
                return true;
            }

            return false;
        }

        public bool lost()
        {
            if (shipList.Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    /*
     * A simple AI, if there's no known "targets" it will choose one at random.
     * If there's a better option (3 in the known board), it will choose one of those at random
     * to avoid predictability. 
     * */
    class AI
    {
        List<List<int>> knownBoard; /* Will have integers as identifiers.
                                     * 0 - unknown
                                     * 1 - hit
                                     * 2 - hit and ship
                                     * 3 - next to ship (might also be ship)
                                     * 4 - dead ship
                                     * */
        int targetX;
        int targetY;

        List<Tuple<int, int>> targetList = new List<Tuple<int, int>>();

        public AI()
        {
            initKnownBoard();
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
                knownBoard.Add(tmpList);                    //add the inner List to the outer List
            }
        }

        void updateKnownBoard(int x, int y, int newValue)
        {
            this.knownBoard[y][x] = newValue;
        }

        Tuple<int, int> chooseFireCoords()
        {
            Random random = new Random();

            if (searchForTarget(3) == true)
            {
                int randomNumber = random.Next(0, targetList.Count + 1);
                Tuple<int, int> target = new Tuple<int, int>(targetList.ElementAt(randomNumber).Item1, targetList.ElementAt(randomNumber).Item2);
                return target;
            }
            else
            {
                int x = random.Next(0, 10);
                int y = random.Next(0, 10);
                Tuple<int, int> target = new Tuple<int, int>(x, y);
                return target;
            }

        }

        bool searchForTarget(int value)
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

    }
}

