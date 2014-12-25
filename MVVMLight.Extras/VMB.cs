using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using NLog;

namespace MVVMLight.Extras
{
    public abstract class VMB : ViewModelBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected virtual void RealSave()
        {
        }

        protected virtual void SaveQuery(string propertyName)
        {
            RealSave();
            logger.Debug("RealSave Executed");
        }

        protected void RaisePropertyChangedNoSave([CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);
        }

        protected override sealed void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);
            SaveQuery(propertyName);
        }

        protected override sealed void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            base.RaisePropertyChanged(propertyExpression);
        }

        protected override sealed void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression, T oldValue,
            T newValue, bool broadcast)
        {
            base.RaisePropertyChanged(propertyExpression, oldValue, newValue, broadcast);
        }

        protected override sealed void RaisePropertyChanged<T>([CallerMemberName] string propertyName = null,
            T oldValue = default(T), T newValue = default(T),
            bool broadcast = false)
        {
            base.RaisePropertyChanged(propertyName, oldValue, newValue, broadcast);
        }

        protected void AssignCommands<T>()
        {
            Type t = GetType();
            IEnumerable<PropertyInfo> commands =
                t.GetProperties().Where(p => p.PropertyType == typeof (ICommand) && p.Name.EndsWith("Command"));
            foreach (PropertyInfo propertyInfo in commands)
            {
                TryToAssign<T>(t, propertyInfo);
            }
        }

        private void TryToAssign<T>(Type type, PropertyInfo propertyInfo)
        {
            string methodname = propertyInfo.Name.RemoveEnd("Command");
            string canmethodname = "Can" + methodname;
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo m = type.GetMethod(methodname, flags, null, Type.EmptyTypes, null);
            MethodInfo canm = type.GetMethod(canmethodname, flags);
            Type cmdtype = typeof (T);
            if (m != null)
            {
                if (canm == null)
                {
                    ConstructorInfo ctor = cmdtype.GetConstructor(new[] {typeof (Action)});
                    var cmd = (ICommand) ctor.Invoke(new object[] {(Action) (() => m.Invoke(this, null))});
                    propertyInfo.SetValue(this, cmd);
                }
                else
                {
                    ConstructorInfo ctor = cmdtype.GetConstructor(new[] {typeof (Action), typeof (Func<bool>)});
                    var cmd =
                        (ICommand)
                            ctor.Invoke(new object[]
                            {(Action) (() => m.Invoke(this, null)), (Func<bool>) (() => (bool) canm.Invoke(this, null))});
                    propertyInfo.SetValue(this, cmd);
                }
            }
        }
    }
}