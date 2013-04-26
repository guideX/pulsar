using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
namespace Pulsar.Plugs.Communications {
  public class SocketController {
    public event ConnectedEventHandler Connected;
    public delegate void ConnectedEventHandler();
    public event ProcessErrorEventHandler ProcessError;
    public delegate void ProcessErrorEventHandler(string _Ex, string _Error);
    public event SocketErrorEventHandler SocketError;
    public delegate void SocketErrorEventHandler(string _Error);
    public event SocketDataArrivalEventHandler SocketDataArrival;
    public delegate void SocketDataArrivalEventHandler(string _Data, string _SocketID);
    private Pulsar.Plugs.Communications.AsyncSocket lSocket;
    private bool lConnected;
    private String lLastData;
    public string ReturnLastDataAndPurge() {
      string msg;
      msg = lLastData;
      lLastData = "";
      return msg;
    }
    public string ReturnLocalIp() {
      try {
        return lSocket.ReturnLocalIp();
      }
      catch (InvalidOperationException ex) {
        ProcessError(ex.Message, "public string ReturnLocalIp() {");
        return "";
      }
    }
    public long ReturnLocalPort() {
      try {
        long functionReturnValue = 0;
        if (lConnected == true) {
          functionReturnValue = lSocket.ReturnLocalPort();
        }
        return functionReturnValue;
      }
      catch (InvalidOperationException ex) {
        ProcessError(ex.Message, "public long ReturnLocalPort() {");
        return 0;
      }
    }
    public void NewSocket() {
      try {
        lSocket = new Communications.AsyncSocket();
        lSocket.socketConnected += lSocket_socketConnected;
        lSocket.socketDataArrival += lSocket_socketDataArrival;
        //lSocket.socketDisconnected += lSocket_socketDisconnected;
        lSocket.socketError += lSocket_socketError;
      }
      catch (InvalidOperationException ex) {
        ProcessError(ex.Message, "public void NewSocket() {");
      }
    }
    public void SendSocket(string lData) {
      try {
        if (lConnected == true) {
          lSocket.Send(lData + Environment.NewLine);
        }
      }
      catch (InvalidOperationException ex) {
        ProcessError(ex.Message, "public void SendSocket(string lData) {");
      }
    }
    public void CloseSocket() {
      try {
        lConnected = false;
        if (lConnected == true) {
          lSocket.Close();
        }
      }
      catch (InvalidOperationException ex) {
        ProcessError(ex.Message, "public void CloseSocket() {");
      }
    }
    public void ConnectSocket(string lIp, long lPort) {
      try {
        if (lConnected == false) {
          lSocket.Connect(lIp, lPort);
        }
      }
      catch (InvalidOperationException ex) {
        ProcessError(ex.Message, "public void ConnectSocket(string lIp, long lPort) {");
      }
    }
    private Boolean IsSocketConnected() {
      try {
        return lConnected;
      }
      catch (InvalidOperationException ex) {
        ProcessError(ex.Message, "private void lSocket_socketConnected(string SocketID) {");
        return false;
      }
    }
    private void lSocket_socketConnected(string SocketID) {
      try {
        Connected();
      }
      catch (InvalidOperationException ex) {
        ProcessError(ex.Message, "private void lSocket_socketConnected(string SocketID) {");
      }
    }
    private void lSocket_socketDataArrival(string SocketID, string SocketData, byte[] lBytes, int lByteRead) {
      try {
        SocketDataArrival(SocketData, SocketID);
        lLastData = lLastData + Environment.NewLine + SocketData;
      }
      catch (InvalidOperationException ex) {
        ProcessError(ex.Message, "private void lSocket_socketDataArrival(string SocketID, string SocketData, byte[] lBytes, int lByteRead) {");
      }
    }
    private void lSocket_socketDisconnected(string SocketID) {
      if (lConnected == true) {
        lSocket_socketDisconnected(SocketID);
      }
    }
    private void lSocket_socketError(string lData) {
      try {
        SocketError(lData);
      }
      catch (InvalidOperationException ex) {
        ProcessError(ex.Message, "private void lSocket_socketError(string lData) {");
      }
    }
  }
}