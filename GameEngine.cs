﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace Battleships
{
    class GameEngine
    {
        int chooseAction()
        {
            do
            {
                int action = 0;
                Console.WriteLine("Choose an action \n (1): Fire \n (2): Place Ship \n (3): Print both boards \n (4): Quit");
                action = int.Parse(Console.ReadLine());

                Console.WriteLine("action: " + action);

                if (action == 1 || action == 2 || action == 3 || action == 4)
                {
                    Console.WriteLine("Breaking loop");
                    return action;
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

       void GameLoop(Board p1, Board p2)
        {

            int turn = 1;
            int action = 0;

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
                    //Place a ship on Player 1s board
                    else if(action == 2){
                        actionPlaceShip(p1);
                        turn = 1;

                    }
                    else if (action == 3)
                    {
                        p1.printBoard();
                        Console.WriteLine("------------------------------");
                        p2.printBoard();
                    }
                    else if (action == 4)
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
                        actionPlaceShip(p2);
                        turn = 2;

                    }
                    else if (action == 3)
                    {
                        p1.printBoard();
                        Console.WriteLine("------------------------------");
                        p2.printBoard();
                    }
                    else if (action == 4)
                    {
                        break;
                    }

                }
                else
                {
                    Console.WriteLine("Something went terribly wrong, turn is invalid. Breaking GameLoop");
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

        void menu()
        {
            Application.Run(new Menu());
        }

        static void Main()
        {
            Board p1 = new Board();
            Board p2 = new Board();
            GameEngine GE = new GameEngine();
            GE.splashScreenStart();
            Menu menu = new Menu();
            menu.ShowDialog();
            int menuButtonValue = menu.buttonValue;

            if(menuButtonValue != 4)
                GE.GameLoop(p1, p2);
            
        }
    }

    class Board
    {
        List<List<Node>> GameBoard = new List<List<Node>>();

        Rules rulebook = new Rules();

        int boardSize;

        //default constructor, defaults to a boardsize of 10x10
        public Board()
        {
            this.boardSize = 10;
            init();
        }
        //Construct a board with a boardsize of size x size
        public Board(int size)
        {
            this.boardSize = size;
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
               printBoard();
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
            Console.WriteLine("setShip");
            this.ship = s;
        }

        public Ship getShip()
        {
            return this.ship;
        }

        public void setHit()
        {
            this.hit = true;
            Console.WriteLine("setHit!");
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
    class Ship
    {

        public Ship(Tuple<int, int> start, Tuple<int, int> end)
        {
            this.size = Math.Max(start.Item1 - end.Item1, start.Item2 - end.Item2);
            this.start = start;
            this.end = end;
        }

        public bool dead()
        {
            return hits == size;
        }

        public void hit()
        {
            hits++;
        }

        Tuple<int, int> start;
        Tuple<int, int> end;

        int size = 1;
        int hits = 0;

    }
    class Rules
    {

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

        public bool win()
        {
            //TODO: Fix a neat way to check win conditions
            return false;
        }

    }

}

