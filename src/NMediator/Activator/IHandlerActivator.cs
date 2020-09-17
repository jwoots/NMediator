using System;

namespace NMediator.Activator
{
    public interface IHandlerActivator
    {
        T GetInstance<T>();
        object GetInstance(Type type);


    }
}
