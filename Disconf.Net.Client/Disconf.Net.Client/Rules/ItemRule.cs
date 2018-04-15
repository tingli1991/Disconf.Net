using System;
using System.Collections.Generic;
using System.Reflection;

namespace Disconf.Net.Client.Rules
{
    /// <summary>
    /// 
    /// </summary>
    public class ItemRule : IItemRule
    {
        /// <summary>
        /// 默认属性名
        /// </summary>
        internal string DefaultPropName { get; private set; }
        /// <summary>
        /// 远程配置值变更时要执行的委托
        /// </summary>
        internal Action<string> Action { get; private set; }
        private List<PropertyMap> _list = new List<PropertyMap>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IItemRule CallBack(Action<string> action)
        {
            if (action != null)
            {
                Action += action;
            }
            return this;
        }

        /// <summary>
        /// 配置文件发生改变
        /// </summary>
        /// <param name="configName">配置文件名称</param>
        /// <param name="changedValue"></param>
        public void ConfigChanged(string configName, string changedValue)
        {
            if (_list != null && _list.Count > 0)
            {
                foreach (var map in _list)
                {
                    try
                    {
                        var pi = map.GetPropertyInfo(DefaultPropName);
                        if (pi != null)
                        {
                            object value;
                            if (map.TypeConvert != null)
                            {
                                value = map.TypeConvert(changedValue);
                            }
                            else
                            {
                                value = Convert.ChangeType(changedValue, map.PropertyInfo.PropertyType);
                            }
                            map.PropertyInfo.SetValue(map.Entity, value);
                        }
                    }
                    catch { }//这里暂时没想好怎么传递异常
                }
            }
            Action?.Invoke(changedValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        public IItemRule MapTo(string propName)
        {
            if (!string.IsNullOrWhiteSpace(propName))
            {
                DefaultPropName = GetPropName(propName);
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        private string GetPropName(string propName)
        {
            int idx = propName.LastIndexOf('.');
            if (idx >= 0)
            {
                return propName.Substring(idx + 1);
            }
            return propName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="prop"></param>
        /// <param name="typeConvert"></param>
        /// <returns></returns>
        public IItemRule SetProperty(object entity, PropertyInfo prop, Func<string, object> typeConvert = null)
        {
            return SetProperty(entity, null, null, prop, typeConvert);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityType"></param>
        /// <param name="propName"></param>
        /// <param name="prop"></param>
        /// <param name="typeConvert"></param>
        /// <returns></returns>
        private IItemRule SetProperty(object entity, Type entityType, string propName, PropertyInfo prop, Func<string, object> typeConvert = null)
        {
            _list.Add(new PropertyMap
            {
                Entity = entity,
                EntityType = entityType,
                PropertyName = propName,
                PropertyInfo = prop,
                TypeConvert = typeConvert
            });
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propName"></param>
        /// <param name="typeConvert"></param>
        /// <returns></returns>
        public IItemRule SetProperty<T>(T entity, string propName = null, Func<string, object> typeConvert = null)
        {
            return SetProperty(entity, typeof(T), propName, null, typeConvert);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="typeConvert"></param>
        /// <returns></returns>
        public IItemRule SetStaticProperty(PropertyInfo prop, Func<string, object> typeConvert = null)
        {
            return SetProperty(null, prop, typeConvert);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propName"></param>
        /// <param name="typeConvert"></param>
        /// <returns></returns>
        public IItemRule SetStaticProperty<T>(string propName = null, Func<string, object> typeConvert = null)
        {
            return SetProperty<T>(default(T), propName, typeConvert);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        IRule IRule.MapTo(string configName)
        {
            return MapTo(configName);
        }

        /// <summary>
        /// 
        /// </summary>
        private class PropertyMap
        {
            public object Entity { get; set; }
            public Type EntityType { get; set; }
            public string PropertyName { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
            public Func<string, object> TypeConvert { get; set; }
            public PropertyInfo GetPropertyInfo(string defaultPropertyName)
            {
                /*
                因为无法确认SetProperty和MapTo方法被调用的先后顺序
                所以通过PropertyName来得到对应的PropertyInfo这个过程只能在最后调用
                */
                PropertyInfo pi = PropertyInfo;
                if (pi == null)
                {
                    string propName = PropertyName;
                    if (string.IsNullOrWhiteSpace(propName))
                    {
                        propName = defaultPropertyName;
                    }
                    if (!string.IsNullOrWhiteSpace(propName))
                    {
                        pi = EntityType.GetProperty(propName);
                    }
                    PropertyInfo = pi;
                }
                return pi;
            }
        }
    }
}