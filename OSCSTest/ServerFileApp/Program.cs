using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ServerFileApp
{
	class Program
	{
		static void Main(string[] args)
		{
			TcpListener server = new TcpListener(IPAddress.IPv6Any, 8080);
			TcpClient client = default(TcpClient);

			try
			{
				server.Start();
				Console.WriteLine("Server has started...");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

            while (true)
            {
                client = server.AcceptTcpClient();

                byte[] receivedBuffer = new byte[2048];
                NetworkStream stream = client.GetStream();

                // Read 'intent' from client

                StreamReader reader = new StreamReader(stream);
                string intent = reader.ReadLine();

                switch (intent)
                {
                    case "Upload":
                        string filename = reader.ReadLine();
                        string owner = reader.ReadLine();
                        string share = reader.ReadLine();
                        Console.Write("FileName: " + filename + "\n");
                        Console.Write("Owner: " + owner + "\n");
                        Console.Write("Shared With: " + share + "\n");
                        stream.CopyTo(new FileStream(@"D:\filetransfer\" + filename, FileMode.Create, FileAccess.Write));
                        break;

                    case "Retrieve":
                        //Use SQL to retrieve database i.e what files are there in 'abc' group
                        break;

                    //case etc...
                }
				Console.WriteLine("Success!");
				client.Close();
				stream.Close();
			}
		}
	}
}
