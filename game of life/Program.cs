internal class Program
{
    public const int l = 300;
    public const int L = 150;
    public static int AverageCells = l * L / 5;
    public static int[,] map = new int[L, l];
    private static void Main(string[] args)
    {
        int choice;
        Initializemap();
        do
        {
            Console.Write("1. Random\n2. Place cells and/or structures\n0. Exit\nChoice : ");
            choice = Convert.ToInt16(Console.ReadLine());

        } while (choice < 0 || choice > 2);

        if (choice == 0)
        {
            Console.WriteLine("Goodbye !");
        }
        else
        {
            if (choice == 1)
            {
                placecells();
            }
            else if (choice == 2)
            {
                Structures.ChooseStructure();
            }

            while (true)
            {
                printmap();
                System.Threading.Thread.Sleep(50);

                checkneighbor();
                Console.SetCursorPosition(0, 0);
            }
        }
    }

    static void placecells()
    {
        for (int i = 0; i < AverageCells; i++)
        {
            Random rand = new Random();

            int x = rand.Next(10, L - 10);
            int y = rand.Next(10, l - 10);

            map[x, y] = 2;
        }

    }

    static void Initializemap()
    {
        for (int i = 0; i < L; i++)
        {
            for (int j = 0; j < l; j++)
            {
                if (i == 0 || j == 0 || i == L - 1 || j == l - 1)
                    map[i, j] = -1;
                else
                    map[i, j] = 0;
            }
        }
    }

    // --- Colors ---
    const string GREEN = "\u001b[32m";
    const string YELLOW = "\u001b[33m";
    const string WHITE = "\u001b[37m";
    const string RESET = "\u001b[0m";

    static void printmap()
    {
        var sb = new System.Text.StringBuilder();

        for (int i = 0; i < L; i++)
        {
            for (int j = 0; j < l; j++)
            {
                if (map[i, j] == 2) 
                    sb.Append(GREEN + "██" + RESET);
                else if (map[i, j] == 1) 
                    sb.Append(YELLOW + "██" + RESET);
                else if (map[i, j] == -1) 
                    sb.Append(WHITE + "██" + RESET);
                else 
                    sb.Append("  ");
            }
            sb.AppendLine();
        }

        Console.Write(sb.ToString());
    }

    static void checkneighbor()
    {
        int[,] lastmap = new int[L, l];
        Array.Copy(map, lastmap, map.Length);

        for (int i = 1; i < L - 1; i++)
        {
            for (int j = 1; j < l - 1; j++)
            {
                int alivecells = 0;
                int semiAliveCells = 0;

                // count neighbors
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0) continue;// skip itself
                        int val = lastmap[i + x, j + y];
                        if (val == 2) alivecells++;
                        else if (val == 1) semiAliveCells++;
                    }
                }

                if (lastmap[i, j] == 2) // alive
                {
                    if (alivecells < 2 || alivecells > 3)
                        map[i, j] = 1;
                    else
                        map[i, j] = 2;
                }
                else if (lastmap[i, j] == 1) // semi
                {
                    if (alivecells == 3)
                        map[i, j] = 2;
                    else if (semiAliveCells >= 5 && alivecells < 3)
                        map[i, j] = 1;
                    else
                        map[i, j] = 0;
                }
                else if (lastmap[i, j] == 0) // dead
                {
                    if (alivecells == 3)
                        map[i, j] = 2;
                    else if (alivecells == 2 && semiAliveCells >= 1)
                        map[i, j] = 1;
                }
            }
        }
    }


    /* Basic game
    static void checkneighbor()
    {
        int[,] lastmap = new int[L, l];
        Array.Copy(map, lastmap, map.Length);

        for (int i = 1; i < L-1; i++)
        {
            for (int j = 1; j < l-1; j++)
            {

                int alivecells = 0;

                if (lastmap[i - 1, j - 1] == 1) alivecells++;
                if (lastmap[i - 1, j] == 1) alivecells++;
                if (lastmap[i - 1, j + 1] == 1) alivecells++;
                if (lastmap[i, j - 1] == 1) alivecells++;
                if (lastmap[i, j + 1] == 1) alivecells++;
                if (lastmap[i + 1, j - 1] == 1) alivecells++;
                if (lastmap[i + 1, j] == 1) alivecells++;
                if (lastmap[i + 1, j + 1] == 1) alivecells++;


                if (lastmap[i, j] == 1)
                {
                    if (alivecells < 2 || alivecells>3)
                        map[i, j] = 0;

                }
                else if (lastmap[i, j] == 0)
                {
                    if(alivecells==3)
                        map[i, j] = 1;
                        
                }
            }
        }
    }*/
}

public static class Structures
{
    // --- Structures ---
    static int[,] gliderUpLeftMap =
    {
        { 0, 0, 1, 0 },
        { 1, 2, 2, 0 },
        { 2, 2, 0, 1 },
        { 0, 1, 2, 0 }
    };
    static int[,] gliderUpRightMap =
    {
        { 0, 1, 0, 0 },
        { 0, 2, 2, 1 },
        { 1, 0, 2, 2 },
        { 0, 2, 1, 0 }
    };
    static int[,] gliderDownLeftMap =
    {
        { 0, 1, 2, 0 },
        { 2, 2, 0, 1 },
        { 1, 2, 2, 0 },
        { 0, 0, 1, 0 }
    };
    static int[,] gliderDownRightMap =
    {
        { 0, 2, 1, 0 },
        { 1, 0, 2, 2 },
        { 0, 2, 2, 1 },
        { 0, 1, 0, 0 }
    };

