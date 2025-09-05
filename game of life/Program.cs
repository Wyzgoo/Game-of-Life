using game_of_life;

internal class Program
{
    // --- Environment variables ---
    public const int l = 300;
    public const int L = 150;
    public static int AverageCells = 5000;
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

            int x = rand.Next(30, L - 30);
            int y = rand.Next(30, l - 30);

            map[x, y] = 3;
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
                    map[i, j] = (score < 10 || score > 22) ? 2 : 3;
                }
                else if (map[i, j] == 2) // Mature
                { 
                    if (score < 8 || score > 24) map[i, j] = 1;
                    else if (score>=23) map[i, j] = 3;
                    else if (score >= 18 && score <= 22) map[i, j] = 3;
                    else if (thrivingNear >= 7 || matureNear >= 6 || thrivingFar >= 6 || matureFar >= 5) map[i, j] = 3;
                    else map[i, j] = 2;
                }
                else if (map[i, j] == 1) // Weak
                {
                    if (score < 6) map[i, j] = 0;
                    else if (score>=17) map[i, j] = 2;
                    else if (score >= 15 && score <= 21) map[i, j] = 2;
                    else if (thrivingNear >= 6 || matureNear >= 5 || thrivingFar>=6 || matureFar>=5) map[i, j] = 2;
                    else map[i, j] = 1;
                }
                else if (map[i, j] == 0) // Dead
                { 
                    map[i, j] = (score >= 8 && score <= 18) ? 1 : 0;
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