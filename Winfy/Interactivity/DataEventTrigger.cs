// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Winfy.Interactivity {

    /// <summary>
    /// Trigger which fires when a CLR event is raised on an object.
    /// Can be used to trigger from events on the data context, as opposed to
    /// a standard EventTrigger which uses routed events on FrameworkElements.
    /// </summary>
    public class DataEventTrigger : TriggerBase<FrameworkElement> {

        /// <summary>Backing DP for the Source property</summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Binding), typeof(DataEventTrigger), new PropertyMetadata(null, DataEventTrigger.HandleSourceChanged));
        /// <summary>Backing DP for the EventName property</summary>
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register("EventName", typeof(string), typeof(DataEventTrigger), new PropertyMetadata(null, DataEventTrigger.HandleEventNameChanged));


        private BindingListener listener;
        private EventInfo currentEvent;
        private Delegate currentDelegate;
        private object currentTarget;

        /// <summary>
        /// Constructor
        /// </summary>
        public DataEventTrigger() {
            this.listener = new BindingListener(this.HandleBindingValueChanged);
            this.listener.Binding = new Binding();
        }

        /// <summary>
        /// The source object for the event
        /// </summary>
        public Binding Source {
            get { return (Binding)this.GetValue(DataEventTrigger.SourceProperty); }
            set { this.SetValue(DataEventTrigger.SourceProperty, value); }
        }

        /// <summary>
        /// The name of the event which triggers this
        /// </summary>
        public string EventName {
            get { return (string)this.GetValue(DataEventTrigger.EventNameProperty); }
            set { this.SetValue(DataEventTrigger.EventNameProperty, value); }
        }

        /// <summary>
        /// Initialization
        /// </summary>
        protected override void OnAttached() {
            base.OnAttached();

            this.listener.Element = this.AssociatedObject;
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        protected override void OnDetaching() {
            base.OnDetaching();

            this.listener.Element = null;
        }

        private static void HandleEventNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ((DataEventTrigger)sender).OnEventNameChanged(e);
        }

        /// <summary>
        /// Notification that the EventName has changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnEventNameChanged(DependencyPropertyChangedEventArgs e) {
            this.UpdateHandler();
        }


        private static void HandleSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ((DataEventTrigger)sender).OnSourceChanged(e);
        }

        /// <summary>
        /// Notification that the Source has changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSourceChanged(DependencyPropertyChangedEventArgs e) {
            this.listener.Binding = this.Source;
        }

        private void HandleBindingValueChanged(object sender, BindingChangedEventArgs e) {
            this.UpdateHandler();
        }


        private void UpdateHandler() {

            if (this.currentEvent != null) {
                this.currentEvent.RemoveEventHandler(this.currentTarget, this.currentDelegate);

                this.currentEvent = null;
                this.currentTarget = null;
                this.currentDelegate = null;
            }

            this.currentTarget = this.listener.Value;

            if (this.currentTarget != null && !string.IsNullOrEmpty(this.EventName)) {

                Type targetType = this.currentTarget.GetType();
                this.currentEvent = targetType.GetEvent(this.EventName);
                if (this.currentEvent != null) {

                    MethodInfo handlerMethod = this.GetType().GetMethod("OnEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                    this.currentDelegate = this.GetDelegate(this.currentEvent, this.OnMethod);
                    this.currentEvent.AddEventHandler(this.currentTarget, this.currentDelegate);
                }
            }
        }

        private Delegate GetDelegate(EventInfo eventInfo, Action action) {
            if (typeof(EventHandler).IsAssignableFrom(eventInfo.EventHandlerType)) {
                MethodInfo method = this.GetType().GetMethod("OnEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                return Delegate.CreateDelegate(eventInfo.EventHandlerType, this, method);
            }

            Type handlerType = eventInfo.EventHandlerType;
            ParameterInfo[] eventParams = handlerType.GetMethod("Invoke").GetParameters();

            IEnumerable<ParameterExpression> parameters = eventParams.Select(p => System.Linq.Expressions.Expression.Parameter(p.ParameterType, "x"));

            MethodCallExpression methodExpression = System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(action), action.GetType().GetMethod("Invoke"));
            LambdaExpression lambdaExpression = System.Linq.Expressions.Expression.Lambda(methodExpression, parameters.ToArray());
            return Delegate.CreateDelegate(handlerType, lambdaExpression.Compile(), "Invoke", false);
        }

        private void OnMethod() {
            this.InvokeActions(null);
        }

        private void OnEvent(object sender, EventArgs e) {
            this.InvokeActions(e);
        }

    }
}