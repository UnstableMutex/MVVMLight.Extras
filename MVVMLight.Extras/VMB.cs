using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using NLog;

namespace MVVMLight.Extras
{
    public abstract class VMB : ViewModelBase
    {


        protected virtual void RealSave()
        {
        }

        //protected virtual bool IsPropertyIgnoredOnSave(string propertyName)
        //{
        //    return false;
        //}

        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected virtual void SaveQuery(string propertyName)
        {

            RealSave();
            logger.Debug("RealSave Executed");

        }

        protected void RaisePropertyChangedNoSave([CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);
        }
        protected sealed override void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);
            SaveQuery(propertyName);
        }

        protected sealed override void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            base.RaisePropertyChanged(propertyExpression);
        }

        protected sealed override void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression, T oldValue, T newValue, bool broadcast)
        {
            base.RaisePropertyChanged(propertyExpression, oldValue, newValue, broadcast);
        }

        protected sealed override void RaisePropertyChanged<T>([CallerMemberName]string propertyName = null, T oldValue = default(T), T newValue = default(T),
            bool broadcast = false)
        {
            base.RaisePropertyChanged(propertyName, oldValue, newValue, broadcast);
        }

        protected void AssignCommands<T>()
        {
            var t = GetType();
            var commands = t.GetProperties().Where(p => p.PropertyType == typeof(ICommand) && p.Name.EndsWith("Command"));
            foreach (var propertyInfo in commands)
            {
                TryToAssign<T>(t, propertyInfo);
            }
        }
        private void TryToAssign<T>(Type type, PropertyInfo propertyInfo)
        {
            var methodname = propertyInfo.Name.RemoveEnd("Command");
            var canmethodname = "Can" + methodname;
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var m = type.GetMethod(methodname, flags, null, Type.EmptyTypes, null);
            var canm = type.GetMethod(canmethodname, flags);
            var cmdtype = typeof(T);
            if (m != null)
            {
                if (canm == null)
                {
                    var ctor = cmdtype.GetConstructor(new[] { typeof(Action) });
                    var cmd = (ICommand)ctor.Invoke(new object[] { (Action)(() => m.Invoke(this, null)) });
                    propertyInfo.SetValue(this, cmd);
                }
                else
                {
                    var ctor = cmdtype.GetConstructor(new[] { typeof(Action), typeof(Func<bool>) });
                    var cmd = (ICommand)ctor.Invoke(new object[] { (Action)(() => m.Invoke(this, null)), (Func<bool>)(() => (bool)canm.Invoke(this, null)) });
                    propertyInfo.SetValue(this, cmd);
                }
            }
        }
    }



}
