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
using System.IO;
using System.IO.Compression;

namespace ClientFileApp
{
	public partial class Form1 : Form
	{
		OpenFileDialog opd = new OpenFileDialog();
		string ipadd = "fe80::2533:af37:2583:16da%6";

		public Form1()
		{
			InitializeComponent();
		}

		private void fileDialogButton_Click(object sender, EventArgs e)
		{
			pathBox.Text = openFileDialog();
		}

		private void sendBtn_Click(object sender, EventArgs e)
		{
			TcpClient client = new TcpClient(ipadd, 8080);

			FileStream source = new FileStream(pathBox.Text, FileMode.Open);

			byte[] sendBuffer = new byte[source.Length];

			NetworkStream stream = client.GetStream();

			source.CopyTo(stream);

			source.Close();
			stream.Close();
			client.Close();
			pathBox.Clear();
		}

		private string openFileDialog()
		{
			if (opd.ShowDialog() == DialogResult.OK)
			{
				return opd.FileName;
			}
			else
			{
				return null;
			}
		}
	}
}
