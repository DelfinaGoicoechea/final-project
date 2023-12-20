using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;

public class TCPServer : MonoBehaviour
{

    private Int32 port = 0;
    private IPAddress ip = IPAddress.Parse("127.0.0.1");
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    public static TCPServer Instance { get; private set; }
	public TMP_Text toPrint;
	//private bool ipUpdated = false;
	private string errorHere = null;

    // Start is called before the first frame update
    void Start()
    {
        tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
		tcpListenerThread.IsBackground = true; 		
		tcpListenerThread.Start();
    }

    void Update () { 		
		//if (eventoMetrica?) {             
			//SendMessage(Metrica);         
		//} 	
		if (errorHere == null)
		{
			//toPrint.text = "ip:" + ip + " port:" + port;
		}
		else
		{
			//toPrint.text = errorHere;
		}
	}

	private void ListenForIncommingRequests () { 		
        try {
            ip = getIP();
			tcpListener = new TcpListener(ip, 0); //TCP port 0 gives first disponible
            tcpListener.Start(); 
            port = ((IPEndPoint)tcpListener.LocalEndpoint).Port; //Busco saber cual es 			             
			Debug.Log("Server is listening on ip: " + ip + " port: " + port);
			// Busco imprimir en el oculus
			Byte[] bytes = new Byte[1024];

			while (true) { 				
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) { 					
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream()) { 						
						int length; 											
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 							
							var incommingData = new byte[length]; 							
							Array.Copy(bytes, 0, incommingData, 0, length);  							
							// Convert byte array to string message. HTTP 							
							string clientMessage = Encoding.ASCII.GetString(incommingData); 							
							Debug.Log("Client message received as: " + clientMessage); 						
						}

                        //Dispatch 	


                        //SendMessage				
					} 				
				} 			
			}

		} 		
		catch (SocketException socketException) { 			
			Debug.LogWarning("SocketException " + socketException.ToString());
			errorHere =	"SocketException " + socketException.ToString();

			// LO SIGUIENTE VA A CREAR UN ERROR EN TIEMPO DE EJECUCION
			toPrint.text = "SocketException " + socketException.ToString();
		}
        /*finally {
            tcpListener.Stop(); //O close? En versiones viejas no mata el stream, verificar
            Debug.Log("Server closedf");
            tcpListenerThread.Abort();
        } */   
	}  	

    private void OnDestroy() {
        tcpListener.Stop(); //O close? En versiones viejas no mata el stream, verificar
        tcpListenerThread.Abort();
        Debug.Log("Server closed");
    }
	
	/*private void SendMessage(Metrica?) { 		
		if (connectedTcpClient == null) {             
			return;         
		}  		
		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = connectedTcpClient.GetStream(); 			
			if (stream.CanWrite) {                 
				string serverMessage = Metrica; 			            
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage); 				               
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);               
				Debug.Log("Server sent his message - should be received by client");           
			}       
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		} 	
	}*/

    private IPAddress getIP() {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return IPAddress.Parse(ip.ToString());
            }
        }
		Debug.LogError("No se encontro la direccion IP");
		return null;
    }

}
