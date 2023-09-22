using System.Collections.Generic;

namespace AvaloniaInside.Shell;

public interface IQueryAttributable
{
    void ApplyQueryAttributes(IDictionary<string, object> query);
}