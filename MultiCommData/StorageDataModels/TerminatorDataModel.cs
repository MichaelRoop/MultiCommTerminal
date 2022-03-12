using CommunicationStack.Net.Stacks;
using StorageFactory.Net.interfaces;
using System;
using System.Collections.Generic;

namespace MultiCommData.Net.StorageDataModels {

    /// <summary>Groups a list of terminator info and the block it defines</summary>
    public class TerminatorDataModel : IDisplayableData,  IIndexible {

        /// <summary>The UID for storage identification</summary>
        public string UId { get; set; } = "";

        public byte[] TerminatorBlock { get; set; } = Array.Empty<byte>();
        public List<TerminatorInfo> TerminatorInfos { get; set; } = new List<TerminatorInfo>();

        public string Display { get; set; } = "";

        public TerminatorDataModel() {
            this.UId = Guid.NewGuid().ToString();
        }

        public TerminatorDataModel(List<TerminatorInfo> infos) : this() {
            this.Init(infos);
        }

        /// <summary>Reset internal values without affecting the UUID for storage</summary>
        /// <param name="infos">The Info list</param>
        public void Init(List<TerminatorInfo> infos) {
            this.TerminatorInfos = infos;
            this.TerminatorBlock = new byte[this.TerminatorInfos.Count];
            for (int i = 0; i < this.TerminatorBlock.Length; i++) {
                this.TerminatorBlock[i] = this.TerminatorInfos[i].Value;
            }
        }

    }
}
