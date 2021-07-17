namespace Player
{
    public interface IStationStateSwitcher
    {
        void SwitchState<T>() where T : BaseState;
    }
    
}