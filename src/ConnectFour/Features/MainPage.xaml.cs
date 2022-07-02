using ConnectFour.Controls;
using ConnectFour.Resources.Texts;
using Microsoft.Maui.Controls.Shapes;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ConnectFour.Features;

public partial class MainPage
{
    Dictionary<Tuple<int, int>, Ellipse> tokens;
    bool isTapping;

    public MainPage(MainViewModel viewModel)
	{
        tokens = new Dictionary<Tuple<int, int>, Ellipse>();
        isTapping = false;
        ViewModel = viewModel;
		InitializeComponent();
		for (int row = 0; row < gridBoard.RowDefinitions.Count; row++)
		{
			for (int colunm = 0; colunm < gridBoard.ColumnDefinitions.Count; colunm++)
			{
				CellBoard cellBoard = new CellBoard() { Column = colunm, Row = row, ZIndex = 2 };
                cellBoard.SetAppThemeColor(CellBoard.BoardColorProperty, (Color)Application.Current.Resources["BoardLight"], (Color)Application.Current.Resources["BoardDark"]);
				TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
				cellBoard.GestureRecognizers.Add(tapGestureRecognizer);
                gridBoard.Add(cellBoard, colunm, row);
            }
		}
    }

    protected override void CreateBindings(CompositeDisposable disposables)
    {
        base.CreateBindings(disposables);

        disposables.Add(this.BindCommand(ViewModel, vm => vm.StartNewGameCommand, v => v.btNewGame));
    }

    protected override void ObserveValues(CompositeDisposable disposables)
    {
        base.ObserveValues(disposables);

        disposables.Add(this.WhenAnyValue(x => x.ViewModel.WinnerCells)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(AnimateWinnerCells));

        disposables.Add(this.WhenAnyValue(x => x.ViewModel.CurrentPlayerWins)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(ManageWinnerState));

        disposables.Add(this.WhenAnyValue(x => x.ViewModel.CurrentPlayer)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(ChangeCurrentPlayer));
    }

    private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        if (isTapping || ViewModel.CurrentPlayerWins || ViewModel.IsAddingToken || ViewModel.IsAddingToken)
            return;

        isTapping = true;
        CellBoard cellBoard = sender as CellBoard;
        int column = cellBoard.Column;

