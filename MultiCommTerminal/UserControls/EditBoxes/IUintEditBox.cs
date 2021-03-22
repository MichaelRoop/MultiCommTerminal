using System;

namespace MultiCommTerminal.NetCore.UserControls.EditBoxes {

    /// <summary>
    /// Interface to allow registration of Edit box common functionality
    /// </summary>
    public interface IUintEditBox {

        /// <summary>Raised when the value is changed</summary>
        event EventHandler<UInt32> OnValueChanged;

        /// <summary>Raised when the value is empty (not zero)</summary>
        event EventHandler OnValueEmpty;

        /// <summary>Register other related edit boxes to update them</summary>
        /// <param name="editBox">The edit box to register</param>
        void RegisterDependant(IUintEditBox editBox);

        /// <summary>Set the value directly without raising an event</summary>
        /// <param name="value">The value to set</param>
        void SetValue(UInt32 value);


        void SetEmpty();

    }
}
