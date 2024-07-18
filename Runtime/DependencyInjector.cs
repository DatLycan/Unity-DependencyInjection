using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatLycan.Packages.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DatLycan.Packages.DependencyInjection {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public sealed class InjectAttribute: Attribute {}        
    
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute: Attribute {}
    
    public static class DependencyInjector {
        private const BindingFlags k_bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private static readonly Dictionary<Type, object> registry = new();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize() {
            registry.Clear();
            
            IEnumerable<IDependencyProvider> providers = FindAllMonoBehaviours().OfType<IDependencyProvider>();
            foreach (IDependencyProvider provider in providers) {
                RegisterProvider(provider);
            }

            IEnumerable<MonoBehaviour> injectables = FindAllMonoBehaviours().Where(IsInjectable);
            foreach (MonoBehaviour injectable in injectables) {
                Inject(injectable);
            }
        }

        private static void Inject(object injectable) {
            Type type = injectable.GetType();
            IEnumerable<FieldInfo> injectableFields = type.GetFields(k_bindingFlags)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (FieldInfo injectableField in injectableFields) {
                Type fieldType = injectableField.FieldType;
                object resolvedInjectables = Resolve(fieldType);
                Preconditions.CheckNotNull(resolvedInjectables);

                injectableField.SetValue(injectable, resolvedInjectables);
                Debug.Log($"[Field] Injected {fieldType.Name} into {type.Name}");
            }

            IEnumerable<MethodInfo> injectableMethods = type.GetMethods(k_bindingFlags)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (MethodInfo injectableMethod in injectableMethods) {
                Type[] requiredParameters = injectableMethod.GetParameters()
                    .Select(parameter => parameter.ParameterType)
                    .ToArray();
                object[] resolvedInjectables = requiredParameters.Select(Resolve).ToArray();
                foreach (object resolvedInjectable in resolvedInjectables) {
                    Preconditions.CheckNotNull(resolvedInjectable);
                }

                injectableMethod.Invoke(injectable, resolvedInjectables);
                Debug.Log($"[Method] Injected {type.Name} into {injectableMethod.Name}");
            }
        }

        private static bool IsInjectable(MonoBehaviour @object) {
            MemberInfo[] members = @object.GetType().GetMembers(k_bindingFlags);
            return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        }

        private static void RegisterProvider(IDependencyProvider provider) {
            MethodInfo[] methods = provider.GetType().GetMethods(k_bindingFlags);

            foreach (MethodInfo method in methods) {
                if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;

                Type returnType = method.ReturnType;
                object providedInstance = method.Invoke(provider, null);
                Preconditions.CheckNotNull(providedInstance);
                registry.Add(returnType, providedInstance);
            }
        }

        private static object Resolve(Type type) {
            registry.TryGetValue(type, out object resolvedInstance);
            return resolvedInstance;
        }

        private static MonoBehaviour[] FindAllMonoBehaviours() => Object.FindObjectsOfType<MonoBehaviour>();
    }
}
