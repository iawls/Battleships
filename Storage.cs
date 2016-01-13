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
    class Storage
    {

        public Storage()
        {
            //Create database
            //Fill it with elements
            path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

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
                            new XElement("Ships",
                                new XElement("Ship",
                                    new XElement("startPos",
                                        new XElement("X", "8"),
                                        new XElement("Y", "0")
                                    ),
                                    new XElement("endPos",
                                        new XElement("X", "8"),
                                        new XElement("Y", "5")
                                    )
                                )
                            )
                        ),
                        new XElement("Misses",
                            new XElement("Node",
                                new XElement("X", "0"),
                                new XElement("Y", "0")
                            )
                        )
                    ),
                    new XElement("Player2",
                            new XElement("Ships",
                                new XElement("Ship",
                                    new XElement("startPos",
                                        new XElement("X", "8"),
                                        new XElement("Y", "0")
                                    ),
                                    new XElement("endPos",
                                        new XElement("X", "8"),
                                        new XElement("Y", "5")
                                    )
                                )
                            )
                        ),
                        new XElement("Misses",
                            new XElement("Node",
                                new XElement("X", "0"),
                                new XElement("Y", "0")
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
            database.Save(path + "\\data.xml");

            IEnumerable<string> test = from huehue in XDocument.Load(path + "\\data.xml")
                                                                 .Descendants("Ship")
                                       where (int)huehue.Element("startPos").Element("X") == 8
                                       select huehue.Element("endPos").Element("Y").Value;

            foreach (string s in test)
            {
                Console.WriteLine(s);
            }
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

        string path;


    }
}
