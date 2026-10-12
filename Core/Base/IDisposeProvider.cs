namespace Core.Base
{
	public interface IDisposeProvider
	{
		DisposableCollection ChildDisposables { get; }
	}
}