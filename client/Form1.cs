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

namespace client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            textBox1.Text = ipAddress.ToString();
        }
        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAddress=IPAddress.Loopback;
        private void button1_Click(object sender, EventArgs e)
        {
            byte[] bytes = new byte[1024];

            try
            {
                // Connect to a Remote server  
                // Get Host IP Address that is used to establish a connection  
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
                // If a host has multiple addresses, you will get a list of addresses  

                
                IPEndPoint remoteEP=new IPEndPoint(IPAddress.Parse(textBox1.Text), 11000);
                
                

                // Create a TCP/IP  socket.    
                Socket sender1 = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.    
                try
                {
                    // Connect to Remote EndPoint  
                    sender1.Connect(remoteEP);

                    

                    // Encode the data string into a byte array.    
                    byte[] msg = Encoding.ASCII.GetBytes(textBox2.Text+"?");

                    // Send the data through the socket.    
                    int bytesSent = sender1.Send(msg);

                    // Receive the response from the remote device.    
                    int bytesRec = sender1.Receive(bytes);
                    label1.Text="Sonuç: "+Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    // Release the socket.    
                    sender1.Shutdown(SocketShutdown.Both);
                    sender1.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception ep)
                {
                    Console.WriteLine("Unexpected exception : {0}", ep.ToString());
                }

            }
            catch (Exception es)
            {
                Console.WriteLine(es.ToString());
            }
        }
    }

    
}
