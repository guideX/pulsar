// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System;
//using System.Collections.ObjectModel;
namespace Pulsar.Communications {
	public class IPEndPointCollection : Collection<IPEndPoint> {
		public IPEndPointCollection ()
		{
		}

		protected override void InsertItem (int index, IPEndPoint item)
		{
			if (item == null)
				throw new ArgumentNullException ("item");

			Items.Insert (index, item);
		}

		protected override void SetItem (int index, IPEndPoint item)
		{
			if (item == null)
				throw new ArgumentNullException ("item");

			Items [index] = item;
		}
	}
}