//a hard drive has partitions, each partition contain clusters of data, each cluster of data contains sectors, each sector contains 512 bytes, each byte can be a group of 8 digits
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Pulsar.System.Devices.Storage {
    public class SectorData {
        public int numbers { get; set; }
    }
    public class SectorDataCollection {
        private SectorData[] sectorData { get; set; }
        public int count;
        public void Add(SectorData data) {
            if(data.Length() != 8) {// MUST BE 8 CHARACTERS AND ALL NUMBERS
                throw new Exception("The data provided is not the correct length!");
            }
            
            SectorData sectorData = new SectorData();
            sectorData.data = data.data;
        }
    }
    public class Sector {
        private SectorDataCollection sectorData { get; set; }
        public Sector() {
            sectorData = new SectorDataCollection();
        }
        public void Add(SectorData data) {
            sectorData.Add(data);
        }
    }
}