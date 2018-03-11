using System;
using System.Threading;

namespace ResetEventAuto.Demo
{
    class Program
    {
        static EventResolver<string> eventResolver;

        static void Main(string[] args)
        {
            eventResolver = new EventResolver<string>();

            eventResolver.SetEvent("fourthEvent");

            Console.WriteLine("Fourth event setted");

            ThreadPool.QueueUserWorkItem((x) => SecondTask());
            ThreadPool.QueueUserWorkItem((x) => ThirdTask());

            FirstTask();

            Console.ReadLine();
        }

        static void FirstTask()
        {
            var @event = eventResolver.RequestEvent("eventOne", "eventTwo", "eventThird", "fourthEvent");
            eventResolver.SetEvent(@event, true);
            Console.WriteLine("First event setted");
        }

        static void SecondTask()
        {
            Console.WriteLine("Begins second task");
            Thread.Sleep(3000);
            eventResolver.SetEvent("eventTwo");
            Console.WriteLine("Second event setted");
        }

        static void ThirdTask()
        {
            Console.WriteLine("Begins third task");
            Thread.Sleep(1000);
            eventResolver.SetEvent("eventThird");
            Console.WriteLine("Third event seted");
        }
    }
}
