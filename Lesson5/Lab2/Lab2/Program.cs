using System;


namespace Lab2
{
    interface IProcess
    {
        void DoWork();
    }

    class Process : IProcess
    {
        public void DoWork()
        {
            Console.WriteLine("Do Work");
        }
    }

    class Client
    {
        public  void StartProcess()
        {
            this.CreateProcess().DoWork();

        }

        protected virtual IProcess CreateProcess()
        {
            return new Process();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            client.StartProcess();
        }
    }
}
