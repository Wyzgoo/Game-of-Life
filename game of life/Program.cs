using game_of_life;

internal class Program
{
    // --- Environment variables ---
    public const int l = 300;
    public const int L = 150;
    public static int AverageCells = l * L / 5;
    public static int SleepTime = 80;
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
                System.Threading.Thread.Sleep(SleepTime);

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
                    else if (semiAliveCells >= 4 && alivecells < 2)
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