using System.Threading.Tasks;

public interface IAction
{
    bool CanExecute();
    Task Execute();
}