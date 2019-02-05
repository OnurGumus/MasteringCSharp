using System;

namespace WhyFunctional
{
    class Car
    {
        private bool isRunning;
        private int speed;
        public void Start()
        {
            if (isRunning) throw new InvalidOperationException("Already started");
            isRunning = true;
        }

        public void Accelerate()
        {
            if (!isRunning) throw new InvalidOperationException("Not yet started");
            speed += 10;

        }

        public void Stop()
        {
            if (!isRunning) throw new InvalidOperationException("Not yet started");
            isRunning = true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var c = new Car();
            c.Start();
            DoSomeThing(c);
            //How can I be sure if the car is still running?
            c.Stop();
        }

        public static void DoSomeThing(Car c)
        {
            //How can I be sure if the car is running?
            c.Accelerate();
            // Should I stop the car before returning?
        }

        public static string GetUserName(int id)
        {
            //what should I return if id is not found ?
            return null;
        }

        public static void ManageUser()
        {
            var userName = GetUserName(1);
            Console.WriteLine(userName.Length);
        }
    }
}
