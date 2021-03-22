using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace MultiCommTerminal.NetCore.UserControls.EditBoxes {

    /// <summary>Base class for common functionality of Edit boxes</summary>
    public abstract class UC_UintEditBoxBase : UserControl, IUintEditBox {

        private ClassLog log = new ClassLog("UC_UintEditBoxBase");
        private Func<string, bool> validateFunc = null;
        private List<IUintEditBox> dependants = new List<IUintEditBox>();

        #region  IUintEditBox

        public event EventHandler<UInt32> OnValueChanged;
        public event EventHandler OnValueEmpty;

        public UC_UintEditBoxBase() {
            this.validateFunc = this.DummyValidator;
        }

        public void RegisterDependant(IUintEditBox editBox) {
            this.dependants.Add(editBox);
        }


        public void SetValue(UInt32 value) {
            this.DoSetValue(value);
        }


        public void SetEmpty() {
            this.DoSetEmpty();
        }


        public void SetValidator(Func<string, bool> func) {
            this.validateFunc = func;
        }

        #endregion

        #region Abstract

        protected abstract void DoSetValue(UInt32 value);
        protected abstract void DoSetEmpty();

        #endregion

        #region Protected

        protected void RaiseValueEmpty() {
            this.OnValueEmpty?.Invoke(this, new EventArgs());
            this.SetDependantsEmpty();
        }

        protected void RaiseValueChanged(UInt32 value) {
            this.OnValueChanged?.Invoke(this, value);
            this.SetDependantsValue(value);
         }


        protected void SetDependantsEmpty() {
            foreach (var d in this.dependants) {
                d.SetEmpty();
            }
        }


        protected void SetDependantsValue(UInt32 value) {
            foreach (var d in this.dependants) {
                d.SetValue(value);
            }
        }


        protected void ValidateRange(string value, KeyEventArgs args) {
            if (!this.validateFunc(value)) {
                args.Handled = true;
            }
        }

        #endregion

        #region Private

        private bool DummyValidator(string value) {
            return true;
        }

        #endregion

    }
}
