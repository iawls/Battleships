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
    public class GameEngine
    {
        private int turn = 1;
        private int action = 0;
        private int phase = 1;
        private int win = 0;
        private Storage xmlStorage;

        private Board p1, p2;
        private Rules rulebook;

        public static FileSystemWatcher watcher;

        DateTime lastReadTime = DateTime.MinValue;

        public GameEngine(Board p1, Board p2, Storage xmlStorage, Rules rulebook)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.xmlStorage = xmlStorage;
            this.rulebook = rulebook;

            watcher = new FileSystemWatcher();
            watcher.Path = xmlStorage.getDirectory();
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.xml";
            watcher.EnableRaisingEvents = false;
            watcher.Changed += (s, e) => OnChanged(p1, p2);

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

        public void setPhase(int phase)
        {
            this.phase = phase;
        }

        public int getAction()
        {
            return this.action;
        }

        public int getWin()
        {
            return this.win;
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

        public void nextTurn()
        {
            watcher.EnableRaisingEvents = false; //turn off the watcher to avoid infinite loading loops

            if (phase == 1) //the place ship phase
            {
                if (turn == 1) //player 1
                {
                    turn = 2; //set to player 2
                    xmlStorage.setTurn(turn); 
                    if (!p2.getIsHuman()) //Player 2 is AI
                    {

                        List<Ship> newShipList = new List<Ship>(p2.getShipList());

                        //Place all the AIs ships
                        for (int i = 0; i < newShipList.Count; ++i)
                        {
                            if (!actionPlaceShip(p2, newShipList.ElementAt(i)))
                            {
                                --i;
                            }
                        }
                        phase = 2;
                        xmlStorage.setPhase(phase);
                        turn = 1;
                        xmlStorage.setTurn(turn);
                    }
                }
                else if (turn == 2)
                {
                    turn = 1;
                    phase = 2;
                    xmlStorage.setTurn(turn);
                    xmlStorage.setPhase(phase);
                }
            }
            else if (phase == 2) //battle phase
            {

                if (turn == 1)
                {
                    turn = 2;
                    xmlStorage.setTurn(turn);
                    if (!p2.getIsHuman())
                    {
                        p2.ai.playTurn(p1); //make ai play its turn
                        turn = 1;
                        xmlStorage.setTurn(turn);
                    }
                }
                else if (turn == 2)
                {
                    turn = 1;
                    xmlStorage.setTurn(turn);
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
                watcher.EnableRaisingEvents = false;
                phase = 1;
                xmlStorage.delete();
                win = 2;
                Console.WriteLine("Player 2 Wins! Shutting down");
            }
            else if (rulebook.lost(p2))
            {
                watcher.EnableRaisingEvents = false;
                phase = 1;
                xmlStorage.delete();
                win = 1;
                Console.WriteLine("Player 1 Wins! Shutting down");
            }

            if (phase != 1)
                watcher.EnableRaisingEvents = true;

        }

        static void Main()
        {
            Storage xmlStorage = new Storage();
            Rules rulebook = new Rules();

            Form splashScreen = new SplashScreen();
            splashScreen.Show();
            Thread.Sleep(1000);
            splashScreen.Close();

            Menu menu = new Menu(xmlStorage.previousGame());
            menu.ShowDialog();
            string menuChoice = menu.buttonEvent;

            if (menuChoice != "EXIT")
            {
                GameEngine GE;
                if (menuChoice == "PLAYER_VS_PLAYER")
                {
                    xmlStorage.clearData();
                    xmlStorage.setPvP("Yes");
                    Board p1 = new Board(true, "Player1", xmlStorage); //Player 1 is human
                    Board p2 = new Board(true, "Player2", xmlStorage); //Player 2 is human
                    GE = new GameEngine(p1, p2, xmlStorage, rulebook);
                    GameScreen gameScreen = new GameScreen(GE, p1, p2);
                    gameScreen.ShowDialog();
                }
                else if (menuChoice == "PLAYER_VS_PC")
                {
                    xmlStorage.clearData();
                    xmlStorage.setPvP("No");
                    Board p1 = new Board(true, "Player1", xmlStorage); //Player 1 is human
                    Board p2 = new Board(false, "Player2", xmlStorage); //Player 2 is PC
                    GE = new GameEngine(p1, p2, xmlStorage, rulebook);
                    GameScreen gameScreen = new GameScreen(GE, p1, p2);
                    gameScreen.ShowDialog();
                }
                else if (menuChoice == "LOAD_SAVED_GAME")
                {
                    //Load from xmlStorage
                    Board p1 = new Board("Player1", xmlStorage);
                    Board p2 = new Board("Player2", xmlStorage);
                    GE = new GameEngine(p1, p2, xmlStorage, rulebook);
                    GE.setTurn(xmlStorage.getTurn());
                    GE.setPhase(xmlStorage.getPhase());
                    GameScreen gameScreen = new GameScreen(GE, p1, p2);
                    gameScreen.ShowDialog();

                }
            }
        }

        private void OnChanged(Board p1, Board p2)
        {
            watcher.EnableRaisingEvents = false;
            DateTime lastWriteTime = File.GetLastWriteTime(xmlStorage.getPath());

            lastWriteTime = lastWriteTime.AddMilliseconds(-lastWriteTime.Millisecond);
            lastReadTime = lastReadTime.AddMilliseconds(-lastReadTime.Millisecond);

            if (lastWriteTime != lastReadTime)
            {
                Console.WriteLine("Watcher detected change in file, loading it now");
                p1.loadGame();
                p2.loadGame();

                lastReadTime = lastWriteTime;
            }

            watcher.EnableRaisingEvents = true;

        }

    }
}

