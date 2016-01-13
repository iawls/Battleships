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

        public Storage()
        {
            //Create database

            XDocument database = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("Huehuehue"),
                new XElement("root",
                    new XElement("States",
                         new XElement("Phase", "0"),
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
                    )
                )
            );

            /*
            *ADD element manually
            database.Element("root").Element("States").Add(
                new XElement("test", "test"));
            */

            Console.WriteLine("Saving to: " + path);
            database.Save(path);

            IEnumerable<string> test = from huehue in XDocument.Load(path)
                                                                 .Descendants("Ship")
                                       where (int)huehue.Element("startPos").Element("X") == 8
                                       select huehue.Element("endPos").Element("Y").Value;

            foreach (string s in test)
            {
                Console.WriteLine(s);
            }
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
            // .Where(x => (int)x.Element("startPos").Element("X").Value == startX && (int)x.Element("startPos").Element("Y").Value == startY).FirstOrDefault()

            /*
            XDocument database = XDocument.Load(path);
            database.Element("root").Element("Player").Element(player).Elements("Ships")
                .Where(x => x.Element("startPos").Element("X").Value == startX.ToString()).SingleOrDefault()
                .SetElementValue("hits", hits);
            */
        }

        public void addHit(string player, string hitOrMiss, int posX, int posY)
        {
            XDocument database = XDocument.Load(path);
            database.Element("root").Element("Player").Element(player).Element("Hits").Add(
                new XElement("Node",
                        new XElement("type", hitOrMiss),
                        new XElement("X", posX),
                        new XElement("Y", posY)));
            database.Save(path);
        }

        public bool addELement()
        {
            return false;
        }

        public bool save()
        {
            //Check if modified since last write
            //if modified, do nothing
            //if not modified save gamestate

            return false;
        }

        public bool load()
        {
            //Load gamestate
            return false;
        }

        string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\data.xml";


    }
}
