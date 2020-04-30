namespace BluetoothLE.Net.interfaces {

    /// <summary>Cross platform property keys</summary>
    public interface IPropertyKeys {

        string IsConnected { get; }
        string IsConnectable { get; }
        string CanPair { get; }
        string IsPaired { get; }
        string ContainerId { get; }
        string IconPath { get; }
        string GlyphIconPath { get; }
        string ItemNameDisplay { get; }

    }
}
