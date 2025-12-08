using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace Audiobookplayer.Controls;

public partial class AudioSeekbar : ContentView
{
    public AudioSeekbar()
	{        
		InitializeComponent();
    }

    public static readonly BindableProperty PositionProperty =
        BindableProperty.Create(nameof(Position), typeof(double), typeof(AudioSeekbar), 0.0, BindingMode.TwoWay);

    public double Position
    {
        get => (double)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }
    
    public static readonly BindableProperty DurationProperty =
    BindableProperty.Create(nameof(Duration), typeof(double), typeof(AudioSeekbar), 0.0, BindingMode.Default);

    public double Duration
    {
        get => (double)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public static readonly BindableProperty PositionTimeSpanProperty =
    BindableProperty.Create(nameof(PositionTimeSpan), typeof(TimeSpan), typeof(AudioSeekbar));

    public TimeSpan PositionTimeSpan
    {
        get => (TimeSpan)GetValue(PositionTimeSpanProperty);
        private set => SetValue(PositionTimeSpanProperty, value);
    }

    public static readonly BindableProperty DurationTimeSpanProperty =
    BindableProperty.Create(nameof(DurationTimeSpan), typeof(TimeSpan), typeof(AudioSeekbar));

    public TimeSpan DurationTimeSpan
    {
        get => (TimeSpan)GetValue(DurationTimeSpanProperty);
        private set => SetValue(DurationTimeSpanProperty, value);
    }

    public static readonly BindableProperty DragCompleteCommandProperty =
    BindableProperty.Create(nameof(DragCompleteCommand), typeof(ICommand), typeof(AudioSeekbar));

    public ICommand DragCompleteCommand
    {
        get => (ICommand)GetValue(DragCompleteCommandProperty);
        set => SetValue(DragCompleteCommandProperty, value);
    }
    public static readonly BindableProperty DragStartCommandProperty =
    BindableProperty.Create(nameof(DragStartCommand), typeof(ICommand), typeof(AudioSeekbar));

    public ICommand DragStartCommand
    {
        get => (ICommand)GetValue(DragStartCommandProperty);
        set => SetValue(DragStartCommandProperty, value);
    }

    public static readonly BindableProperty IsDraggingProperty =
    BindableProperty.Create(nameof(IsDragging), typeof(bool), typeof(AudioSeekbar), false);

    public bool IsDragging
    {
        get => (bool)GetValue(IsDraggingProperty);
        set => SetValue(IsDraggingProperty, value);
    }
}