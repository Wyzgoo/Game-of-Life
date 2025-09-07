using game_of_life;

internal class Program
{
    // --- Environment variables ---
    public const int l = 300;
    public const int L = 150;
    public static int numberStartCells = l * L / 2; // number of starting cells, depending on the side of the grid
    public static int SleepTime = 50; // time (in ms) between each generation
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
                Draw.Drawing();
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
        Random rand = new Random();
        for (int i = 0; i < numberStartCells; i++)
        {
            int x = rand.Next(20, L - 20);
            int y = rand.Next(20, l - 20);
            int decider = rand.Next(1, 7);
            map[x, y] = decider;
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

    static void printmap()
    {
        var sb = new System.Text.StringBuilder();

        for (int i = 0; i < L; i++)
        {
            for (int j = 0; j < l; j++)
            {
                if (map[i, j] == 3) sb.Append("\u001b[32m██\u001b[0m"); // Green Thriving A
                else if (map[i, j] == 2) sb.Append("\u001b[33m██\u001b[0m"); // Yellow Mature A
                else if (map[i, j] == 1) sb.Append("\u001b[31m██\u001b[0m"); // Red Weak A
                else if (map[i, j] == 4) sb.Append("\u001b[36m██\u001b[0m"); // Cyan Weak B
                else if (map[i, j] == 5) sb.Append("\u001b[34m██\u001b[0m"); // Blue Mature B
                else if (map[i, j] == 6) sb.Append("\u001b[35m██\u001b[0m"); // Magenta Thriving B
                else if (map[i, j] == -1) sb.Append("\u001b[37m██\u001b[0m"); // While Wall
                else sb.Append("  ");
            }
            sb.AppendLine();
        }

        Console.Write(sb.ToString());
    }

    static void checkneighbor()
    {
        int[,] lastmap = new int[L, l];
        Array.Copy(map, lastmap, map.Length);
        Random rand = new Random();

        for (int i = 2; i < L - 2; i++)
        {
            for (int j = 2; j < l - 2; j++)
            {
                // distance = 1
                int thrivingNearA = 0, matureNearA = 0, weakNearA = 0;
                int thrivingNearB = 0, matureNearB = 0, weakNearB = 0;

                // distance = 2
                int thrivingFarA = 0, matureFarA = 0, weakFarA = 0;
                int thrivingFarB = 0, matureFarB = 0, weakFarB = 0;

                // --- enemy pressure counters ---
                double enemyPressureA = 0;
                double enemyPressureB = 0;

                for (int dx = -2; dx <= 2; dx++)
                {
                    for (int dy = -2; dy <= 2; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        if (Math.Abs(dx) == 2 && Math.Abs(dy) == 2) continue; // skip corners

                        int state = lastmap[i + dx, j + dy];
                        int dist = (dx == -2 || dx == 2 || dy == 2 || dy == -2) ? 2 : 1;

                        // --- count allies and enemies ---
                        if (dist == 1)
                        {
                            if (state == 6) thrivingNearB++;
                            else if (state == 5) matureNearB++;
                            else if (state == 4) weakNearB++;
                            else if (state == 3) thrivingNearA++;
                            else if (state == 2) matureNearA++;
                            else if (state == 1) weakNearA++;
                        }
                        else if (dist == 2)
                        {
                            if (state == 6) thrivingFarB++;
                            else if (state == 5) matureFarB++;
                            else if (state == 4) weakFarB++;
                            else if (state == 3) thrivingFarA++;
                            else if (state == 2) matureFarA++;
                            else if (state == 1) weakFarA++;
                        }

                        // --- enemy pressure ---
                        if (lastmap[i, j] >= 1 && lastmap[i, j] <= 3) // Team A cell
                        {
                            if (state == 4) enemyPressureA += 0.2;    // Weak B
                            else if (state == 5) enemyPressureA += 0.5; // Mature B
                            else if (state == 6) enemyPressureA += 0.8; // Thriving B
                        }
                        else if (lastmap[i, j] >= 4 && lastmap[i, j] <= 6) // Team B cell
                        {
                            if (state == 1) enemyPressureB += 0.2;    // Weak A
                            else if (state == 2) enemyPressureB += 0.5; // Mature A
                            else if (state == 3) enemyPressureB += 0.8; // Thriving A
                        }
                    }
                }

                double scoreA = (thrivingNearA + 1.5 * thrivingFarA) + (matureNearA + 1 * matureFarA) + (weakNearA + 0.5 * weakFarA);
                double scoreB = (thrivingNearB + 1.5 * thrivingFarB) + (matureNearB + 1 * matureFarB) + (weakNearB + 0.5 * weakFarB);

                // === Team A evolution ===
                if (map[i, j] == 3) // Thriving
                {
                    if (scoreA < 10 || scoreA > 24) map[i, j] = 2;
                    else map[i, j] = 3;
                }
                else if (map[i, j] == 2) // Mature
                {
                    if (scoreA < 8 || scoreA > 26) map[i, j] = 1;
                    else if (scoreA >= 18 && scoreA <= 23)
                    {
                        if ((thrivingNearA < 5 && matureNearA < 6) || matureNearA == 8) map[i, j] = 3;
                        else map[i, j] = 2;
                    }
                    else map[i, j] = 2;
                }
                else if (map[i, j] == 1) // Weak
                {
                    if (scoreA < 6)
                        map[i, j] = 0;
                    else if (scoreA >= 14 && scoreA <= 21 || matureNearA >= 6 || thrivingNearA >= 7 || matureFarA >= 7 || thrivingFarA >= 8)
                        map[i, j] = 2;
                    else
                        map[i, j] = 1;
                }

                // === Team B evolution ===
                else if (map[i, j] == 6) // Thriving
                {
                    if (scoreB < 10 || scoreB > 24) map[i, j] = 5;
                    else map[i, j] = 6;
                }
                else if (map[i, j] == 5) // Mature
                {
                    if (scoreB < 8 || scoreB > 26) map[i, j] = 4;
                    else if (scoreB >= 18 && scoreB <= 23)
                    {
                        if ((thrivingNearB < 5 && matureNearB < 6) || matureNearB == 8) map[i, j] = 6;
                        else map[i, j] = 5;
                    }
                    else map[i, j] = 5;
                }
                else if (map[i, j] == 4) // Weak
                {
                    if (scoreB < 6)
                        map[i, j] = 0;
                    else if (scoreB >= 14 && scoreB <= 21 || matureNearB >= 6 || thrivingNearB >= 7 || matureFarB >= 7 || thrivingFarB >= 8)
                        map[i, j] = 5;
                    else
                        map[i, j] = 4;
                }
                else if (map[i, j] == 0) // Dead
                {
                    if ((scoreA >= 8 && scoreA <= 22 && thrivingNearA < 4) || (scoreB >= 8 && scoreB <= 22 && thrivingNearB < 4))
                    {
                        int decider = rand.Next(2);
                        if (scoreA > scoreB) map[i, j] = 1;
                        else if (scoreA < scoreB) map[i, j] = 4;
                        else map[i, j] = (decider == 0) ? 1 : 4;
                    }
                    else if (scoreA > 22)
                        map[i, j] = 1;
                    else if (scoreB > 22)
                        map[i, j] = 4;
                    else map[i, j] = 0;
                }

                // === Combat Phase (after evolution) ===
                if (map[i, j] >= 1 && map[i, j] <= 3) // Team A
                {
                    double survivalChance = Math.Max(0, 1.0 - enemyPressureA * 0.15);
                    if (rand.NextDouble() > survivalChance)
                    {
                        if (map[i, j] == 3) map[i, j] = 2; // Thriving → Mature
                        else if (map[i, j] == 2) map[i, j] = 1; // Mature → Weak
                        else if (map[i, j] == 1) map[i, j] = 4; // Weak A → Weak B
                    }
                }
                else if (map[i, j] >= 4 && map[i, j] <= 6) // Team B
                {
                    double survivalChance = Math.Max(0, 1.0 - enemyPressureB * 0.15);
                    if (rand.NextDouble() > survivalChance)
                    {
                        if (map[i, j] == 6) map[i, j] = 5; // Thriving → Mature
                        else if (map[i, j] == 5) map[i, j] = 4; // Mature → Weak
                        else if (map[i, j] == 4) map[i, j] = 1; // Weak B → Weak A
                    }
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