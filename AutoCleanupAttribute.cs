using System;
using System.Collections;
using System.Reflection;

public interface IAutoCleanup
{
    void Cleanup();
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class CleanupAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class DoNotCleanupAttribute : Attribute { }

/// <summary>
/// 컴포넌트가 파괴될 때 내부 필드(IList, IDictionary, 참조 타입)를 자동 정리해주는 베이스 클래스입니다.
/// [DoNotCleanup] 애트리뷰트를 부여하면 해당 필드는 무시됩니다.
/// </summary>
public static class AutoCleanupUtility
{
    public static void Cleanup(object target)
    {
        if (target == null) return;

        var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (var field in fields)
        {
            if (field.IsDefined(typeof(DoNotCleanupAttribute), inherit: true))
                continue;

            Type fieldType = field.FieldType;
            object fieldValue = field.GetValue(target);

            if (fieldValue == null)
                continue;

            // IList
            if (typeof(IList).IsAssignableFrom(fieldType))
            {
                if (fieldValue is IList list)
                    list.Clear();
            }
            // IDictionary
            else if (typeof(IDictionary).IsAssignableFrom(fieldType))
            {
                if (fieldValue is IDictionary dict)
                    dict.Clear();
            }
            // Reference type (not value type or string)
            else if (!fieldType.IsValueType && fieldType != typeof(string))
            {
                field.SetValue(target, null);
            }
            // ICleanup
            else if (typeof(IAutoCleanup).IsAssignableFrom(fieldType))
            {
                if (fieldValue is IAutoCleanup cleanup)
                    cleanup.Cleanup();

                field.SetValue(target, null);
            }
        }
    }
}
