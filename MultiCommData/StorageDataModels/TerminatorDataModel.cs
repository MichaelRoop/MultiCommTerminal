using CommunicationStack.Net.Stacks;
using System;
using System.Collections.Generic;

namespace MultiCommData.Net.StorageDataModels {

    /// <summary>Groups a list of terminator info and the block it defines</summary>
    public class TerminatorDataModel {

        /// <summary>The UID for storage identification</summary>
        public string UId { get; set; } = "";

        public byte[] TerminatorBlock { get; set; } = new byte[0];
        public List<TerminatorInfo> TerminatorInfos { get; set; } = new List<TerminatorInfo>();

        public string Name { get; set; } = "";

        public TerminatorDataModel() {
            this.UId = Guid.NewGuid().ToString();
        }

        public TerminatorDataModel(List<TerminatorInfo> infos) : this() {
            this.TerminatorInfos = infos;
            this.TerminatorBlock = new byte[this.TerminatorInfos.Count];
            for (int i = 0; i < this.TerminatorBlock.Length; i++) {
                this.TerminatorBlock[i] = this.TerminatorInfos[i].Value;
            }
        }



    }
}