    static int[,] Osci3StepsMap =
    {
        {0,0,2,2,2,0,0,0,2,2,2,0,0 },
        {0,1,0,1,1,1,1,1,1,1,0,1,0 },
        {2,0,0,1,0,2,0,2,0,1,0,0,2 },
        {2,1,1,0,1,2,0,2,1,0,1,1,2 },
        {2,1,0,1,0,2,0,2,0,1,0,1,2 },
        {0,1,2,2,2,1,0,1,2,2,2,1,0 },
        {0,1,0,0,0,1,0,1,0,0,0,1,0 },
        {0,1,2,2,2,1,0,1,2,2,2,1,0 },
        {2,1,0,1,0,2,0,2,0,1,0,1,2 },
        {2,1,1,0,1,2,0,2,1,0,1,1,2 },
        {2,0,0,1,0,2,0,2,0,1,0,0,2 },
        {0,1,0,1,1,1,1,1,1,1,0,1,0 },
        {0,0,2,2,2,0,0,0,2,2,2,0,0 },
    };

    public static (int, int) ChooseCoordinates()
    {
        Console.WriteLine("Where do you want to set the curser ?");

        int x, y;
        do
        {
            Console.Write("x : ");
            x = Convert.ToInt32(Console.ReadLine());
        } while (x < 1 || x > Program.l - 1);

        do
        {
            Console.Write("y : ");
            y = Convert.ToInt32(Console.ReadLine());
        } while (y < 1 || y > Program.L - 1);

        return (x, y);
    }

    private static void PlaceStructure(int[,] structure, int x, int y)
    {
        for (int i = 0; i < structure.GetLength(0); i++)
        {
            for (int j = 0; j < structure.GetLength(1); j++)
            {
                Program.map[x + i, y + j] = structure[i, j];
            }
        }
    }

    // --- Menu ---
    public static void ChooseStructure()
    {
        int choice;
        do
        {
            Console.Write("Which structure do you want to place ?\n1. Glider\n2. Osci 3 steps\n3. Draw\n0. Start the game\nChoice : ");
            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1: SpawnGlider(); break;
                case 2: SpawnOsci3steps(); break;
                case 3: Draw(); break;
                case 0: Console.WriteLine("Game starting..."); break;
            }
        } while (choice != 0);
    }

    // --- Spawns ---
    public static void SpawnOsci3steps()
    {
        var (x, y) = ChooseCoordinates();
        PlaceStructure(Osci3StepsMap, x, y);
    }

    public static void SpawnGlider()
    {
        var (x, y) = ChooseCoordinates();

        int choice;
        do
        {
            Console.Write("Choose a direction :\n1.Up Left\n2. Up Right\n3. Down Left\n4. Down Right\nChoice : ");
            choice = Convert.ToInt32(Console.ReadLine());
        } while (choice < 1 || choice > 4);

        switch (choice)
        {
            case 1: PlaceStructure(gliderUpLeftMap, x, y); break;
            case 2: PlaceStructure(gliderUpRightMap, x, y); break;
            case 3: PlaceStructure(gliderDownLeftMap, x, y); break;
            case 4: PlaceStructure(gliderDownRightMap, x, y); break;
        }
    }

    static public void Draw()
    {
        int choice;
        do
        {
            Console.Write("1.Place pixels\n2. Draw rectangles/squares/lines\n0. Go back\nChoice : ");
            choice = Convert.ToInt16(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    var (x, y) = ChooseCoordinates();

                    int state;
                    do
                    {
                        Console.Write("Which state (-1 = Wall | 0 = Dead | 1 = Semi-Alive | 2 = Alive) ? : ");
                        state = Convert.ToInt16(Console.ReadLine());
                    } while (state < -1 || state > 2);

                    Program.map[x, y] = state;
                    break;

                case 2:
                    Console.Write("Choose the fisrt position. ");
                    var (x1, y1) = ChooseCoordinates();
                    Console.Write("Choose the second position. ");
                    var (x2, y2) = ChooseCoordinates();

                    int state2;
                    do
                    {
                        Console.Write("Which state (-1 = Wall | 0 = Dead | 1 = Semi-Alive | 2 = Alive) ? : ");
                        state2 = Convert.ToInt16(Console.ReadLine());
                    } while (state2 < -1 || state2 > 2);

                    int i = Math.Min(x1, x2);
                    int j = Math.Min(y1, y2);
                    int endi = Math.Max(x1, x2);
                    int endj = Math.Max(y1, y2);

                    for (; i <= endi; i++)
                    {
                        for (; j <= endj; j++)
                        {
                            Program.map[i, j] = state2;
                        }
                        j = Math.Min(y1, y2);
                    }
                    break;

                case 0: Console.WriteLine("Going back..."); break;
            }
        } while (choice != 0);
    }
}
