using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace Battleships
{
    public class Storage
    {
        string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\data.xml";
        bool savedGame = false;

        public Storage()
        {
            //Create database

            if (File.Exists(path))
            {
                savedGame = true;
            }
            else
            {
                savedGame = false;
            }
            //Testing to see if I can select ships by position
            /*
            IEnumerable<string> test = from huehue in XDocument.Load(path)
                                                               .Descendants("Ship")
                                       where (int)huehue.Element("startPos").Element("X") == 8
                                       select huehue.Element("endPos").Element("Y").Value;

            //Will print the ships endY value that matches the "where"-statement above
            foreach (string s in test)
            {
                Console.WriteLine(s);
            }
            */
        }

        public string getPath()
        {
            return path;
        }

        public void clearData()
        {
            XDocument database = new XDocument(
                   new XDeclaration("1.0", "utf-8", "yes"),
                   new XComment("all your base are belong to us"),
                   new XElement("root",
                       new XElement("States",
                            new XElement("Phase", "1"),
                            new XElement("Turn", "1"),
                            new XElement("PvP", "yes")
                            ),
                       new XElement("Player",
                           new XElement("Player1",
                               new XElement("Ships"),
                               new XElement("Hits")
                           ),
                           new XElement("Player2",
                               new XElement("Ships"),
                               new XElement("Hits")
                           )
                       ),
                       new XElement("AI-Board")
                   )
               );

            Console.WriteLine("Saving to: " + path);
            database.Save(path);
        }

        public void delete()
        {
            File.Delete(path);
        }

        public void setPhase(int phase)
        {
            //Open document
            XDocument db = XDocument.Load(path);

            db.Root.Element("States").SetElementValue("Phase", phase);

            db.Save(path);
        }

        public void setTurn(int turn)
        {
            //Open document
            XDocument db = XDocument.Load(path);

            db.Root.Element("States").SetElementValue("Turn", turn);

            db.Save(path);
        }

        public void setPvP(string pvp)
        {
            //Open document
            XDocument db = XDocument.Load(path);

            db.Root.Element("States").SetElementValue("PvP", pvp);

            db.Save(path);
        }

        public void addShip(string player, int startX, int startY, int endX, int endY, int size)
        {
            XDocument database = XDocument.Load(path);
            database.Element("root").Element("Player").Element(player).Element("Ships").Add(
                new XElement("Ship",
                    new XElement("startPos",
                        new XElement("X", startX),
                        new XElement("Y", startY)),
                    new XElement("endPos",
                        new XElement("X", endX),
                        new XElement("Y", endY)),
                    new XElement("size", size),
                    new XElement("hits", 0)));
            database.Save(path);
        }

        public void alterShip(string player, int startX, int startY, int hits)
        {
            //Open document
            XDocument db = XDocument.Load(path);

            //Find the ship with startX and startY on players board. 
            var tmp = (from ship in db.Root.Elements("Player").Elements(player).Elements("Ships").Elements("Ship")
                       where ship.Element("startPos").Element("X").Value == startX.ToString()
                          && ship.Element("startPos").Element("Y").Value == startY.ToString()
                       select ship).FirstOrDefault(); //select it

            Console.WriteLine(player + "´s ship is hit: startX " + startX + ", startY " + startY + ". The ship now has " + hits + " hits");
            //set its "hits" element to hits
            if (tmp != null)
                tmp.SetElementValue("hits", hits);
            else
                Console.WriteLine("Error: Can't alter ship!");

            //save that mofo
            db.Save(path);

        }

        public void addHit(string player, int posX, int posY)
        {
            XDocument database = XDocument.Load(path);
            database.Element("root").Element("Player").Element(player).Element("Hits").Add(
                new XElement("Node",
                        new XElement("X", posX),
                        new XElement("Y", posY)));
            database.Save(path);
        }

        public void updateKnownNodes(int posX, int posY, int value)
        {
            XDocument db = XDocument.Load(path);

            var tmp = (from node in db.Root.Element("AI-Board").Elements("Node")
                      where node.Element("X").Value == posX.ToString() &&
                            node.Element("Y").Value == posY.ToString()
                      select node).FirstOrDefault();

            if (tmp == null)
            {
                db.Element("root").Element("AI-Board").Add(
                     new XElement("Node",
                         new XElement("X", posX),
                         new XElement("Y", posY),
                         new XElement("Value", value)
                     )
                );
                db.Save(path);
            }
            else
            {
                tmp.SetElementValue("Value", value);
                db.Save(path);
            }
        }

        public int getPhase()
        {
            XDocument db = XDocument.Load(path);
            int phase;
            Int32.TryParse(db.Root.Element("States").Element("Phase").Value, out phase);
            return phase;
        }

        public int getTurn()
        {
            XDocument db = XDocument.Load(path);
            int turn;
            Int32.TryParse(db.Root.Element("States").Element("Turn").Value, out turn);
            return turn;
        }

        public bool isHuman(string player)
        {
            XDocument db = XDocument.Load(path);
            if (db.Root.Element("States").Element("PvP").Value == "No" && player == "Player2")
            {
                return false;
            }
            return true;
        }

        public bool previousGame()
        {
            return savedGame;
        }

    }
}
