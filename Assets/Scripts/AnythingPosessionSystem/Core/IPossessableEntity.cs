public interface IPossessableEntity
{
    public bool IsPossessed { get; }


    public void Possess();
    public void PhaseOut();
}