using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ClientApp
{
	public partial class Form1 : Form
	{
		string ipadd = "fe80::2533:af37:2583:16da%6";
		string name = "Aidil";

		public Form1()
		{
			InitializeComponent();
		}

		private void SubmitBtn_Click(object sender, EventArgs e)
		{
			TcpClient client = new TcpClient(ipadd, 8080);

			int byteCount = Encoding.ASCII.GetByteCount(MessageBox.Text + 1);
			byte[] sendBuffer = new byte[byteCount];
			sendBuffer = Encoding.ASCII.GetBytes(MessageBox.Text + "");

			NetworkStream stream = client.GetStream();

			stream.Write(sendBuffer, 0, sendBuffer.Length);
			outputBox.AppendText(name + ": " + Encoding.ASCII.GetString(sendBuffer, 0, sendBuffer.Length) + "\n");

			MessageBox.Clear();
			stream.Close();
			client.Close();
		}
	}
}
