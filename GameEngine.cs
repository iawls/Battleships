using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    public class GameEngine
    {
        int turn= 1;
        int action = 0;
        int phase = 1;
        int win = 0;

        public Rules rulebook = new Rules();

        Board p1, p2;

        public GameEngine(Board p1, Board p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
        public int getTurn()
        {
            return this.turn;
        }

        public int getPhase()
        {
            return this.phase;
        }

        public void setTurn(int turn)
        {
            this.turn = turn;
        }

        public int getAction()
        {
            return this.action;
        }

        public int getWin()
        {
            return this.win;
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

        bool actionFire(Board p)
        {
            Console.WriteLine("Enter coordinates to fire at");
            Console.Write("x: ");
            int x = int.Parse(Console.ReadLine());
            Console.Write("y: ");
            int y = int.Parse(Console.ReadLine());
            Console.WriteLine("Firing at" + x + ", " + y);
            bool success = p.fire(x, y);
            return success;
        }

        bool actionPlaceShip(Board p, Ship s)
        {
            if (p.getIsHuman())
            {
                int x1, y1, x2, y2;
                Console.WriteLine("Enter coordinates to place a ship at, cant be longer or shorter than " + s.getSize());
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
                if (rulebook.validPlacement(posStart, posEnd, p))
                {
                    s.setStartEnd(posStart, posEnd);
                    p.placeShip(posStart, posEnd, s);
                    Console.WriteLine("Ship Placed!");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Tuple<Tuple<int, int>, Tuple<int, int>> startEnd = p.ai.placeShip(s);
                Tuple<int, int> posStart = new Tuple<int, int>(startEnd.Item1.Item1, startEnd.Item1.Item2);
                Tuple<int, int> posEnd = new Tuple<int, int>(startEnd.Item2.Item1, startEnd.Item2.Item2);

                if (rulebook.validPlacement(posStart, posEnd, p))
                {
                    s.setStartEnd(posStart, posEnd);
                    p.placeShip(posStart, posEnd, s);
                    Console.WriteLine("Ship Placed!");
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //Place each ship
        void placeShipsPhase(Board p)
        {

            List<Ship> newShipList = new List<Ship>(p.getShipList());

            for (int i = 0; i < newShipList.Count; ++i)
            {
                if (!actionPlaceShip(p, newShipList.ElementAt(i)))
                {
                    --i;
                }
            }

            p.printBoard();
            Console.WriteLine("All ships placed, leaving phase");
            phase = 2;
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
                        if(actionFire(p2))
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
                        if(actionFire(p1))
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

                var p1NewList = p1.getShipList().ToList<Ship>(); //makes a copy of the list, since you cant remove elements from a list in a foreach-loop
                var p2NewList = p2.getShipList().ToList<Ship>();


                foreach (Ship s in p1NewList ?? Enumerable.Empty<Ship>())
                {
                    if (s.getDead())
                    {
                        p1.removeShips(s);
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
                        p2.removeShips(s);
                        Console.WriteLine("removed a dead ship");
                    }
                    else
                    {
                        Console.WriteLine("No dead ships in p2 board");
                    }
                }

                Console.WriteLine("Checking Win coniditions");
                //check win-conditions
                if (rulebook.lost(p1))
                {
                    Console.WriteLine("Player 2 Wins! Shutting down");
                    break;
                }
                else if (rulebook.lost(p2))
                {
                    Console.WriteLine("Player 1 Wins! Shutting down");
                    break;

                }
            }
        }

        void newGameLoop()
        {
            while (true)
            {
                playTurn();
            }
        }

        public void AIAction(Board p){
            p.ai.playTurn(p);
        }

        void playTurn()
        {

            if (phase == 1)
            {
                if (turn == 1)
                {
                    turn = 2;
                    if (!p2.getIsHuman())
                        ; ;
                       /* if (!win)
                            playTurn();
                        else
                        {
                            Console.WriteLine("WE HAVE A WINNER");
                            String stop = Console.ReadLine();
                        }*/
                }
            }
        }

        public void nextTurn()
        {
                if (phase == 1)
                {
                    if (turn == 1)
                    {
                        turn = 2;
                        if (!p2.getIsHuman())
                        {   

                            List<Ship> newShipList = new List<Ship>(p2.getShipList());

                            for (int i = 0; i < newShipList.Count; ++i)
                            {
                                if (!actionPlaceShip(p2, newShipList.ElementAt(i)))
                                {
                                    --i;
                                }
                            }
                            phase = 2;
                            turn = 1;
                        }
                    }
                    else if (turn == 2)
                    {
                        turn = 1;
                        phase = 2;
                    }
                }
                else if (phase == 2)
                {
                    if (turn == 1)
                    {
                    turn = 2;
                        if (!p2.getIsHuman())
                        {
                            p2.ai.playTurn(p1);
                            turn = 1;
                        }
                    }
                    else if (turn == 2)
                    {
                        turn = 1;
                    }
                }

                //Remove the dead ships (if any) from the boards

                /* ??-Operator returns the left hand operand as long as it's not null.
                 * It returns the right hand operand in that case.
                 * This is done to prevent the foreach-loop from crashing, since it can't handle it when the list is empty
                 */

                var p1NewList = p1.getShipList().ToList<Ship>(); //makes a copy of the list, since you cant remove elements from a list in a foreach-loop
                var p2NewList = p2.getShipList().ToList<Ship>();

                //Removes dead ships, ignore this =D
                foreach (Ship s in p1NewList ?? Enumerable.Empty<Ship>())
                {
                    if (s.getDead())
                    {
                        p1.removeShips(s);
                        Console.WriteLine("removed a dead ship");
                    }
                }

                foreach (Ship s in p2NewList ?? Enumerable.Empty<Ship>())
                {

                    if (s.getDead())
                    {
                        p2.removeShips(s);
                        Console.WriteLine("removed a dead ship");
                    }
                }
                //Removed the dead ships, stop ignoring here!

                Console.WriteLine("Checking Win coniditions");
                //check win(lost)-conditions
                if (rulebook.lost(p1))
                {
                    win = 2;
                    Console.WriteLine("Player 2 Wins! Shutting down");
                }else if (rulebook.lost(p2))
            {
                    win = 1;
                    Console.WriteLine("Player 1 Wins! Shutting down");
                }
        }

        static void Main()
        {
            Board p1 = new Board(true); //Player 1 is human
            Board p2 = new Board(true); //PLayer 2 is AI
            GameEngine GE = new GameEngine(p1, p2);
            GameScreen gameScreen = new GameScreen(GE, p1, p2);
            gameScreen.ShowDialog();     
        }


    }

    public class Board
    {
        List<List<Node>> GameBoard = new List<List<Node>>();
        public Rules rulebook = new Rules();
        List<Ship> shipList = new List<Ship>();
        int boardSize;
        bool isHuman;
        public AI ai = new AI();
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

           //init shiplist
            initShipList();

        }

       
       public bool placeShip(Tuple<int, int> start, Tuple<int, int> end, Ship s)
       {
           if(rulebook.validPlacement(start, end, this)){

              for (int y = start.Item1; y <= end.Item1; y++)
              {
                  for (int x = start.Item2; x <= end.Item2; x++)
                  {
                      GameBoard.ElementAt(x).ElementAt(y).setShip(s);
                  }
               }
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

               if (this.GameBoard[y][x].getShip() != null)
               {
                   this.GameBoard[y][x].getShip().hit();
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
           return GameBoard.ElementAt(y).ElementAt(x).getHit();

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
           try
           {
               shipList.Remove(s);
           }
           catch
           {
               Console.WriteLine("ERROR: [Board.removeShips] Couldn't remove ship s");
           }
       }



    }

    public class Node
    {
        bool hit;
        Ship ship;
        int id;

        public Node(bool h, Ship s, int id)
        {
            this.hit = h;
            this.ship = s;
            this.id = id;
        }
        public void setShip(Ship s)
        {
            this.ship = s;
            this.ship.setPlaced(true);
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


    }
    public class Ship
    {

        Tuple<int, int> start;
        Tuple<int, int> end;
        bool dead = false;
        bool placed = false;
        int size = 1;
        int hits = 0;

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
            this.placed = true;
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

        public bool getPlaced()
        {
            return this.placed;
        }
        public void setPlaced(bool b)
        {
            this.placed = b;
        }
       
    }
    public class Rules
    {
        public bool validFire(int x, int y, Board b)
        {
            if((x < 0 || x > 10 || y < 0 || y > 10) || (b.isHit(x, y))){
                return false;
            }

            return true;
        }

        public bool validPlacement(Tuple<int, int> start, Tuple<int, int> end, Board p)
        {
            if (start.Item1 < 10 && start.Item1 >= 0 && start.Item2 >= 0 && start.Item2 < 10 && end.Item1 < 10 && end.Item1 >= 0 && end.Item2 < 10 && end.Item1 >= 0)
            {
                if ((start.Item1 == end.Item1) && (start.Item2 != end.Item2) || (start.Item1 != end.Item1) && (start.Item2 == end.Item2))
                {
                    for (int y = start.Item1; y <= end.Item1; y++)
                    {
                        for (int x = start.Item2; x <= end.Item2; x++)
                        {
                            if (p.isShip(y, x))
                            {
                                return false;
                            }
                        }
                    }
                    if (start.Item1 == end.Item1)
                    {
                        //Check above ship
                        if (start.Item2 > 0)
                        {
                            if (p.isShip(start.Item1, start.Item2 - 1))
                            {
                                return false;
                            }
                        }
                        //Check below ship
                        if (start.Item2 < 9)
                        {
                            if (p.isShip(end.Item1, end.Item2 + 1))
                            {
                                return false;
                            }
                        }
                        //Check at either side of ship
                        for (int i = start.Item2; i <= end.Item2; ++i)
                        {
                            if (start.Item1 < 9)
                            {
                                if (p.isShip(start.Item1 + 1, i))
                                {
                                    return false;
                                }
                            }
                            if (start.Item1 > 0)
                            {
                                if (p.isShip(start.Item1 - 1, i))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else if (start.Item2 == end.Item2)
                    {
                        //Check left of ship
                        if (start.Item1 > 0)
                        {
                            if (p.isShip(start.Item1 - 1, start.Item2))
                            {
                                return false;
                            }
                        }
                        //Check right of ship
                        if (start.Item1 < 9)
                        {
                            if (p.isShip(end.Item1 + 1, end.Item2))
                            {
                                return false;
                            }
                        }
                        //Check above and below ship
                        for (int i = start.Item1; i <= end.Item1; ++i)
                        {
                            if (start.Item2 < 9)
                            {
                                //Console.WriteLine("Checking below ship at ["+i+", "+end.Item2+"]");
                                if (p.isShip(i, start.Item2 + 1))
                                {
                                    return false;
                                }
                            }
                            if (start.Item2 > 0)
                            {
                                if (p.isShip(i, start.Item2 - 1))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public bool lost(Board p)
        {
            if (p.getShipList().Count() == 0)
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

        public AI()
        {
            initKnownBoard();
        }

        public void playTurn(Board targetBoard){
            Console.WriteLine("Playing AI turn");
            Tuple<int, int> target;
            do{
                target = chooseFireCoords();
                Console.WriteLine("AI-Target: " + target);
            } while (!targetBoard.fire(target.Item1, target.Item2));

                if (targetBoard.isShip(target.Item1, target.Item2))
                {
                    updateKnownBoard(target.Item1, target.Item2, 2);    //Hit a ship
                    updateKnownBoard(target.Item1+1, target.Item2, 3);  //Update the surrounding nodes
                    updateKnownBoard(target.Item1-1, target.Item2, 3);  
                    updateKnownBoard(target.Item1, target.Item2+1, 3);
                    updateKnownBoard(target.Item1, target.Item2-1, 3);

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

                    if (xMin < 9)
                    {
                        updateKnownBoard(xMin+1, i, 5);
                    }
                    if (xMin > 0)
                    {
                        updateKnownBoard(xMin-1, i, 5);
                    }
                    if (i < 9)
                    {
                        updateKnownBoard(xMin, i + 1, 5);
                    }
                    if (i > 0)
                    {
                        updateKnownBoard(xMin, i - 1, 5);
                    }

                }
            }
            else if(start.Item2 == end.Item2)
            {
                for (int i = xMin; i <= xMax; ++i)
                {
                    updateKnownBoard(i, yMin, 4);

                    if (yMin < 9)
                    {
                        updateKnownBoard(i, yMin + 1, 5);
                    }
                    if (yMin > 0)
                    {
                        updateKnownBoard(i, yMin - 1, 5);
                    }
                    if (i < 9)
                    {
                        updateKnownBoard(i + 1, yMin, 5);
                    }
                    if (i > 0)
                    {
                        updateKnownBoard(i - 1, yMin, 5);
                    }
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
                        break;
                    case 2:
                        if (newValue == 4)
                        {
                            this.knownBoard[y][x] = newValue;
                            Console.WriteLine("[AI.updateKnownBoard] Updating x: " + x + ", y: " + y + " to " + newValue);
                            printBoard();
                        }
                        break; 
                    case 3:
                        if (newValue != 0)
                        {
                            this.knownBoard[y][x] = newValue;
                            Console.WriteLine("[AI.updateKnownBoard] Updating x: " + x + ", y: " + y + " to " + newValue);
                            printBoard();
                        }
                        break;
                    case 5:
                        if (newValue == 4)
                        {
                            this.knownBoard[y][x] = newValue;
                            Console.WriteLine("[AI.updateKnownBoard] Updating x: " + x + ", y: " + y + " to " + newValue);

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
                if (searchForTarget(3))
                {
                    int randomNumber = random.Next(0, targetList.Count - 1);
                    Tuple<int, int> target = new Tuple<int, int>(targetList.ElementAt(randomNumber).Item1, targetList.ElementAt(randomNumber).Item2);
                    targetList.Clear();
                    return target;
                }
                else
                {
                    if (searchForTarget(0))
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

        public Tuple<Tuple<int, int>, Tuple<int, int>> placeShip(Ship s){

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

