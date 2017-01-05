using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    public class Rules
    {
        public bool validFire(int x, int y, Board b)
        {
            if ((x < 0 || x > 10 || y < 0 || y > 10) || (b.isHit(x, y)))
            {
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
}
