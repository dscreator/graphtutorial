using System;

namespace graphtutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(".Net Core Tutorial");
            int choice = -1;



            while (choice != 0)
            {
                Console.WriteLine("0. To exit");
                Console.WriteLine("1. Display access token");
                Console.WriteLine("2. To exit");
                choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 0:
                        break;
                    case 1:
                        //display access token
                        Console.WriteLine("The Access Token is: ");
                        break;
                    case 2:
                        // display calendar events
                        Console.WriteLine("The Calendar events are: ");
                        break;


                }

            }
            

        }
    }
}
