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
        private Action<string> errPostAction = null;
        private List<IUintEditBox> dependants = new List<IUintEditBox>();

        #region  IUintEditBox

        public event EventHandler<UInt32> OnValueChanged;
        public event EventHandler OnValueEmpty;

        public UC_UintEditBoxBase() {
            this.validateFunc = this.DummyValidator;
            this.errPostAction = this.DummyErrReporter;
        }

        public void RegisterDependant(IUintEditBox editBox) {
            this.dependants.Add(editBox);
        }


        public bool SetValue(UInt32 value) {
            if (this.validateFunc(value.ToString())) {
                this.DoSetValue(value);
                return true;
            }
            this.DoSetEmpty();
            return false;
        }


        public void SetEmpty() {
            this.DoSetEmpty();
        }


        public void SetValidator(Func<string, bool> func, Action<string> errReporter) {
            this.validateFunc = func;
            this.errPostAction = errReporter;
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

        protected void PostError(string err) {
            try {
                this.errPostAction.Invoke(err);
            }
            catch(Exception ex) {
                this.log.Exception(9999, err, ex);
            }
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


        protected void ValidateRange(Func<string> valueFunc, KeyEventArgs args) {
            try {
                if (!this.validateFunc(valueFunc.Invoke())) {
                    args.Handled = true;
                }
            }
            catch (Exception ex) {
                args.Handled = true;
                this.log.Exception(9999, "ValidateRange", "", ex);
                this.PostError(ex.Message);
            }
        }

        #endregion

        #region Private

        private bool DummyValidator(string value) {
            return true;
        }

        private void DummyErrReporter(string value) {
        }


        #endregion

    }
}
