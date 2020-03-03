using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommData.UserDisplayData {
    public class MediumGroup {
        public List<CommMedialDisplay> Mediums { get; set; }
  
        public MediumGroup() {
            this.Mediums = new List<CommMedialDisplay>();
        }
    }
}
