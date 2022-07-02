using ConnectFour.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace ConnectFour.Features;

public class MainViewModel : BaseViewModel
{
    private int[,] boardState;
    private int height = 6;
    private int width = 7;

    public MainViewModel()
    {
        boardState = new int[7, 6];
        CurrentPlayer = 1;
        CurrentPlayerWins = false;
        WinnerCells = null;
    }

    [Reactive] public int CurrentPlayer { get; set; }
    [Reactive] public bool CurrentPlayerWins { get; set; }
    [Reactive] public List<Tuple<int, int>> WinnerCells { get; set; }

    public ReactiveCommand<int, int> GetRowToAddTokenCommand { get; private set; }
    public extern bool IsGettingRowToAddToken { [ObservableAsProperty] get; }

    public ReactiveCommand<Tuple<int, int>, Task> AddTokenCommand { get; private set; }
    public extern bool IsAddingToken { [ObservableAsProperty] get; }

    public ReactiveCommand<Unit, Unit> StartNewGameCommand { get; private set; }
    public extern bool IsStartingNewGame { [ObservableAsProperty] get; }

    public override async Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();

        //disposables.Add(GetRowToAddTokenCommand.ThrownExceptions.Subscribe(logService.LogError));
        disposables.Add(GetRowToAddTokenCommand.IsExecuting.ToPropertyEx(this, x => x.IsGettingRowToAddToken));

        //disposables.Add(AddTokenCommand.ThrownExceptions.Subscribe(logService.LogError));
        disposables.Add(AddTokenCommand.IsExecuting.ToPropertyEx(this, x => x.IsAddingToken));

        //disposables.Add(StartNewGameCommand.ThrownExceptions.Subscribe(logService.LogError));
        disposables.Add(StartNewGameCommand.IsExecuting.ToPropertyEx(this, x => x.IsStartingNewGame));
    }

    protected override void CreateCommands()
    {
        base.CreateCommands();

        GetRowToAddTokenCommand = ReactiveCommand.CreateFromTask<int, int>(GetRowToAddTokenAsync);
        AddTokenCommand = ReactiveCommand.Create<Tuple<int, int>, Task>(AddTokenAsync);
        StartNewGameCommand = ReactiveCommand.Create(StartNewGame);
    }

    private Task<int> GetRowToAddTokenAsync(int column)
    {
        return Task.Run(() =>
        {
            // the column is full
            if (boardState[column, 0] != 0)
                return -1;

            for (int i = height - 1; i >= 0; i--)
            {
                if (boardState[column, i] == 0) // this cell is empty
                    return i;
            }

            return -1;
        });
    }

    private Task AddTokenAsync(Tuple<int, int> position)
    {
        return Task.Run(() =>
        {
            boardState[position.Item1, position.Item2] = CurrentPlayer;
            CalculateWinnerCellsForCurrentUser();
            if (WinnerCells != null)
                CurrentPlayerWins = true;
            else
                CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;
        });
    }

    private void CalculateWinnerCellsForCurrentUser()
    {
        if (WinnerCells == null)
            CalculateHorizontalWinnerCellsForCurrentUser();
        if (WinnerCells == null)
            CalculateVerticalWinnerCellsForCurrentUser();
        if (WinnerCells == null)
            CalculateAscendingDiagonalWinnerCellsForCurrentUser();
        if (WinnerCells == null)
            CalculateDescendingDiagonalWinnerCellsForCurrentUser();
    }

    private void CalculateHorizontalWinnerCellsForCurrentUser()
    {
        for (int j = 0; j < height - 3; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (boardState[i, j] == CurrentPlayer &&
                    boardState[i, j + 1] == CurrentPlayer &&
                    boardState[i, j + 2] == CurrentPlayer &&
                    boardState[i, j + 3] == CurrentPlayer)
                {
                    WinnerCells = new List<Tuple<int, int>>()
                    {
                        new Tuple<int, int>(i, j),
                        new Tuple<int, int>(i, j + 1),
                        new Tuple<int, int>(i, j + 2),
                        new Tuple<int, int>(i, j + 3)
                    };
                }
            }
        }
    }

    private void CalculateVerticalWinnerCellsForCurrentUser()
    {
        for (int i = 0; i < width - 3; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (boardState[i, j] == CurrentPlayer &&
                    boardState[i + 1, j] == CurrentPlayer &&
                    boardState[i + 2, j] == CurrentPlayer &&
                    boardState[i + 3, j] == CurrentPlayer)
                {
                    WinnerCells = new List<Tuple<int, int>>()
                    {
                        new Tuple<int, int>(i, j),
                        new Tuple<int, int>(i + 1, j),
                        new Tuple<int, int>(i + 2, j),
                        new Tuple<int, int>(i + 3, j)
                    };
                }
            }
        }
    }

    private void CalculateAscendingDiagonalWinnerCellsForCurrentUser()
    {
        for (int i = 3; i < width; i++)
        {
            for (int j = 0; j < height - 3; j++)
            {
                if (boardState[i, j] == CurrentPlayer &&
                    boardState[i - 1, j + 1] == CurrentPlayer &&
                    boardState[i - 2, j + 2] == CurrentPlayer &&
                    boardState[i - 3, j + 3] == CurrentPlayer)
                {
                    WinnerCells = new List<Tuple<int, int>>()
                    {
                        new Tuple<int, int>(i, j),
                        new Tuple<int, int>(i - 1, j + 1),
                        new Tuple<int, int>(i - 2, j + 2),
                        new Tuple<int, int>(i - 3, j + 3)
                    };
                }
            }
        }
    }

    private void CalculateDescendingDiagonalWinnerCellsForCurrentUser()
    {
        for (int i = 3; i < width; i++)
        {
            for (int j = 3; j < height; j++)
            {
                if (boardState[i, j] == CurrentPlayer &&
                    boardState[i - 1, j - 1] == CurrentPlayer &&
                    boardState[i - 2, j - 2] == CurrentPlayer &&
                    boardState[i - 3, j - 3] == CurrentPlayer)
                {
                    WinnerCells = new List<Tuple<int, int>>()
                    {
                        new Tuple<int, int>(i, j),
                        new Tuple<int, int>(i - 1, j - 1),
                        new Tuple<int, int>(i - 2, j - 2),
                        new Tuple<int, int>(i - 3, j - 3),
                    };
                }
            }
        }
    }

    private void StartNewGame()
    {
        CurrentPlayer = 1;
        boardState = new int[7, 6];
        WinnerCells.Clear();
        WinnerCells = null;
        CurrentPlayerWins = false;
    }
}