using System.Collections.Generic;

namespace MultiCommData.UserDisplayData.Net {
    public class MediumGroup {
        public List<CommMedialDisplay> Mediums { get; set; }
  
        public MediumGroup() {
            this.Mediums = new List<CommMedialDisplay>();
        }
    }
}
