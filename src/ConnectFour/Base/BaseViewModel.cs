using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;

namespace ConnectFour.Base;

public abstract class BaseViewModel : ReactiveObject
{
    protected CompositeDisposable disposables;

    public BaseViewModel()
    {
        CreateCommands();
    }

    public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; private set; }

    public virtual Task OnAppearingAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnDisappearingAsync()
    {
        disposables?.Dispose();
        disposables = null;
        return Task.CompletedTask;
    }

    protected virtual void CreateCommands()
    {
    }
}