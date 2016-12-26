using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NahroTo.SharpFileTransferrer
{
	public class SharpServer
	{
		private const int MAX_PACKET_SIZE = 32768;
		private static FileStream fileStream;
		private static string saveDirectory;

		public static void ReceiveFiles(int port, string saveDir)
		{
			// quick h4x in case directorypath doesn't end with slash.
			saveDirectory = ((saveDir.EndsWith("\\") || saveDir.EndsWith("/") ? saveDir : saveDir + "\\"));
			Thread thread = new Thread(new ParameterizedThreadStart(TcpServerRun));
			thread.IsBackground = true;
			thread.Start(port);
		}

		private static void TcpServerRun(object port)
		{
			TcpListener tcpListener = new TcpListener(IPAddress.Any, (int)port);
			tcpListener.Start();
			while (true)
			{
				TcpClient client = tcpListener.AcceptTcpClient();
				Thread tcpHandlerThread = new Thread(new ParameterizedThreadStart(tcpHandler));
				tcpHandlerThread.IsBackground = true;
				tcpHandlerThread.Start(client);
			}
		}

		private static void tcpHandler(object client)
		{
			try
			{
				using (TcpClient mClient = (TcpClient)client)
				{
					using (NetworkStream stream = mClient.GetStream())
					{
						byte[] amountFilesBuffer = readFromStream(4, stream);
						int amountFiles = BitConverter.ToInt32(amountFilesBuffer, 0);
						for (int i = 0; i < amountFiles; i++)
						{

							string fileName = null;
							while (true)
							{
								byte[] bufferLength = readFromStream(4, stream);
								int dataLength = BitConverter.ToInt32(bufferLength, 0);
								byte[] bufferHeader = readFromStream(1, stream);
								byte header = bufferHeader[0];
								byte[] dataBuffer = readFromStream(dataLength, stream);
								switch (header)
								{
									case 0:
									fileName = Encoding.ASCII.GetString(dataBuffer);
									openFileStream(fileName);
									break;
									case 1:
									case 2:
									writeToFile(fileName, dataBuffer);
									break;
								}

								if (header == 2)
								{
									break;
								}
							}
							closeFileStream();

						}
						stream.Close();

					}
					mClient.Close();
				}
			}
			catch (Exception)
			{
			}
		}

		private static byte[] readFromStream(int dataLength, NetworkStream stream)
		{
			int receivedBytes = 0;
			byte[] data = new byte[dataLength];

			while (receivedBytes < dataLength)
			{
				receivedBytes += stream.Read(data, receivedBytes, dataLength - receivedBytes);
			}
			return data;
		}

		private static void openFileStream(string fileName)
		{
			Directory.CreateDirectory(@saveDirectory);
			fileStream = new FileStream(@saveDirectory + fileName, FileMode.Create);
		}

		private static void writeToFile(string fileName, byte[] dataPacket)
		{
			fileStream.Write(dataPacket, 0, dataPacket.Length);
		}

		private static void closeFileStream()
		{
			fileStream.Close();
		}
	}
}
