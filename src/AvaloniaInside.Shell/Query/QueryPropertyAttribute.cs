using System;

namespace AvaloniaInside.Shell.Query;

/// <summary>To be added.</summary>
/// <remarks>To be added.</remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class QueryPropertyAttribute : Attribute
{
    public string Name { get; }

    public string QueryId { get; }


    public QueryPropertyAttribute(string name, string queryId)
    {
        Name = name;
        QueryId = queryId;
    }

    public QueryPropertyAttribute(string name)
    {
        Name = name;
        QueryId = name;
    }
}