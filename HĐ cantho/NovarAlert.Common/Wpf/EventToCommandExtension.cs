using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace NovaAlert.Common.Wpf
{
    [MarkupExtensionReturnType(typeof(RoutedEventHandler))]
    public sealed class EventToCommandExtension : MarkupExtension
    {
        Type _eventArgsType;

        public EventToCommandExtension()
        {
        }
        public override object ProvideValue(IServiceProvider sp)
        {
            var pvt = sp.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (pvt != null)
            {
                var evt = pvt.TargetProperty as EventInfo;
                var doAction = GetType().GetMethod("DoAction", BindingFlags.NonPublic | BindingFlags.Instance);
                Type dlgType = null;
                if (evt != null)
                {
                    dlgType = evt.EventHandlerType;
                }
                var mi = pvt.TargetProperty as MethodInfo;
                if (mi != null)
                {
                    dlgType = mi.GetParameters()[1].ParameterType;
                }
                if (dlgType != null)
                {
                    _eventArgsType = dlgType.GetMethod("Invoke").GetParameters()[1].ParameterType;
                    return Delegate.CreateDelegate(dlgType, this, doAction);
                }
            }
            return null;
        }

        public EventToCommandExtension(string bindingCommandPath)
        {
            BindingCommandPath = bindingCommandPath;
        }

        void DoAction(object sender, RoutedEventArgs e)
        {
            var dc = (sender as FrameworkElement).DataContext;
            if (BindingCommandPath != null)
            {
                Command = (ICommand)ParsePropertyPath(dc, BindingCommandPath);
            }
            Type eventArgsType = typeof(EventCommandArgs<>).MakeGenericType(_eventArgsType);
            var cmdParams = Activator.CreateInstance(eventArgsType, sender, e);
            if (Command != null && Command.CanExecute(cmdParams))
            {
                Command.Execute(cmdParams);
            }
        }

        public string BindingCommandPath { get; set; }

        // not that useful
        public ICommand Command { get; set; }

        static object ParsePropertyPath(object target, string path)
        {
            var props = path.Split('.');
            foreach (var prop in props)
            {
                target = target.GetType().GetProperty(prop).GetValue(target, null);
            }
            return target;
        }
    }

    public sealed class EventCommandArgs<TEventArgs> where TEventArgs : RoutedEventArgs
    {
        public TEventArgs EventArgs { get; private set; }
        public object Sender { get; private set; }

        public EventCommandArgs(object sender, TEventArgs args)
        {
            Sender = sender;
            EventArgs = args;
        }
    }
}
