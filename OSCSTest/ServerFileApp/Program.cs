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

				stream.CopyTo(new FileStream("D:/filetransfer/test.docx", FileMode.Create, FileAccess.Write));
			}
		}
	}
}
