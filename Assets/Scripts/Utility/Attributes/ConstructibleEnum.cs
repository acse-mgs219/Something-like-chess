using System.Reflection;
using System;

[AttributeUsage(AttributeTargets.Field)]
public class ConstructableEnumAttribute : Attribute
{
    public Type Type { get; set; }

    public ConstructableEnumAttribute(Type type)
    {
        Type = type;
    }
}

public static class AITypeExtension
{
    // Usage example:
    //     AIType myAI = AIType.myAI;
    //     object AI = myAI.Construct();
    public static object Construct(this AITypes.AIType ofAI, Player player)
    {
        object ai = null;
        Type typeOfAI = GetType(ofAI);
        if (typeOfAI != null && typeOfAI.IsAbstract == false)
        {
            ai = Activator.CreateInstance(typeOfAI, args: player);
        }

        return ai;
    }

    private static Type GetType(AITypes.AIType ai)
    {
        ConstructableEnumAttribute attr = (ConstructableEnumAttribute)
            Attribute.GetCustomAttribute
            (ForValue(ai), typeof(ConstructableEnumAttribute));
        return attr.Type;
    }

    private static MemberInfo ForValue(AITypes.AIType ai)
    {
        return typeof(AITypes.AIType).GetField(Enum.GetName(typeof(AITypes.AIType), ai));
    }
}