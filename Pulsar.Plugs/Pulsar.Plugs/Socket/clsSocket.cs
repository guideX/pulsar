using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
namespace Pulsar.Plugs.Communications {
  class StateObject {
    public System.Net.Sockets.Socket WorkSocket = null;
    public int BufferSize = 32767;
    public byte[] Buffer = new byte[32768];
  }
  class AsyncServer {
    private int m_SocketPort;
    public event ConnectionAcceptEventHandler ConnectionAccept;
    public delegate void ConnectionAcceptEventHandler(AsyncSocket tmp_Socket);
    public event SocketErrorEventHandler SocketError;
    public delegate void SocketErrorEventHandler(string lData);
    private bool lClosed;
    public AsyncServer(int lPort) {
      try {
        m_SocketPort = lPort;
        
      }
      catch (Exception ex) {
        if (SocketError != null) {
          SocketError(ex.Message);
        }
      }
    }
    public void Start() {
      try {
        IPAddress lListenIP = IPAddress.Any;
        int lListenPort = m_SocketPort;
        IPEndPoint lListenEP = new IPEndPoint(lListenIP, lListenPort);
        if (lClosed == true) {
          lClosed = false;
          return;
        }
        System.Net.Sockets.Socket obj_Socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        obj_Socket.Bind(lListenEP);
        obj_Socket.Listen(100);
        obj_Socket.BeginAccept(new AsyncCallback(onIncomingConnection), obj_Socket);
      }
      catch (Exception ex) {
        if (SocketError != null) {
          SocketError(ex.Message);
        }
      }
    }
    public void Close() {
      try {
        lClosed = true;
      }
      catch (Exception ex) {
        if (SocketError != null) {
          SocketError(ex.Message);
        }
      }
    }
    private void onIncomingConnection(IAsyncResult lResult) {
      try {
        System.Net.Sockets.Socket obj_Socket = (System.Net.Sockets.Socket)lResult.AsyncState;
        System.Net.Sockets.Socket obj_Connected = obj_Socket.EndAccept(lResult);
        if (lClosed == true) {
          obj_Connected.Shutdown(SocketShutdown.Both);
          obj_Connected.Close();
        }
        else {
          string tmp_GUID = GetGUID();
          if (ConnectionAccept != null) {
            ConnectionAccept(new AsyncSocket(obj_Connected, tmp_GUID));
          }
        }
        obj_Socket.BeginAccept(new AsyncCallback(onIncomingConnection), obj_Socket);
      }
      catch (Exception ex) {
        if (SocketError != null) {
          SocketError(ex.Message);
        }
      }
    }
    private string GetGUID() {
      try {
        return System.Guid.NewGuid().ToString();
      }
      catch {
        return null;
        //if (SocketError != null) {
        //SocketError(ex.Message);
        //}
      }
    }
  }
  class AsyncSocket {
    public string m_SocketID;
    private System.Net.Sockets.Socket m_tmpSocket;
    public event socketDisconnectedEventHandler socketDisconnected;
    public delegate void socketDisconnectedEventHandler(string SocketID);
    public event socketDataArrivalEventHandler socketDataArrival;
    public delegate void socketDataArrivalEventHandler(string SocketID, string SocketData, byte[] lBytes, int lBytesRead);
    public event socketConnectedEventHandler socketConnected;
    public delegate void socketConnectedEventHandler(string SocketID);
    public event socketErrorEventHandler socketError;
    public delegate void socketErrorEventHandler(string lData);
    public AsyncSocket(System.Net.Sockets.Socket tmp_Socket, string tmp_SocketID) {
      try {
        m_SocketID = tmp_SocketID;
        m_tmpSocket = tmp_Socket;
        System.Net.Sockets.Socket obj_Socket = tmp_Socket;
        StateObject obj_SocketState = new StateObject();
        obj_SocketState.WorkSocket = obj_Socket;
        obj_Socket.BeginReceive(obj_SocketState.Buffer, 0, obj_SocketState.BufferSize, 0, new AsyncCallback(onDataArrival), obj_SocketState);
      }
      catch (Exception ex) {
        if (socketError != null) {
          socketError(ex.Message);
        }
      }
    }
    public AsyncSocket() {
      try {
        m_tmpSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      }
      catch (Exception ex) {
        if (socketError != null) {
          socketError(ex.Message);
        }
      }
    }
    private void SendBytes(byte[] Buffer) {
      try {
        StateObject obj_StateObject = new StateObject();
        obj_StateObject.WorkSocket = m_tmpSocket;
        m_tmpSocket.BeginSend(Buffer, 0, Buffer.Length, 0, new AsyncCallback(onSendComplete), obj_StateObject);
      }
      catch (Exception ex) {
        if (socketError != null) {
          socketError(ex.Message);
        }
      }
    }
    public void Send(string tmp_Data) {
      try {
        StateObject obj_StateObject = new StateObject();
        obj_StateObject.WorkSocket = m_tmpSocket;
        byte[] Buffer = Encoding.ASCII.GetBytes(tmp_Data);
        m_tmpSocket.BeginSend(Buffer, 0, Buffer.Length, 0, new AsyncCallback(onSendComplete), obj_StateObject);
      }
      catch (Exception ex) {
        if (socketError != null) {
          socketError(ex.Message);
        }
      }
    }
    public void Close() {
      try {
        m_tmpSocket.Shutdown(SocketShutdown.Both);
        m_tmpSocket.Close();
      }
      catch (Exception ex) {
        if (socketError != null) {
          socketError(ex.Message);
        }
      }
    }
    public void Connect(string hostIP, long hostPort) {
      try {
        IPEndPoint hostEndPoint = new IPEndPoint(Dns.Resolve(hostIP).AddressList[0], Convert.ToInt32(hostPort));
        System.Net.Sockets.Socket obj_Socket = m_tmpSocket;
        obj_Socket.BeginConnect(hostEndPoint, new AsyncCallback(onConnectionComplete), obj_Socket);
      }
      catch (Exception ex) {
        if (socketError != null) {
          socketError(ex.Message);
        }
      }
    }
    private void onDataArrival(IAsyncResult ar) {
      try {
        StateObject obj_SocketState = (StateObject)ar.AsyncState;
        System.Net.Sockets.Socket obj_Socket = obj_SocketState.WorkSocket;
        string sck_Data = null;
        int BytesRead = obj_Socket.EndReceive(ar);
        if (BytesRead > 0) {
          sck_Data = Encoding.ASCII.GetString(obj_SocketState.Buffer, 0, BytesRead);
          if (socketDataArrival != null) {
            socketDataArrival(m_SocketID, sck_Data, obj_SocketState.Buffer, BytesRead);
          }
        }
        obj_Socket.BeginReceive(obj_SocketState.Buffer, 0, obj_SocketState.BufferSize, 0, new AsyncCallback(onDataArrival), obj_SocketState);
      }
      catch {
        if (socketDisconnected != null) {
          socketDisconnected(m_SocketID);
        }
        //RaiseEvent socketError(ex.Message)
      }
    }
    public string ReturnLocalIp() {
      string functionReturnValue = null;
      try {
        functionReturnValue = new WebClient().DownloadString("http://www.whatismyip.com/automation/n09230945.asp");
      }
      catch (Exception ex) {
        functionReturnValue = "";
        if (socketError != null) {
          socketError(ex.Message);
        }
      }
      return functionReturnValue;
    }

