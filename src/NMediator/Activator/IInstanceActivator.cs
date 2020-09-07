using System;

namespace NMediator.Activator
{
    public interface IInstanceActivator
    {
        T GetInstance<T>();
        object GetInstance(Type type);
    }
}
