using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiConnect.Models
{
    public class WavGenViewModel
    { 
        public bool SavFile { get; set; }
        public bool NoWave { get; set; }
        public int NOrd { get; set; }
        public int NCont { get; set; }
        public int NLine { get; set; }
        public List<Elements> FiltValues { get; set; }
    }
}