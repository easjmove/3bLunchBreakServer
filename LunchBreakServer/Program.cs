using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LunchBreakServer
{
    class Program
    {
        public static List<int> results = new List<int>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 10005);
            listener.Start();

            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                Console.WriteLine("New client: " + socket.Client.RemoteEndPoint.ToString());

                Task.Run(() => { NewClient(socket); });
            }
        }

        static void NewClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            writer.WriteLine("Kunne du tænke dig at holde tidligere fri?\r\nSend stem <nr>: 1 for ja, 2 for nej, 3 for blank\r\nVil du se resultatet send result");
            writer.Flush();
            string message = reader.ReadLine();
            if (message.ToLower().StartsWith("stem "))
            {
                int result = int.Parse(message.Split(" ")[1]);
                if (result < 4 && result > 0)
                {
                    results.Add(result);
                    Console.WriteLine("Stemme afgivet: " + result);
                    writer.WriteLine("Tak for din stemme");
                    writer.Flush();
                } else
                {
                    writer.WriteLine("Din hat!");
                    writer.Flush();
                }
            } else if (message.ToLower() == "result")
            {
                int jaVotes = 0;
                int nejVotes = 0;
                int blankVotes = 0;
                foreach (int vote in results)
                {
                    switch (vote)
                    {
                        case 1:
                            jaVotes++;
                            break;
                        case 2:
                            nejVotes++;
                            break;
                        case 3:
                            blankVotes++;
                            break;
                    }
                }
                writer.WriteLine($"Antal ja: {jaVotes}");
                writer.WriteLine($"Antal nej: {nejVotes}");
                writer.WriteLine($"Antal blank: {blankVotes}");
                writer.Flush();
            } else
            {
                writer.WriteLine("Din hat af mug!");
                writer.Flush();
            }
            socket.Close();
        }
    }
}
