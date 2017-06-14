using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerApp
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

				stream.Read(receivedBuffer, 0, receivedBuffer.Length);

				StringBuilder msg = new StringBuilder();

				foreach(byte b in receivedBuffer)
				{
					if (b.Equals(00))
					{
						break;
					}
					else
					{
						msg.Append(Convert.ToChar(b).ToString());
					}
				}
				Console.WriteLine(msg.ToString() + "\t" + msg.Length);
			}
		}
	}
}
