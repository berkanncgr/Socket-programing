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
using System.Threading;

namespace seerver
{
    public partial class Form1 : Form
    {
        public object _locker,_locker_fibo,_locker_fak,_locker_hesap;
        public Form1()
        {
            InitializeComponent();
            _locker = new object();
            _locker_fak = new object();
            _locker_fibo = new object();
            _locker_hesap = new object();
            textBox1.Text = ((IPAddress)ipAddress).ToString();
        }
        
        IPAddress ipAddress = IPAddress.Loopback;
        IPAddress gecici = IPAddress.Loopback;
        public string data = null;
        private void button1_Click(object sender, EventArgs e)
        {
            label2.Text = "Bağlantı Bekleniyor";
            System.Windows.Forms.Form.CheckForIllegalCrossThreadCalls=false;
            Thread server_calistir = new Thread(new ThreadStart(server));
            server_calistir.Priority = ThreadPriority.Highest;
           
            server_calistir.Start();
    
        }
        string sonuc = null,sonuc_fakt=null,sonuc_fibo=null;
        string[] hesap = null;
        public void hesapla()
        {
            lock(_locker_hesap)
            {
                if (hesap[2].Equals("+"))
                {
                    sonuc = (Int32.Parse(hesap[0]) + Int32.Parse(hesap[1])).ToString();
                }
                else if (hesap[2].Equals("-"))
                {
                    sonuc = (Int32.Parse(hesap[0]) - Int32.Parse(hesap[1])).ToString();
                }
                else if (hesap[2].Equals("*"))
                {
                    sonuc = (Int32.Parse(hesap[0]) * Int32.Parse(hesap[1])).ToString();
                }
                else if (hesap[2].Equals("/"))
                {
                    sonuc = (Int32.Parse(hesap[0]) / Int32.Parse(hesap[1])).ToString();
                }
                label2.Text = "İşlem: " + hesap[0] + hesap[2] + hesap[1] + "\nSonuç: " + sonuc;
            }
        }
        int kontrol = 0;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            kontrol = 1;
        }
       

        public void fibonacci()
        {
            int n1 = 0, n2 = 1, n3, j, number;
            number = Int32.Parse(hesap[0]);
            sonuc_fibo = number + " Elemanli Fibonacci Serisi\n" + n1 + " " + n2 + " ";
            for (j = 2; j < number; ++j)
            {
                n3 = n1 + n2;
                sonuc_fibo += n3 + " ";
                n1 = n2;
                n2 = n3;
            }
        }
        public void faktoriyel()
        {
            int number = Int32.Parse(hesap[0]), gecici = 1;
            for (int i = 2; i <= number; ++i)
            {
                gecici = gecici * i;
            }
            sonuc_fakt = number + "! Sonucu: "+ gecici.ToString()+"\n";
        }
        
        public void server()
        {
            //IPEndPoint localEP = new IPEndPoint(ipAddress, 11000);
            //lock(_locker)
            {
                kontrol = 0;
                try
                {

                    

                    // Specify how many requests a Socket can listen before it gives Server busy response.  
                    // We will listen 10 requests at a time 
                    Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // A Socket must be associated with an endpoint using the Bind method  

                    listener.Bind(new IPEndPoint(IPAddress.Parse(textBox1.Text), 11000));
                    listener.Listen(10);
                    while (true)
                    {
                        
                        Socket handler = listener.Accept();
                        // Incoming data from the client.    
                        string data = null;
                        byte[] bytes = null;

                        while (true)
                        {
                            bytes = new byte[1024];
                            int bytesRec = handler.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            if (data.IndexOf("?") > -1)
                            {
                                break;
                            }
                        }
                        hesap = null;
                        hesap = data.Split('#');
                        sonuc = null;
                        byte[] msg;

                        //Thread fibo= new Thread(new ThreadStart(fibonacci));
                        if (hesap.Length > 3)
                        {
                            hesapla();
                            label2.Text = sonuc;
                            msg = Encoding.ASCII.GetBytes(sonuc);
                            handler.Send(msg);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                        }
                        else if (hesap.Length<3 && hesap.Length>0)
                        {
                            Thread faktor = new Thread(new ThreadStart(faktoriyel));
                            Thread fibo = new Thread(new ThreadStart(fibonacci));
                            faktor.Start();
                            fibo.Start();
                            sonuc = sonuc_fakt + sonuc_fibo;
                            label2.Text = sonuc;
                            msg = Encoding.ASCII.GetBytes(sonuc);
                            handler.Send(msg);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                        }
                       
                        else
                        {
                            sonuc = "Hatalı Tuşlama";
                            msg = Encoding.ASCII.GetBytes(sonuc);
                            handler.Send(msg);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                        }
                        Console.Read();
                        if(kontrol==1)
                        {
                            listener.Shutdown(SocketShutdown.Both);
                            listener.Close();
                            break;
                        }
                    }

                }
                catch (Exception et)
                {
                    
                }
            }
           


        }
    }
    
}
