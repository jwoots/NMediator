using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Http.Reflection.QueryStringBinder
{
    public interface IQueryStringBinder
    {
        /// <summary>
        /// Return true if value of given type can be converted to string
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool CanBindToString(Type type, object value);

        /// <summary>
        /// Return true if string value can be converted to given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool CanBindToType(Type type, IEnumerable<string> value);

        /// <summary>
        /// Convert value of given type to string representation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IEnumerable<string> BindToString(Type type, object value);

        /// <summary>
        /// Convert string value to given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        object BindToType(Type type, IEnumerable<string> value);
    }
}
