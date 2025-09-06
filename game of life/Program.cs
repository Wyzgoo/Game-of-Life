using game_of_life;

internal class Program
{
    // --- Environment variables ---
    public const int l = 300;
    public const int L = 150;
    public static int stateStarterCells = 2; // change the state of the starting cells here (1, 2 or 3)
    public static int numberStartCells = l*L/(1+stateStarterCells*2); // number of starting cells, depending on the state (the higher the state, the less cells)
    public static int SleepTime = 80; // time (in ms) between each generation
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
        for (int i = 0; i < numberStartCells; i++)
        {
            Random rand = new Random();

            int x = rand.Next(20, L - 20);
            int y = rand.Next(20, l - 20);

            map[x, y] = stateStarterCells;
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
    const string RED = "\u001b[31m";
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
                if (map[i, j] == 3)
                    sb.Append(GREEN + "██" + RESET);
                else if (map[i, j] == 2) 
                    sb.Append(YELLOW + "██" + RESET);
                else if (map[i, j] == 1) 
                    sb.Append(RED + "██" + RESET);
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

        for (int i = 2; i < L - 2; i++)
        {
            for (int j = 2; j < l - 2; j++)
            {
                // distance = 1
                int thrivingNear = 0;
                int matureNear = 0;
                int weakNear = 0;

                // distance = 2
                int thrivingFar = 0;
                int matureFar = 0;
                int weakFar = 0;

                for (int dx = -2; dx <= 2; dx++)
                {
                    for (int dy = -2; dy <= 2; dy++)
                    {
                        if (dx == 0 && dy == 0) continue; // skip self
                        if (Math.Abs(dx) == 2 && Math.Abs(dy) == 2) continue; // skip corners

                        int state = lastmap[i + dx, j + dy];
                        int dist;
                        if (dx == -2 || dx == 2 || dy == 2 || dy == -2)
                            dist = 2;
                        else
                            dist = 1;

                        if (dist == 1)
                        {
                            if (state == 3) thrivingNear++;
                            else if (state == 2) matureNear++;
                            else if (state == 1) weakNear++;
                        }
                        else if (dist == 2)
                        {
                            if (state == 3) thrivingFar++;
                            else if (state == 2) matureFar++;
                            else if (state == 1) weakFar++;
                        }
                    }
                }

                double score = (thrivingNear + 1.5 * thrivingFar) + (matureNear + 1 * matureFar) +(weakNear + 0.5 * weakFar);

                if (map[i, j] == 3) // Thriving
                {
                    if (score < 10 || score > 24) map[i, j] = 2;   // under/over population → downgrade
                    else map[i, j] = 3;                            // very stable Thriving zone
                }
                else if (map[i, j] == 2) // Mature
                {
                    if (score < 8 || score > 26) map[i, j] = 1;    // extreme cases → weaken
                    else if (score >= 18 && score <= 23)
                    {
                        // allow upgrade to Thriving ONLY if not overcrowded
                        if ((thrivingNear < 5 && matureNear < 6) || matureNear==8) map[i, j] = 3;
                        else map[i, j] = 2; // stay stable in dense areas
                    }
                    else map[i, j] = 2; // default stability
                }
                else if (map[i, j] == 1) // Weak
                {
                    if (score < 6)
                    {
                        map[i, j] = 0; // isolated → Dead
                    }
                    else if (score >= 14 && score <= 21 || matureNear>=6 || thrivingNear>=7 || matureFar>=7 || thrivingFar>=8)
                    {
                        map[i, j] = 2; // enough support → Mature (even if overcrowded)
                    }

                    else
                    {
                        map[i, j] = 1; // otherwise remain Weak
                    }
                }
                else if (map[i, j] == 0) // Dead
                {
                    if (score >= 8 && score <= 22 && thrivingNear < 4)
                        map[i, j] = 1;  // only grow near edges
                    else if (score > 22)
                    {
                        map[i, j] = 1; // overcrowded but not dead → remains Weak
                    }
                    else
                        map[i, j] = 0;
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