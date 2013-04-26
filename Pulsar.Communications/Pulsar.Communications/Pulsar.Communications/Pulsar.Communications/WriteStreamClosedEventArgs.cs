// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System.ComponentModel;
using Pulsar.IO;
namespace System.Net {
	public class WriteStreamClosedEventArgs : EventArgs	{
		Exception exception;
		internal WriteStreamClosedEventArgs (Exception exception)	{
			this.exception = exception;
		}
		public Exception Error {
			get { return exception; }
		}
	}
}