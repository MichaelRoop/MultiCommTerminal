namespace MultiCommData.UserDisplay {

    /// <summary>Group a communication message and its display value</summary>
    public class NameKey {

        /// <summary>String displayed to user</summary>
        public string Display { get; set; }
        /// <summary>String sent or received to and from communication channel</summary>
        public string Msg { get; set; }

    }
}