        ViewModel.GetRowToAddTokenCommand
            .Execute(column)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(async row => await AddToken(column, row));
    }

    private async Task AddToken(int column, int row)
    {
        if (row < 0)
            return;

        double circleWidth = gridBoard.Width / gridBoard.ColumnDefinitions.Count;
        double circleHeight = gridBoard.Height / gridBoard.RowDefinitions.Count;
        double size = circleHeight > circleWidth ? circleWidth : circleHeight;

        double cellHeight = gridBoard.Height / gridBoard.RowDefinitions.Count;

        Ellipse circle = new Ellipse()
        {
            WidthRequest = size,
            HeightRequest = size,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            ZIndex = 1,
            TranslationY = -(cellHeight * row) - cellHeight,
            Opacity = 0,
            Scale = 0
        };
        circle.SetAppTheme<Brush>(Ellipse.FillProperty, GetLightBrushByPlayer(ViewModel.CurrentPlayer), GetDarkBrushByPlayer(ViewModel.CurrentPlayer));
        tokens.Add(new Tuple<int, int>(column, row), circle);
        gridBoard.Add(circle, column, row);

        await Task.WhenAll(
            circle.FadeTo(1, 500),
            circle.ScaleTo(1, 500));

        await Task.Delay(500);

        await circle.TranslateTo(0, 0, 1000, Easing.BounceOut);

        ViewModel.AddTokenCommand
            .Execute(new Tuple<int, int>(column, row))
            .Subscribe(_ => isTapping = false);
    }

    private void ChangeCurrentPlayer(int currentPlayer)
    {
        if (currentPlayer == 1)
        {
            circlePlayerOne.SetAppTheme<Brush>(Ellipse.FillProperty, GetLightBrushByPlayer(1), GetDarkBrushByPlayer(1));
            lbPlayerOne.SetAppThemeColor(Label.TextColorProperty, GetLightColorByPlayer(1), GetDarkColorByPlayer(1));

            circlePlayerTwo.SetAppTheme<Brush>(Ellipse.FillProperty, Brush.DarkGray, Brush.DarkGray);
            lbPlayerTwo.SetAppThemeColor(Label.TextColorProperty, Colors.DarkGray, Colors.DarkGray);
        }
        else
        {
            circlePlayerOne.SetAppTheme<Brush>(Ellipse.FillProperty, Brush.DarkGray, Brush.DarkGray);
            lbPlayerOne.SetAppThemeColor(Label.TextColorProperty, Colors.DarkGray, Colors.DarkGray);

            circlePlayerTwo.SetAppTheme<Brush>(Ellipse.FillProperty, GetLightBrushByPlayer(2), GetDarkBrushByPlayer(2));
            lbPlayerTwo.SetAppThemeColor(Label.TextColorProperty, GetLightColorByPlayer(2), GetDarkColorByPlayer(2));
        }
    }

    private async void AnimateWinnerCells(List<Tuple<int, int>> winnerCells)
    {
        if (winnerCells == null)
            return;

        Ellipse circle0 = tokens[winnerCells[0]];
        Ellipse circle1 = tokens[winnerCells[1]];
        Ellipse circle2 = tokens[winnerCells[2]];
        Ellipse circle3 = tokens[winnerCells[3]];

        for (int i = 0; i < 4; i++)
        {
            await Task.WhenAll(
                circle0.ScaleTo(0.5),
                circle1.ScaleTo(0.5),
                circle2.ScaleTo(0.5),
                circle3.ScaleTo(0.5));

            await Task.WhenAll(
                circle0.ScaleTo(1),
                circle1.ScaleTo(1),
                circle2.ScaleTo(1),
                circle3.ScaleTo(1));
        }

        await Task.WhenAll(
            gridBoard.FadeTo(0.5),
            cvFooter.FadeTo(0.5));
    }

    private void ManageWinnerState(bool currentPlayerWins)
    {
        if (currentPlayerWins)
            _ = ShowWinner();
        else
            _ = StartNewGame();
    }

    private async Task ShowWinner()
    {
        lbWinner.Text = string.Format(TextsResource.PlayerWins, ViewModel.CurrentPlayer);

        frWinner.RotationX = -270;
        await frPlayers.RotateXTo(-90);
        frPlayers.IsVisible = false;
        frWinner.IsVisible = true;
        await frWinner.RotateXTo(-360);
        frWinner.RotationX = 0;

        await Task.WhenAll(
            lbWinner.FadeTo(1),
            lbPlayerOne.FadeTo(0),
            lbPlayerTwo.FadeTo(0));

        btNewGame.IsVisible = true;
        await Task.WhenAll(
            btNewGame.FadeTo(1),
            btNewGame.ScaleTo(1));
    }

    private async Task StartNewGame()
    {
        await Task.WhenAll(
                btNewGame.FadeTo(0),
                btNewGame.ScaleTo(0));
        btNewGame.IsVisible = false;

        frPlayers.RotationX = -270;
        await frWinner.RotateXTo(-90);
        frWinner.IsVisible = false;
        frPlayers.IsVisible = true;
        await frPlayers.RotateXTo(-360);
        frPlayers.RotationX = 0;

        await Task.WhenAll(
            lbWinner.FadeTo(0),
            lbPlayerOne.FadeTo(1),
            lbPlayerTwo.FadeTo(1));


        lbWinner.Text = String.Empty;

        List<Task> removeTokenTasks = new List<Task>();
        foreach (var circle in tokens.Values)
        {
            removeTokenTasks.Add(circle.ScaleTo(0));
            removeTokenTasks.Add(circle.FadeTo(0));
        }

        removeTokenTasks.Add(gridBoard.FadeTo(1));
        removeTokenTasks.Add(cvFooter.FadeTo(1));

        await Task.WhenAll(removeTokenTasks);
        tokens.Clear();
    }

    private Brush GetLightBrushByPlayer(int player) => player == 1 ? (Brush)Application.Current.Resources["PlayerOneLightBrush"] : (Brush)Application.Current.Resources["PlayerTwoLightBrush"];
    private Color GetLightColorByPlayer(int player) => player == 1 ? (Color)Application.Current.Resources["PlayerOneLight"] : (Color)Application.Current.Resources["PlayerTwoLight"];
    private Brush GetDarkBrushByPlayer(int player) => player == 1 ? (Brush)Application.Current.Resources["PlayerOneDarkBrush"] : (Brush)Application.Current.Resources["PlayerTwoDarkBrush"];
    private Color GetDarkColorByPlayer(int player) => player == 1 ? (Color)Application.Current.Resources["PlayerOneDark"] : (Color)Application.Current.Resources["PlayerTwoDark"];
}

