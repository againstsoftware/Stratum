
using System.Collections.Generic;

public interface ICompositeEffectCommand : IEffectCommand //al ejecutarlo lo que hace es determinar que efectos tiene que ejecutar, y los guarda en DerivedEffects
{
    public IReadOnlyList<IEffectCommand> DerivedEffects { get; }
}
