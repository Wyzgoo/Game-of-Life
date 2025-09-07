namespace game_of_life
{
    public class Draw
    {
        

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

        static public void Drawing()
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
                            Console.Write("Which state (-1 = Wall | 0 = DeadA | 1 = WeakA | 2 = MatureA | 3 = ThrivingA | 4 = WeakB | 5 = MatureB | 6 = ThrivingB) ? : ");
                            state = Convert.ToInt16(Console.ReadLine());
                        } while (state < -1 || state > 6);

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
                            Console.Write("Which state (-1 = Wall | 0 = DeadA | 1 = WeakA | 2 = MatureA | 3 = ThrivingA | 4 = WeakB | 5 = MatureB | 6 = ThrivingB) ? : ");
                            state2 = Convert.ToInt16(Console.ReadLine());
                        } while (state2 < -1 || state2 > 6);

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
}