    public long ReturnLocalPort() {
      long functionReturnValue = 0;
      try {
        functionReturnValue = Convert.ToInt64(((IPEndPoint)m_tmpSocket.LocalEndPoint).Port);
      }
      catch (Exception ex) {
        if (socketError != null) {
          socketError(ex.Message);
        }
      }
      return functionReturnValue;
    }
    private void onSendComplete(IAsyncResult ar) {
      try {
        StateObject obj_SocketState = (StateObject)ar.AsyncState;
        System.Net.Sockets.Socket obj_Socket = obj_SocketState.WorkSocket;
      }
      catch (Exception ex) {
        if (socketError != null) {
          socketError(ex.Message);
        }
      }
    }
    private void onConnectionComplete(IAsyncResult ar) {
      try {
        m_tmpSocket = (System.Net.Sockets.Socket)ar.AsyncState;
        m_tmpSocket.EndConnect(ar);
        if (socketConnected != null) {
          socketConnected("null");
        }
        System.Net.Sockets.Socket lTempSocket = m_tmpSocket;
        StateObject lSocketState = new StateObject();
        lSocketState.WorkSocket = lTempSocket;
        lTempSocket.BeginReceive(lSocketState.Buffer, 0, lSocketState.BufferSize, 0, new AsyncCallback(onDataArrival), lSocketState);
      }
      catch (Exception ex) {
        if (socketError != null) {
          socketError(ex.Message);
        }
      }
    }
    public string SocketID {
      get { return m_SocketID; }
    }
  }
  class AsyncSocketController {
    private SortedList m_SocketCol = new SortedList();
    private AsyncServer withEventsField_m_ServerSocket;
    private AsyncServer m_ServerSocket {
      get { return withEventsField_m_ServerSocket; }
      set {
        if (withEventsField_m_ServerSocket != null) {
          withEventsField_m_ServerSocket.ConnectionAccept -= m_ServerSocket_ConnectionAccept;
        }
        withEventsField_m_ServerSocket = value;
        if (withEventsField_m_ServerSocket != null) {
          withEventsField_m_ServerSocket.ConnectionAccept += m_ServerSocket_ConnectionAccept;
        }
      }
    }
    public event onConnectionAcceptEventHandler onConnectionAccept;
    public delegate void onConnectionAcceptEventHandler(string SocketID);
    public event onSocketDisconnectedEventHandler onSocketDisconnected;
    public delegate void onSocketDisconnectedEventHandler(string SocketID);
    public event onDataArrivalEventHandler onDataArrival;
    public delegate void onDataArrivalEventHandler(string SocketID, string SocketData, byte[] lBytes, int lBytesRecieved);
    public event onSocketErrorEventHandler onSocketError;
    public delegate void onSocketErrorEventHandler(string lData);
    public AsyncSocketController(int tmp_Port) {
      try {
        m_ServerSocket = new AsyncServer(tmp_Port);
      }
      catch (Exception ex) {
        if (onSocketError != null) {
          onSocketError(ex.Message);
        }
      }
    }
    public void Start() {
      try {
        m_ServerSocket.Start();
      }
      catch (Exception ex) {
        if (onSocketError != null) {
          onSocketError(ex.Message);
        }
      }
    }
    public void StopServer() {
      try {
        m_ServerSocket.Close();
      }
      catch (Exception ex) {
        if (onSocketError != null) {
          onSocketError(ex.Message);
        }
      }
    }
    public void Send(string tmp_SocketID, string tmp_Data, bool tmp_Return = true) {
      try {
        if (tmp_Return == true) {
          ((AsyncSocket)m_SocketCol[tmp_SocketID]).Send(tmp_Data + "\r\n");
        }
        else {
          ((AsyncSocket)m_SocketCol[tmp_SocketID]).Send(tmp_Data);
        }
      }
      catch (Exception ex) {
        if (onSocketError != null) {
          onSocketError(ex.Message);
        }
      }
    }
    public void Close(string tmp_SocketID) {
      try {
        ((AsyncSocket)m_SocketCol[tmp_SocketID]).Close();
      }
      catch (Exception ex) {
        if (onSocketError != null) {
          onSocketError(ex.Message);
        }
      }
    }
    public void Add(AsyncSocket tmp_Socket) {
      try {
        m_SocketCol.Add(tmp_Socket.SocketID, tmp_Socket);
        tmp_Socket.socketDisconnected += SocketDisconnected;
        tmp_Socket.socketDataArrival += SocketDataArrival;
      }
      catch (Exception ex) {
        if (onSocketError != null) {
          onSocketError(ex.Message);
        }
      }
    }
    public int Count {
      get { return m_SocketCol.Count; }
    }
    private void m_ServerSocket_ConnectionAccept(AsyncSocket tmp_Socket) {
      try {
        Add(tmp_Socket);
        if (onConnectionAccept != null) {
          onConnectionAccept(tmp_Socket.SocketID);
        }
      }
      catch (Exception ex) {
        if (onSocketError != null) {
          onSocketError(ex.Message);
        }
      }
    }
    private void SocketDisconnected(string SocketID) {
      try {
        m_SocketCol.Remove(SocketID);
        if (onSocketDisconnected != null) {
          onSocketDisconnected(SocketID);
        }
      }
      catch (Exception ex) {
        if (onSocketError != null) {
          onSocketError(ex.Message);
        }
      }
    }
    private void SocketDataArrival(string SocketID, string SocketData, byte[] lBytes, int lBytesRecieved) {
      try {
        if (onDataArrival != null) {
          onDataArrival(SocketID, SocketData, lBytes, lBytesRecieved);
        }
      }
      catch (Exception ex) {
        if (onSocketError != null) {
          onSocketError(ex.Message);
        }
      }
    }
  }
}