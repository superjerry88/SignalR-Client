using System;
using Microsoft.AspNet.SignalR.Client;

namespace SignalRClient
{
    class Program
    {
        private static void Main(string[] args)
        {
            var connection = new HubConnection("http://localhost:8000/");
            var myHub = connection.CreateHubProxy("Function1"); // this one use to specify which HUB to use in server

            Console.WriteLine("Enter your AAS name");
            string name = Console.ReadLine();

            connection.Start().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection:{0}", task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Connected");

                    myHub.On<string, string>("addMessage", (s1, s2) => {
                        Console.WriteLine(s1 + ": " + s2);
                    });

                    while (true)
                    {
                        var message = Console.ReadLine();

                        if (string.IsNullOrEmpty(message))
                        {
                            break;
                        }

                        myHub.Invoke<string>("Send", name, message).ContinueWith(task1 => {
                            if (task1.IsFaulted)
                            {
                                Console.WriteLine("There was an error calling send: {0}", task1.Exception.GetBaseException());
                            }
                            else
                            {
                                Console.WriteLine(task1.Result);
                            }
                        });
                    }
                }

            }).Wait();

            Console.Read();
            connection.Stop();
        }
    }
}
