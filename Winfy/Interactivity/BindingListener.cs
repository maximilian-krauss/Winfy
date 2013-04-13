// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Winfy.Interactivity {

    /// <summary>
    /// Helper class for adding Bindings to non-FrameworkElements
    /// </summary>
    public class BindingListener {

        private static List<DependencyPropertyListener> freeListeners = new List<DependencyPropertyListener>();

        /// <summary>
        /// Delegate for when the binding listener has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ChangedHandler(object sender, BindingChangedEventArgs e);

        private DependencyPropertyListener listener;
        private ChangedHandler changedHandler;
        private Binding binding;
        private FrameworkElement target;
        private object value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="changedHandler">Callback whenever the value of this binding has changed.</param>
        public BindingListener(ChangedHandler changedHandler) {
            this.changedHandler = changedHandler;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BindingListener() {
        }

        /// <summary>
        /// The Binding which is to be evaluated
        /// </summary>
        public Binding Binding {
            get { return this.binding; }
            set {
                this.binding = value;
                this.Attach();
            }
        }

        /// <summary>
        /// The element to be used as the context on which to evaluate the binding.
        /// </summary>
        public FrameworkElement Element {
            get { return this.target; }
            set {
                this.target = value;
                this.Attach();
            }
        }

        /// <summary>
        /// The current value of this binding.
        /// </summary>
        public object Value {
            get { return this.value; }
            set {
                if (this.listener != null)
                    this.listener.SetValue(value);
            }
        }


        private void Attach() {
            this.Detach();

            if (this.target != null && this.binding != null) {
                this.listener = this.GetListener();
                this.listener.Attach(target, binding);
            }
        }

        private void Detach() {
            if (this.listener != null)
                this.ReturnListener();
        }

        private DependencyPropertyListener GetListener() {
            DependencyPropertyListener listener;

            if (BindingListener.freeListeners.Count != 0) {
                listener = BindingListener.freeListeners[BindingListener.freeListeners.Count - 1];
                BindingListener.freeListeners.RemoveAt(BindingListener.freeListeners.Count - 1);

                return listener;
            }
            else
                listener = new DependencyPropertyListener();

            listener.Changed += this.HandleValueChanged;

            return listener;
        }

        private void ReturnListener() {
            this.listener.Changed -= this.HandleValueChanged;

            BindingListener.freeListeners.Add(this.listener);

            this.listener = null;
        }

        private void HandleValueChanged(object sender, BindingChangedEventArgs e) {
            this.value = e.EventArgs.NewValue;

            if (this.changedHandler != null)
                this.changedHandler(this, e);
        }

        private class DependencyPropertyListener {

            private DependencyProperty property;
            public event EventHandler<BindingChangedEventArgs> Changed;

            private static int index = 0;
            private FrameworkElement target;

            public DependencyPropertyListener() {
                this.property = DependencyProperty.RegisterAttached("DependencyPropertyListener" + index++, typeof(object), typeof(DependencyPropertyListener), new PropertyMetadata(null, this.HandleValueChanged));
            }

            private void HandleValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {

                if (this.Changed != null)
                    this.Changed(this, new BindingChangedEventArgs(e));
            }

            public void Attach(FrameworkElement element, Binding binding) {
                if (this.target != null)
                    throw new Exception("Cannot attach an already attached listener");

                this.target = element;

                this.target.SetBinding(this.property, binding);
            }

            public void Detach() {
                this.target.ClearValue(this.property);
                this.target = null;
            }

            public void SetValue(object value) {
                this.target.SetValue(this.property, value);
            }
        }
    }

    /// <summary>
    /// Event args for when binding values change.
    /// </summary>
    public class BindingChangedEventArgs : EventArgs {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="e"></param>
        public BindingChangedEventArgs(DependencyPropertyChangedEventArgs e) {
            this.EventArgs = e;
        }

        /// <summary>
        /// Original event args.
        /// </summary>
        public DependencyPropertyChangedEventArgs EventArgs { get; private set; }
    }
}
