﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
//using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Variables
        public TcpClient clientSocket;
        public NetworkStream serverStream = default(NetworkStream);
        string readData = null;
        Thread ctThread;
        string name = Environment.UserName;
        List<string> nowChatting = new List<string>();
        List<string> chat = new List<string>();
        #endregion

        #region Ineffectible variables

        //Dictionary<string, Object> nowChatting = new Dictionary<string, Object>();

        #endregion

        //You can send commands from the server to the client with the following method.
        #region Send Method
        /*
         try
            {
                if (!input.Text.Equals(""))
                {
                    chat.Add("gChat");
                    chat.Add(input.Text);
                    byte[] outStream = ObjectToByteArray(chat);

                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();
                    input.Text = "";
                    chat.Clear();
                }
            }
            catch (Exception er)
            {
                btnConnect.Enabled = true;
            } 
         */

        #endregion

        //If you don't know what you're doing, don't change it.
        #region Setup

        bool SocketConnected(TcpClient s) //check whether client is connected server
        {
            bool flag = false;
            try
            {
                bool part1 = s.Client.Poll(10, SelectMode.SelectRead);
                bool part2 = (s.Available == 0);
                if (part1 && part2)
                {
                    this.Invoke((MethodInvoker)delegate // cross threads
                    {

                    });
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception er)
            {
                Console.WriteLine(er);
            }
            return flag;
        }

        public Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        /*
        public void getUsers(List<string> parts)
        {
            this.Invoke((MethodInvoker)delegate
            {
                listBox1.Items.Clear();
                for (int i = 1; i < parts.Count; i++)
                {
                    listBox1.Items.Add(parts[i]);

                }
            });
        }
        */

        private void getMessage()
        {
            try
            {
                while (true)
                {
                    serverStream = clientSocket.GetStream();
                    byte[] inStream = new byte[10025];
                    serverStream.Read(inStream, 0, inStream.Length);
                    List<string> parts = null;

                    if (!SocketConnected(clientSocket))
                    {
                        MessageBox.Show("You've been Disconnected");
                        ctThread.Abort();
                    }

                    parts = (List<string>)ByteArrayToObject(inStream);
                    switch (parts[0])
                    {
                        case "userList":
                            //getUsers(parts);
                            break;

                        case "gChat":
                            readData = "" + parts[1];
                            msg();
                            break;

                        case "pChat":
                            //managePrivateChat(parts);
                            break;
                    }

                    if (readData[0].Equals('\0'))
                    {
                        readData = "Reconnect Again";
                        msg();

                        this.Invoke((MethodInvoker)delegate // To Write the Received data
                        {

                        });

                        ctThread.Abort();
                        clientSocket.Close();
                        break;
                    }
                    chat.Clear();
                }
            }
            catch (Exception e)
            {
                ctThread.Abort();
                clientSocket.Close();
                Console.WriteLine(e);
            }

        }

        private void msg()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else
                richTextBox1.Text = readData;
        }

        /*
        public void managePrivateChat(List<string> parts)
        {

            this.Invoke((MethodInvoker)delegate // To Write the Received data
            {
                if (parts[3].Equals("new"))
                {
                    formPrivate privateC = new formPrivate(parts[2], clientSocket, name);
                    nowChatting.Add(parts[2]);
                    privateC.Text = "Private Chat with " + parts[2];
                    privateC.Show();
                }
                else
                {
                    if (Application.OpenForms["formPrivate"] != null)
                    {
                        (Application.OpenForms["formPrivate"] as formPrivate).setHistory(parts[3]);
                    }
                }

            });

        }
        */

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
        
        #region Connection setup
        bas:
            clientSocket = new TcpClient();
            try
            {
                clientSocket.Connect("127.0.0.1", 6606);
                readData = "Connected to Server ";
                msg();

                serverStream = clientSocket.GetStream();

                byte[] outStream = Encoding.ASCII.GetBytes(name + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                ctThread = new Thread(getMessage);
                ctThread.Start();
            }

            catch (Exception ex)
            {
                goto bas;
            }

            #endregion


        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Contains("MSGBOX"))
            {
                MessageBox.Show("Test Message");
            }
        }
    }
}
